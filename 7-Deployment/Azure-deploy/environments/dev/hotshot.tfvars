# Azure Region
location = "East US"

# Resource Group
resource_group_name = "rg-hotshot-logistics-dev-eastus"

# Environment
environment = "dev"

# Tags
tags = {
  Environment = "dev"
  Project     = "hotshot-logistics"
  ManagedBy   = "terraform"
  Owner       = "hotshot-team"
}

# SQL Server Configuration
administrator_login = "sqladmin"
# Note: administrator_login_password should be set securely and not committed to version control
# You can set this via environment variable: TF_VAR_administrator_login_password

# SQL Database Configuration
database_name = "hotshot-logistics-db"
sku_name = "GP_S_Gen5_1"
min_capacity = 0.5
max_capacity = 4
auto_pause_delay_in_minutes = 60
