data "aws_alb" "api-collaction" {
  name = "api-collaction"
}

data "aws_alb_listener" "api-collaction" {
  load_balancer_arn = data.aws_alb.api-collaction.id
  port              = 443
}

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

  sg_rds_access = module.rds.inbound_security_group
  subnet_ids    = module.vpc.subnet_ids

  vpc               = module.vpc.default
  route53_zone_name = "collaction.org."
  hostname          = var.hostname
  alb               = data.aws_alb.api-collaction
  alb_listener      = data.aws_alb_listener.api-collaction
}
