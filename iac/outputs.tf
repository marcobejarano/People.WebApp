output "acr_name" {
  value = azurerm_container_registry.container_registry.name
}

output "acr_login_server" {
  value = azurerm_container_registry.container_registry.login_server
}

output "rg_name" {
  value = azurerm_resource_group.rg-tf.name
}

output "cluster_name" {
  value = azurerm_kubernetes_cluster.kubernetes_cluster.name
}

output "aks_pool_node_name" {
  value = azurerm_kubernetes_cluster.kubernetes_cluster.default_node_pool[0].name
}

output "key_vault_name" {
  value = azurerm_key_vault.key_vault.name
}
