resource "random_password" "db_password" {
  length  = 16
  special = false
}

resource "aws_db_instance" "collaction" {
  identifier                            = var.identifier
  final_snapshot_identifier             = "final-${var.identifier}"
  engine                                = "postgres"
  engine_version                        = "12.2"
  storage_type                          = "gp2"
  allocated_storage                     = var.allocated_storage
  instance_class                        = var.instance_class
  name                                  = "collactiondb"
  username                              = "postgres"
  password                              = random_password.db_password.result
  publicly_accessible                   = true
  deletion_protection                   = var.deletion_protection
  backup_retention_period               = var.backup_retention_period
  backup_window                         = "04:00-05:00" 
  performance_insights_enabled          = var.performance_insights_enabled
  performance_insights_retention_period = var.performance_insights_enabled == false ? 0 : 7 # Either 7 or 731 (2 years)
  multi_az                              = var.multi_az

  vpc_security_group_ids = [
    aws_security_group.rds.id,
    aws_security_group.stitch.id,
    aws_security_group.bloom-office.id
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

resource "aws_security_group" "stitch" {
  name        = "stitch-${var.identifier}"
  description = "Provide access to RDS ${var.identifier} for Stitch"

  ingress {
    # See https://www.stitchdata.com/docs/destinations/postgresql/connecting-an-amazon-postgresql-rds-data-warehouse-to-stitch#prerequisites
    description = "Incoming network traffic from Stitch"
    protocol    = "tcp"
    from_port   = 5432
    to_port     = 5432
    cidr_blocks = [
      "52.23.137.21/32",
      "52.204.223.208/32",
      "52.204.228.32/32",
      "52.204.230.227/32"
    ]
  }
}

resource "aws_security_group" "bloom-office" {
  name        = "bloom-${var.identifier}"
  description = "Provide access to RDS ${var.identifier} from Bloom office network"

  ingress {
    # See https://www.stitchdata.com/docs/destinations/postgresql/connecting-an-amazon-postgresql-rds-data-warehouse-to-stitch#prerequisites
    description = "Incoming network traffic from Blook office network"
    protocol    = "tcp"
    from_port   = 5432
    to_port     = 5432
    cidr_blocks = [
      "92.111.16.214/32"
    ]
  }
}