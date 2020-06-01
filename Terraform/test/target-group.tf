data "aws_alb" "api-collaction" {
  name = "api-collaction"
}

data "aws_alb_listener" "api-collaction" {
  load_balancer_arn = data.aws_alb.api-collaction.id
  port              = 443
}

# Target group for the load balancer
resource "aws_alb_target_group" "api-collaction" {
  name        = "api-${var.environment}-collaction"
  port        = 5000
  protocol    = "HTTP"
  vpc_id      = module.vpc.default.id
  target_type = "ip"

  health_check {
    healthy_threshold   = 3
    interval            = 300
    protocol            = "HTTP"
    matcher             = "200-399"
    timeout             = 120
    path                = "/health"
    unhealthy_threshold = 10
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
        var.hostname
      ]
    }
  }
}
