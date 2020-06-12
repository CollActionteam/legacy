terraform {
  backend "remote" {
    organization = "collaction"

    workspaces {
      name = "CollAction-staging"
    }
  }
}