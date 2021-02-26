# The service running the task
resource "aws_ecs_service" "testselect" {
  name            = "testselect-collaction"
  cluster         = aws_ecs_cluster.cluster.id
  task_definition = aws_ecs_task_definition.testselect.arn
  desired_count   = var.testselect_desired_count

  capacity_provider_strategy {
    capacity_provider = var.capacity_provider
    weight            = 100
  }

  network_configuration {
    security_groups = [
      aws_security_group.inbound-ecs.id,  # Allow traffic into the container
      aws_security_group.outbound-ecs.id, # Allow traffic out of the container
    ]
    subnets          = module.vpc.subnet_ids.ids
    assign_public_ip = true
  }

  load_balancer {
    target_group_arn = aws_alb_target_group.testselect.id
    container_name   = "testselect"
    container_port   = 5000
  }

  depends_on = [
    data.aws_alb_listener.api-collaction,
    aws_iam_role_policy_attachment.ecs_task_execution_role
  ]
}
