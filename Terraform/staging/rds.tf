module "rds" {
  source = "../modules/database"

  identifier               = "collaction-${var.environment}"
  instance_class           = var.rds_instance_class
  allocated_storage        = var.rds_allocated_storage
  backup_retention_periode = 0
  deletion_protection      = false
  route53_zone_name        = var.route53_zone_name
  rds_hostname             = var.rds_hostname
}
