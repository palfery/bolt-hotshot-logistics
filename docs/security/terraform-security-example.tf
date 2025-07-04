# Secure Terraform Configuration for Hotshot Logistics Azure Infrastructure

# Variables for security configuration
variable "allowed_ip_ranges" {
  description = "List of IP ranges allowed to access resources"
  type        = list(string)
  default     = []
}

variable "security_contact_email" {
  description = "Email address for security notifications"
  type        = string
}

variable "app_subnet_cidr" {
  description = "CIDR block for application subnet"
  type        = string
  default     = "10.0.1.0/24"
}

variable "db_subnet_cidr" {
  description = "CIDR block for database subnet"
  type        = string
  default     = "10.0.2.0/24"
}

# Network Security Group for SQL Server
resource "azurerm_network_security_group" "sql_nsg" {
  name                = "${var.environment}-sql-nsg"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name

  # Deny all inbound traffic by default
  security_rule {
    name                       = "DenyAllInbound"
    priority                   = 4096
    direction                  = "Inbound"
    access                     = "Deny"
    protocol                   = "*"
    source_port_range          = "*"
    destination_port_range     = "*"
    source_address_prefix      = "*"
    destination_address_prefix = "*"
  }

  # Allow only from application subnet
  security_rule {
    name                       = "AllowAppSubnet"
    priority                   = 100
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = "1433"
    source_address_prefix      = var.app_subnet_cidr
    destination_address_prefix = "*"
  }

  # Allow Azure services if needed (be restrictive)
  security_rule {
    name                       = "AllowAzureServices"
    priority                   = 200
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = "1433"
    source_address_prefix      = "AzureCloud"
    destination_address_prefix = "*"
  }

  tags = var.tags
}

# Storage account for audit logs
resource "azurerm_storage_account" "audit_storage" {
  name                     = "${replace(var.environment, "-", "")}auditstorage${random_integer.storage_suffix.result}"
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = azurerm_resource_group.rg.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  
  # Security configurations
  min_tls_version                 = "TLS1_2"
  allow_nested_items_to_be_public = false
  enable_https_traffic_only       = true

  # Network access rules
  network_rules {
    default_action             = "Deny"
    ip_rules                   = var.allowed_ip_ranges
    virtual_network_subnet_ids = []
    bypass                     = ["AzureServices"]
  }

  # Enable blob encryption
  blob_properties {
    delete_retention_policy {
      days = 30
    }
    versioning_enabled = true
  }

  tags = var.tags
}

resource "random_integer" "storage_suffix" {
  min = 1000
  max = 9999
}

# Enhanced Key Vault configuration
module "key_vault" {
  source  = "Azure/avm-res-keyvault-vault/azurerm"
  version = "0.10.0"
  
  name                = "${var.environment}-hotshot-log-kv-01"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  tenant_id           = data.azurerm_client_config.current.tenant_id
  sku_name            = "standard"
  
  # Security configurations
  enabled_for_disk_encryption     = true
  enabled_for_deployment          = false
  enabled_for_template_deployment = false
  purge_protection_enabled        = true
  soft_delete_retention_days      = 30

  # Network access
  network_acls = {
    bypass         = "AzureServices"
    default_action = "Deny"
    ip_rules       = var.allowed_ip_ranges
  }

  # Diagnostic settings for audit logging
  diagnostic_settings = {
    audit = {
      name                         = "audit-logs"
      log_categories               = ["AuditEvent"]
      log_groups                   = []
      metric_categories            = ["AllMetrics"]
      log_analytics_destination_type = "Dedicated"
      workspace_resource_id        = azurerm_log_analytics_workspace.security_logs.id
      storage_account_resource_id  = azurerm_storage_account.audit_storage.id
    }
  }
  
  secrets = {
    sql_connection_string = {
      name         = "sql-connection-string"
      content_type = "text/plain"
    }
    sql_admin_password = {
      name         = "sql-admin-password"
      content_type = "text/plain"
    }
    jwt_secret_key = {
      name         = "jwt-secret-key"
      content_type = "text/plain"
    }
  }
  
  secrets_value = {
    sql_connection_string = "Server=${module.sql_server.resource.fully_qualified_domain_name};Database=${module.sql_database.name};User Id=${var.administrator_login};Password=${random_password.sql_password.result};Encrypt=true;TrustServerCertificate=false;Connection Timeout=30;"
    sql_admin_password    = random_password.sql_password.result
    jwt_secret_key        = random_password.jwt_secret.result
  }

  tags = var.tags
}

# Generate secure passwords
resource "random_password" "sql_password" {
  length  = 24
  special = true
  upper   = true
  lower   = true
  numeric = true
}

resource "random_password" "jwt_secret" {
  length  = 64
  special = true
  upper   = true
  lower   = true
  numeric = true
}

# Log Analytics Workspace for security monitoring
resource "azurerm_log_analytics_workspace" "security_logs" {
  name                = "${var.environment}-security-logs"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  sku                 = "PerGB2018"
  retention_in_days   = 90

  tags = var.tags
}

# Enhanced SQL Server configuration
module "sql_server" {
  source  = "Azure/avm-res-sql-server/azurerm"
  version = "0.1.5"
  
  name                = "${var.environment}-hotshot-logistics-sql"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  server_version      = "12.0"

  # Security configurations
  administrator_login          = var.administrator_login
  administrator_login_password = random_password.sql_password.result
  public_network_access_enabled = false  # Changed from true
  minimum_tls_version          = "1.2"

  # Azure AD administrator
  azure_ad_administrator = {
    login_username = "sql-admin@yourdomain.com"
    object_id      = data.azurerm_client_config.current.object_id
    tenant_id      = data.azurerm_client_config.current.tenant_id
  }

  # Firewall rules (restrictive)
  firewall_rules = {
    "AllowApplicationSubnet" = {
      start_ip_address = cidrhost(var.app_subnet_cidr, 0)
      end_ip_address   = cidrhost(var.app_subnet_cidr, -1)
    }
  }

  # Advanced Threat Protection
  threat_detection_policy = {
    state                      = "Enabled"
    email_account_admins       = true
    email_addresses           = [var.security_contact_email]
    retention_days            = 30
    storage_account_access_key = azurerm_storage_account.audit_storage.primary_access_key
    storage_endpoint          = azurerm_storage_account.audit_storage.primary_blob_endpoint
  }

  # Auditing configuration
  auditing_policy = {
    state                       = "Enabled"
    storage_account_access_key  = azurerm_storage_account.audit_storage.primary_access_key
    storage_endpoint           = azurerm_storage_account.audit_storage.primary_blob_endpoint
    retention_in_days          = 90
    log_analytics_workspace_id = azurerm_log_analytics_workspace.security_logs.id
  }

  # Vulnerability assessment
  vulnerability_assessment = {
    storage_container_path     = "${azurerm_storage_account.audit_storage.primary_blob_endpoint}vulnerability-assessment"
    storage_account_access_key = azurerm_storage_account.audit_storage.primary_access_key
    recurring_scans = {
      is_enabled                = true
      email_subscription_admins = true
      emails                    = [var.security_contact_email]
    }
  }

  tags = var.tags
}

# Enhanced SQL Database configuration
module "sql_database" {
  source  = "Azure/avm-res-sql-server/azurerm//modules/database"
  version = "0.1.5"

  name       = var.database_name
  sql_server = module.sql_server.resource_id
  sku_name   = var.sku_name

  # Security configurations
  min_capacity                = var.min_capacity
  auto_pause_delay_in_minutes = var.auto_pause_delay_in_minutes

  # Transparent Data Encryption
  transparent_data_encryption_enabled = true

  # Threat detection
  threat_detection_policy = {
    state           = "Enabled"
    retention_days  = 30
    email_addresses = [var.security_contact_email]
  }

  tags = var.tags
}

# Application Insights for security monitoring
resource "azurerm_application_insights" "security_monitoring" {
  name                = "${var.environment}-security-insights"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type    = "web"
  workspace_id        = azurerm_log_analytics_workspace.security_logs.id

  tags = var.tags
}

# App Service Plan with security features
resource "azurerm_service_plan" "app_plan" {
  name                = "${var.environment}-app-plan"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  os_type             = "Linux"
  sku_name            = "P1v3"  # Premium for security features

  tags = var.tags
}

# Function App with security configurations
resource "azurerm_linux_function_app" "hotshot_api" {
  name                = "${var.environment}-hotshot-api"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location

  storage_account_name       = azurerm_storage_account.audit_storage.name
  storage_account_access_key = azurerm_storage_account.audit_storage.primary_access_key
  service_plan_id           = azurerm_service_plan.app_plan.id

  # Security configurations
  https_only = true

  site_config {
    minimum_tls_version = "1.2"
    ftps_state         = "Disabled"
    
    # CORS configuration
    cors {
      allowed_origins = ["https://yourdomain.com"]
      support_credentials = true
    }

    # Application stack
    application_stack {
      dotnet_version = "8.0"
    }
  }

  # App settings with security configurations
  app_settings = {
    "FUNCTIONS_WORKER_RUNTIME"        = "dotnet-isolated"
    "WEBSITE_RUN_FROM_PACKAGE"        = "1"
    "APPINSIGHTS_INSTRUMENTATIONKEY"  = azurerm_application_insights.security_monitoring.instrumentation_key
    
    # Authentication settings
    "AzureAd__TenantId" = data.azurerm_client_config.current.tenant_id
    "AzureAd__ClientId" = "@Microsoft.KeyVault(VaultName=${module.key_vault.resource.name};SecretName=azure-ad-client-id)"
    
    # Database connection (from Key Vault)
    "ConnectionStrings__DefaultConnection" = "@Microsoft.KeyVault(VaultName=${module.key_vault.resource.name};SecretName=sql-connection-string)"
    
    # JWT configuration (from Key Vault)
    "Jwt__SecretKey" = "@Microsoft.KeyVault(VaultName=${module.key_vault.resource.name};SecretName=jwt-secret-key)"
    "Jwt__Issuer"    = "https://${var.environment}-hotshot-api.azurewebsites.net"
    "Jwt__Audience"  = "hotshot-logistics-api"
  }

  # Managed identity for Key Vault access
  identity {
    type = "SystemAssigned"
  }

  tags = var.tags
}

# Grant Function App access to Key Vault
resource "azurerm_key_vault_access_policy" "function_app_access" {
  key_vault_id = module.key_vault.resource.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = azurerm_linux_function_app.hotshot_api.identity[0].principal_id

  secret_permissions = [
    "Get",
    "List"
  ]
}

# Security Center configurations
resource "azurerm_security_center_contact" "security_contact" {
  email = var.security_contact_email
  phone = "+1-555-123-4567"  # Update with actual number

  alert_notifications = true
  alerts_to_admins   = true
}

# Enable Security Center for the subscription
resource "azurerm_security_center_subscription_pricing" "security_center" {
  tier          = "Standard"
  resource_type = "VirtualMachines,SqlServers,AppServices,KeyVaults,StorageAccounts,Containers"
}

# Output sensitive information securely
output "key_vault_name" {
  description = "Name of the Key Vault containing secrets"
  value       = module.key_vault.resource.name
}

output "function_app_name" {
  description = "Name of the Function App"
  value       = azurerm_linux_function_app.hotshot_api.name
}

output "application_insights_key" {
  description = "Application Insights instrumentation key"
  value       = azurerm_application_insights.security_monitoring.instrumentation_key
  sensitive   = true
}

# Don't output sensitive values directly
# output "sql_connection_string" {
#   description = "SQL Server connection string"
#   value       = "Retrieve from Key Vault: ${module.key_vault.resource.vault_uri}secrets/sql-connection-string"
# }