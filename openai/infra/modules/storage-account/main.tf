resource "azurerm_storage_account" "account" {
  name                            = var.storage_account_name
  resource_group_name             = var.resource_group_name
  location                        = var.location
  account_tier                    = "Standard"
  account_replication_type        = "GRS"
  account_kind                    = "StorageV2"
  allow_nested_items_to_be_public = false
  # nfsv3_enabled                   = true
  # is_hns_enabled                  = true

  identity {
    type  = "SystemAssigned"
  }

  network_rules {
    default_action             = "Deny"
  }

  tags = var.tags
}

resource "azurerm_private_endpoint" "blob_private_endpoint" {
  count               = length(var.private_link_subnets)
  name                = "${azurerm_storage_account.account.name}-${var.private_link_subnets[count.index].vnet_name}-blob"
  location            = var.location
  resource_group_name = var.resource_group_name
  subnet_id           = var.private_link_subnets[count.index].id
  tags                = var.tags

  private_service_connection {
    name                           = "${azurerm_storage_account.account.name}-${var.private_link_subnets[count.index].vnet_name}"
    private_connection_resource_id = azurerm_storage_account.account.id
    is_manual_connection           = false
    subresource_names              = ["blob"]
  }

   private_dns_zone_group {
    name = "sa-ep-blob"
    private_dns_zone_ids = [var.blob_private_zone_id]
  }
}

resource "azurerm_private_endpoint" "file_private_endpoint" {
  count               = length(var.private_link_subnets)
  name                = "${azurerm_storage_account.account.name}-${var.private_link_subnets[count.index].vnet_name}-file"
  location            = var.location
  resource_group_name = var.resource_group_name
  subnet_id           = var.private_link_subnets[count.index].id
  tags                = var.tags

  private_service_connection {
    name                           = "${azurerm_storage_account.account.name}-${var.private_link_subnets[count.index].vnet_name}"
    private_connection_resource_id = azurerm_storage_account.account.id
    is_manual_connection           = false
    subresource_names              = ["file"]
  }

   private_dns_zone_group {
    name = "sa-ep-file"
    private_dns_zone_ids = [var.file_private_zone_id]
  }
}