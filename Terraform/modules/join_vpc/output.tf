output "default" {
  value = data.aws_vpc.default
  description = "The default VPC in your account"
}

output "subnet_ids" {
  value = data.aws_subnet_ids.default
  description = "The subnet IDs in your default VPC"
}
