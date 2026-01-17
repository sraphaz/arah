using System.Net;
using System.Text.RegularExpressions;
using Xunit;

namespace Araponga.Tests.Api;

/// <summary>
/// Testes para o Developer Portal (devportal)
/// Verifica que o portal estático é servido corretamente e que todos os recursos existem.
/// </summary>
[Collection("Api")]
public class DevPortalTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public DevPortalTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task DevPortal_IndexHtml_ShouldBeServed()
    {
        // Act
        var response = await _client.GetAsync("/devportal");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Araponga API • Developer Portal", content);
    }

    [Fact]
    public async Task DevPortal_IndexHtml_ShouldHaveCorrectEncoding()
    {
        // Act
        var response = await _client.GetAsync("/devportal");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Contains("charset=\"utf-8\"", content, StringComparison.OrdinalIgnoreCase);
        // Verifica que caracteres UTF-8 são preservados corretamente
        Assert.Contains("território", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DevPortal_CssFile_ShouldBeServed()
    {
        // Act
        var response = await _client.GetAsync("/devportal/assets/css/devportal.css");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/css", response.Content.Headers.ContentType?.MediaType);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotNull(content);
        Assert.True(content.Length > 0);
        // Verifica que o CSS contém variáveis esperadas
        Assert.Contains("--accent", content);
        Assert.Contains("--bg", content);
    }

    [Fact]
    public async Task DevPortal_JavaScriptFile_ShouldBeServed()
    {
        // Act
        var response = await _client.GetAsync("/devportal/assets/js/devportal.js");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("javascript", response.Content.Headers.ContentType?.MediaType ?? "", StringComparison.OrdinalIgnoreCase);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotNull(content);
        Assert.True(content.Length > 0);
    }

    [Theory]
    [InlineData("auth.svg")]
    [InlineData("territory-discovery.svg")]
    [InlineData("feed-listing.svg")]
    [InlineData("post-creation.svg")]
    [InlineData("membership-resident.svg")]
    [InlineData("moderation.svg")]
    [InlineData("marketplace-checkout.svg")]
    [InlineData("notifications-outbox.svg")]
    [InlineData("events-creation.svg")]
    [InlineData("assets-validation.svg")]
    [InlineData("residency-verification.svg")]
    [InlineData("chat-media.svg")]
    [InlineData("map-entities.svg")]
    public async Task DevPortal_DiagramSvg_ShouldExist(string svgFileName)
    {
        // Act
        var response = await _client.GetAsync($"/devportal/assets/images/diagrams/{svgFileName}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/svg+xml", response.Content.Headers.ContentType?.MediaType);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotNull(content);
        Assert.True(content.Length > 0);
        // Verifica que é um SVG válido
        Assert.Contains("<svg", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DevPortal_IndexHtml_ShouldNotLoadMermaidJs()
    {
        // Act
        var response = await _client.GetAsync("/devportal");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        // Verifica que NÃO há script Mermaid.js sendo carregado
        // Nota: Este teste falhará até que o código Mermaid seja completamente removido
        Assert.DoesNotContain("mermaid.min.js", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("cdn.jsdelivr.net/npm/mermaid", content, StringComparison.OrdinalIgnoreCase);
        // Verifica que não há chamadas para mermaid.initialize ou mermaid.run
        Assert.DoesNotContain("mermaid.initialize", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("mermaid.run", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DevPortal_IndexHtml_ShouldHaveDiagramFullscreenButtons()
    {
        // Act
        var response = await _client.GetAsync("/devportal");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        // Verifica que há botões de tela cheia
        var buttonMatches = Regex.Matches(content, @"diagram-fullscreen-btn", RegexOptions.IgnoreCase);
        Assert.True(buttonMatches.Count >= 13, $"Esperado pelo menos 13 botões de tela cheia, mas encontrado {buttonMatches.Count}");
    }

    [Fact]
    public async Task DevPortal_IndexHtml_ShouldNotHaveInlineOnclick()
    {
        // Act
        var response = await _client.GetAsync("/devportal");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        // Verifica que não há onclick inline nos botões de fullscreen (CSP compliance)
        // Nota: Este teste falhará até que todos os onclick sejam substituídos por event listeners
        var onclickMatches = Regex.Matches(content, @"class\s*=\s*[""']diagram-fullscreen-btn[""'].*onclick", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        Assert.True(onclickMatches.Count == 0, 
            $"Encontrado {onclickMatches.Count} onclick inline nos botões de fullscreen. Devem usar event listeners para CSP compliance.");
    }

    [Fact]
    public async Task DevPortal_IndexHtml_ShouldHaveEventListeners()
    {
        // Act
        var response = await _client.GetAsync("/devportal");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        // Verifica que há event listeners configurados
        Assert.Contains("addEventListener", content, StringComparison.OrdinalIgnoreCase);
        // Verifica que há função openDiagramFullscreen (mesmo que ainda use onclick temporariamente)
        Assert.Contains("openDiagramFullscreen", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DevPortal_IndexHtml_ShouldHaveAllDiagrams()
    {
        // Act
        var response = await _client.GetAsync("/devportal");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        // Verifica que todos os 13 diagramas estão referenciados
        var expectedDiagrams = new[]
        {
            "auth.svg",
            "territory-discovery.svg",
            "feed-listing.svg",
            "post-creation.svg",
            "membership-resident.svg",
            "moderation.svg",
            "marketplace-checkout.svg",
            "notifications-outbox.svg",
            "events-creation.svg",
            "assets-validation.svg",
            "residency-verification.svg",
            "chat-media.svg",
            "map-entities.svg"
        };

        foreach (var diagram in expectedDiagrams)
        {
            Assert.Contains(diagram, content, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public async Task DevPortal_IndexHtml_ShouldHaveModalStructure()
    {
        // Act
        var response = await _client.GetAsync("/devportal");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        // Verifica que a estrutura do modal existe no JavaScript
        Assert.Contains("diagram-modal", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("diagram-modal-content", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("diagram-modal-close", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DevPortal_IndexHtml_ShouldHaveSequenceDiagramContainers()
    {
        // Act
        var response = await _client.GetAsync("/devportal");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        // Verifica que há containers para diagramas de sequência
        var containerMatches = Regex.Matches(content, @"sequence-diagram-container", RegexOptions.IgnoreCase);
        Assert.True(containerMatches.Count >= 13, 
            $"Esperado pelo menos 13 containers de diagramas, mas encontrado {containerMatches.Count}");
    }

    [Fact]
    public async Task DevPortal_Css_ShouldHaveDiagramStyles()
    {
        // Act
        var response = await _client.GetAsync("/devportal/assets/css/devportal.css");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        // Verifica que há estilos para diagramas
        Assert.Contains("mermaid-diagram", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("diagram-fullscreen-btn", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("diagram-modal", content, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DevPortal_Css_ShouldHaveFilterStylesForDarkTheme()
    {
        // Act
        var response = await _client.GetAsync("/devportal/assets/css/devportal.css");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        // Verifica que há filtros CSS para ajustar cores dos SVGs ao tema dark
        Assert.Contains("filter:", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("invert(", content, StringComparison.OrdinalIgnoreCase);
    }
}
