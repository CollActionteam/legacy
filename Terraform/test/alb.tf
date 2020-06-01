data "aws_alb" "api-collaction" {
  name = "api-collaction"
}

data "aws_alb_listener" "api-collaction" {
  load_balancer_arn = data.aws_alb.api-collaction.id
  port              = 443
}
