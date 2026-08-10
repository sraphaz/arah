using Arah.Modules.Assets.Domain;
using Xunit;

namespace Arah.Tests.Domain.Assets;

public sealed class TerritoryAssetTypeMatchTests
{
    [Fact]
    public void Matches_Types_MatchesTypeOrSubtype()
    {
        var types = new[] { "river" };
        Assert.True(TerritoryAssetTypeMatch.Matches("river", null, types, null));
        Assert.True(TerritoryAssetTypeMatch.Matches("natural", "river", types, null));
        Assert.False(TerritoryAssetTypeMatch.Matches("natural", "spring", types, null));
        Assert.False(TerritoryAssetTypeMatch.Matches("cultural", null, types, null));
    }

    [Fact]
    public void Matches_Subtypes_RequiresSubtype()
    {
        var subtypes = new[] { "river", "stream" };
        Assert.True(TerritoryAssetTypeMatch.Matches("natural", "river", null, subtypes));
        Assert.False(TerritoryAssetTypeMatch.Matches("natural", "spring", null, subtypes));
        Assert.False(TerritoryAssetTypeMatch.Matches("river", null, null, subtypes));
    }

    [Fact]
    public void Matches_TypesAndSubtypes_AppliesBoth()
    {
        var types = new[] { "natural" };
        var subtypes = new[] { "river" };
        Assert.True(TerritoryAssetTypeMatch.Matches("natural", "river", types, subtypes));
        Assert.False(TerritoryAssetTypeMatch.Matches("natural", "spring", types, subtypes));
        Assert.False(TerritoryAssetTypeMatch.Matches("cultural", "river", types, subtypes));
    }
}
