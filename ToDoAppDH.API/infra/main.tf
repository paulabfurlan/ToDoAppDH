locals {
  name_prefix = "${var.project_name}-${var.environment}"

  tags = {
    Project     = var.project_name
    Environment = var.environment
    ManagedBy   = "terraform"
  }
}

data "aws_availability_zones" "available" {
  state = "available"
}

resource "aws_vpc" "main" {
  cidr_block           = var.vpc_cidr
  enable_dns_hostnames = true
  enable_dns_support   = true

  tags = merge(local.tags, {
    Name = "${local.name_prefix}-vpc"
  })
}

resource "aws_subnet" "private" {
  count = 2

  vpc_id                  = aws_vpc.main.id
  cidr_block              = cidrsubnet(var.vpc_cidr, 8, count.index + 1)
  availability_zone       = data.aws_availability_zones.available.names[count.index]
  map_public_ip_on_launch = false

  tags = merge(local.tags, {
    Name = "${local.name_prefix}-private-${count.index + 1}"
  })
}

resource "aws_security_group" "app_runner_vpc_connector" {
  name        = "${local.name_prefix}-apprunner-vpc-connector-sg"
  description = "Outbound access from App Runner to private resources"
  vpc_id      = aws_vpc.main.id

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = local.tags
}

resource "aws_security_group" "database" {
  name        = "${local.name_prefix}-db-sg"
  description = "SQL Server access from App Runner"
  vpc_id      = aws_vpc.main.id

  ingress {
    description     = "SQL Server from App Runner VPC connector"
    from_port       = 1433
    to_port         = 1433
    protocol        = "tcp"
    security_groups = [aws_security_group.app_runner_vpc_connector.id]
  }

  tags = local.tags
}

resource "aws_db_subnet_group" "database" {
  name       = "${local.name_prefix}-db-subnets"
  subnet_ids = aws_subnet.private[*].id

  tags = local.tags
}

resource "aws_db_instance" "sql_server" {
  identifier                  = "${local.name_prefix}-sqlserver"
  engine                      = var.database_engine
  engine_version              = var.database_engine_version
  instance_class              = var.database_instance_class
  allocated_storage           = var.database_allocated_storage
  storage_type                = "gp3"
  username                    = var.database_username
  manage_master_user_password = true
  license_model               = "license-included"
  db_subnet_group_name        = aws_db_subnet_group.database.name
  vpc_security_group_ids      = [aws_security_group.database.id]
  publicly_accessible         = false
  multi_az                    = false
  backup_retention_period     = 7
  skip_final_snapshot         = var.database_skip_final_snapshot
  final_snapshot_identifier   = var.database_skip_final_snapshot ? null : "${local.name_prefix}-sqlserver-final-snapshot"
  deletion_protection         = !var.database_skip_final_snapshot
  apply_immediately           = true

  tags = local.tags
}

resource "aws_ecr_repository" "api" {
  name                 = "${local.name_prefix}-api"
  image_tag_mutability = "MUTABLE"

  image_scanning_configuration {
    scan_on_push = true
  }

  tags = local.tags
}

resource "aws_iam_role" "app_runner_ecr_access" {
  count = var.create_app_runner_service ? 1 : 0

  name = "${local.name_prefix}-apprunner-ecr-access"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Principal = {
          Service = "build.apprunner.amazonaws.com"
        }
        Action = "sts:AssumeRole"
      }
    ]
  })

  tags = local.tags
}

resource "aws_iam_role_policy_attachment" "app_runner_ecr_access" {
  count = var.create_app_runner_service ? 1 : 0

  role       = aws_iam_role.app_runner_ecr_access[0].name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AWSAppRunnerServicePolicyForECRAccess"
}

resource "aws_iam_role" "app_runner_instance" {
  count = var.create_app_runner_service ? 1 : 0

  name = "${local.name_prefix}-apprunner-instance"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Principal = {
          Service = "tasks.apprunner.amazonaws.com"
        }
        Action = "sts:AssumeRole"
      }
    ]
  })

  tags = local.tags
}

resource "aws_iam_role_policy" "app_runner_secrets_access" {
  count = var.create_app_runner_service ? 1 : 0

  name = "${local.name_prefix}-apprunner-secrets-access"
  role = aws_iam_role.app_runner_instance[0].id

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Action = [
          "secretsmanager:GetSecretValue",
          "secretsmanager:DescribeSecret"
        ]
        Resource = [
          var.todoapp_connection_string_secret_arn,
          var.todoapp_auth_connection_string_secret_arn,
          var.jwt_key_secret_arn
        ]
      }
    ]
  })
}

resource "aws_apprunner_vpc_connector" "api" {
  count = var.create_app_runner_service ? 1 : 0

  vpc_connector_name = "${local.name_prefix}-api-vpc-connector"
  subnets            = aws_subnet.private[*].id
  security_groups    = [aws_security_group.app_runner_vpc_connector.id]

  tags = local.tags
}

resource "aws_apprunner_service" "api" {
  count = var.create_app_runner_service ? 1 : 0

  service_name = "${local.name_prefix}-api"

  source_configuration {
    auto_deployments_enabled = false

    authentication_configuration {
      access_role_arn = aws_iam_role.app_runner_ecr_access[0].arn
    }

    image_repository {
      image_identifier      = var.app_image_identifier
      image_repository_type = "ECR"

      image_configuration {
        port = "8080"

        runtime_environment_variables = {
          ASPNETCORE_ENVIRONMENT  = "Production"
          Cors__AllowedOrigins__0 = var.amplify_origin
          Jwt__Audience           = var.api_public_url
          Jwt__Issuer             = var.api_public_url
        }

        runtime_environment_secrets = {
          ConnectionStrings__ToDoAppConnectionString     = var.todoapp_connection_string_secret_arn
          ConnectionStrings__ToDoAppAuthConnectionString = var.todoapp_auth_connection_string_secret_arn
          Jwt__Key                                       = var.jwt_key_secret_arn
        }
      }
    }
  }

  instance_configuration {
    cpu               = "0.25 vCPU"
    memory            = "0.5 GB"
    instance_role_arn = aws_iam_role.app_runner_instance[0].arn
  }

  network_configuration {
    egress_configuration {
      egress_type       = "VPC"
      vpc_connector_arn = aws_apprunner_vpc_connector.api[0].arn
    }
  }

  health_check_configuration {
    protocol            = "HTTP"
    path                = "/health"
    interval            = 10
    timeout             = 5
    healthy_threshold   = 1
    unhealthy_threshold = 5
  }

  tags = local.tags
}
