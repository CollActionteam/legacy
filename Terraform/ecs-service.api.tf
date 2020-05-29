# The service running the task
resource "aws_ecs_service" "api-collaction" {
  name            = "api-${var.environment}-collaction"
  cluster         = aws_ecs_cluster.collaction.id
  task_definition = aws_ecs_task_definition.api-collaction.arn
  desired_count   = 1
  launch_type     = "FARGATE"

  network_configuration {
    security_groups = [
      aws_security_group.inbound-ecs.id,
      aws_security_group.outbound-ecs.id,
      aws_security_group.access_rds.id
    ]
    subnets          = data.aws_subnet_ids.default.ids
    assign_public_ip = true
  }

  load_balancer {
    target_group_arn = aws_alb_target_group.api-collaction.id
    container_name   = "api"
    container_port   = 5000
  }

  depends_on = [
    aws_alb_listener.api-collaction,
    aws_iam_role_policy_attachment.ecs_task_execution_role
  ]
}

# Target group for the load balancer
resource "aws_alb_target_group" "api-collaction" {
  name        = "api-${var.environment}-collaction"
  port        = 5000
  protocol    = "HTTP"
  vpc_id      = data.aws_vpc.default.id
  target_type = "ip"

  health_check {
    healthy_threshold   = 3
    interval            = 30
    protocol            = "HTTP"
    matcher             = 200
    timeout             = 3
    path                = "/health"
    unhealthy_threshold = 2
  }
}

resource "aws_security_group" "inbound-ecs" {
  name = "inbound-api-${var.environment}-collaction"
  description = "Allowed inbound traffic for ${var.environment} API containers"

  ingress {
    protocol  = "tcp"
    from_port = 5000
    to_port   = 5000

    # Incoming only from load balancers linked to this service
    security_groups = [
      aws_security_group.access-ecs.id
    ]
  }
}

resource "aws_security_group" "access-ecs" {
  name = "access-ecs-api-${var.environment}-collaction"
  description = "Provide access to ${var.environment} API containers"
}

resource "aws_security_group" "outbound-ecs" {
  name = "outbound-api-${var.environment}-collaction"
  description = "Allowed outbound traffic for ${var.environment} API containers"

  egress {
    # Outgoing access to all
    protocol    = "-1"
    from_port   = 0
    to_port     = 0
    cidr_blocks = ["0.0.0.0/0"]
  }
}

# Listener rules for load balancer
resource "aws_lb_listener_rule" "api-collaction" {
  listener_arn = aws_alb_listener.api-collaction.arn

  action {
    type             = "forward"
    target_group_arn = aws_alb_target_group.api-collaction.arn
  }

  condition {
    host_header {
      values = [
        local.hostname
      ]
    }
  }
}
