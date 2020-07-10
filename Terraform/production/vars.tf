variable "environment" {
  description = "The name of this environment"
  default     = "prod"
}

variable "imageversion" {
  description = "The tagname of the image to deploy"
}

variable "api_desired_count" {
  description = "The desired number of instances of the API container"
  default     = 0
}

variable "api_cpu" {
  description = "Assigned number of vCPUs for the API container"
  default     = 512
}

variable "api_memory" {
  description = "Assigned memory (GB) for the API container"
  default     = 1024
}

variable "rds_instance_class" {
  default = "db.t2.medium"
}

variable "rds_allocated_storage" {
  default = 100
}

variable "hostname" {
  default = "api.collaction.org"
}

variable "route53_zone_name" {
  default = "collaction.org."
}

variable "rds_hostname" {
  description = "DNS entry for the RDS database"
  default     = "db.collaction.org"
}