module "ssm" {
  source      = "../modules/configurationparameters"
  environment = var.environment
}

resource "aws_ssm_parameter" "DbHost" {
  name  = "/collaction/${var.environment}/DbHost"
  type  = "String"
  value = data.aws_db_instance.collactiondb.address
  # value = module.rds.address
}

resource "aws_ssm_parameter" "Db" {
  name  = "/collaction/${var.environment}/Db"
  type  = "String"
  value = data.aws_db_instance.collactiondb.db_name
  # value = module.rds.name
}

resource "aws_ssm_parameter" "DbUser" {
  name  = "/collaction/${var.environment}/DbUser"
  type  = "String"
  value = data.aws_db_instance.collactiondb.master_username
  # value = module.rds.username
}

resource "aws_ssm_parameter" "DbPassword" {
  name  = "/collaction/${var.environment}/DbPassword"
  type  = "SecureString"
  value = "todo"
  # value = module.rds.password
  
  lifecycle {
    ignore_changes = [value]
  }
}
