variable "region" {
  default = "eu-central-1"
}

provider "aws" {
  region  = var.region
  profile = "collaction"
}
