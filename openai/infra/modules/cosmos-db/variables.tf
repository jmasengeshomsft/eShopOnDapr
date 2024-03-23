variable "account_name" {
  description = "The name of the CosmosDB account"
  type        = string
}   

variable "resource_group_name" {
  description = "The name of the resource group in which to create the CosmosDB account"
  type        = string
}

variable "location" {
  description = "The location of the resource group in which to create the CosmosDB account"
  type        = string
}

variable "private_link_subnets" {
  description = "The subnets to which the CosmosDB account should be connected"
  type        = list(object({
    id        = string
    vnet_name = string
  }))
}

variable "geo_location" {
  description = "The location of the CosmosDB account"
  type        = string
}

variable "private_zone_id" {
  description = "The ID of the private DNS zone to which the CosmosDB account should be connected"
  type        = string
}

variable "tags" {
  description = "A mapping of tags to assign to the resource"
  type        = map(string)
  default     = {}
  
}