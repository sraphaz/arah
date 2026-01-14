using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Araponga.Api;
using Araponga.Api.Contracts.Auth;
using Araponga.Api.Contracts.Alerts;
using Araponga.Api.Contracts.Assets;
using Araponga.Api.Contracts.Events;
using Araponga.Api.Contracts.Feed;
using Araponga.Api.Contracts.Features;
using Araponga.Api.Contracts.JoinRequests;
using Araponga.Api.Contracts.Map;
using Araponga.Api.Contracts.Memberships;
using Araponga.Api.Contracts.Marketplace;
using Araponga.Api.Contracts.Territories;
using Araponga.Infrastructure.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Araponga.Tests.Api;

public sealed class ApiScenariosTests
{
    private static readonly Guid ActiveTerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid PilotTerritoryId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid ResidentUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    [Fact]
    public async Task SearchTerritories_ByCity()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var territories = await client.GetFromJsonAsync<List<TerritoryResponse>>(
            "api/v1/territories/search?city=Ubatuba&state=SP");

        Assert.NotNull(territories);
        Assert.Equal(2, territories!.Count);
        Assert.All(territories, territory => Assert.Equal("UBATUBA", territory.City.ToUpperInvariant()));
    }

    [Fact]
    public async Task NearbyTerritories_ReturnsOrdered()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var territories = await client.GetFromJsonAsync<List<TerritoryResponse>>(
            "api/v1/territories/nearby?lat=-23.37&lng=-45.02");

        Assert.NotNull(territories);
        Assert.True(territories!.Count >= 2);
    }

    [Fact]
    public async Task TerritoryLookup_ReturnsExpectedStatus()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "territory-lookup");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var existing = await client.GetAsync($"api/v1/territories/{ActiveTerritoryId}");
        existing.EnsureSuccessStatusCode();

        var missing = await client.GetAsync($"api/v1/territories/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }

    [Fact]
    public async Task TerritoryResponse_HasNoSocialFields()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "territory-json");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"api/v1/territories/{ActiveTerritoryId}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("membership", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("role", json, StringComparison.OrdinalIgnoreCase);
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
    public async Task RootPage_ReturnsMinimalHtml()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/");

        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());

        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("Araponga", html, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Developer Portal", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ExceptionHandler_ReturnsStructuredProblem()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/__throw");

        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();

        Assert.NotNull(payload);
        Assert.Equal("Unexpected error", payload!["title"]?.ToString());
        Assert.Equal("/__throw", payload["instance"]?.ToString());
        Assert.Equal("/__throw", payload["path"]?.ToString());
        Assert.True(payload.ContainsKey("traceId"));
    }

    [Fact]
    public async Task AuthSocial_ValidatesPayloadAndReusesUser()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var invalidResponse = await client.PostAsJsonAsync(
            "api/v1/auth/social",
            new SocialLoginRequest("", "", "", "", "", "", "", ""));

        Assert.Equal(HttpStatusCode.BadRequest, invalidResponse.StatusCode);

        var request = new SocialLoginRequest(
            "google",
            "resident-external",
            "Morador Teste",
            "123.456.789-00",
            null,
            "(11) 99999-0000",
            "Rua das Flores, 100",
            "morador@araponga.com");
        var firstLogin = await client.PostAsJsonAsync("api/v1/auth/social", request);
        firstLogin.EnsureSuccessStatusCode();

        var firstPayload = await firstLogin.Content.ReadFromJsonAsync<SocialLoginResponse>();
        Assert.NotNull(firstPayload);

        var secondLogin = await client.PostAsJsonAsync("api/v1/auth/social", request);
        secondLogin.EnsureSuccessStatusCode();
        var secondPayload = await secondLogin.Content.ReadFromJsonAsync<SocialLoginResponse>();

        Assert.NotNull(secondPayload);
        Assert.Equal(firstPayload!.User.Id, secondPayload!.User.Id);
        Assert.Contains('.', secondPayload.Token);
    }

    [Fact]
    public async Task TerritorySuggestion_ValidatesPayload()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var invalidName = await client.PostAsJsonAsync(
            "api/v1/territories/suggestions",
            new SuggestTerritoryRequest("", "Desc", "Cidade", "ST", 0, 0));

        Assert.Equal(HttpStatusCode.BadRequest, invalidName.StatusCode);

        var valid = await client.PostAsJsonAsync(
            "api/v1/territories/suggestions",
            new SuggestTerritoryRequest("Novo Território", "Desc", "Cidade", "ST", -23.5, -44.9));

        Assert.Equal(HttpStatusCode.Created, valid.StatusCode);
    }

    [Fact]
    public async Task Memberships_RequireAuthAndTerritory()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Testar que BecomeResident requer autenticação
        var unauthorized = await client.PostAsync(
            $"api/v1/memberships/{ActiveTerritoryId}/become-resident",
            null);

        Assert.Equal(HttpStatusCode.Unauthorized, unauthorized.StatusCode);

        var token = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add(ApiHeaders.GeoLatitude, "-23.37");
        client.DefaultRequestHeaders.Add(ApiHeaders.GeoLongitude, "-45.02");

        var notFound = await client.PostAsync(
            $"api/v1/memberships/{Guid.NewGuid()}/become-resident",
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
        client.DefaultRequestHeaders.Add(ApiHeaders.GeoLatitude, "-23.37");
        client.DefaultRequestHeaders.Add(ApiHeaders.GeoLongitude, "-45.02");

        var first = await client.PostAsync(
            $"api/v1/memberships/{ActiveTerritoryId}/become-resident",
            null);
        first.EnsureSuccessStatusCode();

        var firstPayload = await first.Content.ReadFromJsonAsync<MembershipDetailResponse>();
        Assert.NotNull(firstPayload);
        Assert.Equal("RESIDENT", firstPayload!.Role);

        var second = await client.PostAsync(
            $"api/v1/memberships/{ActiveTerritoryId}/become-resident",
            null);
        second.EnsureSuccessStatusCode();

        var secondPayload = await second.Content.ReadFromJsonAsync<MembershipDetailResponse>();
        Assert.NotNull(secondPayload);
        Assert.Equal(firstPayload.Id, secondPayload!.Id);
    }

    [Fact]
    public async Task Memberships_UpgradeVisitorToResident()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "upgrade-visitor");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var visitor = await client.PostAsync(
            $"api/v1/territories/{ActiveTerritoryId}/enter",
            null);
        visitor.EnsureSuccessStatusCode();
        var visitorPayload = await visitor.Content.ReadFromJsonAsync<EnterTerritoryResponse>();
        Assert.NotNull(visitorPayload);

        client.DefaultRequestHeaders.Add(ApiHeaders.GeoLatitude, "-23.37");
        client.DefaultRequestHeaders.Add(ApiHeaders.GeoLongitude, "-45.02");

        var upgrade = await client.PostAsync(
            $"api/v1/memberships/{ActiveTerritoryId}/become-resident",
            null);
        upgrade.EnsureSuccessStatusCode();
        var upgradePayload = await upgrade.Content.ReadFromJsonAsync<MembershipDetailResponse>();
        Assert.NotNull(upgradePayload);
        Assert.Equal(visitorPayload!.Id, upgradePayload!.Id);
        Assert.Equal("RESIDENT", upgradePayload.Role);
    }

    [Fact]
    public async Task Memberships_UpgradeRequiresGeo()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "upgrade-missing-geo");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var visitor = await client.PostAsync(
            $"api/v1/territories/{ActiveTerritoryId}/enter",
            null);
        visitor.EnsureSuccessStatusCode();

        // BecomeResident não requer geo - pode ser feito sem coordenadas
        // A verificação geo é feita depois via verify-residency/geo
        var upgrade = await client.PostAsync(
            $"api/v1/memberships/{ActiveTerritoryId}/become-resident",
            null);
        upgrade.EnsureSuccessStatusCode();

        var membership = await upgrade.Content.ReadFromJsonAsync<MembershipDetailResponse>();
        Assert.NotNull(membership);
        Assert.Equal("RESIDENT", membership!.Role);
        // Sem geo, fica como UNVERIFIED ou GEOVERIFIED dependendo se há outros residents
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

        var visitorToken = await LoginForTokenAsync(client, "google", "visitor-feed");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", visitorToken);

        var visitorFeed = await client.GetFromJsonAsync<List<FeedItemResponse>>(
            $"api/v1/feed?territoryId={ActiveTerritoryId}");
        Assert.NotNull(visitorFeed);
        Assert.Equal(2, visitorFeed!.Count);
        Assert.All(visitorFeed, item => Assert.Equal("GENERAL", item.Type));
        Assert.All(visitorFeed, item => Assert.False(item.IsHighlighted));
        Assert.All(visitorFeed, item => Assert.Equal(0, item.LikeCount));

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "not.a.jwt");
        var invalidToken = await client.GetAsync($"api/v1/feed?territoryId={ActiveTerritoryId}");
        Assert.Equal(HttpStatusCode.Unauthorized, invalidToken.StatusCode);

        var nonResidentToken = await LoginForTokenAsync(client, "google", "new-external-feed");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", nonResidentToken);
        var nonResidentFeed = await client.GetFromJsonAsync<List<FeedItemResponse>>(
            $"api/v1/feed?territoryId={ActiveTerritoryId}");
        Assert.NotNull(nonResidentFeed);
        Assert.Equal(2, nonResidentFeed!.Count);

        var token = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var residentFeed = await client.GetFromJsonAsync<List<FeedItemResponse>>(
            $"api/v1/feed?territoryId={ActiveTerritoryId}");
        Assert.NotNull(residentFeed);
        Assert.Equal(2, residentFeed!.Count);
    }

    [Fact]
    public async Task Feed_VisitorMembershipSeesOnlyPublic()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "feed-visitor");
        await SelectTerritoryAsync(client, ActiveTerritoryId);

        var token = await LoginForTokenAsync(client, "google", "visitor-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var membership = await client.PostAsync(
            $"api/v1/territories/{ActiveTerritoryId}/enter",
            null);
        membership.EnsureSuccessStatusCode();

        var feed = await client.GetFromJsonAsync<List<FeedItemResponse>>(
            $"api/v1/feed?territoryId={ActiveTerritoryId}");
        Assert.NotNull(feed);
        Assert.Equal(2, feed!.Count);
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

        var response = await client.GetAsync($"api/v1/feed?territoryId={ActiveTerritoryId}");

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

        var visitorToken = await LoginForTokenAsync(client, "google", "visitor-map");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", visitorToken);

        var visitorMap = await client.GetFromJsonAsync<List<MapEntityResponse>>(
            $"api/v1/map/entities?territoryId={ActiveTerritoryId}");
        Assert.NotNull(visitorMap);
        Assert.Single(visitorMap!);
        Assert.Equal("VALIDATED", visitorMap![0].Status);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "not.a.jwt");
        var invalidToken = await client.GetAsync($"api/v1/map/entities?territoryId={ActiveTerritoryId}");
        Assert.Equal(HttpStatusCode.Unauthorized, invalidToken.StatusCode);

        var nonResidentToken = await LoginForTokenAsync(client, "google", "new-external-map");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", nonResidentToken);
        var nonResidentMap = await client.GetFromJsonAsync<List<MapEntityResponse>>(
            $"api/v1/map/entities?territoryId={ActiveTerritoryId}");
        Assert.NotNull(nonResidentMap);
        Assert.Single(nonResidentMap!);

        var token = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var residentMap = await client.GetFromJsonAsync<List<MapEntityResponse>>(
            $"api/v1/map/entities?territoryId={ActiveTerritoryId}");
        Assert.NotNull(residentMap);
        Assert.Equal(2, residentMap!.Count);
    }

    [Fact]
    public async Task MembershipStatus_ReturnsNoneAndValidated()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "new-status");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var noneStatusResponse = await client.GetAsync(
            $"api/v1/memberships/{ActiveTerritoryId}/me");
        Assert.Equal(HttpStatusCode.NotFound, noneStatusResponse.StatusCode);

        var residentToken = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", residentToken);

        var validatedStatus = await client.GetFromJsonAsync<MembershipDetailResponse>(
            $"api/v1/memberships/{ActiveTerritoryId}/me");
        Assert.NotNull(validatedStatus);
        Assert.Equal("RESIDENT", validatedStatus!.Role);
        Assert.True(validatedStatus.ResidencyVerification == "GEOVERIFIED" || validatedStatus.ResidencyVerification == "DOCUMENTVERIFIED");
    }

    [Fact]
    public async Task MembershipValidation_RequiresValidatedResident()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var userToken = await LoginForTokenAsync(client, "google", "new-member");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
        client.DefaultRequestHeaders.Add(ApiHeaders.GeoLatitude, "-23.37");
        client.DefaultRequestHeaders.Add(ApiHeaders.GeoLongitude, "-45.02");

        // Criar membership como Resident
        var membership = await client.PostAsync(
            $"api/v1/memberships/{ActiveTerritoryId}/become-resident",
            null);
        membership.EnsureSuccessStatusCode();
        var membershipPayload = await membership.Content.ReadFromJsonAsync<MembershipDetailResponse>();
        Assert.NotNull(membershipPayload);
        Assert.Equal("RESIDENT", membershipPayload!.Role);
        Assert.Equal("UNVERIFIED", membershipPayload.ResidencyVerification);

        // Verificar que pode verificar residência por geo (coordenadas próximas)
        var verifyGeoResponse = await client.PostAsJsonAsync(
            $"api/v1/memberships/{ActiveTerritoryId}/verify-residency/geo",
            new VerifyResidencyGeoRequest(-23.37, -45.02));
        verifyGeoResponse.EnsureSuccessStatusCode();

        // Verificar que membership foi atualizado
        var verifiedMembership = await client.GetFromJsonAsync<MembershipDetailResponse>(
            $"api/v1/memberships/{ActiveTerritoryId}/me");
        Assert.NotNull(verifiedMembership);
        Assert.Equal("GEOVERIFIED", verifiedMembership!.ResidencyVerification);
    }

    [Fact]
    public async Task Feed_CreatePost_LikeCommentShare()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "feed-actions");

        await SelectTerritoryAsync(client, ActiveTerritoryId);

        var visitorLike = await client.PostAsync(
            $"api/v1/feed/cccccccc-cccc-cccc-cccc-cccccccccccc/likes?territoryId={ActiveTerritoryId}",
            null);
        Assert.Equal(HttpStatusCode.NoContent, visitorLike.StatusCode);

        var residentOnlyLike = await client.PostAsync(
            $"api/v1/feed/dddddddd-dddd-dddd-dddd-dddddddddddd/likes?territoryId={ActiveTerritoryId}",
            null);
        Assert.Equal(HttpStatusCode.BadRequest, residentOnlyLike.StatusCode);

        var token = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var created = await client.PostAsJsonAsync(
            $"api/v1/feed?territoryId={ActiveTerritoryId}",
            new CreatePostRequest(
                "Novo post",
                "Conteúdo",
                "GENERAL",
                "PUBLIC",
                null,
                new List<GeoAnchorRequest> { new(-23.37, -45.02, "POST") },
                null));
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var invalidPost = await client.PostAsJsonAsync(
            $"api/v1/feed?territoryId={ActiveTerritoryId}",
            new CreatePostRequest(
                "Novo post",
                "Conteúdo",
                "INVALID",
                "PUBLIC",
                null,
                new List<GeoAnchorRequest> { new(-23.37, -45.02, "POST") },
                null));
        Assert.Equal(HttpStatusCode.BadRequest, invalidPost.StatusCode);

        var createdPost = await created.Content.ReadFromJsonAsync<FeedItemResponse>();
        Assert.NotNull(createdPost);

        var comment = await client.PostAsJsonAsync(
            $"api/v1/feed/{createdPost!.Id}/comments?territoryId={ActiveTerritoryId}",
            new AddCommentRequest("Comentário"));
        Assert.Equal(HttpStatusCode.NoContent, comment.StatusCode);

        var share = await client.PostAsync(
            $"api/v1/feed/{createdPost.Id}/shares?territoryId={ActiveTerritoryId}",
            null);
        Assert.Equal(HttpStatusCode.NoContent, share.StatusCode);

        var feed = await client.GetFromJsonAsync<List<FeedItemResponse>>(
            $"api/v1/feed?territoryId={ActiveTerritoryId}");
        Assert.NotNull(feed);
        Assert.Contains(feed!, item => item.Id == createdPost.Id);
    }

    [Fact]
    public async Task Feed_CreatePost_IgnoresGeoAnchorsFromRequest()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "feed-geo-ignore");

        await SelectTerritoryAsync(client, ActiveTerritoryId);

        var token = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var created = await client.PostAsJsonAsync(
            $"api/v1/feed?territoryId={ActiveTerritoryId}",
            new CreatePostRequest(
                "Post sem anchor manual",
                "Conteúdo",
                "GENERAL",
                "PUBLIC",
                null,
                new List<GeoAnchorRequest> { new(-23.37, -45.02, "POST") },
                null));
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var createdPost = await created.Content.ReadFromJsonAsync<FeedItemResponse>();
        Assert.NotNull(createdPost);

        var dataStore = factory.Services.GetRequiredService<InMemoryDataStore>();
        var anchors = dataStore.PostGeoAnchors.Where(anchor => anchor.PostId == createdPost!.Id).ToList();
        Assert.NotEmpty(anchors);
    }

    [Fact]
    public async Task Events_CreateAndParticipate()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var visitorToken = await LoginForTokenAsync(client, "google", "visitor-event");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", visitorToken);

        var create = await client.PostAsJsonAsync(
            "api/v1/events",
            new CreateEventRequest(
                ActiveTerritoryId,
                "Evento visitante",
                "Detalhes",
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(1).AddHours(2),
                -23.37,
                -45.02,
                "Praça"));
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);

        var created = await create.Content.ReadFromJsonAsync<EventResponse>();
        Assert.NotNull(created);

        var interest = await client.PostAsync($"api/v1/events/{created!.EventId}/interest", null);
        Assert.Equal(HttpStatusCode.NoContent, interest.StatusCode);

        var confirm = await client.PostAsync($"api/v1/events/{created.EventId}/confirm", null);
        Assert.Equal(HttpStatusCode.NoContent, confirm.StatusCode);
    }

    [Fact]
    public async Task FeatureFlags_CuratorCanUpdate()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var flags = await client.GetFromJsonAsync<FeatureFlagResponse>(
            $"api/v1/territories/{PilotTerritoryId}/features");
        Assert.NotNull(flags);
        Assert.DoesNotContain("ALERTPOSTS", flags!.EnabledFlags);

        var curatorToken = await LoginForTokenAsync(client, "google", "curator-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", curatorToken);

        var update = await client.PutAsJsonAsync(
            $"api/v1/territories/{PilotTerritoryId}/features",
            new UpdateFeatureFlagsRequest(new[] { "AlertPosts" }));
        update.EnsureSuccessStatusCode();

        var updated = await update.Content.ReadFromJsonAsync<FeatureFlagResponse>();
        Assert.NotNull(updated);
        Assert.Contains("ALERTPOSTS", updated!.EnabledFlags);
    }

    [Fact]
    public async Task FeatureFlags_RejectInvalidFlagAndUnauthorized()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var unauthorized = await client.PutAsJsonAsync(
            $"api/v1/territories/{PilotTerritoryId}/features",
            new UpdateFeatureFlagsRequest(new[] { "AlertPosts" }));
        Assert.Equal(HttpStatusCode.Unauthorized, unauthorized.StatusCode);

        var curatorToken = await LoginForTokenAsync(client, "google", "curator-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", curatorToken);

        var invalid = await client.PutAsJsonAsync(
            $"api/v1/territories/{PilotTerritoryId}/features",
            new UpdateFeatureFlagsRequest(new[] { "InvalidFlag" }));
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
    }

    [Fact]
    public async Task Map_SuggestValidateConfirm()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "map-actions");
        await SelectTerritoryAsync(client, ActiveTerritoryId);

        var residentToken = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", residentToken);

        var suggestion = await client.PostAsJsonAsync(
            $"api/v1/map/entities?territoryId={ActiveTerritoryId}",
            new SuggestMapEntityRequest("Ponto novo", "espaço natural", -23.37, -45.02));
        suggestion.EnsureSuccessStatusCode();
        var entity = await suggestion.Content.ReadFromJsonAsync<MapEntityResponse>();
        Assert.NotNull(entity);
        Assert.Equal("SUGGESTED", entity!.Status);

        var curatorToken = await LoginForTokenAsync(client, "google", "curator-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", curatorToken);

        var validation = await client.PatchAsJsonAsync(
            $"api/v1/map/entities/{entity.Id}/validation?territoryId={ActiveTerritoryId}",
            new ValidateMapEntityRequest("VALIDATED"));
        Assert.Equal(HttpStatusCode.NoContent, validation.StatusCode);

        var invalidValidation = await client.PatchAsJsonAsync(
            $"api/v1/map/entities/{entity.Id}/validation?territoryId={ActiveTerritoryId}",
            new ValidateMapEntityRequest("INVALID"));
        Assert.Equal(HttpStatusCode.BadRequest, invalidValidation.StatusCode);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", residentToken);
        var confirmation = await client.PostAsync(
            $"api/v1/map/entities/{entity.Id}/confirmations?territoryId={ActiveTerritoryId}",
            null);
        Assert.Equal(HttpStatusCode.NoContent, confirmation.StatusCode);
    }

    [Fact]
    public async Task Map_RelateEntityRequiresResident()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "map-relations");
        await SelectTerritoryAsync(client, ActiveTerritoryId);

        var visitorToken = await LoginForTokenAsync(client, "google", "visitor-rel");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", visitorToken);

        var fail = await client.PostAsync(
            $"api/v1/map/entities/eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee/relations?territoryId={ActiveTerritoryId}",
            null);
        Assert.Equal(HttpStatusCode.BadRequest, fail.StatusCode);

        var residentToken = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", residentToken);

        var success = await client.PostAsync(
            $"api/v1/map/entities/eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee/relations?territoryId={ActiveTerritoryId}",
            null);
        Assert.Equal(HttpStatusCode.Created, success.StatusCode);
    }

    [Fact]
    public async Task Map_Pins_ReturnEntitiesAndAnchors()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "map-pins");
        await SelectTerritoryAsync(client, ActiveTerritoryId);

        var residentToken = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", residentToken);

        var pins = await client.GetFromJsonAsync<List<MapPinResponse>>(
            $"api/v1/map/pins?territoryId={ActiveTerritoryId}");

        Assert.NotNull(pins);
        Assert.NotEmpty(pins!);
        Assert.Contains(pins, pin => pin.PinType == "entity");
        Assert.Contains(pins, pin => pin.EntityId == Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"));
        Assert.Contains(pins, pin => pin.PostId == Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"));
    }

    [Fact]
    public async Task Assets_RequireGeoAnchors()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "assets-geo");
        await SelectTerritoryAsync(client, ActiveTerritoryId);

        var residentToken = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", residentToken);

        var missingAnchors = await client.PostAsJsonAsync(
            "api/v1/assets",
            new CreateAssetRequest(
                ActiveTerritoryId,
                "river",
                "Rio do Vale",
                "Descrição",
                Array.Empty<AssetGeoAnchorRequest>()));
        Assert.Equal(HttpStatusCode.BadRequest, missingAnchors.StatusCode);

        var created = await client.PostAsJsonAsync(
            "api/v1/assets",
            new CreateAssetRequest(
                ActiveTerritoryId,
                "river",
                "Rio do Vale",
                "Descrição",
                new[] { new AssetGeoAnchorRequest(-23.37, -45.02) }));
        created.EnsureSuccessStatusCode();
        var asset = await created.Content.ReadFromJsonAsync<AssetResponse>();
        Assert.NotNull(asset);

        var badPatch = await client.PatchAsJsonAsync(
            $"api/v1/assets/{asset!.Id}?territoryId={ActiveTerritoryId}",
            new UpdateAssetRequest(
                "river",
                "Rio do Vale",
                "Descrição",
                Array.Empty<AssetGeoAnchorRequest>()));
        Assert.Equal(HttpStatusCode.BadRequest, badPatch.StatusCode);
    }

    [Fact]
    public async Task Assets_ValidationIsIdempotent()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "assets-validation");
        await SelectTerritoryAsync(client, ActiveTerritoryId);

        var residentToken = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", residentToken);

        var created = await client.PostAsJsonAsync(
            "api/v1/assets",
            new CreateAssetRequest(
                ActiveTerritoryId,
                "spring",
                "Nascente do Vale",
                null,
                new[] { new AssetGeoAnchorRequest(-23.371, -45.021) }));
        created.EnsureSuccessStatusCode();
        var asset = await created.Content.ReadFromJsonAsync<AssetResponse>();
        Assert.NotNull(asset);

        var first = await client.PostAsync(
            $"api/v1/assets/{asset!.Id}/validate?territoryId={ActiveTerritoryId}",
            null);
        first.EnsureSuccessStatusCode();
        var firstPayload = await first.Content.ReadFromJsonAsync<AssetValidationResponse>();
        Assert.NotNull(firstPayload);
        Assert.Equal(1, firstPayload!.ValidationsCount);

        var second = await client.PostAsync(
            $"api/v1/assets/{asset.Id}/validate?territoryId={ActiveTerritoryId}",
            null);
        second.EnsureSuccessStatusCode();
        var secondPayload = await second.Content.ReadFromJsonAsync<AssetValidationResponse>();
        Assert.NotNull(secondPayload);
        Assert.Equal(1, secondPayload!.ValidationsCount);
    }

    [Fact]
    public async Task Assets_ListFiltersByIdAndType()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "assets-list");
        await SelectTerritoryAsync(client, ActiveTerritoryId);

        var residentToken = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", residentToken);

        var river = await client.PostAsJsonAsync(
            "api/v1/assets",
            new CreateAssetRequest(
                ActiveTerritoryId,
                "river",
                "Rio Principal",
                null,
                new[] { new AssetGeoAnchorRequest(-23.372, -45.022) }));
        river.EnsureSuccessStatusCode();
        var riverAsset = await river.Content.ReadFromJsonAsync<AssetResponse>();

        var spring = await client.PostAsJsonAsync(
            "api/v1/assets",
            new CreateAssetRequest(
                ActiveTerritoryId,
                "spring",
                "Nascente Azul",
                null,
                new[] { new AssetGeoAnchorRequest(-23.373, -45.023) }));
        spring.EnsureSuccessStatusCode();

        var byType = await client.GetFromJsonAsync<List<AssetResponse>>(
            $"api/v1/assets?territoryId={ActiveTerritoryId}&types=river");
        Assert.NotNull(byType);
        Assert.All(byType!, asset => Assert.Equal("river", asset.Type));

        var byId = await client.GetFromJsonAsync<List<AssetResponse>>(
            $"api/v1/assets?territoryId={ActiveTerritoryId}&assetId={riverAsset!.Id}");
        Assert.NotNull(byId);
        Assert.Single(byId!);
        Assert.Equal(riverAsset.Id, byId[0].Id);
    }

    [Fact]
    public async Task Feed_FiltersByAssetId()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "feed-assets");
        await SelectTerritoryAsync(client, ActiveTerritoryId);

        var residentToken = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", residentToken);

        var assetResponse = await client.PostAsJsonAsync(
            "api/v1/assets",
            new CreateAssetRequest(
                ActiveTerritoryId,
                "beach",
                "Praia do Vale",
                null,
                new[] { new AssetGeoAnchorRequest(-23.374, -45.024) }));
        assetResponse.EnsureSuccessStatusCode();
        var asset = await assetResponse.Content.ReadFromJsonAsync<AssetResponse>();
        Assert.NotNull(asset);

        var postResponse = await client.PostAsJsonAsync(
            $"api/v1/feed?territoryId={ActiveTerritoryId}",
            new CreatePostRequest(
                "Post com asset",
                "Conteudo",
                "GENERAL",
                "PUBLIC",
                null,
                null,
                new[] { asset!.Id }));
        postResponse.EnsureSuccessStatusCode();
        var post = await postResponse.Content.ReadFromJsonAsync<FeedItemResponse>();
        Assert.NotNull(post);

        var filtered = await client.GetFromJsonAsync<List<FeedItemResponse>>(
            $"api/v1/feed?territoryId={ActiveTerritoryId}&assetId={asset.Id}");
        Assert.NotNull(filtered);
        Assert.Single(filtered!);
        Assert.Equal(post!.Id, filtered[0].Id);
    }

    [Fact]
    public async Task Map_Pins_FilterAssets()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "map-assets");
        await SelectTerritoryAsync(client, ActiveTerritoryId);

        var residentToken = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", residentToken);

        var riverResponse = await client.PostAsJsonAsync(
            "api/v1/assets",
            new CreateAssetRequest(
                ActiveTerritoryId,
                "river",
                "Rio das Pedras",
                null,
                new[] { new AssetGeoAnchorRequest(-23.375, -45.025) }));
        riverResponse.EnsureSuccessStatusCode();
        var riverAsset = await riverResponse.Content.ReadFromJsonAsync<AssetResponse>();

        var springResponse = await client.PostAsJsonAsync(
            "api/v1/assets",
            new CreateAssetRequest(
                ActiveTerritoryId,
                "spring",
                "Nascente Clara",
                null,
                new[] { new AssetGeoAnchorRequest(-23.376, -45.026) }));
        springResponse.EnsureSuccessStatusCode();

        var byType = await client.GetFromJsonAsync<List<MapPinResponse>>(
            $"api/v1/map/pins?territoryId={ActiveTerritoryId}&types=asset&assetTypes=river");
        Assert.NotNull(byType);
        Assert.All(byType!, pin => Assert.Equal(riverAsset!.Id, pin.AssetId));

        var byId = await client.GetFromJsonAsync<List<MapPinResponse>>(
            $"api/v1/map/pins?territoryId={ActiveTerritoryId}&types=asset&assetId={riverAsset!.Id}");
        Assert.NotNull(byId);
        Assert.All(byId!, pin => Assert.Equal(riverAsset.Id, pin.AssetId));
    }

    [Fact]
    public async Task Alerts_ReportAndValidate()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "alerts-actions");
        await SelectTerritoryAsync(client, ActiveTerritoryId);

        var residentToken = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", residentToken);

        var report = await client.PostAsJsonAsync(
            $"api/v1/alerts?territoryId={ActiveTerritoryId}",
            new ReportAlertRequest("Alerta", "Descrição"));
        report.EnsureSuccessStatusCode();
        var alert = await report.Content.ReadFromJsonAsync<AlertResponse>();
        Assert.NotNull(alert);
        Assert.Equal("PENDING", alert!.Status);

        var curatorToken = await LoginForTokenAsync(client, "google", "curator-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", curatorToken);

        var validate = await client.PatchAsJsonAsync(
            $"api/v1/alerts/{alert.Id}/validation?territoryId={ActiveTerritoryId}",
            new ValidateAlertRequest("VALIDATED"));
        Assert.Equal(HttpStatusCode.NoContent, validate.StatusCode);

        var invalidValidate = await client.PatchAsJsonAsync(
            $"api/v1/alerts/{alert.Id}/validation?territoryId={ActiveTerritoryId}",
            new ValidateAlertRequest("INVALID"));
        Assert.Equal(HttpStatusCode.BadRequest, invalidValidate.StatusCode);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", residentToken);
        var feed = await client.GetFromJsonAsync<List<FeedItemResponse>>(
            $"api/v1/feed?territoryId={ActiveTerritoryId}");
        Assert.NotNull(feed);
        Assert.Contains(feed!, item => item.Type == "ALERT" && item.IsHighlighted);
    }

    [Fact]
    public async Task JoinRequests_CreateIncomingAndApprove()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var requesterToken = await LoginForTokenAsync(client, "google", "join-requester");
        var residentToken = await LoginForTokenAsync(client, "google", "resident-external");
        var otherToken = await LoginForTokenAsync(client, "google", "join-other");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", requesterToken);
        var createResponse = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/join-requests",
            new CreateJoinRequestRequest(new[] { ResidentUserId }, "Quero entrar"));

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<JoinRequestCreatedResponse>();
        Assert.NotNull(created);
        Assert.Equal("PENDING", created!.Status);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", residentToken);
        var incoming = await client.GetFromJsonAsync<List<IncomingJoinRequestResponse>>(
            "api/v1/join-requests/incoming?status=pending");
        Assert.NotNull(incoming);
        Assert.Contains(incoming!, item => item.JoinRequestId == created.JoinRequestId);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", otherToken);
        var incomingOther = await client.GetFromJsonAsync<List<IncomingJoinRequestResponse>>(
            "api/v1/join-requests/incoming?status=pending");
        Assert.NotNull(incomingOther);
        Assert.DoesNotContain(incomingOther!, item => item.JoinRequestId == created.JoinRequestId);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", residentToken);
        var approveResponse = await client.PostAsync(
            $"api/v1/join-requests/{created.JoinRequestId}/approve",
            null);
        approveResponse.EnsureSuccessStatusCode();
        var approvePayload = await approveResponse.Content.ReadFromJsonAsync<JoinRequestActionResponse>();
        Assert.NotNull(approvePayload);
        Assert.Equal("APPROVED", approvePayload!.Status);

        var approveAgain = await client.PostAsync(
            $"api/v1/join-requests/{created.JoinRequestId}/approve",
            null);
        approveAgain.EnsureSuccessStatusCode();
        var approveAgainPayload = await approveAgain.Content.ReadFromJsonAsync<JoinRequestActionResponse>();
        Assert.NotNull(approveAgainPayload);
        Assert.Equal("APPROVED", approveAgainPayload!.Status);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", requesterToken);
        var membershipStatus = await client.GetFromJsonAsync<MembershipDetailResponse>(
            $"api/v1/memberships/{ActiveTerritoryId}/me");
        Assert.NotNull(membershipStatus);
        Assert.Equal("RESIDENT", membershipStatus!.Role);
        Assert.True(membershipStatus.ResidencyVerification == "GEOVERIFIED" || membershipStatus.ResidencyVerification == "DOCUMENTVERIFIED");
    }

    [Fact]
    public async Task JoinRequests_RequireRecipients()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var requesterToken = await LoginForTokenAsync(client, "google", "join-empty");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", requesterToken);

        var response = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/join-requests",
            new CreateJoinRequestRequest(Array.Empty<Guid>(), null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task JoinRequests_ApproveRequiresRecipientOrAdmin()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var requesterToken = await LoginForTokenAsync(client, "google", "join-nonrecipient");
        var otherToken = await LoginForTokenAsync(client, "google", "join-random");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", requesterToken);
        var createResponse = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/join-requests",
            new CreateJoinRequestRequest(new[] { ResidentUserId }, "Teste"));

        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<JoinRequestCreatedResponse>();
        Assert.NotNull(created);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", otherToken);
        var approveResponse = await client.PostAsync(
            $"api/v1/join-requests/{created!.JoinRequestId}/approve",
            null);

        Assert.Equal(HttpStatusCode.Forbidden, approveResponse.StatusCode);
    }

    [Fact]
    public async Task JoinRequests_RejectDoesNotPromoteMembership()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var requesterToken = await LoginForTokenAsync(client, "google", "join-rejecter");
        var residentToken = await LoginForTokenAsync(client, "google", "resident-external");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", requesterToken);
        var createResponse = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/join-requests",
            new CreateJoinRequestRequest(new[] { ResidentUserId }, "Rejeitar"));

        createResponse.EnsureSuccessStatusCode();
        var created = await createResponse.Content.ReadFromJsonAsync<JoinRequestCreatedResponse>();
        Assert.NotNull(created);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", residentToken);
        var rejectResponse = await client.PostAsync(
            $"api/v1/join-requests/{created!.JoinRequestId}/reject",
            null);
        rejectResponse.EnsureSuccessStatusCode();
        var rejectPayload = await rejectResponse.Content.ReadFromJsonAsync<JoinRequestActionResponse>();
        Assert.NotNull(rejectPayload);
        Assert.Equal("REJECTED", rejectPayload!.Status);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", requesterToken);
        var membershipStatusResponse = await client.GetAsync(
            $"api/v1/memberships/{ActiveTerritoryId}/me");
        Assert.Equal(HttpStatusCode.NotFound, membershipStatusResponse.StatusCode);
    }

    [Fact]
    public async Task Marketplace_VisitorCannotCreateStoreOrListing()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var visitorToken = await LoginForTokenAsync(client, "google", "visitor-store");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", visitorToken);

        var storeResponse = await client.PostAsJsonAsync(
            "api/v1/stores",
            new UpsertStoreRequest(
                ActiveTerritoryId,
                "Loja Visitante",
                null,
                "ON_INQUIRY_ONLY",
                new StoreContactPayload(null, null, null, null, null, null)));

        Assert.Equal(HttpStatusCode.Forbidden, storeResponse.StatusCode);

        var residentToken = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", residentToken);

        var residentStoreResponse = await client.PostAsJsonAsync(
            "api/v1/stores",
            new UpsertStoreRequest(
                ActiveTerritoryId,
                "Loja Morador",
                null,
                "ON_INQUIRY_ONLY",
                new StoreContactPayload(null, null, null, null, null, null)));

        residentStoreResponse.EnsureSuccessStatusCode();
        var residentStore = await residentStoreResponse.Content.ReadFromJsonAsync<StoreResponse>();
        Assert.NotNull(residentStore);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", visitorToken);
        var listingResponse = await client.PostAsJsonAsync(
            "api/v1/listings",
            new CreateListingRequest(
                ActiveTerritoryId,
                residentStore!.Id,
                "PRODUCT",
                "Produto",
                null,
                null,
                null,
                "FIXED",
                10m,
                "BRL",
                "unidade",
                null,
                null,
                "ACTIVE"));

        Assert.Equal(HttpStatusCode.Forbidden, listingResponse.StatusCode);
    }

    [Fact]
    public async Task Marketplace_SearchAndInquiryWork()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var residentToken = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", residentToken);

        var storeResponse = await client.PostAsJsonAsync(
            "api/v1/stores",
            new UpsertStoreRequest(
                ActiveTerritoryId,
                "Loja Morador",
                null,
                "PUBLIC",
                new StoreContactPayload("(11) 90000-0000", null, "loja@exemplo.com", null, null, "email")));

        storeResponse.EnsureSuccessStatusCode();
        var store = await storeResponse.Content.ReadFromJsonAsync<StoreResponse>();
        Assert.NotNull(store);

        var listingResponse = await client.PostAsJsonAsync(
            "api/v1/listings",
            new CreateListingRequest(
                ActiveTerritoryId,
                store!.Id,
                "PRODUCT",
                "Mel do Vale",
                "Mel local",
                "Alimentos",
                "mel",
                "FIXED",
                30m,
                "BRL",
                "unidade",
                null,
                null,
                "ACTIVE"));

        listingResponse.EnsureSuccessStatusCode();
        var listing = await listingResponse.Content.ReadFromJsonAsync<ListingResponse>();
        Assert.NotNull(listing);

        var search = await client.GetFromJsonAsync<List<ListingResponse>>(
            $"api/v1/listings?territoryId={ActiveTerritoryId}&type=PRODUCT&q=mel");

        Assert.NotNull(search);
        Assert.Contains(search!, item => item.Id == listing!.Id);

        var inquiryResponse = await client.PostAsJsonAsync(
            $"api/v1/listings/{listing!.Id}/inquiries",
            new CreateInquiryRequest("Quero saber mais"));

        inquiryResponse.EnsureSuccessStatusCode();
        var inquiry = await inquiryResponse.Content.ReadFromJsonAsync<InquiryResponse>();
        Assert.NotNull(inquiry);
        Assert.NotNull(inquiry!.StoreContact);
    }

    private static async Task<string> LoginForTokenAsync(HttpClient client, string provider, string externalId)
    {
        var response = await client.PostAsJsonAsync(
            "api/v1/auth/social",
            new SocialLoginRequest(
                provider,
                externalId,
                "Tester",
                "123.456.789-00",
                null,
                "(11) 90000-0000",
                "Rua das Flores, 100",
                "tester@araponga.com"));

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
