# For now, we are going to connect to the existing database
# module "rds" {
#   source = "../modules/database"

#   identifier = "collaction-${var.environment}"
#   instance_class = var.rds_instance_class
#   allocated_storage = var.rds_allocated_storage
# }

data "aws_db_instance" "collactiondb" {
  db_instance_identifier = "collactiondb"
}