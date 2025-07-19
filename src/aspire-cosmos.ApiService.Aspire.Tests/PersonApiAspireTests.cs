using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Aspire.Hosting.Testing;
using Xunit;

namespace aspire_cosmos.ApiService.Aspire.Tests;

public class PersonApiAspireTests
{
    [Fact]
    public async Task CanCreateAndGetPerson_AspireStyle()
    {
        // Arrange
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.aspire_cosmos_AppHost>();
        await using var app = await builder.BuildAsync();
        await app.StartAsync();

        var httpClient = app.CreateHttpClient("apiservice");

        var person = new { FirstName = "Aspire", LastName = "Test", BirthDate = "2000-01-01T00:00:00Z", Sex = "F" };
        var createResp = await httpClient.PostAsJsonAsync("/people/", person);
        Assert.Equal(HttpStatusCode.Created, createResp.StatusCode);
        var created = await createResp.Content.ReadFromJsonAsync<PersonDto>();
        Assert.NotNull(created);
        Assert.Equal("Aspire", created!.FirstName);

        // Act
        var getResp = await httpClient.GetAsync($"/people/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);
        var fetched = await getResp.Content.ReadFromJsonAsync<PersonDto>();
        Assert.NotNull(fetched);
        Assert.Equal("Aspire", fetched!.FirstName);
    }

    private class PersonDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Sex { get; set; } = string.Empty;
        public System.DateTime BirthDate { get; set; }
    }
}
