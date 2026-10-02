using CrookedNumbers.Api.Features.Teams;
using CrookedNumbers.Api.Integrations.Mlb.Models;

namespace CrookedNumbers.Api.Tests;

public sealed class TeamMapperTests
{
    [Fact]
    public void MapsTeamDetailsWithoutLeakingMlbModel()
    {
        var result = TeamMapper.ToDetails(TestData.Team);

        Assert.Equal("NYY", result.Abbreviation);
        Assert.Equal("American League East", result.Division);
        Assert.Equal("Yankee Stadium", result.Venue);
    }

    [Fact]
    public void KeepsInningsPitchedAsDisplayText()
    {
        var split = new MlbStatSplit(
            new MlbPerson(42, "Pitcher Name"),
            new MlbPlayerStat { InningsPitched = "12.1" }
        );

        var result = TeamMapper.ToPitching(split);

        Assert.Equal("12.1", result.InningsPitched);
    }
}
