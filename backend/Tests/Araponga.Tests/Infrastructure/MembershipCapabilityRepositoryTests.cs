using Araponga.Domain.Membership;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Infrastructure;

public sealed class MembershipCapabilityRepositoryTests
{
    private static readonly Guid MembershipId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

    [Fact]
    public async Task MembershipCapabilityRepository_AddAndGetById()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryMembershipCapabilityRepository(sharedStore);

        var capability = new MembershipCapability(
            Guid.NewGuid(),
            MembershipId,
            MembershipCapabilityType.Curator,
            DateTime.UtcNow,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test reason");

        await repository.AddAsync(capability, CancellationToken.None);

        var found = await repository.GetByIdAsync(capability.Id, CancellationToken.None);
        Assert.NotNull(found);
        Assert.Equal(capability.Id, found!.Id);
        Assert.Equal(MembershipCapabilityType.Curator, found.CapabilityType);
    }

    [Fact]
    public async Task MembershipCapabilityRepository_GetActiveByMembershipId()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryMembershipCapabilityRepository(sharedStore);

        var activeCapability = new MembershipCapability(
            Guid.NewGuid(),
            MembershipId,
            MembershipCapabilityType.Curator,
            DateTime.UtcNow,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Active");

        var revokedCapability = new MembershipCapability(
            Guid.NewGuid(),
            MembershipId,
            MembershipCapabilityType.Moderator,
            DateTime.UtcNow,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Revoked");

        revokedCapability.Revoke(DateTime.UtcNow);

        await repository.AddAsync(activeCapability, CancellationToken.None);
        await repository.AddAsync(revokedCapability, CancellationToken.None);

        var active = await repository.GetActiveByMembershipIdAsync(MembershipId, CancellationToken.None);
        Assert.Single(active);
        Assert.Equal(activeCapability.Id, active[0].Id);
    }

    [Fact]
    public async Task MembershipCapabilityRepository_HasCapability_ReturnsTrue_WhenActive()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryMembershipCapabilityRepository(sharedStore);

        var capability = new MembershipCapability(
            Guid.NewGuid(),
            MembershipId,
            MembershipCapabilityType.Curator,
            DateTime.UtcNow,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test");

        await repository.AddAsync(capability, CancellationToken.None);

        var hasCapability = await repository.HasCapabilityAsync(MembershipId, MembershipCapabilityType.Curator, CancellationToken.None);
        Assert.True(hasCapability);
    }

    [Fact]
    public async Task MembershipCapabilityRepository_HasCapability_ReturnsFalse_WhenRevoked()
    {
        var sharedStore = new InMemorySharedStore();
        var repository = new InMemoryMembershipCapabilityRepository(sharedStore);

        var capability = new MembershipCapability(
            Guid.NewGuid(),
            MembershipId,
            MembershipCapabilityType.Curator,
            DateTime.UtcNow,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test");

        capability.Revoke(DateTime.UtcNow);
        await repository.AddAsync(capability, CancellationToken.None);

        var hasCapability = await repository.HasCapabilityAsync(MembershipId, MembershipCapabilityType.Curator, CancellationToken.None);
        Assert.False(hasCapability);
    }
}
