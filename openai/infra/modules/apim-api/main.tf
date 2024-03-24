
data "azurerm_api_management_product" "tenant" {
  product_id          = var.product_id
  api_management_name = var.apim_name
  resource_group_name = var.apim_resource_group_name
}

data "azurerm_application_insights" "ai" {
  name                = var.ai_name
  resource_group_name = var.ai_resource_group_name
}

resource "azurerm_api_management_api" "api" {
  name                = var.api_name
  resource_group_name = var.apim_resource_group_name
  api_management_name = var.apim_name
  service_url         = var.api_service_url
  revision            = "1"
  display_name        = var.api_name
  path                = var.api_path
  protocols           = ["https", "http"]
  subscription_required = var.subscription_required

  import {
    content_format = var.content_type//"swagger-link-json"
    content_value  = var.content_link
  }
}

resource "azurerm_api_management_product_api" "product_api" {
  api_name            = azurerm_api_management_api.api.name
  product_id          = data.azurerm_api_management_product.tenant.product_id
  api_management_name = var.apim_name
  resource_group_name = var.apim_resource_group_name
}


resource "azurerm_api_management_logger" "api_logger" {
  name                = "${azurerm_api_management_api.api.name}-logger"
  api_management_name = var.apim_name
  resource_group_name = var.apim_resource_group_name
  resource_id         = data.azurerm_application_insights.ai.id

  application_insights {
    instrumentation_key = data.azurerm_application_insights.ai.instrumentation_key
  }
}

resource "azurerm_api_management_api_diagnostic" "api_diagnostic" {
  identifier               = "applicationinsights"
  resource_group_name      = var.apim_resource_group_name
  api_management_name      = var.apim_name
  api_name                 = azurerm_api_management_api.api.name
  api_management_logger_id = azurerm_api_management_logger.api_logger.id

  sampling_percentage       = 100.0
  always_log_errors         = true
  log_client_ip             = true
  verbosity                 = "verbose"
  http_correlation_protocol = "W3C"

  frontend_request {
    body_bytes = 32
    headers_to_log = [
      "content-type",
      "accept",
      "origin",
    ]
  }

  frontend_response {
    body_bytes = 32
    headers_to_log = [
      "content-type",
      "content-length",
      "origin",
    ]
  }

  backend_request {
    body_bytes = 32
    headers_to_log = [
      "content-type",
      "accept",
      "origin",
    ]
  }

  backend_response {
    body_bytes = 32
    headers_to_log = [
      "content-type",
      "content-length",
      "origin",
    ]
  }
}
