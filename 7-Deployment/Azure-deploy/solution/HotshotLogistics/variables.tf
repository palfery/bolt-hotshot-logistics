variable "location" {
  description = "The Azure region where resources will be created"
  type        = string
  default     = "East US"
}

variable "resource_group_name" {
  description = "The name of the resource group"
  type        = string
  default     = "hotshot-logistics-rg"
}

variable "subscription_id" {
  description = "The Azure subscription ID"
  type        = string
  sensitive   = true
}

variable "azure_ad_client_id" {
  description = "The Azure AD application client ID for authentication"
  type        = string
  sensitive   = true
}

variable "azure_ad_client_secret" {
  description = "The Azure AD application client secret for authentication"
  type        = string
  sensitive   = true
}

variable "environment" {
  description = "The environment (dev, staging, prod)"
  type        = string
  default     = "dev"
}

variable "tags" {
  description = "Tags to apply to all resources"
  type        = map(string)
  default = {
    Environment = "dev"
    Project     = "hotshot-logistics"
    ManagedBy   = "terraform"
  }
}

# Add more variables as needed 

variable "administrator_login" {
  description = "The administrator login for the SQL Server"
  type        = string
  default     = "sqladmin"
}

variable "administrator_login_password" {
  description = "The administrator login password for the SQL Server"
  type        = string
  sensitive   = true
}

variable "database_name" {
  description = "The name of the SQL database"
  type        = string
  default     = "hotshot-logistics-db"
}

variable "sku_name" {
  description = "The SKU name for the SQL database"
  type        = string
  default     = "GP_S_Gen5_1"
}

variable "min_capacity" {
  description = "The minimum capacity for the serverless SQL database"
  type        = number
  default     = 0.5
}

variable "max_capacity" {
  description = "The maximum capacity for the serverless SQL database"
  type        = number
  default     = 4
}

variable "auto_pause_delay_in_minutes" {
  description = "The auto pause delay in minutes for the serverless SQL database"
  type        = number
  default     = 60
} 