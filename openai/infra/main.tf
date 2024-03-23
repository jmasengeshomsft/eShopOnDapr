//use the module from the ./module folder for azure open ai server
locals {
  ai_service_name      = "${var.app_prefix}-${var.location}-ai-service"
  form_recognizer_name = "${var.app_prefix}-${var.location}-form-recognizer"
  azure_search_name    = "${var.app_prefix}-${var.location}-azure-search"
  storage_account_name = replace("${var.app_prefix}${var.location}storage99", "-", "")
  rag_identity_name    = "${var.app_prefix}-${var.location}-identity"
  cosmos_db_account_name = "${var.app_prefix}-${var.location}-cosmos-db"
  ai_identities        = concat(var.azure_aad_admin_group_ids, [azurerm_user_assigned_identity.user_identity.principal_id])
 // aks_oidc_issuer_url  = var.create_aks_federated_credentials ? data.azurerm_kubernetes_cluster.aks.oidc_issuer_url : null
 private_endpoint_subnets = var.private_link_enabled ? [
    {
      id        = data.azurerm_subnet.private_link_subnet.id
      vnet_name = var.virtual_network_name
    }
  ] : []
}

//azure private link subnet
data "azurerm_subnet" "private_link_subnet" {
  name                 = var.private_link_subnet_name
  virtual_network_name = var.virtual_network_name
  resource_group_name  = var.resource_group_name
}

//get open ai service dns zone id
data "azurerm_private_dns_zone" "ai_service" {
  count               = var.private_link_enabled ? 1 : 0
  name                = "privatelink.openai.azure.com"
  resource_group_name = var.dns_zone_resource_group
}

//get search private dns zone id 
data "azurerm_private_dns_zone" "search" {
  count               = var.private_link_enabled ? 1 : 0
  name                = "privatelink.search.windows.net"
  resource_group_name = var.dns_zone_resource_group
}

//get cognitive service private dns zone id
data "azurerm_private_dns_zone" "cognitive_service" {
  count               = var.private_link_enabled ? 1 : 0
  name                = "privatelink.cognitiveservices.azure.com"
  resource_group_name = var.dns_zone_resource_group
}

//get private dns zone id for the azure search storage account
data "azurerm_private_dns_zone" "storage_account" {
  count               = var.private_link_enabled ? 1 : 0
  name                = "privatelink.blob.core.windows.net"
  resource_group_name = var.dns_zone_resource_group
}

//get private dns zone id for the azure search file storage account
data "azurerm_private_dns_zone" "file_storage_account" {
  count               = var.private_link_enabled ? 1 : 0
  name                = "privatelink.file.core.windows.net"
  resource_group_name = var.dns_zone_resource_group
}

//Get mongodb private dns zone id
data "azurerm_private_dns_zone" "mongodb" {
  count               = var.private_link_enabled ? 1 : 0
  name                = "privatelink.documents.azure.com"
  resource_group_name = var.dns_zone_resource_group
}

//azure managed identity using the azurerm provider
resource "azurerm_user_assigned_identity" "user_identity" {
  name                = local.rag_identity_name
  location            = var.location
  resource_group_name = var.resource_group_name
}

//load aks cluster if create_aks_federated_credentials is true
data "azurerm_kubernetes_cluster" "aks" {
  count               = var.create_aks_federated_credentials ? 1 : 0
  name                = var.aks_cluster_name
  resource_group_name = var.aks_cluster_resource_group_name
}

//create a federated credentials for the azure managed identity
module  "aks_fc" {
  count               = var.create_aks_federated_credentials ? 1 : 0
  source              = "../../../azure-infrastructure/modules/identity-federated-credentials"
  name                = local.rag_identity_name
  resource_group_name = var.resource_group_name
  oidc_issuer_url     = data.azurerm_kubernetes_cluster.aks[0].oidc_issuer_url
  identity_id         = azurerm_user_assigned_identity.user_identity.id
  namespace           = var.rag_demo_namespace
  service_account     = var.rag_demo_service_account
}

//azure open ai service
module "azure-open-ai" {
  source                        = "../../../azure-infrastructure/modules/cognitive-service"
  location                      = var.location
  resource_group_name           = var.resource_group_name
  public_network_access_enabled = var.public_network_access_enabled
  name                          = local.ai_service_name
  kind                          = "OpenAI"
  deployments = [
    {
      name = "chat"
      model = {
        format  = "OpenAI"
        name    = "gpt-35-turbo"
        version = "0613"
      }
      scale = {
        type = "Standard"
      }
    },
    {
      name = "gpt-35-turbo-instruct"
      model = {
        format  = "OpenAI"
        name    = "gpt-35-turbo-instruct"
        version = "0914"
      }
      scale = {
        type = "Standard"
      }
    },
    {
      name = "embedding"
      model = {
        format  = "OpenAI"
        name    = "text-embedding-ada-002"
        version = "2"
      }
      scale = {
        type = "Standard"
      }
    }
  ]
  private_link_subnets = local.private_endpoint_subnets
  private_zone_id = var.private_link_enabled ? data.azurerm_private_dns_zone.ai_service[0].id : null
  tags            = var.tags
}

//integrate with apim using the module open-ai-apim
module "open-ai-apim" {
  count                               = var.integration_open_ai_apim ? 1 : 0
  source                              = "../../../azure-infrastructure/modules/open-ai-apim"
  api_management_name                 = var.api_management_name
  api_management_resource_group       = var.api_management_resource_group
  api_storage_account_name            = var.api_storage_account_name
  api_storage_account_resource_group  = var.api_storage_account_resource_group
  api_storage_account_container       = var.api_storage_account_container
  api_file_name                       = "open-ai-sample-swagger.json"
  open_ai_endpoint                    = module.azure-open-ai.account.endpoint
  open_ai_key                         = module.azure-open-ai.account.primary_access_key
  open_ai_name                        = local.ai_service_name
  application_insights_name           = var.application_insights_name
  application_insights_resource_group = var.application_insights_resource_group
  deploy_auth                         = true
  save_key_to_key_vault               = true
  key_vault_resourd_id                = data.azurerm_key_vault.existing_key_vault[0].id
  openai_api_key                      = module.azure-open-ai.account.primary_access_key
  api_auth_audience                   = var.api_auth_audience
  backed_app_client_id                = var.backed_app_client_id
  tenant_id                           = var.tenant_id
}


//give role to ai_identities to access the azure open ai service. The role id is 5e0bd9bd-7b93-4f28-af87-19fc36ad61bd
resource "azurerm_role_assignment" "ai_service_role_assignment" {
  count                = length(local.ai_identities)
  scope                = module.azure-open-ai.account.id
  role_definition_name = "Cognitive Services OpenAI User"
  principal_id         = local.ai_identities[count.index]
}

//azure form recognizer
module "azure-form-recognizer" {
  source                        = "../../../azure-infrastructure/modules/cognitive-service"
  location                      = var.location
  resource_group_name           = var.resource_group_name
  public_network_access_enabled = var.public_network_access_enabled
  name                          = local.form_recognizer_name
  kind                          = "FormRecognizer"
  private_link_subnets = local.private_endpoint_subnets
  private_zone_id = var.private_link_enabled ? data.azurerm_private_dns_zone.cognitive_service[0].id : null
  tags            = var.tags
}

//give role to ai_identities to access the form recognizer service. The role id is 5e0bd9bd-7b93-4f28-af87-19fc36ad61bd
resource "azurerm_role_assignment" "form_recognizer_role_assignment" {
  count                = length(local.ai_identities)
  scope                = module.azure-form-recognizer.account.id
  role_definition_name = "Cognitive Services User"
  principal_id         = local.ai_identities[count.index]
}

//azure search
module "azure-search" {
  source                        = "../../../azure-infrastructure/modules/azure-search"
  location                      = var.location
  resource_group_name           = var.resource_group_name
  name                          = local.azure_search_name
  public_network_access_enabled = var.public_network_access_enabled
  private_link_subnets = local.private_endpoint_subnets
  private_zone_id = var.private_link_enabled ? data.azurerm_private_dns_zone.search[0].id : null
  tags            = var.tags
}

//give role to ai_identities to access the search service. The role id is 5e0bd9bd-7b93-4f28-af87-19fc36ad61bd
resource "azurerm_role_assignment" "azure_search_user_role_assignment" {
  count                = length(local.ai_identities)
  scope                = module.azure-search.search.id
  role_definition_name = "Search Index Data Reader"
  principal_id         = local.ai_identities[count.index]
}

resource "azurerm_role_assignment" "azure_search_svccontr_role_assignment" {
  count                = length(local.ai_identities)
  scope                = module.azure-search.search.id
  role_definition_name = "Search Service Contributor"
  principal_id         = local.ai_identities[count.index]
}


resource "azurerm_role_assignment" "search_index_data_svccontr_role_assignment" {
  count                = length(local.ai_identities)
  scope                = module.azure-search.search.id
  role_definition_name = "Search Index Data Contributor"
  principal_id         = local.ai_identities[count.index]
}

//create a storage account for the azure search
module "storage_account" {
  count                = var.deploy_new_storage_account ? 1 : 0
  source               = "../../../azure-infrastructure/modules/storage-account/"
  storage_account_name = local.storage_account_name
  location             = var.location
  resource_group_name  = var.resource_group_name
  private_link_subnets = local.private_endpoint_subnets
  blob_private_zone_id = var.private_link_enabled ? data.azurerm_private_dns_zone.storage_account[0].id : null
  file_private_zone_id = var.private_link_enabled ? data.azurerm_private_dns_zone.file_storage_account[0].id : null
  tags                 = var.tags
}

//load the existing storage account is deploy_new_storage_account is false
data "azurerm_storage_account" "storage_account" {
  count               = var.deploy_new_storage_account ? 0 : 1
  name                = var.existing_storage_account_name
  resource_group_name = var.resource_group_name
}


//give role to ai_identities to access the storage account. The role id is 5e0bd9bd-7b93-4f28-af87-19fc36ad61bd
resource "azurerm_role_assignment" "storage_account_user_role_assignment" {
  count                = length(local.ai_identities)
  scope                = var.deploy_new_storage_account ? module.storage_account[0].storage.id : data.azurerm_storage_account.storage_account[0].id
  role_definition_name = "Storage Blob Data Reader"
  principal_id         = local.ai_identities[count.index]
}

resource "azurerm_role_assignment" "storage_account_contr_role_assignment" {
  count                = length(local.ai_identities)
  scope                = var.deploy_new_storage_account ? module.storage_account[0].storage.id : data.azurerm_storage_account.storage_account[0].id
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = local.ai_identities[count.index]
}

resource "azurerm_role_assignment" "aks_contributor_role_assignment" {
  count                            = var.create_aks_federated_credentials ? var.deploy_new_storage_account? 1 : 0 : 0
  principal_id                     = data.azurerm_kubernetes_cluster.aks[0].identity[0].principal_id
  role_definition_name             = "Contributor"
  scope                            = var.deploy_new_storage_account ? module.storage_account[0].storage.id : data.azurerm_storage_account.storage_account[0].id
  skip_service_principal_aad_check = true
}

//load existing key vault if use_existing_key_vault is false
data "azurerm_key_vault" "existing_key_vault" {
  count = var.use_existing_key_vault ? 1 : 0
  name = var.existing_key_vault_name
  resource_group_name = var.existing_key_vault_resource_group_name
}

//assign secret user roles to the key vault for ai_identities
resource "azurerm_role_assignment" "key_vault_secret_user_role_assignment" {
  count                = length(local.ai_identities)
  scope                = data.azurerm_key_vault.existing_key_vault[0].id
  role_definition_name = "Key Vault Secrets Officer"
  principal_id         = local.ai_identities[count.index]
}

//create a cosmos db account
module "cosmos-db" {
  source                    = "../../../azure-infrastructure/modules/cosmos-db"
  account_name              = local.cosmos_db_account_name
  resource_group_name       = var.resource_group_name
  location                  = var.location
  private_link_subnets      = var.private_link_enabled ? local.private_endpoint_subnets : []
  private_zone_id           = var.private_link_enabled ? data.azurerm_private_dns_zone.mongodb[0].id : null
  geo_location              = var.dr_location
  tags                      = var.tags
}

# resource "azurerm_cosmosdb_mongo_database" "cosmos_db_database" {
#   name                = "rag-chat-memory-db"
#   resource_group_name = var.resource_group_name
#   account_name        = module.cosmos-db.account.name
#   throughput          = 400
# }

resource "azurerm_cosmosdb_sql_database" "cosmos_db_database" {
  name                = "rag-chat-memory-db"
  resource_group_name = var.resource_group_name
  account_name        = module.cosmos-db.account.name
  throughput          = 400
}


resource "azurerm_cosmosdb_sql_container" "chatmessages" {
  name                  = "rag-memory-chat-messages"
  resource_group_name   = var.resource_group_name
  account_name          = module.cosmos-db.account.name
  database_name         = azurerm_cosmosdb_sql_database.cosmos_db_database.name
  partition_key_path    = "/chatId"
  partition_key_version = 1
  throughput            = 400

  indexing_policy {
    indexing_mode = "consistent"

    included_path {
      path = "/*"
    }

    excluded_path {
      path = "/_etag/?"
    }
  }
}

# resource "azurerm_cosmosdb_mongo_collection" "cosmos_chatmessages" {
#   name                = "rag-memory-chat-messages"
#   resource_group_name = var.resource_group_name
#   account_name        = module.cosmos-db.account.name
#   database_name       = azurerm_cosmosdb_mongo_database.cosmos_db_database.name

#   default_ttl_seconds = "777"
#   //shard_key           = "uniqueKey"
#   throughput          = 400

#   index {
#     keys   = ["_id"]
#    // unique = true
#   }
# }

resource "azurerm_cosmosdb_sql_container" "chatsessions" {
  name                  = "rag-memory-chat-sessions"
  resource_group_name   = var.resource_group_name
  account_name          = module.cosmos-db.account.name
  database_name         = azurerm_cosmosdb_sql_database.cosmos_db_database.name
  partition_key_path    = "/id"
  partition_key_version = 1
  throughput            = 400

  indexing_policy {
    indexing_mode = "consistent"

    included_path {
      path = "/*"
    }

    excluded_path {
      path = "/_etag/?"
    }
  }
}


resource "azurerm_cosmosdb_sql_container" "chatparticipants" {
  name                  = "rag-memory-chat-participants"
  resource_group_name   = var.resource_group_name
  account_name          = module.cosmos-db.account.name
  database_name         = azurerm_cosmosdb_sql_database.cosmos_db_database.name
  partition_key_path    = "/userId"
  partition_key_version = 1
  throughput            = 400

  indexing_policy {
    indexing_mode = "consistent"

    included_path {
      path = "/*"
    }

    excluded_path {
      path = "/_etag/?"
    }
  }
}


resource "azurerm_cosmosdb_sql_role_definition" "sdk" {
  name                = "sdk-user"
  resource_group_name = var.resource_group_name
  account_name        = module.cosmos-db.account.name
  type                = "CustomRole"
  assignable_scopes   = [module.cosmos-db.account.id]

  permissions {
    data_actions = ["Microsoft.DocumentDB/databaseAccounts/readMetadata", "Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/*", "Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers/items/*"]
  }
}

resource "azurerm_cosmosdb_sql_role_assignment" "sdk" {
  count              = length(local.ai_identities)
  //name                = "736180af-7fbc-4c7f-9004-22735173c1c3"
  resource_group_name = var.resource_group_name
  account_name        = module.cosmos-db.account.name
  role_definition_id  = azurerm_cosmosdb_sql_role_definition.sdk.id
  principal_id        = local.ai_identities[count.index]
  scope               = module.cosmos-db.account.id
}