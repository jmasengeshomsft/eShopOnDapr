

resource "azurerm_search_service" "search" {
  name                = var.name
  identity {
    type = "SystemAssigned"
  }
  resource_group_name = var.resource_group_name
  location            = var.location
  sku                 = "standard"
  public_network_access_enabled = var.public_network_access_enabled
  partition_count     = var.partition_count
  replica_count       = var.replica_count
  hosting_mode        = "default"
  local_authentication_enabled = true
  authentication_failure_mode  = "http401WithBearerChallenge"
  semantic_search_sku = "free"
  tags = var.tags
}

resource "azurerm_private_endpoint" "search_private_endpoint" {
  count               = length(var.private_link_subnets)
  name                = "${azurerm_search_service.search.name}-${var.private_link_subnets[count.index].vnet_name}"
  location            = var.location
  resource_group_name = var.resource_group_name
  subnet_id           = var.private_link_subnets[count.index].id
  tags                = var.tags

  private_service_connection {
    name                           = "${azurerm_search_service.search.name}-${var.private_link_subnets[count.index].vnet_name}"
    private_connection_resource_id = azurerm_search_service.search.id
    is_manual_connection           = false
    subresource_names              = ["searchService"]
  }

   private_dns_zone_group {
    name = "searc-endpoint-zone"
    private_dns_zone_ids = [var.private_zone_id]
  }
}
