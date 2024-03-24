// API
variable "api_name" {
    description = "The API Name"
    type        = string
}

variable "api_service_url" {
    description = "The API Service Url"
    type        = string
}

variable "api_path" {
    description = "The API path"
    type        = string
}

variable "content_link" {
    description = "The API content link"
    type        = string
}

variable "content_type" {
    description = "The API content Type"
    type        = string
}

variable "apim_name" {
    description = "The API Management Name"
    type        = string
}

variable "apim_resource_group_name" {
    description = "The API Management Resource Group Name"
    type        = string
}

variable "product_id" {
    description = "The Product Id"
    type        = string
}

variable "ai_name" {
    description = "The AI Id"
    type        = string
}

variable "ai_resource_group_name" {
    description = "The AI Resource Group Name"
    type        = string
}

variable "rg" {
    description = "The tenant resource group"
    type        = string
}

variable "subscription_required" {
    description = "The subscription required"
    type        = bool
    default     = true
}