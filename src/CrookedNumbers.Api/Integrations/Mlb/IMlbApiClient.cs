using CrookedNumbers.Api.Integrations.Mlb.Models;

namespace CrookedNumbers.Api.Integrations.Mlb;

public interface IMlbApiClient
{
    Task<IReadOnlyList<MlbTeam>> GetTeamsAsync(CancellationToken cancellationToken);
    Task<MlbTeam?> GetTeamAsync(int teamId, CancellationToken cancellationToken);
    Task<IReadOnlyList<MlbRosterEntry>> GetRosterAsync(
        int teamId,
        CancellationToken cancellationToken
    );
    Task<MlbStatsResponse> GetStatsAsync(
        int teamId,
        string group,
        int season,
        CancellationToken cancellationToken
    );
    Task<MlbScheduleResponse> GetScheduleAsync(
        int teamId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken
    );
}
