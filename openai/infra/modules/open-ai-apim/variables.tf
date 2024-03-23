#create all variables used in main.tf file including application insights
variable "api_management_name" {
  description = "Name of the API Management instance"
  type        = string
}

variable "api_management_resource_group" {
  description = "Name of the resource group where the API Management instance is located"
  type        = string
}

variable "api_storage_account_name" {
  description = "Name of the storage account where the API file is located"
  type        = string
}

variable "api_storage_account_resource_group" {
  description = "Name of the resource group where the storage account is located"
  type        = string
}


variable "api_storage_account_container" {
  description = "Name of the storage account container where the API file is located"
  type        = string
}

variable "api_file_name" {
  description = "Name of the API file in the storage account container"
  type        = string
}

variable "application_insights_name" {
  description = "Name of the Application Insights instance"
  type        = string
}

variable "open_ai_name" {
  description = "Name of the Open AI API"
  type        = string 
}

variable "open_ai_endpoint" {
  description = "Endpoint of the Open AI API"
  type        = string  
}

variable "open_ai_key" {
  description = "Key for the Open AI API"
  type        = string  
}
# make this variable optional
variable "api_auth_audience" {
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

variable "deploy_auth" {
  description = "Deploy auth policy"
  type        = bool
  default     = false
}

variable "save_key_to_key_vault" {
  description = "Use existing key vault"
  type        = bool
  default     = false
}

variable "key_vault_resourd_id" {
  description = "Id of the key vault"
  type        = string
  default     = ""  
}

variable "openai_api_key" {
  description = "Key for the Open AI API"
  type        = string
  default     = ""  
} 
# variable "key_vault_resource_group" {
#   description = "Name of the resource group where the key vault is located"
#   type        = string
#   default     = ""  
# }

variable "application_insights_resource_group" {
  description = "Name of the resource group where the Application Insights instance is located"
  type        = string
}







