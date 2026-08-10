using Arah.Modules.Assets.Domain;
using Xunit;

namespace Arah.Tests.Domain.Assets;

/// <summary>
/// Domínio NaturalAsset ponto (WA-N1 / FASE24.0a). Cobre AC-WA-1 (parcial ponto), AC-WA-2, AC-WA-6.
/// </summary>
public sealed class WaterBodyDomainTests
{
    private static readonly Guid TerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    [Theory]
    [InlineData("SPRING")]
    [InlineData("waterfall")]
    [InlineData("POTABLE_WATER")]
    public void CreatePending_PointTypes_StartsPending(string type)
    {
        var now = DateTime.UtcNow;
        var asset = NaturalAsset.CreatePending(
            TerritoryId,
            type,
            "Nascente do Morro",
            "Cuidado coletivo",
            new WaterPointDetails(-23.55, -46.63),
            UserId,
            now);

        Assert.Equal(NaturalAssetStatus.Pending, asset.Status);
        Assert.Equal(type.Trim().ToUpperInvariant(), asset.Type);
        Assert.Equal(TerritoryId, asset.TerritoryId);
        Assert.Equal(-23.55, asset.WaterPoint.Latitude);
    }

    [Fact]
    public void CreatePending_RequiresValidGeo()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            NaturalAsset.CreatePending(
                TerritoryId,
                NaturalAssetType.Spring,
                "X",
                null,
                new WaterPointDetails(999, 0),
                UserId,
                DateTime.UtcNow));
    }

    [Fact]
    public void CreatePending_River_RejectedInThisSlice()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            NaturalAsset.CreatePending(
                TerritoryId,
                "RIVER",
                "Rio",
                null,
                new WaterPointDetails(-23.55, -46.63),
                UserId,
                DateTime.UtcNow));

        Assert.Contains("WATERCOURSE_DETAILS", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Publish_FromPending_SetsPublished()
    {
        var now = DateTime.UtcNow;
        var asset = NaturalAsset.CreatePending(
            TerritoryId,
            NaturalAssetType.Spring,
            "Nascente",
            null,
            new WaterPointDetails(-23.55, -46.63, WaterPointWaterType.Well),
            UserId,
            now);

        var curator = Guid.NewGuid();
        asset.Publish(curator, now.AddMinutes(1));

        Assert.Equal(NaturalAssetStatus.Published, asset.Status);
        Assert.Equal(curator, asset.UpdatedByUserId);
    }

    [Fact]
    public void Publish_FromPublished_Throws()
    {
        var now = DateTime.UtcNow;
        var asset = NaturalAsset.CreatePending(
            TerritoryId,
            NaturalAssetType.Waterfall,
            "Cachoeira",
            null,
            new WaterPointDetails(-23.55, -46.63),
            UserId,
            now);
        asset.Publish(UserId, now.AddMinutes(1));

        Assert.Throws<InvalidOperationException>(() => asset.Publish(UserId, now.AddMinutes(2)));
    }

    [Fact]
    public void MarketplaceExposure_AlwaysRejected()
    {
        Assert.False(NaturalAsset.IsMarketplaceEligible);

        var asset = NaturalAsset.CreatePending(
            TerritoryId,
            NaturalAssetType.PotableWater,
            "Torneira comunitária",
            null,
            new WaterPointDetails(-23.55, -46.63, WaterPointWaterType.Tap),
            UserId,
            DateTime.UtcNow);

        Assert.False(asset.TryExposeAsMarketplaceItem(out var error));
        Assert.Contains("never sellable", error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void WaterPoint_InvalidWaterType_Throws()
    {
        Assert.Throws<ArgumentException>(() => new WaterPointDetails(-23.55, -46.63, "LAKE"));
    }
}
