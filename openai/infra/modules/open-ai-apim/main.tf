terraform {
  required_providers {
    azapi = {
      source  = "Azure/azapi"
      # version = "=0.4.0"
    }
  }
}

//INTEGRATE WITH APIM
data "azurerm_storage_account" "storage_account" {
  name                = var.api_storage_account_name
  resource_group_name = var.api_storage_account_resource_group
}

//load application insights
data "azurerm_application_insights" "app_insights" {
  name                = var.application_insights_name
  resource_group_name = var.application_insights_resource_group
}

data "azurerm_storage_account_blob_container_sas" "api_storage" {
  connection_string = data.azurerm_storage_account.storage_account.primary_connection_string
  container_name    = var.api_storage_account_container
  https_only        = true

  start  = formatdate("YYYY-MM-DD", timestamp())
  expiry = formatdate("YYYY-MM-DD", timeadd(timestamp(), "24h"))

  permissions {
    read   = true
    add    = false
    create = false
    write  = false
    delete = false
    list   = false
  }
}

data "azurerm_api_management" "apim_instance" {
  name                = var.api_management_name
  resource_group_name = var.api_management_resource_group
}

//loop through apis variable and create module apim-api
module "open_ai_api" {
  source                           = "../apim-api"
  api_name                         = var.open_ai_name
  api_service_url                  = "${var.open_ai_endpoint}openai"
  api_path                         = "openai"
  content_link                     = "https://${var.api_storage_account_name}.blob.core.windows.net/${var.api_storage_account_container}/${var.api_file_name}${data.azurerm_storage_account_blob_container_sas.api_storage.sas}"
  content_type                     = "openapi+json-link"   
  apim_name                        = data.azurerm_api_management.apim_instance.name
  apim_resource_group_name         = data.azurerm_api_management.apim_instance.resource_group_name
  rg                               = var.api_management_resource_group
  product_id                       = "Starter"
  ai_name                          = data.azurerm_application_insights.app_insights.name
  ai_resource_group_name           = data.azurerm_application_insights.app_insights.resource_group_name
  subscription_required            = true
}


resource "azurerm_key_vault_secret" "azure_open_ai_key" {
  count = var.save_key_to_key_vault ? 1 : 0
  name         = "${var.open_ai_name}-key"
  value        = var.open_ai_key
  key_vault_id = var.key_vault_resourd_id
}

//give APIM Managed Identity access to Key Vault secret specifically. Use RBAC for general access
resource "azurerm_role_assignment" "Api_kv_secret" {
  count                = var.save_key_to_key_vault ? 1 : 0
  scope                = azurerm_key_vault_secret.azure_open_ai_key[0].resource_versionless_id
  role_definition_name = "Key Vault Secrets User"
  principal_id         = data.azurerm_api_management.apim_instance.identity.0.principal_id
}


resource "azurerm_api_management_named_value" "open_ai_key" {
  name                = "${var.open_ai_name}"
  api_management_name = data.azurerm_api_management.apim_instance.name
  resource_group_name = data.azurerm_api_management.apim_instance.resource_group_name
  display_name        = "${var.open_ai_name}-key"
  value               = var.open_ai_key
  # value_from_key_vault {
  #   identity_client_id = data.azurerm_api_management.apim_instance.identity.0.principal_id
  #   secret_id          = azurerm_key_vault_secret.azure_open_ai_key[0].id
  # }
  secret              = true
  depends_on = [ azurerm_role_assignment.Api_kv_secret]
}

# resource "azapi_resource" "open_ai_api" {
#   type = "Microsoft.ApiManagement/service/namedValues@2023-05-01-preview"
#   name =  "${var.open_ai_name}-key"
#   parent_id =  data.azurerm_api_management.apim_instance.id
#   body = jsonencode({
#     properties = {
#       displayName = "${var.open_ai_name}-key"
#       keyVault = {
#         identityClientId = data.azurerm_api_management.apim_instance.identity.0.principal_id
#         secretIdentifier = azurerm_key_vault_secret.azure_open_ai_key[0].id
#       }
#       secret = true
#       # tags = [
#       #   "string"
#       # ]
#       //value = "string"
#     }
#   })
# }



resource "azurerm_api_management_api_policy" "api_policy" {
  api_name            = module.open_ai_api.api.name
  api_management_name = data.azurerm_api_management.apim_instance.name
  resource_group_name = data.azurerm_api_management.apim_instance.resource_group_name

  xml_content = <<XML
  <policies>
    <inbound>
        <base />
        <set-header name="api-key" exists-action="override">
            <value>{${var.open_ai_name}-key}</value>
        </set-header>
    </inbound>
    <backend>
        <base />
    </backend>
    <outbound>
        <base />
    </outbound>
    <on-error>
        <base />
    </on-error>
 </policies>
XML
}

# resource "azurerm_api_management_api_policy" "open_ai_auth" {
#   count               = var.deploy_auth == true ? 1 : 0
#   api_name            = module.open_ai_api.api.name
#   api_management_name = data.azurerm_api_management.apim_instance.name
#   resource_group_name = data.azurerm_api_management.apim_instance.resource_group_name

#   xml_content = <<XML
# <validate-jwt header-name="Authorization" failed-validation-httpcode="401" failed-validation-error-message="Unauthorized. Access token is missing or invalid.">
#     <openid-config url="https://login.microsoftonline.com/contoso.onmicrosoft.com/.well-known/openid-configuration" />
#     <audiences>
#         <audience>25eef6e4-c905-4a07-8eb4-0d08d5df8b3f</audience>
#     </audiences>
#     <required-claims>
#         <claim name="id" match="all">
#             <value>somevalue</value>
#         </claim>
#     </required-claims>
# </validate-jwt>
# XML
# }





