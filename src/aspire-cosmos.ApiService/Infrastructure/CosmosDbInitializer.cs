using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using aspire_cosmos.ApiService.Data;

namespace aspire_cosmos.ApiService.Infrastructure
{
    /// <summary>
    /// Ensures the Cosmos DB container is created at application startup.
    /// </summary>
    public class CosmosDbInitializer : IHostedService
    {
        private readonly IServiceProvider serviceProvider;

        public CosmosDbInitializer(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PersonDbContext>();
            await db.Database.EnsureCreatedAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
