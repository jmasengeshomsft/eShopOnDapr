resource "azurerm_cosmosdb_account" "db" {
  name                = var.account_name
  location            = var.location
  resource_group_name = var.resource_group_name
  offer_type          = "Standard"
  kind                = "GlobalDocumentDB"

  enable_automatic_failover = false

#   public_network_access_enabled = false
  is_virtual_network_filter_enabled = true

  //add a filter for an IP address
  ip_range_filter = "98.52.113.131"


#   capabilities {
#     name = "EnableAggregationPipeline"
#   }

#   capabilities {
#     name = "mongoEnableDocLevelTTL"
#   }

#   capabilities {
#     name = "MongoDBv3.4"
#   }

#   capabilities {
#     name = "EnableNoSQL"
#   }

  consistency_policy {
    consistency_level       = "BoundedStaleness"
    max_interval_in_seconds = 300
    max_staleness_prefix    = 100000
  }

  geo_location {
    location          = var.location
    failover_priority = 0
  }

#   geo_location {
#     location          = var.geo_location
#     failover_priority = 1
#   }

  identity {
    type = "SystemAssigned"
  }

  tags = var.tags
}


resource "azurerm_private_endpoint" "cognitive_private_endpoint" {
  count               = length(var.private_link_subnets)
  name                = "${azurerm_cosmosdb_account.db.name}-${var.private_link_subnets[count.index].vnet_name}"
  location            = var.location
  resource_group_name = var.resource_group_name
  subnet_id           = var.private_link_subnets[count.index].id
  tags                = var.tags

  private_service_connection {
    name                           = "${azurerm_cosmosdb_account.db.name}-${var.private_link_subnets[count.index].vnet_name}"
    private_connection_resource_id = azurerm_cosmosdb_account.db.id
    is_manual_connection           = false
    subresource_names              = ["Sql"]
  }

   private_dns_zone_group {
    name = "account-endpoint-zone"
    private_dns_zone_ids = [var.private_zone_id]
  }
}
