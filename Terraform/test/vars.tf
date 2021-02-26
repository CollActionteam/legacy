variable "environment" {
  default = "test"
}

variable "capacity_provider" {
  default = "FARGATE_SPOT"
}

variable "api_desired_count" {
  description = "The desired number of instances of the API container"
  default     = 1
}

variable "testselect_desired_count" {
  description = "The desired number of instances of the API container"
  default     = 1
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