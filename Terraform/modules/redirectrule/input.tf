variable "alb" {
  description = "The ALB for this environment"
}

variable "alb_listener" {
  description = "The ALB listener in which to create the redirect rule"
}

variable "domain" {
  description = "The custom domain forward rule to set up"
}

variable "redirect_to_host" {
  default     = "www.collaction.org"
  description = "The new host to redirect the domain to. Defaults to www.collaction.org"
}
variable "redirect_to_path" {
  default     = "/"
  description = "The path to redirect to. Defaults to /"
}

variable "route53_zone_name" {
  description = "The zone in which to create a hostname to this environment. If not specifying, then the domain won't be registered"
}
