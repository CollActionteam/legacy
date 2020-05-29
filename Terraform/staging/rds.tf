module "rds" {
  source = "../modules/database"

  identifier = "collaction-staging"
  instance_class = "db.t2.micro" 
  allocated_storage = 20
}
