variable "aws_region" {
  description = "AWS region where the Terraform state backend will be created."
  type        = string
  default     = "sa-east-1"
}

variable "project_name" {
  description = "Project name used in backend resource names."
  type        = string
  default     = "todoapp"
}

variable "environment" {
  description = "Environment name used in backend resource names."
  type        = string
  default     = "prod"
}

variable "state_bucket_name" {
  description = "Globally unique S3 bucket name for Terraform state. Leave empty to use the default naming pattern."
  type        = string
  default     = ""
}

variable "lock_table_name" {
  description = "DynamoDB table name for Terraform state locking. Leave empty to use the default naming pattern."
  type        = string
  default     = ""
}

variable "force_destroy_state_bucket" {
  description = "Whether Terraform can destroy the state bucket even when it contains objects. Keep false for safety."
  type        = bool
  default     = false
}
