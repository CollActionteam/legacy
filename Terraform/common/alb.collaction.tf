# Load-balancer in the public subnets
# Note: this load balancer services all environments. Traffic is sent to test, staging or production containers by listener rules
# defined with the ecs-services definitions.
resource "aws_alb" "api-collaction" {
  name            = "api-collaction"
  subnets         = module.vpc.subnet_ids.ids
  security_groups = [
    aws_security_group.inbound-alb.id,
    aws_security_group.outbound-alb.id
  ]
}

# The listener for this load balancer
resource "aws_alb_listener" "api-collaction" {
  load_balancer_arn = aws_alb.api-collaction.id
  port              = 443
  protocol          = "HTTPS"
  certificate_arn   = data.aws_acm_certificate.collaction.arn
  ssl_policy        = "ELBSecurityPolicy-FS-2018-06"

  # By default, this load balancer will return a 404. 
  # Additional actions are added in the ecs-service resources.
  default_action {
    type = "fixed-response"

    fixed_response {
      content_type = "text/plain"
      message_body = "Not found"
      status_code  = "404"
    }
  }
}

# Load balancer protection
resource "aws_security_group" "inbound-alb" {
  name        = "inbound-alb"
  description = "Allow inbound access from ALB"
  vpc_id      = module.vpc.default.id

  ingress {
    # Incoming access for https traffic
    protocol    = "tcp"
    from_port   = 443
    to_port     = 443
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    # Outgoing traffic 
    protocol    = "tcp"
    from_port   = -1
    to_port     = -1
    cidr_blocks = ["0.0.0.0/0"]
  }
}

resource "aws_security_group" "outbound-alb" {
  name = "outbound-alb"
  description = "Provide access for the ALB"
}

# data "aws_route53_zone" "collaction" {
#   name = "collaction.org."
# }

# resource "aws_route53_record" "api-collaction" {
#   zone_id = data.aws_route53_zone.collaction.zone_id
#   name    = local.hostname
#   type    = "CNAME"
#   ttl     = "300"
#   records = [aws_alb.api-collaction.dns_name]
# }
