provider "azurerm" {
  # version = ">=3.77.0"
  features {
    resource_group {
      prevent_deletion_if_contains_resources = false
    }
  }
}

terraform {
  required_providers {
    azapi = {
      source = "Azure/azapi"
    }
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "3.90.0"
    }
  }
}


terraform {
  # backend "azurerm" {
  # }
}

provider "azapi" {

}

terraform {
  required_version = ">= 0.12"
}