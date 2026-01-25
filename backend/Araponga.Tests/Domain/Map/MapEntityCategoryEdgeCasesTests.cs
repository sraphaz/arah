using Araponga.Domain.Map;
using Xunit;

namespace Araponga.Tests.Domain.Map;

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
    public void TryNormalize_WithEstabelecimentoWithSpaces_ReturnsTrue()
    {
        var ok = MapEntityCategory.TryNormalize("  estabelecimento  ", out var normalized);

        Assert.True(ok);
        Assert.Equal("estabelecimento", normalized);
    }

    [Fact]
    public void TryNormalize_WithOrgaoDoGoverno_ReturnsTrue()
    {
        var ok = MapEntityCategory.TryNormalize("órgão do governo", out var normalized);

        Assert.True(ok);
        Assert.Equal("órgão do governo", normalized);
    }

    [Fact]
    public void TryNormalize_WithOrgaoDoGovernoUnderscore_ReturnsTrue()
    {
        var ok = MapEntityCategory.TryNormalize("órgão_do_governo", out var normalized);

        Assert.True(ok);
        Assert.Equal("órgão do governo", normalized);
    }

    [Fact]
    public void TryNormalize_WithEspacoPublico_ReturnsTrue()
    {
        var ok = MapEntityCategory.TryNormalize("espaço público", out var normalized);

        Assert.True(ok);
        Assert.Equal("espaço público", normalized);
    }

    [Fact]
    public void TryNormalize_WithEspacoNatural_ReturnsTrue()
    {
        var ok = MapEntityCategory.TryNormalize("espaço natural", out var normalized);

        Assert.True(ok);
        Assert.Equal("espaço natural", normalized);
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
