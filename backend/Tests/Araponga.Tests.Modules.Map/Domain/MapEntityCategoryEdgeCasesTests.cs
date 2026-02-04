using Araponga.Modules.Map.Domain;
using Xunit;

namespace Araponga.Tests.Modules.Map.Domain;

/// <summary>
/// Edge case tests for MapEntityCategory (TryNormalize, AllowedList),
/// focusing on branch coverage and normalization rules.
/// </summary>
public sealed class MapEntityCategoryEdgeCasesTests
{
    [Fact]
    public void TryNormalize_WithNull_ReturnsFalse()
    {
        var ok = MapEntityCategory.TryNormalize(null, out var normalized);

        Assert.False(ok);
        Assert.Equal(string.Empty, normalized);
    }

    [Fact]
    public void TryNormalize_WithWhitespace_ReturnsFalse()
    {
        var ok = MapEntityCategory.TryNormalize("   ", out var normalized);

        Assert.False(ok);
        Assert.Equal(string.Empty, normalized);
    }

    [Fact]
    public void TryNormalize_WithEmpty_ReturnsFalse()
    {
        var ok = MapEntityCategory.TryNormalize("", out var normalized);

        Assert.False(ok);
        Assert.Equal(string.Empty, normalized);
    }

    [Fact]
    public void TryNormalize_WithEstabelecimento_ReturnsTrue()
    {
        var ok = MapEntityCategory.TryNormalize("estabelecimento", out var normalized);

        Assert.True(ok);
        Assert.Equal("estabelecimento", normalized);
    }

    [Fact]
    public void TryNormalize_WithEstabelecimentoUpperCase_ReturnsTrueAndNormalizes()
    {
        var ok = MapEntityCategory.TryNormalize("ESTABELECIMENTO", out var normalized);

        Assert.True(ok);
        Assert.Equal("estabelecimento", normalized);
    }

    [Fact]
    public void TryNormalize_WithInvalidCategory_ReturnsFalse()
    {
        var ok = MapEntityCategory.TryNormalize("invalid-category", out var normalized);

        Assert.False(ok);
        Assert.Equal(string.Empty, normalized);
    }

    [Fact]
    public void AllowedList_ContainsExpectedCategories()
    {
        var list = MapEntityCategory.AllowedList;

        Assert.Contains("estabelecimento", list);
        Assert.Contains("órgão do governo", list);
        Assert.Contains("espaço público", list);
        Assert.Contains("espaço natural", list);
    }
}
