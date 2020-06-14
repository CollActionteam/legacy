variable "environment" {
  description = "The name of this environment"
  default = "staging"
}

variable "imageversion" {
  description = "The tagname of the image to deploy"
}

variable "api_desired_count" {
  description = "The desired number of instances of the API container"  
  default = 1
}

variable "api_cpu" {
  description = "Assigned number of vCPUs for the API container"
  default = 256
}

variable "api_memory" {
  description = "Assigned memory (GB) for the API container"
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