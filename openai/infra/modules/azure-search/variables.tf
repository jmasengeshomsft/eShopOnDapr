
variable "location" {
  type = string
}

variable "resource_group_name" {
  type = string 
}

variable "name" {
  type = string
}

variable "partition_count" {
  type = number
  default = 1
}

variable "replica_count" {
  type = number
  default = 1
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

variable "public_network_access_enabled" {
  type = bool
  default = false
}

variable "tags" {
  type = map(string)
}