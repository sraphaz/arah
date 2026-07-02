using Arah.Core.Application;
using Arah.Core.Domain;
using Xunit;

namespace Arah.Tests.Core;

public sealed class CoreAvailabilityCacheTests
{
    [Fact]
    public void RememberRegistration_AndReleases_RetrievableWhenCoreUnavailable()
    {
        var cache = new InMemoryCoreAvailabilityCache();
        var registry = new InMemoryCoreInstanceRegistry();
        var registration = registry.Register("standalone", new Uri("https://cache.example"), "1.0.0");
        cache.RememberRegistration(registration.Instance, registration.InstanceAuthToken);
        cache.RememberReleases(
            new[] { new CoreRelease(Guid.NewGuid(), "1.0.0", "stable", 1, DateTimeOffset.UtcNow) },
            "stable");

        Assert.NotNull(cache.GetCachedInstance(registration.Instance.Id));
        Assert.Single(cache.GetCachedReleases("stable"));
    }
}

public sealed class CoreReleaseServiceTests
{
    [Fact]
    public void Publish_AddsReleaseToChannel()
    {
        var catalog = new InMemoryCoreReleaseCatalog();
        var cache = new InMemoryCoreAvailabilityCache();
        var service = new CoreReleaseService(catalog, cache);

        var release = service.Publish("2.0.0", "stable", 1);
        var list = service.ListByChannel("stable");

        Assert.Equal("2.0.0", release.Version);
        Assert.Contains(list, r => r.Version == "2.0.0");
        Assert.Single(cache.GetCachedReleases("stable"));
    }
}
