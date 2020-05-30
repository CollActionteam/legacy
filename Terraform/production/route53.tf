# Add hostname to the DNS 
data "aws_route53_zone" "collaction" {
  name = var.route53_zone_name
}

# Add DNS entry to the front-end website on Netlify
resource "aws_route53_record" "netlify" {
  zone_id = data.aws_route53_zone.collaction.zone_id
  name    = "www.collaction.org"
  type    = "CNAME"
  ttl     = "300"
  records = [
    "prod-collaction.netlify.app"
  ]
}
