
variable "app_prefix" {
  type = string
}
variable "location" {
  type = string
}

variable "dr_location" {
  description = "The location of the disaster recovery region"
  type        = string
  default     = "centralus"
}

variable "resource_group_name" {
  type = string
}

variable "virtual_network_name" {
  type = string
}

variable "private_link_subnet_name" {
  type = string
}

variable "dns_zone_resource_group" {
  type = string
}

variable "public_network_access_enabled" {
  type    = bool
  default = false
}

variable "private_link_enabled" {
  type    = bool
  default = true
}

variable "azure_aad_admin_group_ids" {
  type        = list(string)
  description = "Default group member"
}

variable "create_aks_federated_credentials" {
  type    = bool
  default = false
}

variable "aks_cluster_name" {
  type    = string
  default = null
}

variable "aks_cluster_resource_group_name" {
  type    = string
  default = null
}

variable "rag_demo_namespace" {
  type    = string
  default = null
}

variable "rag_demo_service_account" {
  type    = string
  default = null
}

variable "deploy_new_storage_account" {
  type    = bool
  default = false 
}

variable "existing_storage_account_name" {
  type    = string
  default = null
}

variable "use_existing_key_vault" {
  type    = bool
  default = true
  
}

variable "existing_key_vault_name" {
  type    = string
  default = null
}

variable "existing_key_vault_resource_group_name" {
  type    = string
  default = null
}

# apim integration

variable "integration_open_ai_apim" {
  type    = bool
  default = true
}


#variable for storage account connection string
variable "api_storage_account_container" {
   description = "The storage account container"
   type        = string
}

variable "api_storage_account_resource_group" {
   description = "The storage account resource group"
   type        = string
   default     = ""
}

variable "api_storage_account_name" {
   description = "The storage account name"
   type        = string
   default     = ""
}

variable "api_management_name" {
   description = "The API Management Name"
   type        = string
   default     = ""
}

variable "api_management_resource_group" {
   description = "The API Management Resource Group Name"
   type        = string
   default     = ""
}

variable "application_insights_name" {
   description = "The Application Insights Name"
   type        = string
   default     = ""
}

variable "application_insights_resource_group" {
   description = "The Application Insights Name"
   type        = string
   default     = ""
}

variable "api_auth_audience"{
    description = "Audience for the API Auth"
    type        = string
    default     = ""
}

variable "backed_app_client_id" {
    description = "Client ID for the backed app"
    type        = string 
    default     = ""
}

variable "tenant_id" {
    description = "Tenant ID for the backed app"
    type        = string 
    default     = ""
}

variable "tags" {
  description = "A mapping of tags to assign to the resource."
  default = {
    "appName" = "rag-demo"
  }
}
