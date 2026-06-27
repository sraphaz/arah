using Arah.Bff.Journeys;
using Xunit;

namespace Arah.Tests.Bff.Journeys;

public sealed class BffJourneyRegistryTests
{
    [Fact]
    public void BasePath_IsJourneysPrefix()
    {
        Assert.NotEmpty(BffJourneyRegistry.BasePath);
        Assert.EndsWith("/", BffJourneyRegistry.BasePath);
    }

    [Fact]
    public void AllPathPrefixes_ContainsAllTwentyJourneys()
    {
        var prefixes = BffJourneyRegistry.AllPathPrefixes;
        Assert.Equal(BffJourneyRegistry.AllEndpoints.Count, prefixes.Count);
        foreach (var journey in BffJourneyRegistry.AllPathPrefixes)
            Assert.Contains(journey, prefixes);
    }

    [Fact]
    public void CacheableGetEndpoints_HasEntryForJourneysWithCacheableGets()
    {
        var cacheable = BffJourneyRegistry.CacheableGetEndpoints;
        Assert.Equal(BffJourneyRegistry.AllEndpoints.Count - 1, cacheable.Count);
        foreach (var journey in BffJourneyRegistry.AllPathPrefixes)
        {
            if (journey == BffJourneyRegistry.Auth)
                Assert.False(cacheable.ContainsKey(journey));
            else
                Assert.True(cacheable.ContainsKey(journey));
        }
    }

    [Theory]
    [InlineData("onboarding", "suggested-territories", "onboarding/suggested-territories")]
    [InlineData("feed", "territory-feed", "feed/territory-feed")]
    [InlineData("events", "territory-events", "events/territory-events")]
    [InlineData("marketplace", "search", "marketplace/search")]
    [InlineData("auth", "login", "auth/login")]
    [InlineData("me", "profile", "me/profile")]
    public void PathAndQuery_ReturnsExpectedPath(string journey, string subPath, string expected)
    {
        Assert.Equal(expected, BffJourneyRegistry.PathAndQuery(journey, subPath));
    }

    [Theory]
    [InlineData("onboarding", "api/v2/journeys/onboarding")]
    [InlineData("feed", "api/v2/journeys/feed")]
    [InlineData("auth", "api/v1/auth")]
    [InlineData("me", "api/v1/users/me")]
    [InlineData("connections", "api/v1/connections")]
    [InlineData("territories", "api/v1/territories")]
    [InlineData("membership", "api/v1/memberships")]
    [InlineData("map", "api/v1/map")]
    [InlineData("assets", "api/v1/assets")]
    [InlineData("media", "api/v1/media")]
    [InlineData("subscription-plans", "api/v1/subscription-plans")]
    [InlineData("subscriptions", "api/v1/subscriptions")]
    [InlineData("notifications", "api/v1/notifications")]
    [InlineData("marketplace-v1", "api/v1")]
    [InlineData("moderation", "api/v1/territories")]
    [InlineData("governance", "api/v1/territories")]
    [InlineData("chat", "api/v1/chat")]
    [InlineData("alerts", "api/v1/alerts")]
    [InlineData("admin", "api/v1/admin")]
    [InlineData("unknown", "api/v2/journeys/unknown")]
    public void GetApiPathBase_ReturnsExpectedApiPath(string journeyName, string expectedBase)
    {
        Assert.Equal(expectedBase, BffJourneyRegistry.GetApiPathBase(journeyName));
    }

    [Fact]
    public void AllEndpoints_HasEntryForEachJourney()
    {
        var all = BffJourneyRegistry.AllEndpoints;
        Assert.Equal(BffJourneyRegistry.AllEndpoints.Count, all.Count);
        foreach (var kv in BffJourneyRegistry.AllEndpoints)
            Assert.Equal(kv.Value.Count, all[kv.Key].Count);
    }

    [Fact]
    public void AllEndpoints_Onboarding_ContainsGetAndPost()
    {
        var list = BffJourneyRegistry.AllEndpoints[BffJourneyRegistry.Onboarding];
        var paths = list.Select(e => e.Path).ToHashSet();
        Assert.Contains("suggested-territories", paths);
        Assert.Contains("complete", paths);
        Assert.Contains(list, e => e.Method == "GET");
        Assert.Contains(list, e => e.Method == "POST");
    }

    [Fact]
    public void AllEndpoints_Feed_ContainsTerritoryFeedCreatePostInteract()
    {
        var list = BffJourneyRegistry.AllEndpoints[BffJourneyRegistry.Feed];
        var paths = list.Select(e => e.Path).ToHashSet();
        Assert.Contains("territory-feed", paths);
        Assert.Contains("create-post", paths);
        Assert.Contains("interact", paths);
    }

    [Fact]
    public void AllEndpoints_Events_ContainsTerritoryEventsCreateParticipate()
    {
        var list = BffJourneyRegistry.AllEndpoints[BffJourneyRegistry.Events];
        var paths = list.Select(e => e.Path).ToHashSet();
        Assert.Contains("territory-events", paths);
        Assert.Contains("create-event", paths);
        Assert.Contains("participate", paths);
    }

    [Fact]
    public void AllEndpoints_Marketplace_ContainsSearchAddToCartCheckout()
    {
        var list = BffJourneyRegistry.AllEndpoints[BffJourneyRegistry.Marketplace];
        var paths = list.Select(e => e.Path).ToHashSet();
        Assert.Contains("search", paths);
        Assert.Contains("add-to-cart", paths);
        Assert.Contains("checkout", paths);
    }

    [Fact]
    public void CacheableGetEndpoints_Onboarding_HasSuggestedTerritories()
    {
        var list = BffJourneyRegistry.CacheableGetEndpoints[BffJourneyRegistry.Onboarding];
        Assert.Single(list);
        Assert.Equal("suggested-territories", list[0].Path);
        Assert.Equal("GET", list[0].Method);
    }

    [Fact]
    public void CacheableGetEndpoints_Feed_HasTerritoryFeed()
    {
        var list = BffJourneyRegistry.CacheableGetEndpoints[BffJourneyRegistry.Feed];
        Assert.Single(list);
        Assert.Equal("territory-feed", list[0].Path);
    }

    [Fact]
    public void CacheableGetEndpoints_Events_HasTerritoryEvents()
    {
        var list = BffJourneyRegistry.CacheableGetEndpoints[BffJourneyRegistry.Events];
        Assert.Single(list);
        Assert.Equal("territory-events", list[0].Path);
    }

    [Fact]
    public void CacheableGetEndpoints_Marketplace_HasSearch()
    {
        var list = BffJourneyRegistry.CacheableGetEndpoints[BffJourneyRegistry.Marketplace];
        Assert.Single(list);
        Assert.Equal("search", list[0].Path);
    }

    [Fact]
    public void CacheableGetEndpoints_Me_HasProfileAndPreferences()
    {
        var list = BffJourneyRegistry.CacheableGetEndpoints[BffJourneyRegistry.Me];
        Assert.True(list.Count >= 2);
        var paths = list.Select(e => e.Path).ToHashSet();
        Assert.Contains("profile", paths);
        Assert.Contains("preferences", paths);
    }

    [Fact]
    public void CacheableGetEndpoints_Connections_HasListAndPending()
    {
        var list = BffJourneyRegistry.CacheableGetEndpoints[BffJourneyRegistry.Connections];
        var paths = list.Select(e => e.Path).ToHashSet();
        Assert.Contains("", paths);
        Assert.Contains("pending", paths);
        Assert.Contains("suggestions", paths);
    }

    [Fact]
    public void CacheableGetEndpoints_Territories_HasListAndPaged()
    {
        var list = BffJourneyRegistry.CacheableGetEndpoints[BffJourneyRegistry.Territories];
        var paths = list.Select(e => e.Path).ToHashSet();
        Assert.Contains("", paths);
        Assert.Contains("paged", paths);
    }

    [Fact]
    public void CacheableGetEndpoints_Membership_HasMe()
    {
        var list = BffJourneyRegistry.CacheableGetEndpoints[BffJourneyRegistry.Membership];
        Assert.Contains(list, e => e.Path == "me");
    }

    [Fact]
    public void CacheableGetEndpoints_Map_HasEntitiesAndPins()
    {
        var list = BffJourneyRegistry.CacheableGetEndpoints[BffJourneyRegistry.Map];
        var paths = list.Select(e => e.Path).ToHashSet();
        Assert.Contains("entities", paths);
        Assert.Contains("pins", paths);
    }

    [Fact]
    public void CacheableGetEndpoints_Assets_HasListAndPaged()
    {
        var list = BffJourneyRegistry.CacheableGetEndpoints[BffJourneyRegistry.Assets];
        Assert.Contains(list, e => e.Path == "");
        Assert.Contains(list, e => e.Path == "paged");
    }

    [Fact]
    public void CacheableGetEndpoints_Media_HasInfo()
    {
        var list = BffJourneyRegistry.CacheableGetEndpoints[BffJourneyRegistry.Media];
        Assert.Contains(list, e => e.Path == "{id}/info");
    }

    [Fact]
    public void CacheableGetEndpoints_Subscriptions_HasMe()
    {
        var list = BffJourneyRegistry.CacheableGetEndpoints[BffJourneyRegistry.Subscriptions];
        Assert.Contains(list, e => e.Path == "me");
    }

    [Fact]
    public void CacheableGetEndpoints_Notifications_HasListAndPaged()
    {
        var list = BffJourneyRegistry.CacheableGetEndpoints[BffJourneyRegistry.Notifications];
        Assert.Contains(list, e => e.Path == "paged");
    }

    [Fact]
    public void CacheableGetEndpoints_MarketplaceV1_HasCart()
    {
        var list = BffJourneyRegistry.CacheableGetEndpoints[BffJourneyRegistry.MarketplaceV1];
        Assert.Contains(list, e => e.Path == "cart");
    }

    [Fact]
    public void AllEndpoints_Governance_ContainsVotingsLifecycle()
    {
        var list = BffJourneyRegistry.AllEndpoints[BffJourneyRegistry.Governance];
        var paths = list.Select(e => e.Path).ToHashSet();
        Assert.Contains("{territoryId}/votings", paths);
        Assert.Contains("{territoryId}/votings/{votingId}", paths);
        Assert.Contains("{territoryId}/votings/{votingId}/vote", paths);
        Assert.Contains("{territoryId}/votings/{votingId}/close", paths);
        Assert.Contains("{territoryId}/votings/{votingId}/results", paths);
        Assert.Contains(list, e => e.Method == "GET");
        Assert.Contains(list, e => e.Method == "POST");
    }

    [Fact]
    public void CacheableGetEndpoints_Governance_HasVotingsReads()
    {
        var list = BffJourneyRegistry.CacheableGetEndpoints[BffJourneyRegistry.Governance];
        var paths = list.Select(e => e.Path).ToHashSet();
        Assert.Contains("{territoryId}/votings", paths);
        Assert.Contains("{territoryId}/votings/{votingId}/results", paths);
        Assert.All(list, e => Assert.Equal("GET", e.Method));
    }
}
