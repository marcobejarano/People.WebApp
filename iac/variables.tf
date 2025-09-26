variable "resource_group_name" {
  description = "Name of the Azure resource group"
  default     = "rg-aks-people-tf"
}

variable "location" {
  description = "Azure region for all resources"
  default     = "West US 2"
}
