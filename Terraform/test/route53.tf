# Add hostname to the DNS 
data "aws_route53_zone" "collaction" {
  name = var.route53_zone_name
}

# For now, we keep the old test environment and just add a DNS entry 
# to it here. Needs to be replaced with a Fargate test environment in the future
resource "aws_route53_record" "api-collaction" {
  zone_id = data.aws_route53_zone.collaction.zone_id
  name    = var.hostname
  type    = "CNAME"
  ttl     = "300"
  records = [
    data.aws_alb.api-collaction.dns_name
  ]
}


# Add DNS entry to the front-end website on Netlify (not atm)
# resource "aws_route53_record" "netlify" {
#   zone_id = data.aws_route53_zone.collaction.zone_id
#   name    = "staging.collaction.org"
#   type    = "CNAME"
#   ttl     = "300"
#   records = [
#     "staging-collaction.netlify.app"
#   ]
# }
