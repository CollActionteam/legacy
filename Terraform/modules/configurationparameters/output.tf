output "securestrings" {
  description = "The secure parameters that were created"
  value = aws_ssm_parameter.securestring_parameters
}

output "strings" {
  description = "The string parameters that were created"
  value = aws_ssm_parameter.string_parameters
}