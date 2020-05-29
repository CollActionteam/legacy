locals {
  hostname = var.environment != "production" ? "api-${var.environment}.collaction.org" : "api.collaction.org"
}
