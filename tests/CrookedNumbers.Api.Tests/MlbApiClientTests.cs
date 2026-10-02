using System.Net;
using CrookedNumbers.Api.Integrations.Mlb;
using Microsoft.Extensions.Logging.Abstractions;

namespace CrookedNumbers.Api.Tests;

public sealed class MlbApiClientTests
{
    [Fact]
    public async Task GetTeamsAsyncDeserializesTheFieldsWeConsume()
    {
        const string json = """
            {
              "teams": [{
                "id": 147,
                "name": "New York Yankees",
                "abbreviation": "NYY",
                "teamName": "Yankees",
                "locationName": "Bronx",
                "active": true,
                "venue": { "id": 3313, "name": "Yankee Stadium" },
                "division": { "id": 201, "name": "American League East" }
              }]
            }
            """;
        using var httpClient = CreateClient(HttpStatusCode.OK, json);
        var client = new MlbApiClient(httpClient, NullLogger<MlbApiClient>.Instance);

        var teams = await client.GetTeamsAsync(CancellationToken.None);

        var team = Assert.Single(teams);
        Assert.Equal(147, team.Id);
        Assert.Equal("NYY", team.Abbreviation);
        Assert.Equal("Yankee Stadium", team.Venue?.Name);
    }

    [Fact]
    public async Task GetTeamAsyncThrowsForMalformedJson()
    {
        using var httpClient = CreateClient(HttpStatusCode.OK, "not-json");
        var client = new MlbApiClient(httpClient, NullLogger<MlbApiClient>.Instance);

        await Assert.ThrowsAsync<MlbResponseException>(() =>
            client.GetTeamAsync(147, CancellationToken.None)
        );
    }

    [Fact]
    public async Task GetTeamAsyncThrowsForNotFoundResponse()
    {
        using var httpClient = CreateClient(HttpStatusCode.NotFound, "{}");
        var client = new MlbApiClient(httpClient, NullLogger<MlbApiClient>.Instance);

        await Assert.ThrowsAsync<MlbNotFoundException>(() =>
            client.GetTeamAsync(9999, CancellationToken.None)
        );
    }

    private static HttpClient CreateClient(HttpStatusCode statusCode, string content)
    {
        return new HttpClient(new StubHttpMessageHandler(statusCode, content))
        {
            BaseAddress = new Uri("https://statsapi.mlb.com/api/"),
        };
    }

    private sealed class StubHttpMessageHandler(HttpStatusCode statusCode, string content)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        ) =>
            Task.FromResult(
                new HttpResponseMessage(statusCode) { Content = new StringContent(content) }
            );
    }
}
