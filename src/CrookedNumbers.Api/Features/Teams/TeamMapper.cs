using CrookedNumbers.Api.Integrations.Mlb.Models;

namespace CrookedNumbers.Api.Features.Teams;

public static class TeamMapper
{
    public static TeamSummary ToSummary(MlbTeam team) =>
        new(team.Id, team.Name, team.Abbreviation ?? string.Empty);

    public static TeamDetails ToDetails(MlbTeam team) =>
        new(
            team.Id,
            team.Name,
            team.Abbreviation ?? string.Empty,
            team.TeamName,
            team.LocationName,
            team.Division?.Name,
            team.Venue?.Name
        );

    public static PlayerSummary ToPlayer(MlbRosterEntry entry) =>
        new(
            entry.Person.Id,
            entry.Person.FullName,
            entry.JerseyNumber,
            entry.Position.Name,
            entry.Position.Abbreviation
        );

    public static PlayerBattingStats ToBatting(MlbStatSplit split) =>
        new(
            split.Player.Id,
            split.Player.FullName,
            split.Stat.GamesPlayed,
            split.Stat.PlateAppearances,
            split.Stat.AtBats,
            split.Stat.Runs,
            split.Stat.Hits,
            split.Stat.Doubles,
            split.Stat.Triples,
            split.Stat.HomeRuns,
            split.Stat.Rbi,
            split.Stat.Walks,
            split.Stat.Strikeouts,
            split.Stat.StolenBases,
            split.Stat.BattingAverage ?? ".000",
            split.Stat.OnBasePercentage ?? ".000",
            split.Stat.SluggingPercentage ?? ".000",
            split.Stat.Ops ?? ".000"
        );

    public static PlayerPitchingStats ToPitching(MlbStatSplit split) =>
        new(
            split.Player.Id,
            split.Player.FullName,
            split.Stat.GamesPitched,
            split.Stat.GamesStarted,
            split.Stat.InningsPitched ?? "0.0",
            split.Stat.Wins,
            split.Stat.Losses,
            split.Stat.Era ?? "-.--",
            split.Stat.Whip ?? "-.--",
            split.Stat.Strikeouts,
            split.Stat.Walks,
            split.Stat.Hits,
            split.Stat.HomeRuns
        );

    public static ScheduledGame ToGame(MlbGame game) =>
        new(
            game.GamePk,
            game.GameDate,
            game.Status.AbstractGameState,
            game.Status.DetailedState,
            new ScheduleTeam(game.Teams.Away.Team.Id, game.Teams.Away.Team.Name),
            new ScheduleTeam(game.Teams.Home.Team.Id, game.Teams.Home.Team.Name),
            game.Teams.Away.Score,
            game.Teams.Home.Score,
            game.Venue?.Name
        );
}
