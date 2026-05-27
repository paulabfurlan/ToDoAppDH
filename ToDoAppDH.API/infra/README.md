# Terraform AWS infrastructure

This folder provisions the AWS resources for the To Do App API:

- VPC with private subnets
- Security groups for App Runner to reach RDS
- RDS SQL Server for a clean database setup
- ECR repository for the API Docker image
- Optional App Runner service for the API container
- App Runner runtime access to Secrets Manager

## Credential model

The Terraform configuration is prepared so application secrets are not stored in `terraform.tfvars`:

- RDS master password is managed by RDS in AWS Secrets Manager.
- App connection strings are stored in AWS Secrets Manager.
- JWT signing key is stored in AWS Secrets Manager.
- App Runner receives secret ARNs and injects the secret values as environment variables at runtime.

Terraform state can still contain sensitive metadata and secret ARNs. For real deployment, use an encrypted S3 backend with DynamoDB locking.

## 1. Prepare remote Terraform state

Create these resources with the separate bootstrap Terraform project in `../infra-bootstrap`:

- S3 bucket for Terraform state, with versioning and encryption enabled.
- DynamoDB table for Terraform locking, with a string partition key named `LockID`.

After applying `../infra-bootstrap`, copy the `backend_tf` output into this folder:

```powershell
../infra/backend.tf
```

The existing `backend.tf.example` remains as a reference.

## 2. Configure variables

Copy the example variables file:

```powershell
Copy-Item terraform.tfvars.example terraform.tfvars
```

Edit `terraform.tfvars` and set:

- `amplify_origin`
- `aws_region`, if you do not want `sa-east-1`

Keep `create_app_runner_service = false` for the first apply.

## 3. Create base infrastructure

```powershell
terraform init
terraform plan
terraform apply
```

This creates the VPC, private RDS SQL Server instance and ECR repository.

Save these outputs:

- `ecr_repository_url`
- `rds_endpoint`
- `rds_master_user_secret_arn`

## 4. Create application secrets

Read the RDS master password from the RDS-managed secret:

```powershell
aws secretsmanager get-secret-value --secret-id <rds_master_user_secret_arn>
```

Create three Secrets Manager secrets:

```powershell
aws secretsmanager create-secret `
  --name todoapp/prod/connectionstrings/todoappdb `
  --secret-string "Server=<rds_endpoint>,1433;Database=ToDoAppDb;User Id=todoappadmin;Password=<rds-password>;TrustServerCertificate=True;Encrypt=True"

aws secretsmanager create-secret `
  --name todoapp/prod/connectionstrings/todoappauthdb `
  --secret-string "Server=<rds_endpoint>,1433;Database=ToDoAppAuthDb;User Id=todoappadmin;Password=<rds-password>;TrustServerCertificate=True;Encrypt=True"

aws secretsmanager create-secret `
  --name todoapp/prod/jwt/key `
  --secret-string "<new-strong-jwt-secret>"
```

Save the three secret ARNs.

## 5. Push the API image to ECR

Use the `ecr_repository_url` Terraform output as the image repository.

```powershell
aws ecr get-login-password --region <region> | docker login --username AWS --password-stdin <account-id>.dkr.ecr.<region>.amazonaws.com
docker build -t todoapp-api ../Project/ToDoApp.API
docker tag todoapp-api:latest <ecr_repository_url>:latest
docker push <ecr_repository_url>:latest
```

## 6. Create App Runner

Update `terraform.tfvars`:

```hcl
create_app_runner_service = true
app_image_identifier      = "<ecr_repository_url>:latest"
api_public_url            = "https://temporary-placeholder/"

todoapp_connection_string_secret_arn      = "<todoappdb-connection-string-secret-arn>"
todoapp_auth_connection_string_secret_arn = "<todoappauthdb-connection-string-secret-arn>"
jwt_key_secret_arn                        = "<jwt-key-secret-arn>"
```

Run:

```powershell
terraform apply
```

After App Runner is created, copy the `app_runner_service_url` output and update `api_public_url` to:

```hcl
api_public_url = "https://<app-runner-service-url>/"
```

Run `terraform apply` again so JWT issuer/audience match the public API URL.

## 7. Create the clean database schema

The Terraform AWS provider creates the RDS instance, not the SQL Server user databases. Apply the EF migrations against RDS:

```powershell
dotnet ef database update --context ToDoAppDbContext --project ../Project/ToDoApp.API/ToDoApp.API.csproj
dotnet ef database update --context ToDoAppAuthDbContext --project ../Project/ToDoApp.API/ToDoApp.API.csproj
```

If your RDS instance is private, run the migration commands from a machine with network access to the VPC, such as a temporary EC2 instance, VPN, or a CI runner inside the VPC.

## Notes

- Do not commit `terraform.tfvars`, local state, plans, database passwords or generated JWT keys.
- Commit `.terraform.lock.hcl` after `terraform init`; it pins provider versions.
- The current setup is intentionally small and suitable for a portfolio/dev production environment. For higher availability, consider Multi-AZ RDS, backups, custom domain, WAF and a stricter secret management setup.
