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

variable "route53_zone_name" {
  description = "The zone in which to create a hostname to this environment"
}

variable "rds_hostname" {
  description = "The DNS hostname for the database instance"
}

variable "backup_retention_period" {
  description = "Number of days to keep a backup"
}

variable "performance_insights_enabled" {
  default = false
}

variable "multi_az" {
  default = false
}
