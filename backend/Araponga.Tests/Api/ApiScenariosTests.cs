using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Araponga.Api;
using Araponga.Api.Contracts.Auth;
using Araponga.Api.Contracts.Feed;
using Araponga.Api.Contracts.Map;
using Araponga.Api.Contracts.Memberships;
using Araponga.Api.Contracts.Territories;
using Xunit;

namespace Araponga.Tests.Api;

public sealed class ApiScenariosTests
{
    private static readonly Guid ActiveTerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    [Fact]
    public async Task ListTerritories_FiltersAndSorts()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var territories = await client.GetFromJsonAsync<List<TerritoryResponse>>("api/v1/territories");

        Assert.NotNull(territories);
        Assert.Equal(2, territories!.Count);
        Assert.Equal("Sertão do Camburi", territories[0].Name);
        Assert.Equal("Vale do Itamambuca", territories[1].Name);
        Assert.All(territories, territory => Assert.Contains(territory.Status, new[] { "ACTIVE", "PILOT" }));
    }

    [Fact]
    public async Task TerritoryLookup_ReturnsExpectedStatus()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var existing = await client.GetAsync($"api/v1/territories/{ActiveTerritoryId}");
        existing.EnsureSuccessStatusCode();

        var missing = await client.GetAsync($"api/v1/territories/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }

    [Fact]
    public async Task TerritorySelection_RequiresSessionHeader()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "api/v1/territories/selection",
            new TerritorySelectionRequest(ActiveTerritoryId));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task TerritorySelection_RejectsUnknownTerritory()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "session-1");

        var response = await client.PostAsJsonAsync(
            "api/v1/territories/selection",
            new TerritorySelectionRequest(Guid.NewGuid()));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task TerritorySelection_CanSetAndGet()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "session-2");

        var setResponse = await client.PostAsJsonAsync(
            "api/v1/territories/selection",
            new TerritorySelectionRequest(ActiveTerritoryId));

        setResponse.EnsureSuccessStatusCode();
        var selection = await setResponse.Content.ReadFromJsonAsync<TerritorySelectionResponse>();

        Assert.NotNull(selection);
        Assert.Equal(ActiveTerritoryId, selection!.TerritoryId);

        var getResponse = await client.GetAsync("api/v1/territories/selection");
        getResponse.EnsureSuccessStatusCode();

        var getSelection = await getResponse.Content.ReadFromJsonAsync<TerritorySelectionResponse>();
        Assert.NotNull(getSelection);
        Assert.Equal(ActiveTerritoryId, getSelection!.TerritoryId);
    }

    [Fact]
    public async Task TerritorySelection_GetRequiresSession()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("api/v1/territories/selection");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task TerritorySelection_GetReturnsNotFoundWithoutSelection()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "session-3");

        var response = await client.GetAsync("api/v1/territories/selection");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AuthSocial_ValidatesPayloadAndReusesUser()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var invalidResponse = await client.PostAsJsonAsync(
            "api/v1/auth/social",
            new SocialLoginRequest("", "", "", ""));

        Assert.Equal(HttpStatusCode.BadRequest, invalidResponse.StatusCode);

        var request = new SocialLoginRequest("google", "resident-external", "Morador Teste", "morador@araponga.com");
        var firstLogin = await client.PostAsJsonAsync("api/v1/auth/social", request);
        firstLogin.EnsureSuccessStatusCode();

        var firstPayload = await firstLogin.Content.ReadFromJsonAsync<SocialLoginResponse>();
        Assert.NotNull(firstPayload);

        var secondLogin = await client.PostAsJsonAsync("api/v1/auth/social", request);
        secondLogin.EnsureSuccessStatusCode();
        var secondPayload = await secondLogin.Content.ReadFromJsonAsync<SocialLoginResponse>();

        Assert.NotNull(secondPayload);
        Assert.Equal(firstPayload!.User.Id, secondPayload!.User.Id);
        Assert.StartsWith("user:", secondPayload.Token);
    }

    [Fact]
    public async Task TerritoryCreation_ValidatesSensitivityAndName()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var emptySensitivity = await client.PostAsJsonAsync(
            "api/v1/territories",
            new CreateTerritoryRequest("Território", "Desc", "", false));

        Assert.Equal(HttpStatusCode.BadRequest, emptySensitivity.StatusCode);

        var invalidSensitivity = await client.PostAsJsonAsync(
            "api/v1/territories",
            new CreateTerritoryRequest("Território", "Desc", "INVALID", false));

        Assert.Equal(HttpStatusCode.BadRequest, invalidSensitivity.StatusCode);

        var invalidName = await client.PostAsJsonAsync(
            "api/v1/territories",
            new CreateTerritoryRequest("", "Desc", "LOW", false));

        Assert.Equal(HttpStatusCode.BadRequest, invalidName.StatusCode);

        var valid = await client.PostAsJsonAsync(
            "api/v1/territories",
            new CreateTerritoryRequest("Novo Território", "Desc", "LOW", false));

        Assert.Equal(HttpStatusCode.Created, valid.StatusCode);
    }

    [Fact]
    public async Task Memberships_RequireAuthAndTerritory()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var unauthorized = await client.PostAsync(
            $"api/v1/territories/{ActiveTerritoryId}/memberships",
            null);

        Assert.Equal(HttpStatusCode.Unauthorized, unauthorized.StatusCode);

        var token = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var notFound = await client.PostAsync(
            $"api/v1/territories/{Guid.NewGuid()}/memberships",
            null);

        Assert.Equal(HttpStatusCode.NotFound, notFound.StatusCode);
    }

    [Fact]
    public async Task Memberships_CreatePendingAndReuse()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "new-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var first = await client.PostAsync($"api/v1/territories/{ActiveTerritoryId}/memberships", null);
        first.EnsureSuccessStatusCode();

        var firstPayload = await first.Content.ReadFromJsonAsync<MembershipResponse>();
        Assert.NotNull(firstPayload);
        Assert.Equal("PENDING", firstPayload!.Status);

        var second = await client.PostAsync($"api/v1/territories/{ActiveTerritoryId}/memberships", null);
        second.EnsureSuccessStatusCode();

        var secondPayload = await second.Content.ReadFromJsonAsync<MembershipResponse>();
        Assert.NotNull(secondPayload);
        Assert.Equal(firstPayload.Id, secondPayload!.Id);
    }

    [Fact]
    public async Task Feed_RespectsSessionAndVisibility()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var missingSession = await client.GetAsync("api/v1/feed");
        Assert.Equal(HttpStatusCode.BadRequest, missingSession.StatusCode);

        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "feed-session");

        var missingSelection = await client.GetAsync("api/v1/feed");
        Assert.Equal(HttpStatusCode.BadRequest, missingSelection.StatusCode);

        await SelectTerritoryAsync(client, ActiveTerritoryId);

        var visitorFeed = await client.GetFromJsonAsync<List<FeedItemResponse>>("api/v1/feed");
        Assert.NotNull(visitorFeed);
        Assert.Single(visitorFeed!);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "user:not-a-guid");
        var invalidToken = await client.GetAsync("api/v1/feed");
        Assert.Equal(HttpStatusCode.Unauthorized, invalidToken.StatusCode);

        var nonResidentToken = await LoginForTokenAsync(client, "google", "new-external-feed");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", nonResidentToken);
        var nonResidentFeed = await client.GetFromJsonAsync<List<FeedItemResponse>>("api/v1/feed");
        Assert.NotNull(nonResidentFeed);
        Assert.Single(nonResidentFeed!);

        var token = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var residentFeed = await client.GetFromJsonAsync<List<FeedItemResponse>>("api/v1/feed");
        Assert.NotNull(residentFeed);
        Assert.Equal(2, residentFeed!.Count);
    }

    [Fact]
    public async Task Feed_RejectsInvalidAuthorizationHeader()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "feed-session-2");

        await SelectTerritoryAsync(client, ActiveTerritoryId);

        client.DefaultRequestHeaders.Remove(ApiHeaders.Authorization);
        client.DefaultRequestHeaders.Add(ApiHeaders.Authorization, "Token abc");

        var response = await client.GetAsync("api/v1/feed");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Map_RespectsSessionAndVisibility()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var missingSession = await client.GetAsync("api/v1/map/entities");
        Assert.Equal(HttpStatusCode.BadRequest, missingSession.StatusCode);

        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "map-session");

        var missingSelection = await client.GetAsync("api/v1/map/entities");
        Assert.Equal(HttpStatusCode.BadRequest, missingSelection.StatusCode);

        await SelectTerritoryAsync(client, ActiveTerritoryId);

        var visitorMap = await client.GetFromJsonAsync<List<MapEntityResponse>>("api/v1/map/entities");
        Assert.NotNull(visitorMap);
        Assert.Single(visitorMap!);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "user:not-a-guid");
        var invalidToken = await client.GetAsync("api/v1/map/entities");
        Assert.Equal(HttpStatusCode.Unauthorized, invalidToken.StatusCode);

        var nonResidentToken = await LoginForTokenAsync(client, "google", "new-external-map");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", nonResidentToken);
        var nonResidentMap = await client.GetFromJsonAsync<List<MapEntityResponse>>("api/v1/map/entities");
        Assert.NotNull(nonResidentMap);
        Assert.Single(nonResidentMap!);

        var token = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var residentMap = await client.GetFromJsonAsync<List<MapEntityResponse>>("api/v1/map/entities");
        Assert.NotNull(residentMap);
        Assert.Equal(2, residentMap!.Count);
    }

    private static async Task<string> LoginForTokenAsync(HttpClient client, string provider, string externalId)
    {
        var response = await client.PostAsJsonAsync(
            "api/v1/auth/social",
            new SocialLoginRequest(provider, externalId, "Tester", "tester@araponga.com"));

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<SocialLoginResponse>();
        Assert.NotNull(payload);
        return payload!.Token;
    }

    private static async Task SelectTerritoryAsync(HttpClient client, Guid territoryId)
    {
        var response = await client.PostAsJsonAsync(
            "api/v1/territories/selection",
            new TerritorySelectionRequest(territoryId));

        response.EnsureSuccessStatusCode();
    }
}
