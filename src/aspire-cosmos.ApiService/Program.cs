
using Aspire.Microsoft.EntityFrameworkCore.Cosmos;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();


// Add services to the container.
builder.Services.AddProblemDetails();

// Add EF Core Cosmos DB context using Aspire integration
builder.AddCosmosDbContext<aspire_cosmos.ApiService.Data.PersonDbContext>("appdb");



// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();





var app = builder.Build();



// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


// Map Person minimal APIs
aspire_cosmos.ApiService.Endpoints.PersonEndpoints.MapPersonEndpoints(app);

app.MapDefaultEndpoints();

app.Run();
