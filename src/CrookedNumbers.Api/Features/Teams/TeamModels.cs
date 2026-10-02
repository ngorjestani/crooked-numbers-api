namespace CrookedNumbers.Api.Features.Teams;

public sealed record TeamSummary(int Id, string Name, string Abbreviation);

public sealed record TeamDetails(
    int Id,
    string Name,
    string Abbreviation,
    string? TeamName,
    string? LocationName,
    string? Division,
    string? Venue
);

public sealed record PlayerSummary(
    int PlayerId,
    string FullName,
    string? JerseyNumber,
    string Position,
    string PositionAbbreviation
);

public sealed record TeamRosterResponse(IReadOnlyList<PlayerSummary> Players);

public sealed record PlayerBattingStats(
    int PlayerId,
    string PlayerName,
    int GamesPlayed,
    int PlateAppearances,
    int AtBats,
    int Runs,
    int Hits,
    int Doubles,
    int Triples,
    int HomeRuns,
    int Rbi,
    int Walks,
    int Strikeouts,
    int StolenBases,
    string BattingAverage,
    string OnBasePercentage,
    string SluggingPercentage,
    string Ops
);

public sealed record PlayerPitchingStats(
    int PlayerId,
    string PlayerName,
    int Games,
    int GamesStarted,
    string InningsPitched,
    int Wins,
    int Losses,
    string Era,
    string Whip,
    int Strikeouts,
    int Walks,
    int Hits,
    int HomeRuns
);

public sealed record TeamStatsResponse(
    IReadOnlyList<PlayerBattingStats> Batting,
    IReadOnlyList<PlayerPitchingStats> Pitching
);

public sealed record ScheduleTeam(int Id, string Name);

public sealed record ScheduledGame(
    long GamePk,
    DateTimeOffset GameDate,
    string Status,
    string DetailedStatus,
    ScheduleTeam AwayTeam,
    ScheduleTeam HomeTeam,
    int? AwayScore,
    int? HomeScore,
    string? Venue
);

public sealed record TeamScheduleResponse(
    DateOnly StartDate,
    DateOnly EndDate,
    IReadOnlyList<ScheduledGame> Games
);
