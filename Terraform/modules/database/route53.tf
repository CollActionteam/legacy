# Add hostname to the DNS 
data "aws_route53_zone" "collaction" {
  name = var.route53_zone_name
}

resource "aws_route53_record" "db-collaction" {
  zone_id = data.aws_route53_zone.collaction.zone_id
  name    = var.rds_hostname
  type    = "CNAME"
  ttl     = "300"
  records = [
    aws_db_instance.collaction.address
  ]
}
