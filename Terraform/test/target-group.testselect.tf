# Target group for the load balancer
resource "aws_alb_target_group" "testselect" {
  name        = "testselect"
  port        = 5000
  protocol    = "HTTP"
  vpc_id      = module.vpc.default.id
  target_type = "ip"

  health_check {
    healthy_threshold   = 3
    interval            = 30
    protocol            = "HTTP"
    matcher             = "200-399"
    timeout             = 3
    path                = "/health"
    unhealthy_threshold = 2
  }
}

# Listener rules for load balancer
resource "aws_lb_listener_rule" "testselect" {
  listener_arn = data.aws_alb_listener.api-collaction.arn

  action {
    type             = "forward"
    target_group_arn = aws_alb_target_group.testselect.arn
  }

  condition {
    host_header {
      values = [
        "testselect.collaction.org"
      ]
    }
  }
}

# For now, we keep the old test environment and just add a DNS entry 
# to it here. Needs to be replaced with a Fargate test environment in the future
resource "aws_route53_record" "testselect" {
  zone_id = data.aws_route53_zone.collaction.zone_id
  name    = "testselect.collaction.org"
  type    = "CNAME"
  ttl     = "300"
  records = [
    data.aws_alb.api-collaction.dns_name
  ]
}
