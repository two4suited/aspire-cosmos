
#pragma warning disable ASPIRECOSMOSDB001
using Aspire.Hosting.Azure;

var builder = DistributedApplication.CreateBuilder(args);

// Add Cosmos DB resource and a database, run as ARM64-compatible preview emulator for local development
var cosmosDb = builder.AddAzureCosmosDB("cosmosdb")
    .RunAsPreviewEmulator(emulator =>
    {
        // You can add emulator configuration here if needed, e.g. emulator.WithDataExplorer();
    });
var cosmosDatabase = cosmosDb.AddCosmosDatabase("appdb");


var apiService = builder.AddProject<Projects.aspire_cosmos_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WaitFor(cosmosDatabase)
    .WithReference(cosmosDatabase);

builder.AddProject<Projects.aspire_cosmos_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
