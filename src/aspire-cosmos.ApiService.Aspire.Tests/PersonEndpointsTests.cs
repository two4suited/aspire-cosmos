using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using aspire_cosmos.ApiService.Models;
using Aspire.Hosting.Testing;
using Xunit;

namespace aspire_cosmos.ApiService.Aspire.Tests;

public class PersonEndpointsTests
{
    [Fact]
    public async Task CanCreatePerson()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.aspire_cosmos_AppHost>();
        await using var app = await builder.BuildAsync();
        await app.StartAsync();

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        await app.ResourceNotifications.WaitForResourceHealthyAsync("apiservice", cts.Token);

        var client = app.CreateHttpClient("apiservice");
        var person = new Person { FirstName = "John", LastName = "Doe", BirthDate = new DateTime(1990, 1, 1), Sex = "M" };
        var response = await client.PostAsJsonAsync("/people/", person);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<Person>();
        Assert.NotNull(created);
        Assert.Equal("John", created!.FirstName);
    }

    [Fact]
    public async Task CanGetAllPeople()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.aspire_cosmos_AppHost>();
        await using var app = await builder.BuildAsync();
        await app.StartAsync();

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        await app.ResourceNotifications.WaitForResourceHealthyAsync("apiservice", cts.Token);

        var client = app.CreateHttpClient("apiservice");
        var person = new Person { FirstName = "Jane", LastName = "Smith", BirthDate = new DateTime(1985, 5, 5), Sex = "F" };
        await client.PostAsJsonAsync("/people/", person);
        var response = await client.GetAsync("/people/");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var people = await response.Content.ReadFromJsonAsync<Person[]>();
        Assert.NotNull(people);
        Assert.Contains(people!, p => p.FirstName == "Jane");
    }

    [Fact]
    public async Task CanGetPersonById()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.aspire_cosmos_AppHost>();
        await using var app = await builder.BuildAsync();
        await app.StartAsync();

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        await app.ResourceNotifications.WaitForResourceHealthyAsync("apiservice", cts.Token);

        var client = app.CreateHttpClient("apiservice");
        var person = new Person { FirstName = "Alice", LastName = "Johnson", BirthDate = new DateTime(2000, 2, 2), Sex = "F" };
        var createResp = await client.PostAsJsonAsync("/people/", person);
        var created = await createResp.Content.ReadFromJsonAsync<Person>();
        var response = await client.GetAsync($"/people/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var fetched = await response.Content.ReadFromJsonAsync<Person>();
        Assert.NotNull(fetched);
        Assert.Equal("Alice", fetched!.FirstName);
    }

    [Fact]
    public async Task CanUpdatePerson()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.aspire_cosmos_AppHost>();
        await using var app = await builder.BuildAsync();
        await app.StartAsync();

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        await app.ResourceNotifications.WaitForResourceHealthyAsync("apiservice", cts.Token);

        var client = app.CreateHttpClient("apiservice");
        var person = new Person { FirstName = "Bob", LastName = "Brown", BirthDate = new DateTime(1975, 3, 3), Sex = "M" };
        var createResp = await client.PostAsJsonAsync("/people/", person);
        var created = await createResp.Content.ReadFromJsonAsync<Person>();
        created!.FirstName = "Robert";
        var updateResp = await client.PutAsJsonAsync($"/people/{created.Id}", created);
        Assert.Equal(HttpStatusCode.NoContent, updateResp.StatusCode);
        var getResp = await client.GetAsync($"/people/{created.Id}");
        var updated = await getResp.Content.ReadFromJsonAsync<Person>();
        Assert.Equal("Robert", updated!.FirstName);
    }

    [Fact]
    public async Task CanDeletePerson()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.aspire_cosmos_AppHost>();
        await using var app = await builder.BuildAsync();
        await app.StartAsync();

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        await app.ResourceNotifications.WaitForResourceHealthyAsync("apiservice", cts.Token);

        var client = app.CreateHttpClient("apiservice");
        var person = new Person { FirstName = "Eve", LastName = "White", BirthDate = new DateTime(1995, 4, 4), Sex = "F" };
        var createResp = await client.PostAsJsonAsync("/people/", person);
        var created = await createResp.Content.ReadFromJsonAsync<Person>();
        var deleteResp = await client.DeleteAsync($"/people/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResp.StatusCode);
        var getResp = await client.GetAsync($"/people/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResp.StatusCode);
    }
}
