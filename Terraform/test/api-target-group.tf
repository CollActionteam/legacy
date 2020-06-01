# For now, only the target group is under terraform, not the tasks themselves.
# Change that once we have an alternative to the testselect software (or change it so it works with Fargate)

# Target group for the load balancer
resource "aws_alb_target_group" "api-collaction" {
  name        = "api-${var.environment}-collaction"
  port        = 80
  protocol    = "HTTP"
  vpc_id      = module.vpc.default.id
  target_type = "instance"

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
resource "aws_lb_listener_rule" "api-collaction" {
  listener_arn = data.aws_alb_listener.api-collaction.arn

  action {
    type             = "forward"
    target_group_arn = aws_alb_target_group.api-collaction.arn
  }

  condition {
    host_header {
      values = [
        "test.collaction.org"
      ]
    }
  }
}

# For now, we keep the old test environment and just add a DNS entry 
# to it here. Needs to be replaced with a Fargate test environment in the future
resource "aws_route53_record" "api-collaction" {
  zone_id = data.aws_route53_zone.collaction.zone_id
  name    = "test.collaction.org"
  type    = "CNAME"
  ttl     = "300"
  records = [
    data.aws_alb.api-collaction.dns_name
  ]
}
