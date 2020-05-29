# Create parameters in the parameter store. 
# Values are initialized with the value 'todo'. After the script runs, update the values and restart the container.
resource "aws_ssm_parameter" "securestring_parameters" {
  for_each = var.securestring_parameters

  name  = "/collaction/${var.environment}/${each.key}"
  type  = "SecureString"
  value = "todo"

  lifecycle {
    ignore_changes = [value]
  }
}

resource "aws_ssm_parameter" "string_parameters" {
  for_each = var.string_parameters

  name  = "/collaction/${var.environment}/${each.key}"
  type  = "String"
  value = "todo"

  lifecycle {
    ignore_changes = [value]
  }
}

resource "aws_ssm_parameter" "DbHost" {
  name  = "/collaction/${var.environment}/DbHost"
  type  = "String"
  value = aws_db_instance.collaction.address
}

resource "aws_ssm_parameter" "Db" {
  name  = "/collaction/${var.environment}/Db"
  type  = "String"
  value = aws_db_instance.collaction.name
}

resource "aws_ssm_parameter" "DbUser" {
  name  = "/collaction/${var.environment}/DbUser"
  type  = "String"
  value = aws_db_instance.collaction.username
}

resource "aws_ssm_parameter" "DbPassword" {
  name  = "/collaction/${var.environment}/DbPassword"
  type  = "SecureString"
  value = random_password.db_password.result
}
