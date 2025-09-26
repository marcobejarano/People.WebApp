terraform {
  backend "azurerm" {
    resource_group_name   = "rg-aks-people"
    storage_account_name  = "peoplewebappstore"
    container_name        = "terraform"
    key                   = "terraform.tfstate"
  }
}
