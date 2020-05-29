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
  default = 512
}

variable "api_memory" {
  default = 1024
}

variable "hostname" {
  default = "api-staging.collaction.org"
}