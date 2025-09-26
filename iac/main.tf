resource "azurerm_resource_group" "rg-tf" {
  name     = var.resource_group_name
  location = var.location
}

resource "azurerm_container_registry" "container_registry" {
  name                = "peopletf"
  resource_group_name = azurerm_resource_group.rg-tf.name
  location            = azurerm_resource_group.rg-tf.location
  sku                 = "Basic"
  admin_enabled       = false
}

resource "azurerm_kubernetes_cluster" "kubernetes_cluster" {
  name                = "aks-people-tf"
  resource_group_name = azurerm_resource_group.rg-tf.name
  location            = azurerm_resource_group.rg-tf.location
  dns_prefix          = "aks-people-tf-dns"

  default_node_pool {
    name                 = "agentpool"    
    vm_size              = "standard_a4_v2"    
    auto_scaling_enabled = true
    min_count            = 1
    max_count            = 3
    os_disk_size_gb      = 30
    os_disk_type         = "Managed"
    type                 = "VirtualMachineScaleSets"
  }

  identity {
    type = "SystemAssigned"
  }

  network_profile {
    network_plugin = "kubenet"
  }  

  sku_tier = "Free"

  depends_on = [
    azurerm_container_registry.container_registry
  ]
}

resource "azurerm_role_assignment" "acr_pull" {
  principal_id                     = azurerm_kubernetes_cluster.kubernetes_cluster.kubelet_identity[0].object_id
  role_definition_name             = "AcrPull"
  scope                            = azurerm_container_registry.container_registry.id
  skip_service_principal_aad_check = true
}

resource "azurerm_mssql_server" "sql_server" {
  name                         = "aks-sql-people-tf"
  resource_group_name          = azurerm_resource_group.rg-tf.name
  location                     = azurerm_resource_group.rg-tf.location
  version                      = "12.0"
  administrator_login          = "azure"
  administrator_login_password = "P@ssw0rD"
  public_network_access_enabled = true
}

resource "azurerm_mssql_database" "sql_database" {
  name                = "people"  
  server_id           = azurerm_mssql_server.sql_server.id 
  sku_name            = "Basic"
}

resource "azurerm_mssql_firewall_rule" "sql_firewall_rule" {
  name                = "allow-azure-services"
  server_id           = azurerm_mssql_server.sql_server.id         
  start_ip_address    = "0.0.0.0"
  end_ip_address      = "0.0.0.0"
}

locals {
  sql_connection_string="Server=tcp:${azurerm_mssql_server.sql_server.fully_qualified_domain_name},1433;Initial Catalog=${azurerm_mssql_database.sql_database.name};Persist Security Info=False;User ID=${azurerm_mssql_server.sql_server.administrator_login};Password=${azurerm_mssql_server.sql_server.administrator_login_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"  
}

data "azurerm_client_config" "current" {}

resource "azurerm_key_vault" "key_vault" {
  name                        = "kv-people-tf-marco"
  resource_group_name         = azurerm_resource_group.rg-tf.name
  location                    = azurerm_resource_group.rg-tf.location
  tenant_id                   = data.azurerm_client_config.current.tenant_id
  sku_name                    = "standard"
}

resource "azurerm_key_vault_secret" "key_vault_secret" {
  name         = "connection-string"
  value        = local.sql_connection_string
  key_vault_id = azurerm_key_vault.key_vault.id

  depends_on = [        
    azurerm_key_vault_access_policy.self, 
    azurerm_kubernetes_cluster.kubernetes_cluster
  ]
}

resource "azurerm_key_vault_access_policy" "self" {
  key_vault_id = azurerm_key_vault.key_vault.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = data.azurerm_client_config.current.object_id
  
  secret_permissions = ["Get", "Set", "Delete", "Purge"]
}

resource "azurerm_key_vault_access_policy" "cluster" {
  key_vault_id = azurerm_key_vault.key_vault.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = azurerm_kubernetes_cluster.kubernetes_cluster.kubelet_identity[0].object_id

  secret_permissions = ["Get", "List"]

  depends_on = [    
    azurerm_key_vault.key_vault,
    azurerm_kubernetes_cluster.kubernetes_cluster    
  ]
}
