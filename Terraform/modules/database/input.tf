variable "identifier" {
  description = "The RDS instance identifier"
}

variable "allocated_storage" {
  description = "Nr of GBs to allocate as storage for this database"
  default = 20
}

variable "instance_class" {
  description = "The instance class for this database"
  default = "db.t2.micro"
}