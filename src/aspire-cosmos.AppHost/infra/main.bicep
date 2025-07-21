targetScope = 'subscription'

@minLength(1)
@maxLength(64)
@description('Name of the environment that can be used as part of naming resource convention, the name of the resource group for your application will use this name, prefixed with rg-')
param environmentName string

@minLength(1)
@description('The location used for all deployed resources')
param location string

@description('Id of the user or app to assign application roles')
param principalId string = ''


var tags = {
  'azd-env-name': environmentName
}

resource rg 'Microsoft.Resources/resourceGroups@2022-09-01' = {
  name: 'rg-${environmentName}'
  location: location
  tags: tags
}

module apiservice_identity 'apiservice-identity/apiservice-identity.module.bicep' = {
  name: 'apiservice-identity'
  scope: rg
  params: {
    location: location
  }
}
module apiservice_roles_cosmosdb 'apiservice-roles-cosmosdb/apiservice-roles-cosmosdb.module.bicep' = {
  name: 'apiservice-roles-cosmosdb'
  scope: rg
  params: {
    cosmosdb_outputs_name: cosmosdb.outputs.name
    location: location
    principalId: apiservice_identity.outputs.principalId
  }
}
module cosmosdb 'cosmosdb/cosmosdb.module.bicep' = {
  name: 'cosmosdb'
  scope: rg
  params: {
    location: location
  }
}
module infra 'infra/infra.module.bicep' = {
  name: 'infra'
  scope: rg
  params: {
    location: location
    userPrincipalId: principalId
  }
}
output APISERVICE_IDENTITY_CLIENTID string = apiservice_identity.outputs.clientId
output APISERVICE_IDENTITY_ID string = apiservice_identity.outputs.id
output AZURE_CONTAINER_REGISTRY_ENDPOINT string = infra.outputs.AZURE_CONTAINER_REGISTRY_ENDPOINT
output COSMOSDB_CONNECTIONSTRING string = cosmosdb.outputs.connectionString
output INFRA_AZURE_CONTAINER_REGISTRY_ENDPOINT string = infra.outputs.AZURE_CONTAINER_REGISTRY_ENDPOINT
output INFRA_AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_CLIENT_ID string = infra.outputs.AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_CLIENT_ID
output INFRA_AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID string = infra.outputs.AZURE_CONTAINER_REGISTRY_MANAGED_IDENTITY_ID
output INFRA_PLANID string = infra.outputs.planId
