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