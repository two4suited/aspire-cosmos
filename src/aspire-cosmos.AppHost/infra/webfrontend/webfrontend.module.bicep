@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param infra_outputs_azure_container_registry_endpoint string

param infra_outputs_planid string

param infra_outputs_azure_container_registry_managed_identity_id string

param infra_outputs_azure_container_registry_managed_identity_client_id string

param webfrontend_containerimage string

param webfrontend_containerport string

resource mainContainer 'Microsoft.Web/sites/sitecontainers@2024-04-01' = {
  name: 'main'
  properties: {
    authType: 'UserAssigned'
    image: webfrontend_containerimage
    isMain: true
    userManagedIdentityClientId: infra_outputs_azure_container_registry_managed_identity_client_id
  }
  parent: webapp
}

resource webapp 'Microsoft.Web/sites@2024-04-01' = {
  name: take('${toLower('webfrontend')}-${uniqueString(resourceGroup().id)}', 60)
  location: location
  properties: {
    serverFarmId: infra_outputs_planid
    siteConfig: {
      linuxFxVersion: 'SITECONTAINERS'
      acrUseManagedIdentityCreds: true
      acrUserManagedIdentityID: infra_outputs_azure_container_registry_managed_identity_client_id
      appSettings: [
        {
          name: 'OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EXCEPTION_LOG_ATTRIBUTES'
          value: 'true'
        }
        {
          name: 'OTEL_DOTNET_EXPERIMENTAL_OTLP_EMIT_EVENT_LOG_ATTRIBUTES'
          value: 'true'
        }
        {
          name: 'OTEL_DOTNET_EXPERIMENTAL_OTLP_RETRY'
          value: 'in_memory'
        }
        {
          name: 'ASPNETCORE_FORWARDEDHEADERS_ENABLED'
          value: 'true'
        }
        {
          name: 'HTTP_PORTS'
          value: webfrontend_containerport
        }
        {
          name: 'services__apiservice__http__0'
          value: 'http://${take('${toLower('apiservice')}-${uniqueString(resourceGroup().id)}', 60)}.azurewebsites.net'
        }
        {
          name: 'services__apiservice__https__0'
          value: 'https://${take('${toLower('apiservice')}-${uniqueString(resourceGroup().id)}', 60)}.azurewebsites.net'
        }
      ]
    }
  }
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${infra_outputs_azure_container_registry_managed_identity_id}': { }
    }
  }
}