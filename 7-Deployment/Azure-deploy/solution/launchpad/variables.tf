variable "location" {
  description = "The Azure region where resources will be created"
  type        = string
  default     = "East US 2"
}

variable "environment" {
  description = "The environment (e.g., dev, prod)"
  type        = string
  default     = "dev"
} 