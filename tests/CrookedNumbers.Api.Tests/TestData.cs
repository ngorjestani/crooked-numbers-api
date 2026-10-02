using CrookedNumbers.Api.Integrations.Mlb;
using CrookedNumbers.Api.Integrations.Mlb.Models;

namespace CrookedNumbers.Api.Tests;

internal static class TestData
{
    public static readonly MlbTeam Team = new(
        147,
        "New York Yankees",
        "NYY",
        "Yankees",
        "Bronx",
        true,
        new MlbNamedEntity(3313, "Yankee Stadium"),
        new MlbNamedEntity(201, "American League East")
    );
}

internal sealed class FakeMlbApiClient : IMlbApiClient
{
    public int TeamsCallCount { get; private set; }

    public Task<IReadOnlyList<MlbTeam>> GetTeamsAsync(CancellationToken cancellationToken)
    {
        TeamsCallCount++;
        return Task.FromResult<IReadOnlyList<MlbTeam>>([TestData.Team]);
    }

    public Task<MlbTeam?> GetTeamAsync(int teamId, CancellationToken cancellationToken) =>
        Task.FromResult<MlbTeam?>(teamId == TestData.Team.Id ? TestData.Team : null);

    public Task<IReadOnlyList<MlbRosterEntry>> GetRosterAsync(
        int teamId,
        CancellationToken cancellationToken
    ) => Task.FromResult<IReadOnlyList<MlbRosterEntry>>([]);

    public Task<MlbStatsResponse> GetStatsAsync(
        int teamId,
        string group,
        int season,
        CancellationToken cancellationToken
    ) => Task.FromResult(new MlbStatsResponse([]));

    public Task<MlbScheduleResponse> GetScheduleAsync(
        int teamId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken
    ) => Task.FromResult(new MlbScheduleResponse([]));
}
