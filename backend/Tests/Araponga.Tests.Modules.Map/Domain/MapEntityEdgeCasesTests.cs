using Araponga.Modules.Map.Domain;
using Xunit;

namespace Araponga.Tests.Modules.Map.Domain;

/// <summary>
/// Edge case tests for MapEntity domain entity,
/// focusing on validation, ConfirmationCount overflow, and category branches.
/// </summary>
public sealed class MapEntityEdgeCasesTests
{
    private static readonly Guid TerritoryId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly DateTime CreatedAt = DateTime.UtcNow;

    [Fact]
    public void Constructor_WithValidData_CreatesSuccessfully()
    {
        var e = new MapEntity(
            Guid.NewGuid(),
            TerritoryId,
            UserId,
            "Test Entity",
            "estabelecimento",
            0.0,
            0.0,
            MapEntityStatus.Validated,
            MapEntityVisibility.Public,
            0,
            CreatedAt);

        Assert.Equal("Test Entity", e.Name);
        Assert.Equal("estabelecimento", e.Category);
        Assert.Equal(0, e.ConfirmationCount);
    }

    [Fact]
    public void Constructor_WithLargeConfirmationCount_StoresValue()
    {
        var e = new MapEntity(
            Guid.NewGuid(),
            TerritoryId,
            UserId,
            "Test",
            "estabelecimento",
            0.0,
            0.0,
            MapEntityStatus.Validated,
            MapEntityVisibility.Public,
            1_000_000,
            CreatedAt);

        Assert.Equal(1_000_000, e.ConfirmationCount);
    }

    [Fact]
    public void Constructor_WithEmptyTerritoryId_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new MapEntity(
            Guid.NewGuid(),
            Guid.Empty,
            UserId,
            "Test",
            "estabelecimento",
            0.0,
            0.0,
            MapEntityStatus.Validated,
            MapEntityVisibility.Public,
            0,
            CreatedAt));

        Assert.Contains("Territory", ex.Message);
    }

    [Fact]
    public void Constructor_WithEmptyCreatedByUserId_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new MapEntity(
            Guid.NewGuid(),
            TerritoryId,
            Guid.Empty,
            "Test",
            "estabelecimento",
            0.0,
            0.0,
            MapEntityStatus.Validated,
            MapEntityVisibility.Public,
            0,
            CreatedAt));

        Assert.Contains("Created-by", ex.Message);
    }

    [Fact]
    public void Constructor_WithNullName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new MapEntity(
            Guid.NewGuid(),
            TerritoryId,
            UserId,
            null!,
            "estabelecimento",
            0.0,
            0.0,
            MapEntityStatus.Validated,
            MapEntityVisibility.Public,
            0,
            CreatedAt));
    }

    [Fact]
    public void Constructor_WithWhitespaceName_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new MapEntity(
            Guid.NewGuid(),
            TerritoryId,
            UserId,
            "   ",
            "estabelecimento",
            0.0,
            0.0,
            MapEntityStatus.Validated,
            MapEntityVisibility.Public,
            0,
            CreatedAt));
    }

    [Fact]
    public void Constructor_WithInvalidCategory_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new MapEntity(
            Guid.NewGuid(),
            TerritoryId,
            UserId,
            "Test",
            "invalid-category",
            0.0,
            0.0,
            MapEntityStatus.Validated,
            MapEntityVisibility.Public,
            0,
            CreatedAt));

        Assert.Contains("Category", ex.Message);
    }

    [Fact]
    public void Constructor_WithInvalidCoordinates_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MapEntity(
            Guid.NewGuid(),
            TerritoryId,
            UserId,
            "Test",
            "estabelecimento",
            91.0,
            0.0,
            MapEntityStatus.Validated,
            MapEntityVisibility.Public,
            0,
            CreatedAt));
    }

    [Fact]
    public void Constructor_TrimsName()
    {
        var e = new MapEntity(
            Guid.NewGuid(),
            TerritoryId,
            UserId,
            "  Trimmed  ",
            "estabelecimento",
            0.0,
            0.0,
            MapEntityStatus.Validated,
            MapEntityVisibility.Public,
            0,
            CreatedAt);

        Assert.Equal("Trimmed", e.Name);
    }
}
