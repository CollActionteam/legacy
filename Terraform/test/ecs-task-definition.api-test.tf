resource "aws_ecs_task_definition" "api-collaction" {
  family                   = "api-${var.environment}-collaction-task"
  execution_role_arn       = aws_iam_role.ecs_task_execution_role.arn
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = 256
  memory                   = 512
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
            "value": "Staging"
          },
          {
            "name": "ASPNETCORE_URLS",
            "value": "http://*:5000"
          },
          {
            "name": "DbHost",
            "value": "localhost"
          }
        ],
        "secrets": [
          %{for param in module.ssm.securestrings}
          {
            "name": "${param.description}", 
            "valueFrom": "${param.arn}"
          },
          %{endfor}
          %{for param in module.ssm.strings}
          {
            "name": "${param.description}", 
            "valueFrom": "${param.arn}"
          },
          %{endfor}
          {
            "name": "Db",
            "valueFrom": "${aws_ssm_parameter.Db.arn}"
          },
          {
            "name": "DbUser",
            "valueFrom": "${aws_ssm_parameter.DbUser.arn}"
          },
          {
            "name": "DbPassword",
            "valueFrom": "${aws_ssm_parameter.DbPassword.arn}"
          }
        ]
      },
      {
        "name": "db",
        "image": "postgres:12-alpine",
        "networkMode": "awsvpc",
        "portMappings": [
          {
            "containerPort": 5432,
            "hostPort": 5432
          }
        ],
        "secrets": [
          {
            "name": "POSTGRES_DB",
            "valueFrom": "${aws_ssm_parameter.Db.arn}"
          },
          {
            "name": "POSTGRES_USER",
            "valueFrom": "${aws_ssm_parameter.DbUser.arn}"
          },
          {
            "name": "POSTGRES_PASSWORD",
            "valueFrom": "${aws_ssm_parameter.DbPassword.arn}"
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