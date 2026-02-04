using Araponga.Application.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO;
using System.Text;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for EmailTemplateService,
/// focusing on missing templates, invalid data, Unicode handling, and template rendering edge cases.
/// </summary>
public sealed class EmailTemplateServiceEdgeCasesTests : IDisposable
{
    private readonly Mock<ILogger<EmailTemplateService>> _loggerMock;
    private readonly string _templatesPath;
    private readonly EmailTemplateService _service;

    public EmailTemplateServiceEdgeCasesTests()
    {
        _loggerMock = new Mock<ILogger<EmailTemplateService>>();
        _templatesPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_templatesPath);
        _service = new EmailTemplateService(_loggerMock.Object, _templatesPath);
    }

    public void Dispose()
    {
        if (Directory.Exists(_templatesPath))
        {
            Directory.Delete(_templatesPath, recursive: true);
        }
    }

    [Fact]
    public async Task RenderTemplateAsync_WithNullTemplateName_ThrowsArgumentNullException()
    {
        var data = new { Name = "Test" };
        // ArgumentException.ThrowIfNullOrWhiteSpace lança ArgumentNullException para null
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _service.RenderTemplateAsync(null!, data, CancellationToken.None));
    }

    [Fact]
    public async Task RenderTemplateAsync_WithEmptyTemplateName_ThrowsArgumentException()
    {
        var data = new { Name = "Test" };
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.RenderTemplateAsync("", data, CancellationToken.None));
    }

    [Fact]
    public async Task RenderTemplateAsync_WithWhitespaceTemplateName_ThrowsArgumentException()
    {
        var data = new { Name = "Test" };
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.RenderTemplateAsync("   ", data, CancellationToken.None));
    }

    [Fact]
    public async Task RenderTemplateAsync_WithNonExistentTemplate_ThrowsFileNotFoundException()
    {
        var data = new { Name = "Test" };
        await Assert.ThrowsAsync<FileNotFoundException>(async () =>
            await _service.RenderTemplateAsync("nonexistent", data, CancellationToken.None));
    }

    [Fact]
    public async Task RenderTemplateAsync_WithSimpleTemplate_ReplacesPlaceholders()
    {
        var templateContent = "<h1>Hello {{Name}}</h1>";
        var templateFile = Path.Combine(_templatesPath, "test.html");
        await File.WriteAllTextAsync(templateFile, templateContent, Encoding.UTF8);

        var layoutFile = Path.Combine(_templatesPath, "_layout.html");
        await File.WriteAllTextAsync(layoutFile, "<html><body>{{CONTENT}}</body></html>", Encoding.UTF8);

        var data = new { Name = "World" };
        var result = await _service.RenderTemplateAsync("test", data, CancellationToken.None);

        Assert.Contains("Hello World", result);
    }

    [Fact]
    public async Task RenderTemplateAsync_WithUnicodeInTemplate_HandlesCorrectly()
    {
        var templateContent = "<h1>Olá {{Name}} - café, naïve, 文字</h1>";
        var templateFile = Path.Combine(_templatesPath, "unicode.html");
        await File.WriteAllTextAsync(templateFile, templateContent, Encoding.UTF8);

        var layoutFile = Path.Combine(_templatesPath, "_layout.html");
        await File.WriteAllTextAsync(layoutFile, "<html><body>{{CONTENT}}</body></html>", Encoding.UTF8);

        var data = new { Name = "Mundo" };
        var result = await _service.RenderTemplateAsync("unicode", data, CancellationToken.None);

        Assert.Contains("Olá Mundo", result);
        Assert.Contains("café", result);
        Assert.Contains("文字", result);
    }

    [Fact]
    public async Task RenderTemplateAsync_WithMissingLayout_UsesMinimalLayout()
    {
        var templateContent = "<p>Content</p>";
        var templateFile = Path.Combine(_templatesPath, "nolayout.html");
        await File.WriteAllTextAsync(templateFile, templateContent, Encoding.UTF8);

        var data = new { Name = "Test" };
        var result = await _service.RenderTemplateAsync("nolayout", data, CancellationToken.None);

        Assert.Contains("<!DOCTYPE html>", result);
        Assert.Contains("Content", result);
    }

    [Fact]
    public async Task RenderTemplateAsync_WithConditional_ProcessesCorrectly()
    {
        var templateContent = "{{#if Show}}Visible{{/if}}{{#if Hide}}Hidden{{/if}}";
        var templateFile = Path.Combine(_templatesPath, "conditional.html");
        await File.WriteAllTextAsync(templateFile, templateContent, Encoding.UTF8);

        var layoutFile = Path.Combine(_templatesPath, "_layout.html");
        await File.WriteAllTextAsync(layoutFile, "<html><body>{{CONTENT}}</body></html>", Encoding.UTF8);

        var data = new { Show = "yes", Hide = "" };
        var result = await _service.RenderTemplateAsync("conditional", data, CancellationToken.None);

        Assert.Contains("Visible", result);
        Assert.DoesNotContain("Hidden", result);
    }

    [Fact]
    public async Task RenderTemplateAsync_WithLoop_ProcessesCorrectly()
    {
        var templateContent = "{{#each Items}}<li>{{Name}}</li>{{/each}}";
        var templateFile = Path.Combine(_templatesPath, "loop.html");
        await File.WriteAllTextAsync(templateFile, templateContent, Encoding.UTF8);

        var layoutFile = Path.Combine(_templatesPath, "_layout.html");
        await File.WriteAllTextAsync(layoutFile, "<html><body>{{CONTENT}}</body></html>", Encoding.UTF8);

        var data = new { Items = new[] { new { Name = "Item1" }, new { Name = "Item2" } } };
        var result = await _service.RenderTemplateAsync("loop", data, CancellationToken.None);

        Assert.Contains("Item1", result);
        Assert.Contains("Item2", result);
    }

    [Fact]
    public async Task RenderTemplateAsync_WithEmptyLoop_HandlesGracefully()
    {
        var templateContent = "{{#each Items}}<li>{{Name}}</li>{{/each}}";
        var templateFile = Path.Combine(_templatesPath, "emptyloop.html");
        await File.WriteAllTextAsync(templateFile, templateContent, Encoding.UTF8);

        var layoutFile = Path.Combine(_templatesPath, "_layout.html");
        await File.WriteAllTextAsync(layoutFile, "<html><body>{{CONTENT}}</body></html>", Encoding.UTF8);

        var data = new { Items = Array.Empty<object>() };
        var result = await _service.RenderTemplateAsync("emptyloop", data, CancellationToken.None);

        Assert.DoesNotContain("<li>", result);
    }

    [Fact]
    public async Task RenderTemplateAsync_WithNestedProperty_ReplacesCorrectly()
    {
        var templateContent = "<p>{{User.Name}} - {{User.Email}}</p>";
        var templateFile = Path.Combine(_templatesPath, "nested.html");
        await File.WriteAllTextAsync(templateFile, templateContent, Encoding.UTF8);

        var layoutFile = Path.Combine(_templatesPath, "_layout.html");
        await File.WriteAllTextAsync(layoutFile, "<html><body>{{CONTENT}}</body></html>", Encoding.UTF8);

        var data = new { User = new { Name = "John", Email = "john@example.com" } };
        var result = await _service.RenderTemplateAsync("nested", data, CancellationToken.None);

        Assert.Contains("John", result);
        Assert.Contains("john@example.com", result);
    }

    [Fact]
    public async Task RenderTemplateAsync_WithArrayIndex_ReplacesCorrectly()
    {
        var templateContent = "<p>{{Items.0.Name}} and {{Items.1.Name}}</p>";
        var templateFile = Path.Combine(_templatesPath, "array.html");
        await File.WriteAllTextAsync(templateFile, templateContent, Encoding.UTF8);

        var layoutFile = Path.Combine(_templatesPath, "_layout.html");
        await File.WriteAllTextAsync(layoutFile, "<html><body>{{CONTENT}}</body></html>", Encoding.UTF8);

        var data = new { Items = new[] { new { Name = "First" }, new { Name = "Second" } } };
        var result = await _service.RenderTemplateAsync("array", data, CancellationToken.None);

        Assert.Contains("First", result);
        Assert.Contains("Second", result);
    }

    [Fact]
    public async Task RenderTemplateAsync_WithDateTime_FormatsCorrectly()
    {
        var templateContent = "<p>Date: {{CreatedAt}}</p>";
        var templateFile = Path.Combine(_templatesPath, "date.html");
        await File.WriteAllTextAsync(templateFile, templateContent, Encoding.UTF8);

        var layoutFile = Path.Combine(_templatesPath, "_layout.html");
        await File.WriteAllTextAsync(layoutFile, "<html><body>{{CONTENT}}</body></html>", Encoding.UTF8);

        var data = new { CreatedAt = new DateTime(2024, 1, 15, 10, 30, 0) };
        var result = await _service.RenderTemplateAsync("date", data, CancellationToken.None);

        Assert.Contains("15/01/2024", result);
        Assert.Contains("10:30", result);
    }

    [Fact]
    public async Task RenderTemplateAsync_WithDecimal_FormatsAsCurrency()
    {
        var templateContent = "<p>Price: {{Price}}</p>";
        var templateFile = Path.Combine(_templatesPath, "price.html");
        await File.WriteAllTextAsync(templateFile, templateContent, Encoding.UTF8);

        var layoutFile = Path.Combine(_templatesPath, "_layout.html");
        await File.WriteAllTextAsync(layoutFile, "<html><body>{{CONTENT}}</body></html>", Encoding.UTF8);

        var data = new { Price = 1234.56m };
        var result = await _service.RenderTemplateAsync("price", data, CancellationToken.None);

        Assert.Contains("R$", result);
        Assert.Contains("1.234,56", result);
    }

    [Fact]
    public async Task RenderTemplateAsync_WithNullProperty_HandlesGracefully()
    {
        var templateContent = "<p>{{Name}} - {{Optional}}</p>";
        var templateFile = Path.Combine(_templatesPath, "null.html");
        await File.WriteAllTextAsync(templateFile, templateContent, Encoding.UTF8);

        var layoutFile = Path.Combine(_templatesPath, "_layout.html");
        await File.WriteAllTextAsync(layoutFile, "<html><body>{{CONTENT}}</body></html>", Encoding.UTF8);

        var data = new { Name = "Test", Optional = (string?)null };
        var result = await _service.RenderTemplateAsync("null", data, CancellationToken.None);

        Assert.Contains("Test", result);
        // O template service substitui null por string vazia, então o placeholder é removido
        Assert.Contains("Test -", result);
        Assert.DoesNotContain("{{Optional}}", result);
    }

    [Fact]
    public async Task RenderTemplateAsync_WithTemplateExtension_RemovesExtension()
    {
        var templateContent = "<p>Content</p>";
        var templateFile = Path.Combine(_templatesPath, "test.html");
        await File.WriteAllTextAsync(templateFile, templateContent, Encoding.UTF8);

        var layoutFile = Path.Combine(_templatesPath, "_layout.html");
        await File.WriteAllTextAsync(layoutFile, "<html><body>{{CONTENT}}</body></html>", Encoding.UTF8);

        var data = new { Name = "Test" };
        var result = await _service.RenderTemplateAsync("test.html", data, CancellationToken.None);

        Assert.Contains("Content", result); // Deve funcionar mesmo com extensão
    }

    [Fact]
    public async Task RenderTemplateAsync_CachesTemplate_OnSecondCall()
    {
        var templateContent = "<p>Content</p>";
        var templateFile = Path.Combine(_templatesPath, "cache.html");
        await File.WriteAllTextAsync(templateFile, templateContent, Encoding.UTF8);

        var layoutFile = Path.Combine(_templatesPath, "_layout.html");
        await File.WriteAllTextAsync(layoutFile, "<html><body>{{CONTENT}}</body></html>", Encoding.UTF8);

        var data = new { Name = "Test" };
        var result1 = await _service.RenderTemplateAsync("cache", data, CancellationToken.None);

        // Modificar arquivo (cache deve manter versão antiga)
        await File.WriteAllTextAsync(templateFile, "<p>Modified</p>", Encoding.UTF8);
        var result2 = await _service.RenderTemplateAsync("cache", data, CancellationToken.None);

        Assert.Contains("Content", result2); // Deve usar cache
    }
}
