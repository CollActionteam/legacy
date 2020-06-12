resource "aws_ecs_cluster" "cluster" {
  name = "collaction-${var.environment}"
  capacity_providers = [var.capacity_provider]

  default_capacity_provider_strategy {
    capacity_provider = var.capacity_provider
  }
}
