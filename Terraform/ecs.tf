resource "aws_ecs_cluster" "collaction" {
  name = "collaction-v2"
}

resource "aws_ecs_task_definition" "staging-api" {
  family                   = "ca-staging-api-task"
  execution_role_arn       = aws_iam_role.ecs_task_execution_role.arn
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = 256
  memory                   = 512
  container_definitions    = <<EOF
    [
      {
        "name": "ca-staging-api",
        "image": "collaction/backend:v2.0.0-beta1",
        "networkMode": "awsvpc",
        "portMappings": [
          {
            "containerPort": 44301,
            "hostPort": 44301
          }
        ],
        "logConfiguration": {
            "logDriver": "awslogs",
            "options": {
              "awslogs-group": "/ecs/staging/collaction-api",
              "awslogs-region": "${var.aws_region}",
              "awslogs-stream-prefix": "ecs"
            }
        }
      }
    ]
    EOF
  depends_on = [
    aws_cloudwatch_log_group.staging-collaction-api
  ]
  lifecycle {
    ignore_changes = [
      container_definitions
    ]
  }
}

# Container output is logged to CloudWatch
resource "aws_cloudwatch_log_group" "staging-collaction-api" {
  name              = "/ecs/staging/collaction-api"
  retention_in_days = 7
}

resource "aws_cloudwatch_log_stream" "staging-collaction-api" {
  name           = "staging-collaction-api"
  log_group_name = aws_cloudwatch_log_group.staging-collaction-api.name
}
