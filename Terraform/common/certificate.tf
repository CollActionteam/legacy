data "aws_acm_certificate" "collaction" {
  domain      = "*.collaction.org"
  statuses    = ["ISSUED"]
  most_recent = true
}