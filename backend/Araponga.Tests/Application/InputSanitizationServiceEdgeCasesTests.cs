using Araponga.Application.Services;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for InputSanitizationService (SanitizeHtml, SanitizePath, SanitizeUrl, SanitizeText, SanitizeSql).
/// </summary>
public sealed class InputSanitizationServiceEdgeCasesTests
{
    private readonly InputSanitizationService _service = new();

    [Fact]
    public void SanitizeHtml_WithNull_ReturnsEmpty()
    {
        Assert.Equal("", _service.SanitizeHtml(null));
    }

    [Fact]
    public void SanitizeHtml_WithWhitespace_ReturnsEmpty()
    {
        Assert.Equal("", _service.SanitizeHtml("   \t\n  "));
    }

    [Fact]
    public void SanitizeHtml_WithTags_RemovesTags()
    {
        var result = _service.SanitizeHtml("<script>alert(1)</script>hello");
        Assert.DoesNotContain("<", result);
        Assert.DoesNotContain(">", result);
        Assert.Contains("hello", result);
    }

    [Fact]
    public void SanitizeHtml_WithAmpersand_Escapes()
    {
        var result = _service.SanitizeHtml("a & b");
        Assert.Contains("&amp;", result);
    }

    [Fact]
    public void SanitizePath_WithNull_ReturnsEmpty()
    {
        Assert.Equal("", _service.SanitizePath(null));
    }

    [Fact]
    public void SanitizePath_WithParentRefs_RemovesThem()
    {
        var result = _service.SanitizePath("foo/../bar/../baz");
        Assert.DoesNotContain("..", result);
    }

    [Fact]
    public void SanitizePath_WithBackslash_NormalizesToForward()
    {
        var result = _service.SanitizePath("a\\b\\c");
        Assert.DoesNotContain("\\", result);
        Assert.Contains("/", result);
    }

    [Fact]
    public void SanitizeUrl_WithNull_ReturnsNull()
    {
        Assert.Null(_service.SanitizeUrl(null));
    }

    [Fact]
    public void SanitizeUrl_WithInvalid_ReturnsNull()
    {
        Assert.Null(_service.SanitizeUrl("not-a-url"));
    }

    [Fact]
    public void SanitizeUrl_WithHttps_ReturnsSame()
    {
        var url = "https://example.com/path";
        Assert.Equal(url, _service.SanitizeUrl(url));
    }

    [Fact]
    public void SanitizeUrl_WithHttp_ReturnsSame()
    {
        var url = "http://example.com/path";
        Assert.Equal(url, _service.SanitizeUrl(url));
    }

    [Fact]
    public void SanitizeText_WithNull_ReturnsEmpty()
    {
        Assert.Equal("", _service.SanitizeText(null));
    }

    [Fact]
    public void SanitizeText_WithMultipleSpaces_Normalizes()
    {
        var result = _service.SanitizeText("a   b   c");
        Assert.Equal("a b c", result);
    }

    [Fact]
    public void SanitizeSql_WithNull_ReturnsEmpty()
    {
        Assert.Equal("", _service.SanitizeSql(null));
    }

    [Fact]
    public void SanitizeSql_WithSemicolon_RemovesIt()
    {
        var result = _service.SanitizeSql("foo; drop table x");
        Assert.DoesNotContain(";", result);
    }
}
