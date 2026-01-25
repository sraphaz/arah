using Araponga.Domain.Territories;
using Xunit;

namespace Araponga.Tests.Domain.Territories;

/// <summary>
/// Edge case tests for TerritoryCharacterization domain entity,
/// focusing on null/empty tags, normalization, and UpdateTags branches.
/// </summary>
public sealed class TerritoryCharacterizationEdgeCasesTests
{
    private static readonly Guid TerritoryId = Guid.NewGuid();
    private static readonly DateTime UpdatedAt = DateTime.UtcNow;

    [Fact]
    public void Constructor_WithNullTags_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() =>
            new TerritoryCharacterization(TerritoryId, null!, UpdatedAt));

        Assert.Equal("tags", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithEmptyList_ProducesEmptyTags()
    {
        var c = new TerritoryCharacterization(TerritoryId, Array.Empty<string>(), UpdatedAt);

        Assert.NotNull(c.Tags);
        Assert.Empty(c.Tags);
    }

    [Fact]
    public void Constructor_WithWhitespaceOnlyTags_FiltersThemOut()
    {
        var c = new TerritoryCharacterization(
            TerritoryId,
            new[] { "  ", "", "\t", " valid " },
            UpdatedAt);

        Assert.Single(c.Tags);
        Assert.Equal("valid", c.Tags[0]);
    }

    [Fact]
    public void Constructor_WithDuplicates_RemovesThem()
    {
        var c = new TerritoryCharacterization(
            TerritoryId,
            new[] { "tag1", "TAG1", " tag1 " },
            UpdatedAt);

        Assert.Single(c.Tags);
        Assert.Equal("tag1", c.Tags[0]);
    }

    [Fact]
    public void Constructor_WithValidTags_NormalizesToLowercase()
    {
        var c = new TerritoryCharacterization(
            TerritoryId,
            new[] { "CAFE", "  Parque  " },
            UpdatedAt);

        Assert.Equal(2, c.Tags.Count);
        Assert.Contains("cafe", c.Tags);
        Assert.Contains("parque", c.Tags);
    }

    [Fact]
    public void UpdateTags_WithNull_ThrowsArgumentNullException()
    {
        var c = new TerritoryCharacterization(TerritoryId, new[] { "a" }, UpdatedAt);

        var ex = Assert.Throws<ArgumentNullException>(() => c.UpdateTags(null!));

        Assert.Equal("tags", ex.ParamName);
    }

    [Fact]
    public void UpdateTags_WithEmptyList_ClearsTags()
    {
        var c = new TerritoryCharacterization(TerritoryId, new[] { "a", "b" }, UpdatedAt);

        c.UpdateTags(new List<string>());

        Assert.NotNull(c.Tags);
        Assert.Empty(c.Tags);
    }

    [Fact]
    public void UpdateTags_WithNewTags_ReplacesAndNormalizes()
    {
        var c = new TerritoryCharacterization(TerritoryId, new[] { "old" }, UpdatedAt);

        c.UpdateTags(new[] { "  NEW1  ", "new2", "NEW1" });

        Assert.Equal(2, c.Tags.Count);
        Assert.Contains("new1", c.Tags);
        Assert.Contains("new2", c.Tags);
    }
}
