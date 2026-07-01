using Arah.Core.Application;
using Arah.Core.Domain;
using Xunit;

namespace Arah.Tests.Core;

public sealed class CoreDirectoryServiceTests
{
    [Fact]
    public void PublishTerritory_WhenInstanceExists_AddsEntry()
    {
        var instances = new InMemoryCoreInstanceRegistry();
        var directory = new InMemoryCoreDirectoryRegistry();
        var service = new CoreDirectoryService(directory, instances);

        var registration = instances.Register("standalone", new Uri("https://a.example"), "1.0.0");
        var territoryId = Guid.NewGuid();
        var entry = service.PublishTerritory(territoryId, registration.Instance.Id, "Território Teste");

        Assert.Equal(territoryId, entry.TerritoryId);
        Assert.Single(service.ListTerritories());
    }

    [Fact]
    public void PublishTerritory_WhenInstanceMissing_Throws()
    {
        var instances = new InMemoryCoreInstanceRegistry();
        var directory = new InMemoryCoreDirectoryRegistry();
        var service = new CoreDirectoryService(directory, instances);

        Assert.Throws<InvalidOperationException>(() =>
            service.PublishTerritory(Guid.NewGuid(), Guid.NewGuid(), "Missing"));
    }
}

public sealed class FederationIdentityServiceTests
{
    [Fact]
    public void ProvisionAndResolve_ReturnsSameUser()
    {
        var instances = new InMemoryCoreInstanceRegistry();
        var identities = new InMemoryFederationIdentityRegistry();
        var service = new FederationIdentityService(identities, instances);

        var registration = instances.Register("federated", new Uri("https://b.example"), "2.0.0");
        var user = service.Provision(registration.Instance.Id, "João");

        var resolved = service.Resolve(user.GlobalUserId);
        Assert.NotNull(resolved);
        Assert.Equal(user.GlobalUserId, resolved!.GlobalUserId);
        Assert.Equal("João", resolved.DisplayName);
    }
}
