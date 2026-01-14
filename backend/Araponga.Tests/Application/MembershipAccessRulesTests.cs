using Araponga.Application.Services;
using Araponga.Domain.Membership;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class MembershipAccessRulesTests
{
    private static readonly Guid TerritoryId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    [Fact]
    public async Task CanCreateStoreAsync_RequiresResidentAndVerified()
    {
        var dataStore = new InMemoryDataStore();
        var membershipRepository = new InMemoryTerritoryMembershipRepository(dataStore);
        var settingsRepository = new InMemoryMembershipSettingsRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var featureFlags = new InMemoryFeatureFlagService();
        var rules = new MembershipAccessRules(membershipRepository, settingsRepository, userRepository, featureFlags);

        // Sem membership
        var canCreate1 = await rules.CanCreateStoreAsync(UserId, TerritoryId, CancellationToken.None);
        Assert.False(canCreate1);

        // Visitor
        var visitorMembership = new TerritoryMembership(
            Guid.NewGuid(),
            UserId,
            TerritoryId,
            MembershipRole.Visitor,
            ResidencyVerification.None,
            null,
            null,
            DateTime.UtcNow);
        await membershipRepository.AddAsync(visitorMembership, CancellationToken.None);

        var canCreate2 = await rules.CanCreateStoreAsync(UserId, TerritoryId, CancellationToken.None);
        Assert.False(canCreate2);

        // Resident não verificado
        visitorMembership.UpdateRole(MembershipRole.Resident);
        var canCreate3 = await rules.CanCreateStoreAsync(UserId, TerritoryId, CancellationToken.None);
        Assert.False(canCreate3);

        // Resident verificado
        visitorMembership.UpdateResidencyVerification(ResidencyVerification.GeoVerified);
        var canCreate4 = await rules.CanCreateStoreAsync(UserId, TerritoryId, CancellationToken.None);
        Assert.True(canCreate4);
    }

    [Fact]
    public async Task CanCreateStoreAsync_Fails_ForVisitor()
    {
        var dataStore = new InMemoryDataStore();
        var membershipRepository = new InMemoryTerritoryMembershipRepository(dataStore);
        var settingsRepository = new InMemoryMembershipSettingsRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var featureFlags = new InMemoryFeatureFlagService();
        var rules = new MembershipAccessRules(membershipRepository, settingsRepository, userRepository, featureFlags);

        var visitorMembership = new TerritoryMembership(
            Guid.NewGuid(),
            UserId,
            TerritoryId,
            MembershipRole.Visitor,
            ResidencyVerification.None,
            null,
            null,
            DateTime.UtcNow);
        await membershipRepository.AddAsync(visitorMembership, CancellationToken.None);

        var canCreate = await rules.CanCreateStoreAsync(UserId, TerritoryId, CancellationToken.None);
        Assert.False(canCreate);
    }

    [Fact]
    public async Task CanCreateStoreAsync_Fails_ForUnverifiedResident()
    {
        var dataStore = new InMemoryDataStore();
        var membershipRepository = new InMemoryTerritoryMembershipRepository(dataStore);
        var settingsRepository = new InMemoryMembershipSettingsRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var featureFlags = new InMemoryFeatureFlagService();
        var rules = new MembershipAccessRules(membershipRepository, settingsRepository, userRepository, featureFlags);

        var residentMembership = new TerritoryMembership(
            Guid.NewGuid(),
            UserId,
            TerritoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            DateTime.UtcNow);
        await membershipRepository.AddAsync(residentMembership, CancellationToken.None);

        var canCreate = await rules.CanCreateStoreAsync(UserId, TerritoryId, CancellationToken.None);
        Assert.False(canCreate);
    }

    [Fact]
    public async Task CanCreateItemAsync_SameAsStoreRule()
    {
        var dataStore = new InMemoryDataStore();
        var membershipRepository = new InMemoryTerritoryMembershipRepository(dataStore);
        var settingsRepository = new InMemoryMembershipSettingsRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var featureFlags = new InMemoryFeatureFlagService();
        var rules = new MembershipAccessRules(membershipRepository, settingsRepository, userRepository, featureFlags);

        var residentMembership = new TerritoryMembership(
            Guid.NewGuid(),
            UserId,
            TerritoryId,
            MembershipRole.Resident,
            ResidencyVerification.GeoVerified,
            DateTime.UtcNow,
            null,
            DateTime.UtcNow);
        await membershipRepository.AddAsync(residentMembership, CancellationToken.None);

        var canCreateStore = await rules.CanCreateStoreAsync(UserId, TerritoryId, CancellationToken.None);
        var canCreateItem = await rules.CanCreateItemAsync(UserId, TerritoryId, CancellationToken.None);

        Assert.Equal(canCreateStore, canCreateItem);
        Assert.True(canCreateItem);
    }

    [Fact]
    public async Task IsVerifiedResidentAsync_ChecksRoleAndVerification()
    {
        var dataStore = new InMemoryDataStore();
        var membershipRepository = new InMemoryTerritoryMembershipRepository(dataStore);
        var settingsRepository = new InMemoryMembershipSettingsRepository(dataStore);
        var userRepository = new InMemoryUserRepository(dataStore);
        var featureFlags = new InMemoryFeatureFlagService();
        var rules = new MembershipAccessRules(membershipRepository, settingsRepository, userRepository, featureFlags);

        // Sem membership
        var isVerified1 = await rules.IsVerifiedResidentAsync(UserId, TerritoryId, CancellationToken.None);
        Assert.False(isVerified1);

        // Visitor
        var visitorMembership = new TerritoryMembership(
            Guid.NewGuid(),
            UserId,
            TerritoryId,
            MembershipRole.Visitor,
            ResidencyVerification.None,
            null,
            null,
            DateTime.UtcNow);
        await membershipRepository.AddAsync(visitorMembership, CancellationToken.None);

        var isVerified2 = await rules.IsVerifiedResidentAsync(UserId, TerritoryId, CancellationToken.None);
        Assert.False(isVerified2);

        // Resident verificado
        visitorMembership.UpdateRole(MembershipRole.Resident);
        visitorMembership.UpdateResidencyVerification(ResidencyVerification.DocumentVerified);
        var isVerified3 = await rules.IsVerifiedResidentAsync(UserId, TerritoryId, CancellationToken.None);
        Assert.True(isVerified3);
    }
}
