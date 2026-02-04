using Araponga.Bff.Journeys;
using Araponga.Bff.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Xunit;

namespace Araponga.Tests.Bff.Journeys;

/// <summary>
/// Garante que cada jornada (path cacheável) usa o TTL configurado no cache.
/// </summary>
public sealed class JourneyCacheTtlTests
{
    private static IJourneyResponseCache CreateCacheWithJourneyTtls()
    {
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var options = new BffOptions
        {
            EnableCache = true,
            CacheTtlSeconds = 60,
            CacheTtlByPath = new Dictionary<string, int>
            {
                [BffJourneyRegistry.Onboarding] = 300,
                [BffJourneyRegistry.Feed] = 30,
                [BffJourneyRegistry.Events] = 45,
                [BffJourneyRegistry.Marketplace] = 60
            }
        };
        return new JourneyResponseCache(memoryCache, Options.Create(options));
    }

    [Theory]
    [InlineData("onboarding/suggested-territories", 300)]
    [InlineData("feed/territory-feed", 30)]
    [InlineData("events/territory-events", 45)]
    [InlineData("marketplace/search", 60)]
    public void GetTtlSeconds_ForEachJourneyGetPath_ReturnsConfiguredTtl(string pathAndQuery, int expectedTtl)
    {
        var cache = CreateCacheWithJourneyTtls();
        Assert.Equal(expectedTtl, cache.GetTtlSeconds(pathAndQuery));
    }

    [Theory]
    [InlineData("onboarding/suggested-territories")]
    [InlineData("feed/territory-feed")]
    [InlineData("events/territory-events")]
    [InlineData("marketplace/search")]
    public void ShouldCache_ForEachJourneyGetPath_ReturnsTrue(string pathAndQuery)
    {
        var cache = CreateCacheWithJourneyTtls();
        Assert.True(cache.ShouldCache("GET", pathAndQuery, 200));
    }
}
