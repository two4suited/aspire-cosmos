using Microsoft.Identity.Client;
using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);
#pragma warning disable ASPIRECOSMOSDB001
// Add Cosmos DB resource and a database, run as ARM64-compatible preview emulator for local development
var cosmosDb = builder.AddAzureCosmosDB("cosmosdb")
    .RunAsPreviewEmulator(emulator =>
    {
        emulator.WithDataExplorer();
    });
var cosmosDatabase = cosmosDb.AddCosmosDatabase("appdb");
var container = cosmosDatabase.AddContainer("People","/id");

builder.AddAzureAppServiceEnvironment("infra");

var apiService = builder.AddProject<Projects.aspire_cosmos_ApiService>("apiservice")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithHttpHealthCheck("/health")
    .WaitFor(cosmosDatabase)
    .WithReference(cosmosDatabase)
    .WithExternalHttpEndpoints()
    .WithReference(container).WaitFor(container);

builder.AddProject<Projects.aspire_cosmos_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
