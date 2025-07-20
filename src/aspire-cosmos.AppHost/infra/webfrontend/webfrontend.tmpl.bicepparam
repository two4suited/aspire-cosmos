using './webfrontend.module.bicep'

param infra_outputs_azure_container_registry_endpoint = '{{ .Env.INFRA_AZURE_CONTAINER_REGISTRY_ENDPOINT }}'
param infra_outputs_azure_container_registry_managed_identity_client_id = '{{ .Env.INFRA_AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_CLIENT_ID }}'
param infra_outputs_azure_container_registry_managed_identity_id = '{{ .Env.INFRA_AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID }}'
param infra_outputs_planid = '{{ .Env.INFRA_PLANID }}'
param webfrontend_containerimage = '{{ .Image }}'
param webfrontend_containerport = '{{ targetPortOrDefault 8080 }}'
