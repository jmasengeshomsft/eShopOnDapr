variable "storage_account_name" {
    description = "The account name"
}

variable "resource_group_name" {
    description = "The name of the resource group"
}

variable "location" {
    description = "Location"
}

variable "private_link_subnets" {
  type = list(object({
    id = string
    vnet_name = string
  }))
  default = []
}

variable "blob_private_zone_id" {
  type = string
  default = ""
}

variable "file_private_zone_id" {
  type = string
  default = ""
}

variable "tags" {
  description = "A mapping of tags to assign to the resource."
  default     =  {
  }
}



