# Terraform AWS infrastructure

This folder provisions the AWS resources for the To Do App API:

- VPC with private subnets
- Security groups for App Runner to reach RDS
- RDS SQL Server for a clean database setup
- ECR repository for the API Docker image
- Optional App Runner service for the API container

## 1. Configure variables

Copy the example variables file:

```powershell
Copy-Item terraform.tfvars.example terraform.tfvars
```

Edit `terraform.tfvars` and set:

- `database_password`
- `amplify_origin`
- `aws_region`, if you do not want `sa-east-1`

Keep `create_app_runner_service = false` for the first apply.

## 2. Create base infrastructure

```powershell
terraform init
terraform plan
terraform apply
```

This creates the VPC, private RDS SQL Server instance and ECR repository.

## 3. Push the API image to ECR

Use the `ecr_repository_url` Terraform output as the image repository.

```powershell
aws ecr get-login-password --region <region> | docker login --username AWS --password-stdin <account-id>.dkr.ecr.<region>.amazonaws.com
docker build -t todoapp-api ../Project/ToDoApp.API
docker tag todoapp-api:latest <ecr_repository_url>:latest
docker push <ecr_repository_url>:latest
```

## 4. Create App Runner

Update `terraform.tfvars`:

```hcl
create_app_runner_service = true
app_image_identifier      = "<ecr_repository_url>:latest"
api_public_url            = "https://temporary-placeholder/"
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

## 5. Create the clean database schema

The Terraform AWS provider creates the RDS instance, not the SQL Server user databases. Apply the EF migrations against RDS:

```powershell
dotnet ef database update --context ToDoAppDbContext --project ../Project/ToDoApp.API/ToDoApp.API.csproj
dotnet ef database update --context ToDoAppAuthDbContext --project ../Project/ToDoApp.API/ToDoApp.API.csproj
```

If your RDS instance is private, run the migration commands from a machine with network access to the VPC, such as a temporary EC2 instance, VPN, or a CI runner inside the VPC.

## Notes

- Terraform state will contain sensitive values such as the RDS password and generated JWT key. Use encrypted remote state before using this in a shared/professional environment.
- The current setup is intentionally small and suitable for a portfolio/dev production environment. For higher availability, consider Multi-AZ RDS, backups, custom domain, WAF and a stricter secret management setup.
