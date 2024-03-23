
variable "location" {
  type = string
}

variable "resource_group_name" {
  type = string 
}

variable "name" {
  type = string
}

variable "deployments" {
  type = list(object({
    name = string
    model = object({
      format = string
      name = string
      version = string
    })
    scale = object({
      type = string
    })
  }))
 default = []
}

variable "public_network_access_enabled" {
  type = bool
  default = false
}

variable "kind" {
  type = string
}

variable "private_link_subnets" {
  type = list(object({
    id = string
    vnet_name = string
  }))
  default = []
}

variable "private_zone_id" {
  type = string
  default = ""
}


variable "tags" {
  type = map(string)
}