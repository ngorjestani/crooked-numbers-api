using System.Text.Json.Serialization;

namespace CrookedNumbers.Api.Integrations.Mlb.Models;

public sealed record MlbTeamsResponse(
    [property: JsonPropertyName("teams")] IReadOnlyList<MlbTeam> Teams
);

public sealed record MlbTeam(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("abbreviation")] string? Abbreviation,
    [property: JsonPropertyName("teamName")] string? TeamName,
    [property: JsonPropertyName("locationName")] string? LocationName,
    [property: JsonPropertyName("active")] bool Active,
    [property: JsonPropertyName("venue")] MlbNamedEntity? Venue,
    [property: JsonPropertyName("division")] MlbNamedEntity? Division
);

public sealed record MlbNamedEntity(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name
);

public sealed record MlbRosterResponse(
    [property: JsonPropertyName("roster")] IReadOnlyList<MlbRosterEntry> Roster
);

public sealed record MlbRosterEntry(
    [property: JsonPropertyName("person")] MlbPerson Person,
    [property: JsonPropertyName("jerseyNumber")] string? JerseyNumber,
    [property: JsonPropertyName("position")] MlbPosition Position
);

public sealed record MlbPerson(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("fullName")] string FullName
);

public sealed record MlbPosition(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("abbreviation")] string Abbreviation
);

public sealed record MlbStatsResponse(
    [property: JsonPropertyName("stats")] IReadOnlyList<MlbStatGroup> Stats
);

public sealed record MlbStatGroup(
    [property: JsonPropertyName("splits")] IReadOnlyList<MlbStatSplit> Splits
);

public sealed record MlbStatSplit(
    [property: JsonPropertyName("player")] MlbPerson Player,
    [property: JsonPropertyName("stat")] MlbPlayerStat Stat
);

public sealed record MlbPlayerStat
{
    [JsonPropertyName("gamesPlayed")]
    public int GamesPlayed { get; init; }

    [JsonPropertyName("plateAppearances")]
    public int PlateAppearances { get; init; }

    [JsonPropertyName("atBats")]
    public int AtBats { get; init; }

    [JsonPropertyName("runs")]
    public int Runs { get; init; }

    [JsonPropertyName("hits")]
    public int Hits { get; init; }

    [JsonPropertyName("doubles")]
    public int Doubles { get; init; }

    [JsonPropertyName("triples")]
    public int Triples { get; init; }

    [JsonPropertyName("homeRuns")]
    public int HomeRuns { get; init; }

    [JsonPropertyName("rbi")]
    public int Rbi { get; init; }

    [JsonPropertyName("baseOnBalls")]
    public int Walks { get; init; }

    [JsonPropertyName("strikeOuts")]
    public int Strikeouts { get; init; }

    [JsonPropertyName("stolenBases")]
    public int StolenBases { get; init; }

    [JsonPropertyName("avg")]
    public string? BattingAverage { get; init; }

    [JsonPropertyName("obp")]
    public string? OnBasePercentage { get; init; }

    [JsonPropertyName("slg")]
    public string? SluggingPercentage { get; init; }

    [JsonPropertyName("ops")]
    public string? Ops { get; init; }

    [JsonPropertyName("gamesPitched")]
    public int GamesPitched { get; init; }

    [JsonPropertyName("gamesStarted")]
    public int GamesStarted { get; init; }

    [JsonPropertyName("inningsPitched")]
    public string? InningsPitched { get; init; }

    [JsonPropertyName("wins")]
    public int Wins { get; init; }

    [JsonPropertyName("losses")]
    public int Losses { get; init; }

    [JsonPropertyName("era")]
    public string? Era { get; init; }

    [JsonPropertyName("whip")]
    public string? Whip { get; init; }
}

public sealed record MlbScheduleResponse(
    [property: JsonPropertyName("dates")] IReadOnlyList<MlbScheduleDate> Dates
);

public sealed record MlbScheduleDate(
    [property: JsonPropertyName("games")] IReadOnlyList<MlbGame> Games
);

public sealed record MlbGame(
    [property: JsonPropertyName("gamePk")] long GamePk,
    [property: JsonPropertyName("gameDate")] DateTimeOffset GameDate,
    [property: JsonPropertyName("status")] MlbGameStatus Status,
    [property: JsonPropertyName("teams")] MlbGameTeams Teams,
    [property: JsonPropertyName("venue")] MlbNamedEntity? Venue
);

public sealed record MlbGameStatus(
    [property: JsonPropertyName("abstractGameState")] string AbstractGameState,
    [property: JsonPropertyName("detailedState")] string DetailedState
);

public sealed record MlbGameTeams(
    [property: JsonPropertyName("away")] MlbGameTeam Away,
    [property: JsonPropertyName("home")] MlbGameTeam Home
);

public sealed record MlbGameTeam(
    [property: JsonPropertyName("score")] int? Score,
    [property: JsonPropertyName("team")] MlbNamedEntity Team
);
