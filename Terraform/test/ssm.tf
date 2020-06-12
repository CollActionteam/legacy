module "ssm" {
  source      = "../modules/configurationparameters"
  environment = var.environment
}

resource "aws_ssm_parameter" "Db" {
  name  = "/collaction/${var.environment}/Db"
  type  = "String"
  value = "CollActionDb"
}

resource "aws_ssm_parameter" "DbUser" {
  name  = "/collaction/${var.environment}/DbUser"
  type  = "String"
  value = "postgres"
}

resource "random_password" "db_password" {
  length  = 16
  special = false
}

resource "aws_ssm_parameter" "DbPassword" {
  name  = "/collaction/${var.environment}/DbPassword"
  type  = "SecureString"
  value = random_password.db_password.result
}
