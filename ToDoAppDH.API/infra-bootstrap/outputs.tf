output "state_bucket_name" {
  description = "S3 bucket name for Terraform remote state."
  value       = aws_s3_bucket.terraform_state.bucket
}

output "lock_table_name" {
  description = "DynamoDB table name for Terraform state locking."
  value       = aws_dynamodb_table.terraform_lock.name
}

output "backend_tf" {
  description = "Backend block to copy into infra/backend.tf."
  value       = <<EOT
terraform {
  backend "s3" {
    bucket         = "${aws_s3_bucket.terraform_state.bucket}"
    key            = "todoapp-api/prod/terraform.tfstate"
    region         = "${var.aws_region}"
    encrypt        = true
    dynamodb_table = "${aws_dynamodb_table.terraform_lock.name}"
  }
}
EOT
}
