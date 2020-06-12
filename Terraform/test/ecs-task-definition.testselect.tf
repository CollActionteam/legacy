resource "aws_ecs_task_definition" "testselect" {
  family                   = "testselect-task"
  execution_role_arn       = aws_iam_role.ecs_task_execution_role.arn
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = 256
  memory                   = 512
  container_definitions    = <<DEFINITION
    [
      {
        "name": "testselect",
        "image": "collaction/testselect:latest",
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
              "awslogs-group": "/ecs/${var.environment}/testselect",
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
          {
            "name": "AwsSecretAccessKey",
            "valueFrom": "${aws_ssm_parameter.AwsSecretAccessKey.arn}"
          },
          {
            "name": "AwsSecretAccessKeyID",
            "valueFrom": "${aws_ssm_parameter.AwsSecretAccessKeyID.arn}"
          },
          {
            "name": "Username",
            "valueFrom": "${aws_ssm_parameter.Username.arn}"
          },
          {
            "name": "Password",
            "valueFrom": "${aws_ssm_parameter.Password.arn}"
          }
        ]
      }
    ]
    DEFINITION
  depends_on = [
    aws_cloudwatch_log_group.testselect
  ]
}

# Container output is logged to CloudWatch
resource "aws_cloudwatch_log_group" "testselect" {
  name              = "/ecs/${var.environment}/testselect"
  retention_in_days = 7
}

resource "aws_cloudwatch_log_stream" "testselect" {
  name           = "api-${var.environment}-testselect"
  log_group_name = aws_cloudwatch_log_group.testselect.name
}