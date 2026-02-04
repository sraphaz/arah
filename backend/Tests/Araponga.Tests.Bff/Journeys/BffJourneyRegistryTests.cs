using Araponga.Bff.Journeys;
using Xunit;

namespace Araponga.Tests.Bff.Journeys;

public sealed class BffJourneyRegistryTests
{
    [Fact]
    public void BasePath_IsJourneysPrefix()
    {
        Assert.Equal("/api/v2/journeys/", BffJourneyRegistry.BasePath);
    }

    [Fact]
    public void AllPathPrefixes_ContainsAllFourJourneys()
    {
        var prefixes = BffJourneyRegistry.AllPathPrefixes;
        Assert.Equal(4, prefixes.Count);
        Assert.Contains(BffJourneyRegistry.Onboarding, prefixes);
        Assert.Contains(BffJourneyRegistry.Feed, prefixes);
        Assert.Contains(BffJourneyRegistry.Events, prefixes);
        Assert.Contains(BffJourneyRegistry.Marketplace, prefixes);
    }

    [Fact]
    public void CacheableGetEndpoints_HasEntryForEachJourney()
    {
        var cacheable = BffJourneyRegistry.CacheableGetEndpoints;
        Assert.Equal(4, cacheable.Count);
        Assert.True(cacheable.ContainsKey(BffJourneyRegistry.Onboarding));
        Assert.True(cacheable.ContainsKey(BffJourneyRegistry.Feed));
        Assert.True(cacheable.ContainsKey(BffJourneyRegistry.Events));
        Assert.True(cacheable.ContainsKey(BffJourneyRegistry.Marketplace));
    }

    [Theory]
    [InlineData("onboarding", "suggested-territories", "onboarding/suggested-territories")]
    [InlineData("feed", "territory-feed", "feed/territory-feed")]
    [InlineData("events", "territory-events", "events/territory-events")]
    [InlineData("marketplace", "search", "marketplace/search")]
    public void PathAndQuery_ReturnsExpectedPath(string journey, string subPath, string expected)
    {
        Assert.Equal(expected, BffJourneyRegistry.PathAndQuery(journey, subPath));
    }

    [Fact]
    public void AllEndpoints_HasEntryForEachJourney()
    {
        var all = BffJourneyRegistry.AllEndpoints;
        Assert.Equal(4, all.Count);
        Assert.Equal(2, all[BffJourneyRegistry.Onboarding].Count);   // GET suggested-territories, POST complete
        Assert.Equal(3, all[BffJourneyRegistry.Feed].Count);          // GET territory-feed, POST create-post, POST interact
        Assert.Equal(3, all[BffJourneyRegistry.Events].Count);        // GET territory-events, POST create-event, POST participate
        Assert.Equal(3, all[BffJourneyRegistry.Marketplace].Count);  // GET search, POST add-to-cart, POST checkout
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
}
