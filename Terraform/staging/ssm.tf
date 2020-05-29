module "ssm" {
  source      = "../modules/configurationparameters"
  environment = var.environment
}

resource "aws_ssm_parameter" "DbHost" {
  name  = "/collaction/${var.environment}/DbHost"
  type  = "String"
  value = module.rds.address
}

resource "aws_ssm_parameter" "Db" {
  name  = "/collaction/${var.environment}/Db"
  type  = "String"
  value = module.rds.name
}

resource "aws_ssm_parameter" "DbUser" {
  name  = "/collaction/${var.environment}/DbUser"
  type  = "String"
  value = module.rds.username
}

resource "aws_ssm_parameter" "DbPassword" {
  name  = "/collaction/${var.environment}/DbPassword"
  type  = "SecureString"
  value = module.rds.password
}
