resource "aws_ecs_task_definition" "api-collaction" {
  family                   = "api-${var.environment}-collaction-task"
  execution_role_arn       = aws_iam_role.ecs_task_execution_role.arn
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = var.ecs_api_cpu
  memory                   = var.ecs_api_memory
  container_definitions    = <<DEFINITION
    [
      {
        "name": "api",
        "image": "collaction/backend:${var.imageversion}",
        "networkMode": "awsvpc",
        "portMappings": [
          {
            "containerPort": 5000,
            "hostPort": 5000
          }
        ],
        "logConfiguration": {
            "logDriver": "awslogs",
            "options": {
              "awslogs-group": "/ecs/${var.environment}/api-collaction",
              "awslogs-region": "${var.region}",
              "awslogs-stream-prefix": "ecs"
            }
        },
        "environment": [
          {
            "name": "ASPNETCORE_ENVIRONMENT",
            "value": "Production"
          },
          {
            "name": "ASPNETCORE_URLS",
            "value": "http://*:5000"
          }
        ],
        "secrets": [
          %{for param in var.ssm_securestrings}
          {
            "name": "${param.description}", 
            "valueFrom": "${param.arn}"
          },
          %{endfor}
          %{for param in var.ssm_strings}
          {
            "name": "${param.description}", 
            "valueFrom": "${param.arn}"
          },
          %{endfor}
          {
            "name": "DbHost",
            "valueFrom": "${var.ssm_dbhost.arn}"
          },
          {
            "name": "Db",
            "valueFrom": "${var.ssm_db.arn}"
          },
          {
            "name": "DbUser",
            "valueFrom": "${var.ssm_dbuser.arn}"
          },
          {
            "name": "DbPassword",
            "valueFrom": "${var.ssm_dbpassword.arn}"
          }
        ]
      }
    ]
    DEFINITION
  depends_on = [
    aws_cloudwatch_log_group.api-collaction
  ]
}

# Container output is logged to CloudWatch
resource "aws_cloudwatch_log_group" "api-collaction" {
  name              = "/ecs/${var.environment}/api-collaction"
  retention_in_days = 7
}

resource "aws_cloudwatch_log_stream" "api-collaction" {
  name           = "api-${var.environment}-collaction"
  log_group_name = aws_cloudwatch_log_group.api-collaction.name
}