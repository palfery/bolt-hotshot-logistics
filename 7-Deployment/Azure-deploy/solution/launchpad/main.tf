terraform {
  backend "azurerm" {
    resource_group_name  = "rg-launchpad-dev-eastus2"
    storage_account_name = "hotshotlogisticsstate"
    container_name       = "tfstate"
    key                  = "launchpad.tfstate"
  }
}

provider "azurerm" {
  features {}
}

resource "azurerm_resource_group" "launchpad" {
  name     = "rg-launchpad-dev-eastus2"
  location = "East US 2"
}

resource "azurerm_storage_account" "remote_state" {
  name                     = "hotshotlogisticsstate"
  resource_group_name      = azurerm_resource_group.launchpad.name
  location                 = azurerm_resource_group.launchpad.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
} 