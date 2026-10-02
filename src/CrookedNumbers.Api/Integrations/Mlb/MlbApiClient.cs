using System.Net;
using System.Net.Http.Json;
using CrookedNumbers.Api.Integrations.Mlb.Models;

namespace CrookedNumbers.Api.Integrations.Mlb;

public sealed class MlbApiClient(HttpClient httpClient, ILogger<MlbApiClient> logger)
    : IMlbApiClient
{
    public async Task<IReadOnlyList<MlbTeam>> GetTeamsAsync(CancellationToken cancellationToken)
    {
        var response = await GetAsync<MlbTeamsResponse>(
            "v1/teams?sportId=1&hydrate=venue,division",
            cancellationToken
        );
        return response.Teams;
    }

    public async Task<MlbTeam?> GetTeamAsync(int teamId, CancellationToken cancellationToken)
    {
        var response = await GetAsync<MlbTeamsResponse>(
            $"v1/teams/{teamId}?hydrate=venue,division",
            cancellationToken
        );
        return response.Teams.SingleOrDefault();
    }

    public async Task<IReadOnlyList<MlbRosterEntry>> GetRosterAsync(
        int teamId,
        CancellationToken cancellationToken
    )
    {
        var response = await GetAsync<MlbRosterResponse>(
            $"v1/teams/{teamId}/roster?rosterType=active&hydrate=person",
            cancellationToken
        );
        return response.Roster;
    }

    public Task<MlbStatsResponse> GetStatsAsync(
        int teamId,
        string group,
        int season,
        CancellationToken cancellationToken
    ) =>
        GetAsync<MlbStatsResponse>(
            $"v1/stats?stats=season&group={Uri.EscapeDataString(group)}&playerPool=ALL&teamId={teamId}&season={season}&hydrate=player",
            cancellationToken
        );

    public Task<MlbScheduleResponse> GetScheduleAsync(
        int teamId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken
    ) =>
        GetAsync<MlbScheduleResponse>(
            $"v1/schedule?sportId=1&teamId={teamId}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&hydrate=venue",
            cancellationToken
        );

    private async Task<T> GetAsync<T>(string requestUri, CancellationToken cancellationToken)
    {
        try
        {
            using var response = await httpClient.GetAsync(requestUri, cancellationToken);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new MlbNotFoundException(requestUri);
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(cancellationToken)
                ?? throw new MlbResponseException("MLB returned an empty response.");
        }
        catch (MlbApiException)
        {
            throw;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning("The MLB request timed out: {RequestUri}", requestUri);
            throw new MlbApiException("The MLB service timed out.");
        }
        catch (HttpRequestException exception)
        {
            logger.LogWarning(exception, "The MLB request failed: {RequestUri}", requestUri);
            throw new MlbApiException("The MLB service is currently unavailable.", exception);
        }
        catch (System.Text.Json.JsonException exception)
        {
            logger.LogError(exception, "MLB returned malformed data: {RequestUri}", requestUri);
            throw new MlbResponseException("MLB returned an unexpected response.", exception);
        }
        catch (NotSupportedException exception)
        {
            logger.LogError(exception, "MLB returned unsupported data: {RequestUri}", requestUri);
            throw new MlbResponseException("MLB returned an unexpected response.", exception);
        }
    }
}
