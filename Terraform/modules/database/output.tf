output "inbound_security_group" {
  value       = aws_security_group.access_rds
  description = "The security group that was created to allow inbound access to this database"
}

output "address" {
  value       = aws_db_instance.collaction.address
  description = "The address of the created database instance"
}

output "name" {
  value       = aws_db_instance.collaction.name
  description = "The name of the database that was created"
}

output "username" {
  value       = aws_db_instance.collaction.username
  description = "The username that was created to access this database"
}

output "password" {
  value       = random_password.db_password.result
  description = "The password that was created to access this database"
}
