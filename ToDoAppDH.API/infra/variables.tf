variable "aws_region" {
  description = "AWS region where the API infrastructure will be provisioned."
  type        = string
  default     = "sa-east-1"
}

variable "project_name" {
  description = "Prefix used for AWS resource names."
  type        = string
  default     = "todoapp"
}

variable "environment" {
  description = "Environment name used in tags and resource names."
  type        = string
  default     = "prod"
}

variable "vpc_cidr" {
  description = "CIDR block for the application VPC."
  type        = string
  default     = "10.42.0.0/16"
}

variable "database_username" {
  description = "Master username for the RDS SQL Server instance."
  type        = string
  default     = "todoappadmin"
}

variable "database_password" {
  description = "Master password for the RDS SQL Server instance."
  type        = string
  sensitive   = true
}

variable "database_instance_class" {
  description = "RDS instance class for SQL Server."
  type        = string
  default     = "db.t3.small"
}

variable "database_allocated_storage" {
  description = "Allocated RDS storage in GB."
  type        = number
  default     = 20
}

variable "database_engine" {
  description = "RDS SQL Server engine. Use sqlserver-ex for Express or sqlserver-web/std/ee if licensed for the workload."
  type        = string
  default     = "sqlserver-ex"
}

variable "database_engine_version" {
  description = "Optional SQL Server engine version. Leave null to let AWS choose the default for the selected engine."
  type        = string
  default     = null
}

variable "database_skip_final_snapshot" {
  description = "Whether to skip the final RDS snapshot when destroying this environment."
  type        = bool
  default     = false
}

variable "amplify_origin" {
  description = "Allowed frontend origin for CORS, for example https://main.xxxxx.amplifyapp.com."
  type        = string
}

variable "create_app_runner_service" {
  description = "Set to true after pushing the Docker image to ECR."
  type        = bool
  default     = false
}

variable "app_image_identifier" {
  description = "Full ECR image URI used by App Runner, for example account.dkr.ecr.region.amazonaws.com/todoapp-api:latest."
  type        = string
  default     = ""
}

variable "api_public_url" {
  description = "Public API URL used as JWT issuer/audience after App Runner or a custom domain is available."
  type        = string
  default     = ""
}
