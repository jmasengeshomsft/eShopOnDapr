

resource "azurerm_cognitive_account" "account" {
  name                = "${var.name}-deployment"
  location            = var.location
  resource_group_name = var.resource_group_name
  kind                = var.kind
  public_network_access_enabled = var.public_network_access_enabled
  custom_subdomain_name = var.name

  sku_name = "S0"

  tags = var.tags
}

resource "azurerm_cognitive_deployment" "deployments" {
  for_each = { for deployment in var.deployments : deployment.name => deployment }
  name                 = each.value.name
  cognitive_account_id = azurerm_cognitive_account.account.id
  model {
    format  = each.value.model.format
    name    = each.value.model.name
    version = each.value.model.version
  }
  scale {
    type = each.value.scale.type
  }
}

resource "azurerm_private_endpoint" "cognitive_private_endpoint" {
  count               = length(var.private_link_subnets)
  name                = "${azurerm_cognitive_account.account.name}-${var.private_link_subnets[count.index].vnet_name}"
  location            = var.location
  resource_group_name = var.resource_group_name
  subnet_id           = var.private_link_subnets[count.index].id
  tags                = var.tags

  private_service_connection {
    name                           = "${azurerm_cognitive_account.account.name}-${var.private_link_subnets[count.index].vnet_name}"
    private_connection_resource_id = azurerm_cognitive_account.account.id
    is_manual_connection           = false
    subresource_names              = ["account"]
  }

   private_dns_zone_group {
    name = "account-endpoint-zone"
    private_dns_zone_ids = [var.private_zone_id]
  }
}