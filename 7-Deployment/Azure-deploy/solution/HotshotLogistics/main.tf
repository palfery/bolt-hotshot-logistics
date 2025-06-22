terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
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
}

data "azurerm_client_config" "current" {}

resource "azurerm_resource_group" "rg" {
  name     = var.resource_group_name
  location = var.location
}

module "app_service_plan" {
  source  = "Azure/avm-res-web-serverfarm/azurerm"
  version = "0.7.0"
  name                = "hotshot-logistics-plan"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  sku_name            = "Y1"
  kind                = "functionapp"
}

resource "azurerm_storage_account" "storage" {
  name                     = "hotshotlogisticsstorage"
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

module "function_app" {
  source  = "Azure/avm-res-web-site/azurerm"
  version = "0.17.2"
  name                = "hotshot-logistics-function"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  server_farm_id      = module.app_service_plan.id
  kind                = "functionapp"
  # Add other required properties as needed
}

module "app_configuration" {
  source  = "Azure/avm-res-appconfiguration-configurationstore/azure"
  version = "0.1.0"
  name                = "hotshot-logistics-config"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
}

module "key_vault" {
  source  = "Azure/avm-res-keyvault-vault/azurerm"
  version = "0.10.0"
  name                = "hotshot-logistics-kv"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  tenant_id           = data.azurerm_client_config.current.tenant_id
  sku_name            = "standard"
}

# Add your Azure resources here 