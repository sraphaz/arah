using System.Net;
using System.Net.Http.Json;
using Araponga.Api.Contracts.Events;
using Araponga.Tests.TestHelpers;
using Xunit;

namespace Araponga.Tests.Api;

/// <summary>
/// Testes para o EventsController - aumentar cobertura
/// </summary>
public sealed class EventsControllerTests
{
    private static readonly Guid ActiveTerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    [Fact]
    public async Task CreateEvent_RequiresAuthentication()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var request = new CreateEventRequest(
            ActiveTerritoryId,
            "Test Event",
            "Description",
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddHours(2),
            -23.5505,
            -46.6333,
            "São Paulo");

        var response = await client.PostAsJsonAsync("api/v1/events", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateEvent_ValidatesInput()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await AuthTestHelper.LoginForTokenAsync(client, "google", "test-events");
        AuthTestHelper.SetupAuthenticatedClient(client, token);

        // Título vazio
        var request = new CreateEventRequest(
            ActiveTerritoryId,
            "",
            "Description",
            DateTime.UtcNow.AddDays(1),
            DateTime.UtcNow.AddDays(1).AddHours(2),
            -23.5505,
            -46.6333,
            "São Paulo");

        var response = await client.PostAsJsonAsync("api/v1/events", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateEvent_RequiresAuthentication()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var eventId = Guid.NewGuid();
        var request = new UpdateEventRequest(
            "Updated Title",
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);

        var response = await client.PatchAsJsonAsync($"api/v1/events/{eventId}", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CancelEvent_RequiresAuthentication()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var eventId = Guid.NewGuid();
        var response = await client.PostAsync($"api/v1/events/{eventId}/cancel", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ExpressInterest_RequiresAuthentication()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var eventId = Guid.NewGuid();
        var response = await client.PostAsync($"api/v1/events/{eventId}/interest", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ConfirmParticipation_RequiresAuthentication()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var eventId = Guid.NewGuid();
        var response = await client.PostAsync($"api/v1/events/{eventId}/confirm", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetEvents_RequiresAuthentication()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"api/v1/events?territoryId={ActiveTerritoryId}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetEventsPaged_RequiresAuthentication()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"api/v1/events/paged?territoryId={ActiveTerritoryId}&pageNumber=1&pageSize=10");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetEventsNearby_RequiresAuthentication()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"api/v1/events/nearby?territoryId={ActiveTerritoryId}&lat=-23.37&lng=-45.02");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetEventsNearbyPaged_RequiresAuthentication()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"api/v1/events/nearby/paged?territoryId={ActiveTerritoryId}&lat=-23.37&lng=-45.02&pageNumber=1&pageSize=10");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
