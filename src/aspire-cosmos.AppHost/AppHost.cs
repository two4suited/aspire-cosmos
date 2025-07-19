
using Aspire.Hosting.Azure;

var builder = DistributedApplication.CreateBuilder(args);

// Add Cosmos DB resource and a database, run as emulator for local development
var cosmosDb = builder.AddAzureCosmosDB("cosmosdb").RunAsEmulator();
var cosmosDatabase = cosmosDb.AddCosmosDatabase("appdb");

var apiService = builder.AddProject<Projects.aspire_cosmos_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(cosmosDatabase);

builder.AddProject<Projects.aspire_cosmos_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
