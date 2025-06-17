resource "azurerm_resource_group" "launchpad" {
  name     = "rg-launchpad-dev-eastus2"
  location = "East US 2"
}

resource "azurerm_storage_account" "remote_state" {
  name                     = "hotshotlogisticsremotestate"
  resource_group_name      = azurerm_resource_group.launchpad.name
  location                 = azurerm_resource_group.launchpad.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
} 