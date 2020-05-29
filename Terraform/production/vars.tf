variable "environment" {
  description = "The name of this environment"
  default = "prod"
}

variable "imageversion" {
  description = "The tagname of the image to deploy"
  default = "v2.0.0-beta2"
}

variable "api_desired_count" {
  description = "The desired number of instances of the API container"
  default = 0
}

variable "api_cpu" {
  description = "Assigned number of vCPUs for the API container"
  default = 512
}

variable "api_memory" {
  description = "Assigned memory (GB) for the API container"
  default = 1024
}

variable "hostname" {
  description = "The hostname under which to register the environment"
  default = "api.collaction.org"
}