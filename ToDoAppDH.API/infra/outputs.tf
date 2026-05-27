output "ecr_repository_url" {
  description = "ECR repository URL for the API Docker image."
  value       = aws_ecr_repository.api.repository_url
}

output "rds_endpoint" {
  description = "RDS SQL Server endpoint."
  value       = aws_db_instance.sql_server.address
}

output "rds_master_user_secret_arn" {
  description = "Secrets Manager ARN for the RDS-managed master user secret."
  value       = aws_db_instance.sql_server.master_user_secret[0].secret_arn
}

output "app_runner_service_url" {
  description = "Generated App Runner service URL. Empty until create_app_runner_service is true."
  value       = try(aws_apprunner_service.api[0].service_url, "")
}

output "app_runner_service_arn" {
  description = "App Runner service ARN. Empty until create_app_runner_service is true."
  value       = try(aws_apprunner_service.api[0].arn, "")
}
