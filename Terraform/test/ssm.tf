module "ssm" {
  source      = "../modules/configurationparameters"
  environment = var.environment
}

# Database settings for the database task
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

# Settings for the testselect task
resource "aws_ssm_parameter" "AwsSecretAccessKey" {
  name  = "/testselect/AwsSecretAccessKey"
  type  = "SecureString"
  value = "todo"

  lifecycle {
    ignore_changes = [value]
  }
}

resource "aws_ssm_parameter" "AwsSecretAccessKeyID" {
  name  = "/testselect/AwsSecretAccessKeyID"
  type  = "SecureString"
  value = "todo"

  lifecycle {
    ignore_changes = [value]
  }
}

resource "aws_ssm_parameter" "Username" {
  name  = "/testselect/Username"
  type  = "SecureString"
  value = "todo"

  lifecycle {
    ignore_changes = [value]
  }
}

resource "aws_ssm_parameter" "Password" {
  name  = "/testselect/Password"
  type  = "SecureString"
  value = "todo"

  lifecycle {
    ignore_changes = [value]
  }
}
