data "aws_alb" "api-collaction" {
  name = "api-collaction"
}

data "aws_alb_listener" "api-collaction" {
  load_balancer_arn = data.aws_alb.api-collaction.id
  port              = 443
}

module "fargate" {
  source = "../modules/fargate"

  region            = var.region
  cluster           = "collaction-${var.environment}"
  capacity_provider = "FARGATE_SPOT"
  environment       = var.environment

  imageversion      = var.imageversion
  api_cpu           = var.api_cpu
  api_memory        = var.api_memory
  api_desired_count = var.api_desired_count

  ssm_securestrings = module.ssm.securestrings
  ssm_strings       = module.ssm.strings

  ssm_dbhost     = aws_ssm_parameter.DbHost
  ssm_db         = aws_ssm_parameter.Db
  ssm_dbuser     = aws_ssm_parameter.DbUser
  ssm_dbpassword = aws_ssm_parameter.DbPassword

  sg_rds_access = module.rds.inbound_security_group
  subnet_ids    = module.vpc.subnet_ids

  vpc               = module.vpc.default
  route53_zone_name = var.route53_zone_name
  hostname          = var.hostname
  alb               = data.aws_alb.api-collaction
  alb_listener      = data.aws_alb_listener.api-collaction
}
