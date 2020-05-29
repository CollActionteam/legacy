module "fargate" {
  source = "../modules/fargate"

  region      = var.region
  cluster     = "collaction-${var.environment}"
  environment = var.environment

  imageversion   = var.imageversion
  ecs_api_cpu    = 256
  ecs_api_memory = 512

  ssm_securestrings = module.ssm.securestrings
  ssm_strings       = module.ssm.strings

  ssm_dbhost     = aws_ssm_parameter.DbHost
  ssm_db         = aws_ssm_parameter.Db
  ssm_dbuser     = aws_ssm_parameter.DbUser
  ssm_dbpassword = aws_ssm_parameter.DbPassword
}
