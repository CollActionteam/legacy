variable "aws_region" {
  description = "The AWS region things are created in"
  default     = "eu-central-1"
}

variable "environment" {
  description = "Name of the environment to manage"
  default     = "dev"
}
