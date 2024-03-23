variable "namespace" {
  type = string
}

variable "service_account" {
  type = string 
}

variable "oidc_issuer_url" {
  type = string 
}

variable "identity_id" {
  type = string
}

variable "name" {
  type    = string
}

variable "resource_group_name" {
  type    = string
}


resource "azurerm_federated_identity_credential" "aks_fc" {
  name                = var.name
  resource_group_name = var.resource_group_name
  audience            = ["api://AzureADTokenExchange"]
  issuer              = var.oidc_issuer_url
  parent_id           = var.identity_id
  subject             = "system:serviceaccount:${var.namespace}:${var.service_account}"
}