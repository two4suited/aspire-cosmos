@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param infra_outputs_azure_container_registry_endpoint string

param infra_outputs_planid string

param infra_outputs_azure_container_registry_managed_identity_id string

param infra_outputs_azure_container_registry_managed_identity_client_id string

param apiservice_containerimage string

param apiservice_containerport string

param cosmosdb_outputs_connectionstring string

param appinsights_outputs_appinsightsconnectionstring string

param apiservice_identity_outputs_id string

param apiservice_identity_outputs_clientid string

resource mainContainer 'Microsoft.Web/sites/sitecontainers@2024-04-01' = {
  name: 'main'
  properties: {
    authType: 'UserAssigned'
    image: apiservice_containerimage
    isMain: true
    userManagedIdentityClientId: infra_outputs_azure_container_registry_managed_identity_client_id
  }
  parent: webapp
}

resource webapp 'Microsoft.Web/sites@2024-04-01' = {
  name: take('${toLower('apiservice')}-${uniqueString(resourceGroup().id)}', 60)
  location: location
  properties: {
    serverFarmId: infra_outputs_planid
    keyVaultReferenceIdentity: apiservice_identity_outputs_id
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
          value: apiservice_containerport
        }
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Development'
        }
        {
          name: 'ConnectionStrings__appdb'
          value: 'AccountEndpoint=${cosmosdb_outputs_connectionstring};Database=appdb'
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appinsights_outputs_appinsightsconnectionstring
        }
        {
          name: 'ConnectionStrings__People'
          value: 'AccountEndpoint=${cosmosdb_outputs_connectionstring};Database=appdb;Container=People'
        }
        {
          name: 'AZURE_CLIENT_ID'
          value: apiservice_identity_outputs_clientid
        }
      ]
    }
  }
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${infra_outputs_azure_container_registry_managed_identity_id}': { }
      '${apiservice_identity_outputs_id}': { }
    }
  }
}