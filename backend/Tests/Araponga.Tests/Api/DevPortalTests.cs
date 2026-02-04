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
        var contentType = response.Content.Headers.ContentType?.MediaType ?? "";
        Assert.True(
            contentType.Equals("application/javascript", StringComparison.OrdinalIgnoreCase) ||
            contentType.Equals("text/javascript", StringComparison.OrdinalIgnoreCase),
            $"Content-Type deve ser 'application/javascript' ou 'text/javascript', mas foi '{contentType}'");
        
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
        // Procura especificamente por onclick= após diagram-fullscreen-btn
        var onclickMatches = Regex.Matches(content, @"diagram-fullscreen-btn[^>]*onclick\s*=", RegexOptions.IgnoreCase);
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
    public async Task DevPortal_DiagramSvg_ShouldHaveCorrectBackgroundColor(string svgFileName)
    {
        // Act
        var response = await _client.GetAsync($"/devportal/assets/images/diagrams/{svgFileName}");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        // Verifica que o fundo é cinza escuro (#141a21)
        Assert.Contains("#141a21", content, StringComparison.OrdinalIgnoreCase);
        // Verifica que está aplicado no background-color do SVG
        Assert.True(
            content.Contains("background-color: #141a21", StringComparison.OrdinalIgnoreCase) ||
            content.Contains("background-color:#141a21", StringComparison.OrdinalIgnoreCase),
            $"Diagrama {svgFileName} deve ter background-color: #141a21");
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
    public async Task DevPortal_DiagramSvg_ShouldHaveCorrectLineColors(string svgFileName)
    {
        // Act
        var response = await _client.GetAsync($"/devportal/assets/images/diagrams/{svgFileName}");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        // Verifica que as linhas principais são azul claro (#7dd3ff)
        Assert.Contains("#7dd3ff", content, StringComparison.OrdinalIgnoreCase);
        
        // Verifica que os estilos CSS customizados estão presentes
        Assert.Contains("Cores customizadas Araponga", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(".messageLine0", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(".messageLine1", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(".actor-line", content, StringComparison.OrdinalIgnoreCase);
        
        // Verifica que as cores azul estão aplicadas com !important
        var hasBlueInStyles = content.Contains(".messageLine0") && 
                             (content.Contains("stroke: #7dd3ff") || content.Contains("stroke:#7dd3ff")) &&
                             content.Contains("!important");
        Assert.True(hasBlueInStyles, 
            $"Diagrama {svgFileName} deve ter .messageLine0 com stroke: #7dd3ff !important");
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
    public async Task DevPortal_DiagramSvg_ShouldHaveCorrectAccentColors(string svgFileName)
    {
        // Act
        var response = await _client.GetAsync($"/devportal/assets/images/diagrams/{svgFileName}");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        // Verifica que os contornos/labels são verde água (#4dd4a8)
        Assert.Contains("#4dd4a8", content, StringComparison.OrdinalIgnoreCase);
        
        // Verifica que os estilos CSS para labels e loops estão presentes
        Assert.Contains(".labelBox", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains(".loopLine", content, StringComparison.OrdinalIgnoreCase);
        
        // Verifica que as cores verde água estão aplicadas
        var hasGreenInStyles = (content.Contains(".labelBox") || content.Contains(".loopLine")) &&
                              (content.Contains("stroke: #4dd4a8") || content.Contains("stroke:#4dd4a8"));
        Assert.True(hasGreenInStyles, 
            $"Diagrama {svgFileName} deve ter .labelBox ou .loopLine com stroke: #4dd4a8");
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
    public async Task DevPortal_DiagramSvg_ShouldNotHaveOldColors(string svgFileName)
    {
        // Act
        var response = await _client.GetAsync($"/devportal/assets/images/diagrams/{svgFileName}");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        // Verifica que não há cores antigas hardcoded que não foram corrigidas
        // Cores antigas que não devem estar presentes:
        Assert.DoesNotContain("#eaeaea", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("#666\"", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("stroke=\"#999\"", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("stroke=\"#333\"", content, StringComparison.OrdinalIgnoreCase);
        
        // Verifica que não há fill com cor antiga
        Assert.DoesNotContain("fill=\"#eaeaea\"", content, StringComparison.OrdinalIgnoreCase);
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
    public async Task DevPortal_DiagramSvg_ShouldHaveCustomStylesWithImportant(string svgFileName)
    {
        // Act
        var response = await _client.GetAsync($"/devportal/assets/images/diagrams/{svgFileName}");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        // Verifica que os estilos CSS customizados estão presentes com !important
        Assert.Contains("Cores customizadas Araponga", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("!important", content, StringComparison.OrdinalIgnoreCase);
        
        // Verifica que os estilos principais estão presentes
        Assert.Contains("#mermaid-svg .messageLine0", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("#mermaid-svg .actor-line", content, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("#mermaid-svg .actor", content, StringComparison.OrdinalIgnoreCase);
        
        // Verifica que pelo menos um estilo tem !important
        var importantMatches = Regex.Matches(content, @"!important", RegexOptions.IgnoreCase);
        Assert.True(importantMatches.Count >= 5, 
            $"Diagrama {svgFileName} deve ter pelo menos 5 estilos com !important, mas encontrado {importantMatches.Count}");
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
    public async Task DevPortal_DiagramSvg_ShouldHaveCorrectActorColors(string svgFileName)
    {
        // Act
        var response = await _client.GetAsync($"/devportal/assets/images/diagrams/{svgFileName}");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        // Verifica que os atores têm as cores corretas
        // Fundo dos atores: #1a2129 (ou fill="#1a2129")
        Assert.Contains("#1a2129", content, StringComparison.OrdinalIgnoreCase);
        
        // Borda dos atores: azul #7dd3ff
        Assert.Contains("#7dd3ff", content, StringComparison.OrdinalIgnoreCase);
        
        // Verifica que há pelo menos um ator com as cores corretas
        var hasCorrectActorFill = content.Contains("fill=\"#1a2129\"", StringComparison.OrdinalIgnoreCase) ||
                                  content.Contains("fill='#1a2129'", StringComparison.OrdinalIgnoreCase);
        var hasCorrectActorStroke = content.Contains("stroke=\"#7dd3ff\"", StringComparison.OrdinalIgnoreCase) ||
                                    content.Contains("stroke='#7dd3ff'", StringComparison.OrdinalIgnoreCase);
        
        Assert.True(hasCorrectActorFill || hasCorrectActorStroke, 
            $"Diagrama {svgFileName} deve ter atores com fill=\"#1a2129\" e stroke=\"#7dd3ff\"");
    }

}
