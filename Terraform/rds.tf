resource "random_password" "db_password" {
  length  = 16
  special = false
}

resource "aws_db_instance" "collaction" {
  identifier                = "collaction-${var.environment}"
  final_snapshot_identifier = "final-collaction-${var.environment}"
  engine                    = "postgres"
  engine_version            = "12.2"
  storage_type              = "gp2"
  allocated_storage         = var.rds_allocated_storage
  instance_class            = var.rds_instance_class
  name                      = "collaction${var.environment}"
  username                  = "collaction"
  password                  = random_password.db_password.result
  vpc_security_group_ids = [
    aws_security_group.rds.id
  ]
}

resource "aws_security_group" "rds" {
  name        = "incoming-${var.environment}-rds"
  description = "Allow access to ${var.environment} RDS database"

  ingress {
    description = "Allow access from API containers"
    protocol    = "tcp"
    from_port   = "5432"
    to_port     = "5432"
    security_groups = [
      aws_security_group.access_rds.id
    ]
  }
}

resource "aws_security_group" "access_rds" {
  name        = "access-rds-${var.environment}"
  description = "Provide access to ${var.environment} RDS database"
}
