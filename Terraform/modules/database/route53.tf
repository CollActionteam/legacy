# Add hostname to the DNS 
data "aws_route53_zone" "collaction" {
  name = var.route53_zone_name
}

resource "aws_route53_record" "api-collaction" {
  zone_id = data.aws_route53_zone.collaction.zone_id
  name    = aws_db_instance.collaction.address
  type    = "CNAME"
  ttl     = "300"
  records = [
    var.rds_hostname
  ]
}
