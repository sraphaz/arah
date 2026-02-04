using System.Net;
using System.Net.Http.Json;
using Araponga.Bff;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Araponga.Tests.Bff;

/// <summary>
/// Testes de integração do endpoint GET /bff/journeys que lista todas as jornadas expostas pelo BFF.
/// </summary>
public sealed class BffJourneysEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public BffJourneysEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetBffJourneys_ReturnsOk()
    {
        var response = await _client.GetAsync("/bff/journeys");
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetBffJourneys_ReturnsAllFourJourneys()
    {
        var response = await _client.GetAsync("/bff/journeys");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<BffJourneysResponse>();
        Assert.NotNull(json);
        Assert.NotNull(json.Journeys);
        Assert.Equal(4, json.Journeys.Count);
        var names = json.Journeys.Select(j => j.Journey).ToHashSet();
        Assert.Contains("onboarding", names);
        Assert.Contains("feed", names);
        Assert.Contains("events", names);
        Assert.Contains("marketplace", names);
    }

    [Fact]
    public async Task GetBffJourneys_EachJourneyHasBasePathAndAllEndpoints()
    {
        var response = await _client.GetAsync("/bff/journeys");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<BffJourneysResponse>();
        Assert.NotNull(json);
        var expectedCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["onboarding"] = 2,
            ["feed"] = 3,
            ["events"] = 3,
            ["marketplace"] = 3
        };
        foreach (var j in json.Journeys!)
        {
            Assert.NotNull(j.BasePath);
            Assert.StartsWith("/api/v2/journeys/", j.BasePath);
            Assert.NotNull(j.Endpoints);
            Assert.True(expectedCounts.TryGetValue(j.Journey!, out var expected), $"Journey {j.Journey} should be known.");
            Assert.Equal(expected, j.Endpoints.Count);
        }
    }

    private sealed class BffJourneysResponse
    {
        public List<JourneyItem>? Journeys { get; set; }
    }

    private sealed class JourneyItem
    {
        public string? Journey { get; set; }
        public string? BasePath { get; set; }
        public List<EndpointItem>? Endpoints { get; set; }
    }

    private sealed class EndpointItem
    {
        public string? Path { get; set; }
        public string? Method { get; set; }
        public string? Description { get; set; }
    }
}
