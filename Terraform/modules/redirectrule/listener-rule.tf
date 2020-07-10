# Listener rules for load balancer
resource "aws_lb_listener_rule" "redirect_rule" {
  listener_arn = var.alb_listener.arn

  action {
    type = "redirect"

    redirect {
      status_code = "HTTP_302"
      port        = "443"
      protocol    = "HTTPS"
      host        = var.redirect_to_host
      path        = var.redirect_to_path
    }
  }

  condition {
    host_header {
      values = [
        var.domain
      ]
    }
  }
}
