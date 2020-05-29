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
