using CrookedNumbers.Api.Features.Teams;
using CrookedNumbers.Api.Integrations.Mlb;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace CrookedNumbers.Api.Tests;

public sealed class TeamHandlerTests
{
    private static readonly IOptions<MlbApiOptions> Options =
        Microsoft.Extensions.Options.Options.Create(
            new MlbApiOptions { BaseUrl = new Uri("https://example.test/api/") }
        );

    [Fact]
    public async Task GetTeamsCachesTransformedResponse()
    {
        var client = new FakeMlbApiClient();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var handler = new GetTeamsHandler(client, cache, Options);

        var first = await handler.HandleAsync(CancellationToken.None);
        var second = await handler.HandleAsync(CancellationToken.None);

        Assert.Single(first);
        Assert.Same(first, second);
        Assert.Equal(1, client.TeamsCallCount);
    }

    [Fact]
    public async Task GetTeamThrowsWhenUpstreamReturnsNoTeam()
    {
        var client = new FakeMlbApiClient();
        using var cache = new MemoryCache(new MemoryCacheOptions());
        var handler = new GetTeamHandler(new MlbTeamLookup(client, cache, Options));

        await Assert.ThrowsAsync<MlbNotFoundException>(() =>
            handler.HandleAsync(9999, CancellationToken.None)
        );
    }
}
