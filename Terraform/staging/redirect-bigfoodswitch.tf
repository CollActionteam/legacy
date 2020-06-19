module "bigfoodswitch" {
  source = "../modules/redirectrule"

  alb               = data.aws_alb.api-collaction
  alb_listener      = data.aws_alb_listener.api-collaction
  route53_zone_name = "bigfoodswitch.org."
  domain            = "bigfoodswitch.org"
  redirect_to_path  = "/crowdactions/the-big-food-switch/201"
}
