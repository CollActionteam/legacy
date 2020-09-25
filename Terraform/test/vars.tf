variable "environment" {
  default = "test"
}

variable "capacity_provider" {
  default = "FARGATE_SPOT"
}

variable "imageversion" {
  description = "The tagname of the image to deploy"
  default     = "latest"
}

variable "hostname" {
  default = "api-test.collaction.org"
}

variable "route53_zone_name" {
  default = "collaction.org."
}