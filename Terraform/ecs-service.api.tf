resource "aws_ecs_task_definition" "api" {
  family                   = "ca-${var.environment}-api-task"
  execution_role_arn       = aws_iam_role.ecs_task_execution_role.arn
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = 256
  memory                   = 512
  container_definitions    = <<DEFINITION
    [
      {
        "name": "ca-${var.environment}-api",
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
              "awslogs-group": "/ecs/${var.environment}/collaction-api",
              "awslogs-region": "${var.aws_region}",
              "awslogs-stream-prefix": "ecs"
            }
        },
        "secrets": [
          %{ for param in var.securestring_parameters }
          {
            "name": "${param}",
            "valueFrom": "arn:aws:ssm:${var.aws_region}:${data.aws_caller_identity.current.account_id}:parameter/collaction/${var.environment}/${param}"
          },
          %{ endfor }
          %{ for param in var.string_parameters }
          {
            "name": "${param}",
            "valueFrom": "arn:aws:ssm:${var.aws_region}:${data.aws_caller_identity.current.account_id}:parameter/collaction/${var.environment}/${param}"
          },
          %{ endfor }
          {
            "name": "${aws_ssm_parameter.dummy_final.name}",
            "valueFrom": "${aws_ssm_parameter.dummy_final.arn}"
          }
        ]
      }
    ]
    DEFINITION
  depends_on = [
    aws_cloudwatch_log_group.collaction-api
  ]
  lifecycle {
    ignore_changes = [
      container_definitions
    ]
  }
}

# Container output is logged to CloudWatch
resource "aws_cloudwatch_log_group" "collaction-api" {
  name              = "/ecs/${var.environment}/collaction-api"
  retention_in_days = 7
}

resource "aws_cloudwatch_log_stream" "collaction-api" {
  name           = "${var.environment}-collaction-api"
  log_group_name = aws_cloudwatch_log_group.collaction-api.name
}
