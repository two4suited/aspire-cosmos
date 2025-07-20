@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

resource cosmosdb 'Microsoft.DocumentDB/databaseAccounts@2024-08-15' = {
  name: take('cosmosdb-${uniqueString(resourceGroup().id)}', 44)
  location: location
  properties: {
    locations: [
      {
        locationName: location
        failoverPriority: 0
      }
    ]
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    databaseAccountOfferType: 'Standard'
    disableLocalAuth: true
  }
  kind: 'GlobalDocumentDB'
  tags: {
    'aspire-resource-name': 'cosmosdb'
  }
}

resource appdb 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2024-08-15' = {
  name: 'appdb'
  location: location
  properties: {
    resource: {
      id: 'appdb'
    }
  }
  parent: cosmosdb
}

resource People 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2024-08-15' = {
  name: 'People'
  location: location
  properties: {
    resource: {
      id: 'People'
      partitionKey: {
        paths: [
          '/id'
        ]
      }
    }
  }
  parent: appdb
}

output connectionString string = cosmosdb.properties.documentEndpoint

output name string = cosmosdb.name