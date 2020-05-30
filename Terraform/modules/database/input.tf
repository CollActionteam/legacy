variable "identifier" {
  description = "The RDS instance identifier"
}

variable "allocated_storage" {
  description = "Nr of GBs to allocate as storage for this database"
  default     = 20
}

variable "instance_class" {
  description = "The instance class for this database"
  default     = "db.t2.micro"
}

variable "backup_retention_periode" {
  description = "The days to retain backups for. Set to 0 for no backups"
  default     = 7
}

variable "deletion_protection" {
  description = "If the DB instance should have deletion protection enabled."
  default     = true
}

variable "publicly_accessible" {
  description = "Set if the database is publicly available"
  default     = false
}
