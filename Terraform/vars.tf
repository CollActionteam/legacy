variable "aws_region" {
  description = "The AWS region things are created in"
  default     = "eu-central-1"
}

variable "environment" {
  description = "Name of the environment to manage"
  default     = "dev"
}

variable "imageversion" {
  description = "Version of the image to deploy"
  default     = "CA-707"
}

variable "desiredcount" {
  description = "The desired number of containers to run"
  default     = "1"
}

variable "rds_instance_class" {
  description = "The instance class of the RDS database"
  default     = "db.t2.micro"
}

variable "rds_allocated_storage" {
  description = "Storage allocation (GB) for the database"
  default     = "20"
}

variable "ecs_api_cpu" {
  description = "vCPUs for the API container"
  default     = 256
}

variable "ecs_api_memory" {
  description = "Memory (GB) for the API container"
  default     = 512
}