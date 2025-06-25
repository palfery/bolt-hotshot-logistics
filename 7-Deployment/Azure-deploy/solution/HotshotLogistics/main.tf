terraform {

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.34.0"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.0"
    }
  }
  
  backend "azurerm" {
    resource_group_name  = "rg-launchpad-dev-eastus"
    storage_account_name = "hotshotlogisticsstate"
    container_name       = "tfstate"
    key                  = "hotshotlogistics.tfstate"
  }
}

provider "azurerm" {
  features {}
  subscription_id = var.subscription_id
}

data "azurerm_client_config" "current" {}

resource "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
  location = var.location
  tags     = var.tags
}

module "app_service_plan" {
  source  = "Azure/avm-res-web-serverfarm/azurerm"
  version = "0.7.0"
  name                = "${var.environment}-hotshot-logistics-plan"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  sku_name            = "B1"
  os_type             = "Linux"
  zone_balancing_enabled = false
  tags                = var.tags
}

resource "azurerm_storage_account" "storage" {
  name                     = "${var.environment}hotshotstorage"
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  tags                     = var.tags
}

module "function_app" {
  source  = "Azure/avm-res-web-site/azurerm"
  version = "0.17.2"
  name                      = "${var.environment}-hotshot-logistics-function"
  resource_group_name       = azurerm_resource_group.rg.name
  location                  = azurerm_resource_group.rg.location
  service_plan_resource_id  = module.app_service_plan.resource.id
  os_type                   = "Linux"
  kind                      = "functionapp"
  storage_account_name      = azurerm_storage_account.storage.name
  tags                      = var.tags
  
  # Add other required properties as needed
}

module "app_configuration" {
  source  = "Azure/avm-res-appconfiguration-configurationstore/azure"
  version = "0.1.0"
  name                        = "${var.environment}-hotshot-logistics-config"
  resource_group_resource_id  = azurerm_resource_group.rg.id
  location                    = azurerm_resource_group.rg.location
  tags                        = var.tags
}


module "sql_server" {
  source  = "Azure/avm-res-sql-server/azurerm"
  version = "0.1.5"

  # Required
  name                = "${var.environment}-hotshot-logistics-sql"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  server_version      = "12.0"

  # Optional
  administrator_login          = var.administrator_login
  administrator_login_password = var.administrator_login_password

  # Networking
  public_network_access_enabled = true
  tags= var.tags
}

module "sql_database" {
  source  = "Azure/avm-res-sql-server/azurerm//modules/database"
  version = "0.1.5"

  name                = var.database_name

  sql_server          = module.sql_server.resource_id

  sku_name            = var.sku_name   # Example: "GP_S_Gen5_2"
  #compute_model       = "Serverless"
  min_capacity        = 0.5

  auto_pause_delay_in_minutes = 60
  tags = var.tags

  # Add additional configuration as needed
}

resource "random_password" "sql_password" {
  length  = 16
  special = true
}

module "key_vault" {
  source  = "Azure/avm-res-keyvault-vault/azurerm"
  version = "0.10.0"
  name                = "${var.environment}-hotshot-log-kv-01"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  tenant_id           = data.azurerm_client_config.current.tenant_id
  sku_name            = "standard"
  tags                = var.tags
  
  secrets = {
    sql_connection_string = {
      name         = "sql-connection-string"
      content_type = "text/plain"
    }
    sql_admin_password = {
      name         = "sql-admin-password"
      content_type = "text/plain"
    }
  }
  
  secrets_value = {
    sql_connection_string = "Server=${module.sql_server.resource.fully_qualified_domain_name};Database=${module.sql_database.name};User Id=sqladmin;Password=${random_password.sql_password.result};TrustServerCertificate=true;"
    sql_admin_password  = random_password.sql_password.result
  }
}

# Add your Azure resources here 
