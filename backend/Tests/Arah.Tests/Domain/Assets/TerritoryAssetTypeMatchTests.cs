using Arah.Modules.Assets.Domain;
using Xunit;

namespace Arah.Tests.Domain.Assets;

public sealed class TerritoryAssetTypeMatchTests
{
    [Fact]
    public void Matches_Types_MatchesTypeOnly()
    {
        var types = new[] { "river" };
        Assert.True(TerritoryAssetTypeMatch.Matches("river", null, types, null));
        Assert.False(TerritoryAssetTypeMatch.Matches("natural", "river", types, null));
        Assert.False(TerritoryAssetTypeMatch.Matches("cultural", null, types, null));

        var mixedTypes = new[] { "RiVeR" };
        Assert.True(TerritoryAssetTypeMatch.Matches("river", null, mixedTypes, null));
        Assert.True(TerritoryAssetTypeMatch.Matches("RIVER", null, mixedTypes, null));
        Assert.False(TerritoryAssetTypeMatch.Matches("natural", "RiVeR", mixedTypes, null));
    }

    [Fact]
    public void Matches_TypesOrSubtypes_MatchesTypeOrSubtype()
    {
        var keys = new[] { "river" };
        Assert.True(TerritoryAssetTypeMatch.Matches("river", null, null, null, keys));
        Assert.True(TerritoryAssetTypeMatch.Matches("natural", "river", null, null, keys));
        Assert.False(TerritoryAssetTypeMatch.Matches("natural", "spring", null, null, keys));
        Assert.False(TerritoryAssetTypeMatch.Matches("cultural", null, null, null, keys));

        var mixedKeys = new[] { "RiVeR" };
        Assert.True(TerritoryAssetTypeMatch.Matches("RIVER", null, null, null, mixedKeys));
        Assert.True(TerritoryAssetTypeMatch.Matches("natural", "river", null, null, mixedKeys));
        Assert.False(TerritoryAssetTypeMatch.Matches("natural", "SPRING", null, null, mixedKeys));
    }

    [Fact]
    public void Matches_Subtypes_RequiresSubtype()
    {
        var subtypes = new[] { "river", "stream" };
        Assert.True(TerritoryAssetTypeMatch.Matches("natural", "river", null, subtypes));
        Assert.False(TerritoryAssetTypeMatch.Matches("natural", "spring", null, subtypes));
        Assert.False(TerritoryAssetTypeMatch.Matches("river", null, null, subtypes));

        var mixedSubtypes = new[] { "RiVeR", "STREAM" };
        Assert.True(TerritoryAssetTypeMatch.Matches("natural", "river", null, mixedSubtypes));
        Assert.False(TerritoryAssetTypeMatch.Matches("natural", "Spring", null, mixedSubtypes));
        Assert.False(TerritoryAssetTypeMatch.Matches("RIVER", null, null, mixedSubtypes));
    }

    [Fact]
    public void Matches_TypesAndSubtypes_AppliesBoth()
    {
        var types = new[] { "natural" };
        var subtypes = new[] { "river" };
        Assert.True(TerritoryAssetTypeMatch.Matches("natural", "river", types, subtypes));
        Assert.False(TerritoryAssetTypeMatch.Matches("natural", "spring", types, subtypes));
        Assert.False(TerritoryAssetTypeMatch.Matches("cultural", "river", types, subtypes));

        var mixedTypes = new[] { "NaTuRaL" };
        var mixedSubtypes = new[] { "RiVeR" };
        Assert.True(TerritoryAssetTypeMatch.Matches("natural", "river", mixedTypes, mixedSubtypes));
        Assert.False(TerritoryAssetTypeMatch.Matches("NATURAL", "spring", mixedTypes, mixedSubtypes));
        Assert.False(TerritoryAssetTypeMatch.Matches("cultural", "RIVER", mixedTypes, mixedSubtypes));
    }
}
