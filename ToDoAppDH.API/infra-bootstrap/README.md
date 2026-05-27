# Terraform backend bootstrap

Run this Terraform project before the main `infra/` project. It creates:

- S3 bucket for Terraform remote state
- DynamoDB table for Terraform state locking

The bootstrap itself starts with local state. After it creates the backend resources, keep its state file private and backed up, or later migrate it to the same S3 backend under a different key.

## 1. Configure

```powershell
cd infra-bootstrap
Copy-Item terraform.tfvars.example terraform.tfvars
```

Edit `terraform.tfvars` if needed. The default bucket name includes the AWS account ID to avoid collisions:

```text
todoapp-prod-terraform-state-<aws-account-id>
```

## 2. Apply

```powershell
terraform init
terraform plan
terraform apply
```

## 3. Configure the main infra backend

After apply, copy the `backend_tf` output into:

```text
../infra/backend.tf
```

Then initialize the main infra:

```powershell
cd ../infra
terraform init
```

Do this before running `terraform plan` or `terraform apply` in `infra/`.

## Notes

- Do not commit `terraform.tfvars` or `terraform.tfstate`.
- Commit `.terraform.lock.hcl` after `terraform init`.
- Keep `force_destroy_state_bucket = false` unless you are intentionally tearing down the backend and have already backed up state.
