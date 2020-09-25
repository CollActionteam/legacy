# Add hostname to the DNS 
data "aws_route53_zone" "zone" {
  name = var.route53_zone_name
}

# Add DNS entry to the front-end website on Netlify
resource "aws_route53_record" "domain" {
  zone_id = data.aws_route53_zone.zone.zone_id
  name    = var.domain
  type    = "CNAME"
  ttl     = "300"
  records = [
    var.alb.dns_name
  ]
}
