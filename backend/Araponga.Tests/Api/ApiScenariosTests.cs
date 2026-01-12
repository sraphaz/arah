using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Araponga.Api;
using Araponga.Api.Contracts.Auth;
using Araponga.Api.Contracts.Alerts;
using Araponga.Api.Contracts.Assets;
using Araponga.Api.Contracts.Feed;
using Araponga.Api.Contracts.Features;
using Araponga.Api.Contracts.Map;
using Araponga.Api.Contracts.Memberships;
using Araponga.Api.Contracts.Territories;
using Araponga.Infrastructure.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Araponga.Tests.Api;

public sealed class ApiScenariosTests
{
    private static readonly Guid ActiveTerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid PilotTerritoryId = Guid.Parse("11111111-1111-1111-1111-111111111111");

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
        Assert.Contains("Araponga API", html, StringComparison.OrdinalIgnoreCase);
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

        var unauthorized = await client.PostAsync(
            $"api/v1/territories/{ActiveTerritoryId}/membership",
            null);

        Assert.Equal(HttpStatusCode.UnsupportedMediaType, unauthorized.StatusCode);

        var token = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add(ApiHeaders.GeoLatitude, "-23.37");
        client.DefaultRequestHeaders.Add(ApiHeaders.GeoLongitude, "-45.02");

        var notFound = await client.PostAsJsonAsync(
            $"api/v1/territories/{Guid.NewGuid()}/membership",
            new DeclareMembershipRequest("RESIDENT"));

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

        var first = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/membership",
            new DeclareMembershipRequest("RESIDENT"));
        first.EnsureSuccessStatusCode();

        var firstPayload = await first.Content.ReadFromJsonAsync<MembershipResponse>();
        Assert.NotNull(firstPayload);
        Assert.Equal("RESIDENT", firstPayload!.Role);
        Assert.Equal("PENDING", firstPayload.VerificationStatus);

        var second = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/membership",
            new DeclareMembershipRequest("RESIDENT"));
        second.EnsureSuccessStatusCode();

        var secondPayload = await second.Content.ReadFromJsonAsync<MembershipResponse>();
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

        var visitor = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/membership",
            new DeclareMembershipRequest("VISITOR"));
        visitor.EnsureSuccessStatusCode();
        var visitorPayload = await visitor.Content.ReadFromJsonAsync<MembershipResponse>();
        Assert.NotNull(visitorPayload);

        client.DefaultRequestHeaders.Add(ApiHeaders.GeoLatitude, "-23.37");
        client.DefaultRequestHeaders.Add(ApiHeaders.GeoLongitude, "-45.02");

        var upgrade = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/membership",
            new DeclareMembershipRequest("RESIDENT"));
        upgrade.EnsureSuccessStatusCode();
        var upgradePayload = await upgrade.Content.ReadFromJsonAsync<MembershipResponse>();
        Assert.NotNull(upgradePayload);
        Assert.Equal(visitorPayload!.Id, upgradePayload!.Id);
        Assert.Equal("RESIDENT", upgradePayload.Role);
        Assert.Equal("PENDING", upgradePayload.VerificationStatus);
    }

    [Fact]
    public async Task Memberships_UpgradeRequiresGeo()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "upgrade-missing-geo");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var visitor = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/membership",
            new DeclareMembershipRequest("VISITOR"));
        visitor.EnsureSuccessStatusCode();

        var upgrade = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/membership",
            new DeclareMembershipRequest("RESIDENT"));
        Assert.Equal(HttpStatusCode.BadRequest, upgrade.StatusCode);
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
        Assert.Single(visitorFeed!);
        Assert.Equal("GENERAL", visitorFeed![0].Type);
        Assert.False(visitorFeed[0].IsHighlighted);
        Assert.Equal(0, visitorFeed[0].LikeCount);

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "not.a.jwt");
        var invalidToken = await client.GetAsync($"api/v1/feed?territoryId={ActiveTerritoryId}");
        Assert.Equal(HttpStatusCode.Unauthorized, invalidToken.StatusCode);

        var nonResidentToken = await LoginForTokenAsync(client, "google", "new-external-feed");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", nonResidentToken);
        var nonResidentFeed = await client.GetFromJsonAsync<List<FeedItemResponse>>(
            $"api/v1/feed?territoryId={ActiveTerritoryId}");
        Assert.NotNull(nonResidentFeed);
        Assert.Single(nonResidentFeed!);

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

        var membership = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/membership",
            new DeclareMembershipRequest("VISITOR"));
        membership.EnsureSuccessStatusCode();

        var feed = await client.GetFromJsonAsync<List<FeedItemResponse>>(
            $"api/v1/feed?territoryId={ActiveTerritoryId}");
        Assert.NotNull(feed);
        Assert.Single(feed!);
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

        var noneStatus = await client.GetFromJsonAsync<MembershipStatusResponse>(
            $"api/v1/territories/{ActiveTerritoryId}/membership/me");
        Assert.NotNull(noneStatus);
        Assert.Equal("NONE", noneStatus!.Role);
        Assert.Equal("NONE", noneStatus.VerificationStatus);

        var residentToken = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", residentToken);

        var validatedStatus = await client.GetFromJsonAsync<MembershipStatusResponse>(
            $"api/v1/territories/{ActiveTerritoryId}/membership/me");
        Assert.NotNull(validatedStatus);
        Assert.Equal("RESIDENT", validatedStatus!.Role);
        Assert.Equal("VALIDATED", validatedStatus.VerificationStatus);
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

        var membership = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/membership",
            new DeclareMembershipRequest("RESIDENT"));
        membership.EnsureSuccessStatusCode();
        var membershipPayload = await membership.Content.ReadFromJsonAsync<MembershipResponse>();
        Assert.NotNull(membershipPayload);

        var fail = await client.PatchAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/membership/{membershipPayload!.Id}/validation",
            new ValidateMembershipRequest("VALIDATED"));
        Assert.Equal(HttpStatusCode.Forbidden, fail.StatusCode);

        var residentToken = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", residentToken);

        var ok = await client.PatchAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/membership/{membershipPayload.Id}/validation",
            new ValidateMembershipRequest("VALIDATED"));
        Assert.Equal(HttpStatusCode.NoContent, ok.StatusCode);

        var invalidStatus = await client.PatchAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/membership/{membershipPayload.Id}/validation",
            new ValidateMembershipRequest("INVALID"));
        Assert.Equal(HttpStatusCode.BadRequest, invalidStatus.StatusCode);
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
    public async Task Feed_EventRequiresApprovalForVisitor()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "feed-events");

        await SelectTerritoryAsync(client, ActiveTerritoryId);

        var visitorToken = await LoginForTokenAsync(client, "google", "visitor-event");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", visitorToken);

        var created = await client.PostAsJsonAsync(
            $"api/v1/feed?territoryId={ActiveTerritoryId}",
            new CreatePostRequest(
                "Evento visitante",
                "Detalhes",
                "EVENT",
                "PUBLIC",
                null,
                new List<GeoAnchorRequest> { new(-23.37, -45.02, "EVENT") },
                null));
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var createdPost = await created.Content.ReadFromJsonAsync<FeedItemResponse>();
        Assert.NotNull(createdPost);

        var residentToken = await LoginForTokenAsync(client, "google", "resident-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", residentToken);

        var approve = await client.PatchAsJsonAsync(
            $"api/v1/feed/{createdPost!.Id}/approval?territoryId={ActiveTerritoryId}",
            new ApproveEventRequest("PUBLISHED"));
        Assert.Equal(HttpStatusCode.NoContent, approve.StatusCode);
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
        Assert.Contains(pins, pin => pin.EntityId == Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"));
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
