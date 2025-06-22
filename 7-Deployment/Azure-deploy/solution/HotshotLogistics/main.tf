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
    resource_group_name  = "rg-launchpad-dev-eastus2"
    storage_account_name = "hotshotlogisticsstate"
    container_name       = "hotshotlogistics-tfstate"
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
  sku_name            = "F1"
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

module "key_vault" {
  source  = "Azure/avm-res-keyvault-vault/azurerm"
  version = "0.10.0"
  name                = "${var.environment}-hotshot-logistics-kv"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  tenant_id           = data.azurerm_client_config.current.tenant_id
  sku_name            = "standard"
  tags                = var.tags
}

module "mysql_server" {
  source  = "Azure/avm-res-dbformysql-flexibleserver/azurerm"
  version = "0.1.1"
  name                = "${var.environment}-hotshot-logistics-mysql"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  administrator_login = "mysqladmin"
  administrator_password = random_password.mysql_password.result
  backup_retention_days = 7
  geo_redundant_backup_enabled = false
  sku_name            = "B1ms"
  tags                = var.tags
}

resource "random_password" "mysql_password" {
  length  = 16
  special = true
}

resource "azurerm_key_vault_secret" "mysql_connection_string" {
  name         = "mysql-connection-string"
  value        = "Server=${module.mysql_server.resource.fqdn};Database=hotshotlogistics;Uid=mysqladmin;Pwd=${random_password.mysql_password.result};SslMode=Required;"
  key_vault_id = module.key_vault.id
  tags         = var.tags
}

# Add your Azure resources here 
