resource "aws_ecs_cluster" "cluster" {
  name = var.cluster
  capacity_providers = [var.capacity_provider]

  default_capacity_provider_strategy {
    capacity_provider = var.capacity_provider
  }
}
