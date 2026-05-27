# AWS migration plan

## Recommended target

- Frontend: keep AWS Amplify.
- API: AWS App Runner or ECS Fargate running the .NET 8 container in `Project/ToDoApp.API/Dockerfile`.
- Database: Amazon RDS for SQL Server to avoid changing Entity Framework provider and migrations.
- Infrastructure as Code: Terraform files in `infra/`.
- Secrets/config: App Runner/ECS environment variables or AWS Secrets Manager.
- Logs: stdout/stderr to CloudWatch Logs. File logging is disabled in `appsettings.Production.json`.

## Required AWS configuration

Set these environment variables in the API service:

```text
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__ToDoAppConnectionString=Server=<rds-endpoint>,1433;Database=ToDoAppDb;User Id=<user>;Password=<password>;TrustServerCertificate=True;Encrypt=True
ConnectionStrings__ToDoAppAuthConnectionString=Server=<rds-endpoint>,1433;Database=ToDoAppAuthDb;User Id=<user>;Password=<password>;TrustServerCertificate=True;Encrypt=True
Jwt__Key=<new-strong-secret>
Jwt__Issuer=https://<api-domain>/
Jwt__Audience=https://<api-domain>/
Cors__AllowedOrigins__0=https://<your-amplify-domain>
```

Use only the Amplify origin, not page paths such as `/index.html`, because browsers send only the origin in CORS requests.

## Database setup

This migration starts with a clean AWS database. Do not export or import data from the current Azure SQL database.

1. Create an RDS SQL Server instance.
2. Create the two empty databases: `ToDoAppDb` and `ToDoAppAuthDb`.
3. Point the API connection strings to the new RDS databases.
4. Apply the existing Entity Framework migrations to create the schema and seed data:

```powershell
dotnet ef database update --context ToDoAppDbContext --project Project/ToDoApp.API/ToDoApp.API.csproj
dotnet ef database update --context ToDoAppAuthDbContext --project Project/ToDoApp.API/ToDoApp.API.csproj
```

5. Run the app and check `/health`, register/login, user CRUD and task CRUD.

## Security cleanup

- Rotate the Azure SQL password that appeared in publish artifacts before this repository is shared or pushed.
- Remove already tracked `bin/`, `obj/`, `Logs/`, `Properties/PublishProfiles/` and `Properties/ServiceDependencies/` files from Git history or at least from the next commit.
- Generate a new `Jwt__Key` for AWS instead of reusing the development key in `appsettings.json`.

## Deploy flow

1. Use Terraform in `infra/` to create the base AWS resources: VPC, RDS SQL Server and ECR.
2. Build and push the Docker image to the ECR repository created by Terraform.
3. Enable `create_app_runner_service` in Terraform and apply again to create App Runner.
4. Configure the final App Runner URL as `api_public_url` and apply once more so JWT issuer/audience match the API URL.
5. Apply the EF migrations against the clean RDS databases.
6. Update the Amplify frontend API base URL to the new AWS API URL.
7. After validation, cut traffic from Azure to AWS and decommission the Azure resources.
