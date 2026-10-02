using CrookedNumbers.Api.Integrations.Mlb.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace CrookedNumbers.Api.Integrations.Mlb;

public sealed class MlbTeamLookup(
    IMlbApiClient client,
    IMemoryCache cache,
    IOptions<MlbApiOptions> options
)
{
    public async Task<MlbTeam> GetRequiredAsync(int teamId, CancellationToken cancellationToken)
    {
        return (
            await cache.GetOrCreateAsync(
                $"mlb:team:{teamId}:source",
                async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(
                        options.Value.TeamCacheMinutes
                    );
                    return await client.GetTeamAsync(teamId, cancellationToken)
                        ?? throw new MlbNotFoundException($"team {teamId}");
                }
            )
        )!;
    }
}
