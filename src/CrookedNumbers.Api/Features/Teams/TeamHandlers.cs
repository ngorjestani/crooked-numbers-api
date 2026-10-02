using CrookedNumbers.Api.Integrations.Mlb;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace CrookedNumbers.Api.Features.Teams;

public sealed class GetTeamsHandler(
    IMlbApiClient client,
    IMemoryCache cache,
    IOptions<MlbApiOptions> options
)
{
    public async Task<IReadOnlyList<TeamSummary>> HandleAsync(CancellationToken cancellationToken)
    {
        return (
            await cache.GetOrCreateAsync(
                "mlb:teams",
                async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(
                        options.Value.TeamsCacheMinutes
                    );
                    var teams = await client.GetTeamsAsync(cancellationToken);
                    return teams.Where(team => team.Active).Select(TeamMapper.ToSummary).ToArray();
                }
            )
        )!;
    }
}

public sealed class GetTeamHandler(MlbTeamLookup teamLookup)
{
    public async Task<TeamDetails> HandleAsync(int teamId, CancellationToken cancellationToken)
    {
        var team = await teamLookup.GetRequiredAsync(teamId, cancellationToken);
        return TeamMapper.ToDetails(team);
    }
}

public sealed class GetTeamRosterHandler(
    IMlbApiClient client,
    MlbTeamLookup teamLookup,
    IMemoryCache cache,
    IOptions<MlbApiOptions> options
)
{
    public async Task<TeamRosterResponse> HandleAsync(
        int teamId,
        CancellationToken cancellationToken
    )
    {
        _ = await teamLookup.GetRequiredAsync(teamId, cancellationToken);
        return (
            await cache.GetOrCreateAsync(
                $"mlb:team:{teamId}:roster",
                async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(
                        options.Value.RosterCacheMinutes
                    );
                    var roster = await client.GetRosterAsync(teamId, cancellationToken);
                    return new TeamRosterResponse(
                        roster
                            .Select(TeamMapper.ToPlayer)
                            .OrderBy(player => player.Position)
                            .ThenBy(player => player.FullName)
                            .ToArray()
                    );
                }
            )
        )!;
    }
}

public sealed class GetTeamStatsHandler(
    IMlbApiClient client,
    MlbTeamLookup teamLookup,
    IMemoryCache cache,
    IOptions<MlbApiOptions> options,
    TimeProvider timeProvider
)
{
    public async Task<TeamStatsResponse> HandleAsync(
        int teamId,
        CancellationToken cancellationToken
    )
    {
        _ = await teamLookup.GetRequiredAsync(teamId, cancellationToken);
        var season = timeProvider.GetUtcNow().Year;
        return (
            await cache.GetOrCreateAsync(
                $"mlb:team:{teamId}:stats:{season}",
                async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(
                        options.Value.StatsCacheMinutes
                    );
                    var battingTask = client.GetStatsAsync(
                        teamId,
                        "hitting",
                        season,
                        cancellationToken
                    );
                    var pitchingTask = client.GetStatsAsync(
                        teamId,
                        "pitching",
                        season,
                        cancellationToken
                    );
                    await Task.WhenAll(battingTask, pitchingTask);
                    var batting = (await battingTask).Stats.SelectMany(group => group.Splits);
                    var pitching = (await pitchingTask).Stats.SelectMany(group => group.Splits);
                    return new TeamStatsResponse(
                        batting
                            .Select(TeamMapper.ToBatting)
                            .OrderByDescending(player => player.PlateAppearances)
                            .ToArray(),
                        pitching.Select(TeamMapper.ToPitching).ToArray()
                    );
                }
            )
        )!;
    }
}

public sealed class GetTeamScheduleHandler(
    IMlbApiClient client,
    MlbTeamLookup teamLookup,
    IMemoryCache cache,
    IOptions<MlbApiOptions> options,
    TimeProvider timeProvider
)
{
    public async Task<TeamScheduleResponse> HandleAsync(
        int teamId,
        DateOnly? startDate,
        DateOnly? endDate,
        CancellationToken cancellationToken
    )
    {
        _ = await teamLookup.GetRequiredAsync(teamId, cancellationToken);
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        var from = startDate ?? today.AddDays(-14);
        var through = endDate ?? today.AddDays(14);
        if (through < from)
        {
            throw new ArgumentException("endDate must be on or after startDate.");
        }

        if (through.DayNumber - from.DayNumber > 93)
        {
            throw new ArgumentException("The schedule window cannot exceed 93 days.");
        }

        return (
            await cache.GetOrCreateAsync(
                $"mlb:team:{teamId}:schedule:{from:yyyyMMdd}:{through:yyyyMMdd}",
                async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(
                        options.Value.ScheduleCacheMinutes
                    );
                    var schedule = await client.GetScheduleAsync(
                        teamId,
                        from,
                        through,
                        cancellationToken
                    );
                    return new TeamScheduleResponse(
                        from,
                        through,
                        schedule
                            .Dates.SelectMany(date => date.Games)
                            .Select(TeamMapper.ToGame)
                            .OrderBy(game => game.GameDate)
                            .ToArray()
                    );
                }
            )
        )!;
    }
}
