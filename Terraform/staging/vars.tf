variable "environment" {
  default = "staging"
}

variable "imageversion" {
  default = "v2.0.0-beta2"
}

variable "api_desired_count" {
  default = 1
}

variable "api_cpu" {
  default = 256
}

variable "api_memory" {
  default = 512
}

variable "rds_instance_class" {
  default = "db.t2.micro"
}

variable "rds_allocated_storage" {
  default = 20
}

variable "hostname" {
  default = "api-staging.collaction.org"
}

variable "route53_zone_name" {
  default = "collaction.org."
}