variable "environment" {
  default = "test"
}

variable "capacity_provider" {
  default = "FARGATE_SPOT"
}

variable "imageversion" {
  default = "v2.0.0"
}

variable "hostname" {
  default = "api-test.collaction.org"
}

variable "route53_zone_name" {
  default = "collaction.org."
}