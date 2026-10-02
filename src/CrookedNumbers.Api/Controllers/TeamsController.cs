using System.ComponentModel.DataAnnotations;
using CrookedNumbers.Api.Features.Teams;
using Microsoft.AspNetCore.Mvc;

namespace CrookedNumbers.Api.Controllers;

[ApiController]
[Route("api/teams")]
public sealed class TeamsController(
    GetTeamsHandler getTeams,
    GetTeamHandler getTeam,
    GetTeamRosterHandler getRoster,
    GetTeamStatsHandler getStats,
    GetTeamScheduleHandler getSchedule
) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<TeamSummary>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<TeamSummary>>> GetTeams(
        CancellationToken cancellationToken
    ) => Ok(await getTeams.HandleAsync(cancellationToken));

    [HttpGet("{teamId:int}")]
    [ProducesResponseType<TeamDetails>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeamDetails>> GetTeam(
        [Range(1, int.MaxValue)] int teamId,
        CancellationToken cancellationToken
    ) => Ok(await getTeam.HandleAsync(teamId, cancellationToken));

    [HttpGet("{teamId:int}/roster")]
    [ProducesResponseType<TeamRosterResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<TeamRosterResponse>> GetRoster(
        [Range(1, int.MaxValue)] int teamId,
        CancellationToken cancellationToken
    ) => Ok(await getRoster.HandleAsync(teamId, cancellationToken));

    [HttpGet("{teamId:int}/stats")]
    [ProducesResponseType<TeamStatsResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<TeamStatsResponse>> GetStats(
        [Range(1, int.MaxValue)] int teamId,
        CancellationToken cancellationToken
    ) => Ok(await getStats.HandleAsync(teamId, cancellationToken));

    [HttpGet("{teamId:int}/schedule")]
    [ProducesResponseType<TeamScheduleResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TeamScheduleResponse>> GetSchedule(
        [Range(1, int.MaxValue)] int teamId,
        [FromQuery] DateOnly? startDate,
        [FromQuery] DateOnly? endDate,
        CancellationToken cancellationToken
    ) => Ok(await getSchedule.HandleAsync(teamId, startDate, endDate, cancellationToken));
}
