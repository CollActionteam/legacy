resource "random_password" "db_password" {
  length  = 16
  special = false
}

resource "aws_db_instance" "collaction" {
  identifier                = var.identifier
  final_snapshot_identifier = "final-${var.identifier}"
  engine                    = "postgres"
  engine_version            = "12.2"
  storage_type              = "gp2"
  allocated_storage         = var.allocated_storage
  instance_class            = var.instance_class
  name                      = "collactiondb"
  username                  = "postgres"
  password                  = random_password.db_password.result
  vpc_security_group_ids = [
    aws_security_group.rds.id
  ]
}

resource "aws_security_group" "rds" {
  name        = "incoming-${var.identifier}"
  description = "Allow access to RDS database ${var.identifier}"

  ingress {
    description = "Allow access to RDS ${var.identifier}"
    protocol    = "tcp"
    from_port   = "5432"
    to_port     = "5432"
    security_groups = [
      aws_security_group.access_rds.id
    ]
  }
}

resource "aws_security_group" "access_rds" {
  name        = "access-rds-${var.identifier}"
  description = "Provide access to RDS ${var.identifier}"
}
