module "slowfashionseason" {
  source = "../modules/redirectrule"

  alb               = aws_alb.api-collaction
  alb_listener      = aws_alb_listener.api-collaction
  route53_zone_name = "slowfashionseason.org."
  domain            = "www.slowfashionseason.org"
  redirect_to_path  = "/crowdactions/slow-fashion-season-2020/174"
}