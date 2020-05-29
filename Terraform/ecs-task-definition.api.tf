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
              "awslogs-region": "${var.aws_region}",
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
          %{for param in var.securestring_parameters}
          {
            "name": "${replace(param, "/", ":")}", 
            "valueFrom": "arn:aws:ssm:${var.aws_region}:${data.aws_caller_identity.current.account_id}:parameter/collaction/${var.environment}/${param}"
          },
          %{endfor}
          %{for param in var.string_parameters}
          {
            "name": "${replace(param, "/", ":")}", 
            "valueFrom": "arn:aws:ssm:${var.aws_region}:${data.aws_caller_identity.current.account_id}:parameter/collaction/${var.environment}/${param}"
          },
          %{endfor}
          {
            "name": "DbHost",
            "valueFrom": "${aws_ssm_parameter.DbHost.arn}"
          },
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