namespace CrookedNumbers.Api.Integrations.Mlb;

public sealed class MlbApiOptions
{
    public const string SectionName = "MlbApi";

    public required Uri BaseUrl { get; init; }
    public int TeamsCacheMinutes { get; init; } = 60;
    public int TeamCacheMinutes { get; init; } = 30;
    public int RosterCacheMinutes { get; init; } = 15;
    public int StatsCacheMinutes { get; init; } = 10;
    public int ScheduleCacheMinutes { get; init; } = 5;
}
