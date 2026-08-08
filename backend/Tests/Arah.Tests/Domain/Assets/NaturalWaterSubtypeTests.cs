using Arah.Modules.Assets.Domain;
using Xunit;

namespace Arah.Tests.Domain.Assets;

public class NaturalWaterSubtypeTests
{
    [Theory]
    [InlineData("river")]
    [InlineData("STREAM")]
    [InlineData(" Spring ")]
    [InlineData("waterfall")]
    [InlineData("well")]
    [InlineData("potable_water")]
    public void TryValidate_NaturalWithAllowedSubtype_Succeeds(string subtype)
    {
        var normalized = NaturalWaterSubtype.Normalize(subtype);
        Assert.True(NaturalWaterSubtype.TryValidate(NaturalWaterSubtype.NaturalType, normalized, out var error));
        Assert.Null(error);
    }

    [Fact]
    public void TryValidate_SubtypeWithoutNaturalType_Fails()
    {
        Assert.False(NaturalWaterSubtype.TryValidate("cultural", "river", out var error));
        Assert.Contains("natural", error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TryValidate_NaturalWithUnknownSubtype_Fails()
    {
        Assert.False(NaturalWaterSubtype.TryValidate("natural", "lake", out var error));
        Assert.Contains("Invalid natural water subtype", error);
    }

    [Fact]
    public void TryValidate_NullSubtype_Succeeds()
    {
        Assert.True(NaturalWaterSubtype.TryValidate("natural", null, out _));
        Assert.True(NaturalWaterSubtype.TryValidate("cultural", null, out _));
    }

    [Fact]
    public void ToNaturalAssetType_MapsWellToPotableWater()
    {
        Assert.Equal("POTABLE_WATER", NaturalWaterSubtype.ToNaturalAssetType("well"));
        Assert.Equal("RIVER", NaturalWaterSubtype.ToNaturalAssetType("river"));
    }

    [Fact]
    public void TerritoryAsset_StoresSubtype()
    {
        var now = DateTime.UtcNow;
        var asset = new TerritoryAsset(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "natural",
            "Rio Claro",
            null,
            AssetStatus.Suggested,
            Guid.NewGuid(),
            now,
            Guid.NewGuid(),
            now,
            null,
            null,
            null,
            "river");

        Assert.Equal("river", asset.Subtype);

        asset.UpdateDetails("natural", "Rio Claro", null, Guid.NewGuid(), now.AddMinutes(1), "stream");
        Assert.Equal("stream", asset.Subtype);
    }
}
