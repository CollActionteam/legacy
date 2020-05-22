# Load-balancer in the public subnets
resource "aws_alb" "collaction-api" {
  name            = "collaction-api"
  subnets         = data.aws_subnet_ids.default.ids
  security_groups = [aws_security_group.collaction-api.id]
  tags = {
    Application = "poa-profiles"
    Environment = "D"
  }
}

# The listener for this load balancer
resource "aws_alb_listener" "collaction-api" {
  load_balancer_arn = aws_alb.collaction-api.id
  port              = 443
  protocol          = "HTTPS"

  # By default, this load balancer will return a 404. 
  # Additional actions are added in the ecs-service resources.
  default_action {
    type = "fixed-response"

    fixed_response {
      content_type = "text/plain"
      message_body = "Not found"
      status_code  = "404"
    }
  }
}

# Load balancer protection
resource "aws_security_group" "collaction-api" {
  name        = "alb-collaction-api"
  description = "Allow inbound access to ALB collaction-api"
  vpc_id      = data.aws_vpc.default.id

  ingress {
    # Incoming access for http traffic
    protocol    = "tcp"
    from_port   = 443
    to_port     = 44301
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    # Outgoing access to all
    protocol    = "-1"
    from_port   = 0
    to_port     = 0
    cidr_blocks = ["0.0.0.0/0"]
  }
}
