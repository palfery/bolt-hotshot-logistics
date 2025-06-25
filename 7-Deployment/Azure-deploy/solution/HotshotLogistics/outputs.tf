output "resource_group_name" {
  description = "The name of the resource group"
  value       = azurerm_resource_group.rg.name
}

output "sql_server_name" {
  description = "The name of the SQL Server"
  value       = module.sql_server.resource.name
}

output "sql_server_fqdn" {
  description = "The fully qualified domain name of the SQL Server"
  value       = module.sql_server.resource.fully_qualified_domain_name
  sensitive   = true
}

output "sql_database_name" {
  description = "The name of the SQL Database"
  value       = module.sql_database.resource.name
}

# Add more outputs as needed 