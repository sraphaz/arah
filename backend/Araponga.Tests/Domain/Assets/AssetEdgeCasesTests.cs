using Araponga.Domain.Assets;
using Xunit;

namespace Araponga.Tests.Domain.Assets;

/// <summary>
/// Edge case tests for TerritoryAsset domain entity,
/// focusing on invalid GeoAnchors, status transitions, and Unicode in names/descriptions.
/// </summary>
public class AssetEdgeCasesTests
{
    private static readonly DateTime TestDate = DateTime.UtcNow;
    private static readonly Guid TestTerritoryId = Guid.NewGuid();
    private static readonly Guid TestUserId = Guid.NewGuid();

    [Fact]
    public void TerritoryAsset_Constructor_WithValidData_CreatesSuccessfully()
    {
        var asset = new TerritoryAsset(
            Guid.NewGuid(),
            TestTerritoryId,
            "natural",
            "Parque",
            "Descrição",
            AssetStatus.Active,
            TestUserId,
            TestDate,
            TestUserId,
            TestDate,
            null,
            null,
            null);

        Assert.Equal("Parque", asset.Name);
        Assert.Equal(AssetStatus.Active, asset.Status);
    }

    [Fact]
    public void TerritoryAsset_Constructor_WithEmptyTerritoryId_Allows()
    {
        // TerritoryAsset não valida TerritoryId no construtor
        var asset = new TerritoryAsset(
            Guid.NewGuid(),
            Guid.Empty,
            "natural",
            "Parque",
            null,
            AssetStatus.Active,
            TestUserId,
            TestDate,
            TestUserId,
            TestDate,
            null,
            null,
            null);

        Assert.Equal(Guid.Empty, asset.TerritoryId);
    }

    [Fact]
    public void TerritoryAsset_Constructor_WithUnicodeName_StoresCorrectly()
    {
        var asset = new TerritoryAsset(
            Guid.NewGuid(),
            TestTerritoryId,
            "cultural",
            "Café & Cia 🏪",
            null,
            AssetStatus.Active,
            TestUserId,
            TestDate,
            TestUserId,
            TestDate,
            null,
            null,
            null);

        Assert.Contains("Café", asset.Name);
        Assert.Contains("🏪", asset.Name);
    }

    [Fact]
    public void TerritoryAsset_Constructor_WithUnicodeDescription_StoresCorrectly()
    {
        var asset = new TerritoryAsset(
            Guid.NewGuid(),
            TestTerritoryId,
            "natural",
            "Parque",
            "Descrição com café, naïve, résumé, 文字 e emoji 🏞️",
            AssetStatus.Active,
            TestUserId,
            TestDate,
            TestUserId,
            TestDate,
            null,
            null,
            null);

        Assert.Contains("café", asset.Description!);
        Assert.Contains("文字", asset.Description!);
        Assert.Contains("🏞️", asset.Description!);
    }

    [Fact]
    public void TerritoryAsset_UpdateDetails_WithUnicode_UpdatesSuccessfully()
    {
        var asset = new TerritoryAsset(
            Guid.NewGuid(),
            TestTerritoryId,
            "natural",
            "Original",
            null,
            AssetStatus.Active,
            TestUserId,
            TestDate,
            TestUserId,
            TestDate,
            null,
            null,
            null);

        asset.UpdateDetails(
            "cultural",
            "Atualizado: Café & Cia 🏪",
            "Nova descrição com 文字",
            TestUserId,
            TestDate.AddHours(1));

        Assert.Contains("Café", asset.Name);
        Assert.Contains("文字", asset.Description!);
    }

    [Fact]
    public void TerritoryAsset_Archive_UpdatesStatus()
    {
        var asset = new TerritoryAsset(
            Guid.NewGuid(),
            TestTerritoryId,
            "natural",
            "Parque",
            null,
            AssetStatus.Active,
            TestUserId,
            TestDate,
            TestUserId,
            TestDate,
            null,
            null,
            null);

        asset.Archive(TestUserId, TestDate.AddHours(1), "Reason", TestUserId, TestDate.AddHours(1));

        Assert.Equal(AssetStatus.Archived, asset.Status);
        Assert.Equal("Reason", asset.ArchiveReason);
    }

    [Fact]
    public void TerritoryAsset_Approve_UpdatesStatus()
    {
        var asset = new TerritoryAsset(
            Guid.NewGuid(),
            TestTerritoryId,
            "natural",
            "Parque",
            null,
            AssetStatus.Suggested,
            TestUserId,
            TestDate,
            TestUserId,
            TestDate,
            null,
            null,
            null);

        asset.Approve(TestUserId, TestDate.AddHours(1));

        Assert.Equal(AssetStatus.Active, asset.Status);
    }

    [Fact]
    public void TerritoryAsset_Reject_UpdatesStatus()
    {
        var asset = new TerritoryAsset(
            Guid.NewGuid(),
            TestTerritoryId,
            "natural",
            "Parque",
            null,
            AssetStatus.Suggested,
            TestUserId,
            TestDate,
            TestUserId,
            TestDate,
            null,
            null,
            null);

        asset.Reject(TestUserId, TestDate.AddHours(1), "Invalid");

        Assert.Equal(AssetStatus.Rejected, asset.Status);
        Assert.Equal("Invalid", asset.ArchiveReason);
    }

    [Fact]
    public void TerritoryAsset_Reject_WithNullReason_StoresNull()
    {
        var asset = new TerritoryAsset(
            Guid.NewGuid(),
            TestTerritoryId,
            "natural",
            "Parque",
            null,
            AssetStatus.Suggested,
            TestUserId,
            TestDate,
            TestUserId,
            TestDate,
            null,
            null,
            null);

        asset.Reject(TestUserId, TestDate.AddHours(1), null);

        Assert.Equal(AssetStatus.Rejected, asset.Status);
        Assert.Null(asset.ArchiveReason);
    }

    [Fact]
    public void TerritoryAsset_Reject_WithEmptyReason_StoresNull()
    {
        var asset = new TerritoryAsset(
            Guid.NewGuid(),
            TestTerritoryId,
            "natural",
            "Parque",
            null,
            AssetStatus.Suggested,
            TestUserId,
            TestDate,
            TestUserId,
            TestDate,
            null,
            null,
            null);

        asset.Reject(TestUserId, TestDate.AddHours(1), "   ");

        Assert.Equal(AssetStatus.Rejected, asset.Status);
        Assert.Null(asset.ArchiveReason);
    }

    [Fact]
    public void TerritoryAsset_Constructor_WithAllStatuses_CreatesSuccessfully()
    {
        var statuses = new[]
        {
            AssetStatus.Active,
            AssetStatus.Archived,
            AssetStatus.Suggested,
            AssetStatus.Rejected
        };

        foreach (var status in statuses)
        {
            var asset = new TerritoryAsset(
                Guid.NewGuid(),
                TestTerritoryId,
                "natural",
                "Parque",
                null,
                status,
                TestUserId,
                TestDate,
                TestUserId,
                TestDate,
                null,
                null,
                null);

            Assert.Equal(status, asset.Status);
        }
    }
}
