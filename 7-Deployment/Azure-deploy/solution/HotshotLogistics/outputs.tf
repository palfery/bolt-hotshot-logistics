output "resource_group_name" {
  description = "The name of the resource group"
  value       = azurerm_resource_group.rg.name
}

output "mysql_server_name" {
  description = "The name of the MySQL Flexible Server"
  value       = module.mysql_server.resource.name
}

output "mysql_server_fqdn" {
  description = "The fully qualified domain name of the MySQL Flexible Server"
  value       = module.mysql_server.resource.fqdn
  sensitive   = true
}

output "mysql_connection_string_secret_name" {
  description = "The name of the Key Vault secret containing the MySQL connection string"
  value       = module.mysql_connection_string_secret.resource.name
}

# Add more outputs as needed 