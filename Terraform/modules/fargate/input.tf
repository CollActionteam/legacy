variable "region" {
  description = "The region where the cloudwatch logs are created"
}

variable "cluster" {
  description = "Name of the Fargate cluster"
}

variable "imageversion" {
  description = "The version of the image to deploy to this environment"
}

variable "environment" {
  description = "Name of the environment this fargate environment hosts"
}

variable "ecs_api_cpu" {
  description = "Nr of vCPUs to assign to the API task"
  default = 256
}

variable "ecs_api_memory" {
  description = "Memory (GB) to assign to the API task"
  default = 512
}

variable "ssm_securestrings" {
  description = "The SSM parameters to add to the task's environment as secure parameters"
}

variable "ssm_strings" {
  description = "The SSM parameters to add to the task's environment as normal parameters"
}

variable "ssm_dbhost" {
  description = "ARN to the SSM parameter holding the dbhost value"
}

variable "ssm_db" {
  description = "ARN to the SSM parameter holding the db value"
}

variable "ssm_dbuser" {
  description = "ARN to the SSM parameter holding the dbuser value"
}

variable "ssm_dbpassword" {
  description = "ARN to the SSM parameter holding the dbpassword value"
}
