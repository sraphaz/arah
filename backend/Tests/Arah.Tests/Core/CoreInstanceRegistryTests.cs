using Arah.Core.Application;
using Arah.Core.Domain;
using Xunit;

namespace Arah.Tests.Core;

public sealed class CoreInstanceRegistryTests
{
    [Fact]
    public void Register_WhenValid_ReturnsPendingInstance()
    {
        var registry = new InMemoryCoreInstanceRegistry();
        var registration = registry.Register("standalone", new Uri("https://instance.example"), "1.0.0");
        var instance = registration.Instance;

        Assert.NotEqual(Guid.Empty, instance.Id);
        Assert.False(string.IsNullOrWhiteSpace(instance.PublicKeyPem));
        Assert.False(string.IsNullOrWhiteSpace(registration.PrivateKeyPem));
        Assert.Equal(CoreInstanceStatus.Pending, instance.Status);
    }

    [Fact]
    public void RecordHeartbeat_WhenRegistered_MarksOnline()
    {
        var registry = new InMemoryCoreInstanceRegistry();
        var registration = registry.Register("standalone", new Uri("https://instance.example"), "1.0.0");
        var instance = registration.Instance;
        var at = DateTimeOffset.UtcNow;
        var report = new HealthCheckReport(
            instance.Id,
            new Dictionary<string, string> { ["api"] = "healthy" },
            TimeSpan.FromHours(1),
            at);

        registry.RecordHeartbeat(instance.Id, report);

        var updated = registry.GetById(instance.Id);
        Assert.NotNull(updated);
        Assert.Equal(CoreInstanceStatus.Online, updated!.Status);
        Assert.Equal(at, updated.LastHeartbeatUtc);
        Assert.Equal("healthy", updated.LastServices!["api"]);
        Assert.Equal(TimeSpan.FromHours(1), updated.LastUptime);
    }

    [Fact]
    public void ListAll_ReturnsRegisteredInOrder()
    {
        var registry = new InMemoryCoreInstanceRegistry();
        registry.Register("a", new Uri("https://a.example"), "1.0.0");
        registry.Register("b", new Uri("https://b.example"), "2.0.0");

        var list = registry.ListAll();
        Assert.Equal(2, list.Count);
        Assert.Equal("a", list[0].Mode);
        Assert.Equal("b", list[1].Mode);
    }

    [Fact]
    public void RecordHeartbeat_WhenUnknown_Throws()
    {
        var registry = new InMemoryCoreInstanceRegistry();
        var report = new HealthCheckReport(
            Guid.NewGuid(),
            new Dictionary<string, string>(),
            TimeSpan.Zero,
            DateTimeOffset.UtcNow);

        Assert.Throws<InvalidOperationException>(() => registry.RecordHeartbeat(report.InstanceId, report));
    }
}

public sealed class CoreReleaseCatalogTests
{
    [Fact]
    public void ListByChannel_ReturnsMatchingReleases()
    {
        var catalog = new InMemoryCoreReleaseCatalog();
        catalog.Publish(new CoreRelease(Guid.NewGuid(), "1.0.0", "stable", 1, DateTimeOffset.UtcNow));
        catalog.Publish(new CoreRelease(Guid.NewGuid(), "1.1.0-beta", "beta", 1, DateTimeOffset.UtcNow));

        var stable = catalog.ListByChannel("stable");
        Assert.Single(stable);
        Assert.Equal("1.0.0", stable[0].Version);
    }
}
