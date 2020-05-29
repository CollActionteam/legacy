# The service running the task
resource "aws_ecs_service" "api-collaction" {
  name            = "api-${var.environment}-collaction"
  cluster         = aws_ecs_cluster.cluster.id
  task_definition = aws_ecs_task_definition.api-collaction.arn
  desired_count   = var.api_desired_count
  launch_type     = "FARGATE"

  network_configuration {
    security_groups = [
      aws_security_group.inbound-ecs.id,  # Allow traffic into the container
      aws_security_group.outbound-ecs.id, # Allow traffic out of the container
      var.sg_rds_access.id                # Allow access to the database
    ]
    subnets          = var.subnet_ids.ids
    assign_public_ip = true
  }

  # load_balancer {
  #   target_group_arn = aws_alb_target_group.api-collaction.id
  #   container_name   = "api"
  #   container_port   = 5000
  # }

  depends_on = [
    # var.alb_listener,
    aws_iam_role_policy_attachment.ecs_task_execution_role
  ]
}

# Allow traffic out of the container
resource "aws_security_group" "outbound-ecs" {
  name        = "outbound-api-${var.environment}-collaction"
  description = "Allowed outbound traffic for ${var.environment} API containers"

  egress {
    # Outgoing access to all
    protocol    = "-1"
    from_port   = 0
    to_port     = 0
    cidr_blocks = ["0.0.0.0/0"]
  }
}


resource "aws_security_group" "inbound-ecs" {
  name        = "inbound-api-${var.environment}-collaction"
  description = "Allowed inbound traffic for ${var.environment} API containers"

  ingress {
    protocol  = "tcp"
    from_port = 5000
    to_port   = 5000

    security_groups = [
      data.aws_security_group.outbound-alb.id # Outbound ALB traffic is allowed into the container
    ]
  }
}

# Security group that allows access from the ALB is created in the commons section, with the ALB.
data "aws_security_group" "outbound-alb" {
  name = "outbound-alb"
}
