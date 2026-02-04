using Araponga.Application.Services;
using Araponga.Domain.Membership;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.TestHelpers;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class AccessEvaluatorTests
{
    private static readonly Guid TerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid OtherUserId = Guid.Parse("99999999-9999-9999-9999-999999999999");

    private static AccessEvaluator CreateEvaluator(InMemorySharedStore sharedStore)
    {
        var membershipRepository = new InMemoryTerritoryMembershipRepository(sharedStore);
        var settingsRepository = new InMemoryMembershipSettingsRepository(sharedStore);
        var userRepository = new InMemoryUserRepository(sharedStore);
        var capabilityRepository = new InMemoryMembershipCapabilityRepository(sharedStore);
        var systemPermissionRepository = new InMemorySystemPermissionRepository(sharedStore);
        var featureFlags = new InMemoryFeatureFlagService();
        var cache = CacheTestHelper.CreateDistributedCacheService();

        var accessRules = new MembershipAccessRules(
            membershipRepository,
            settingsRepository,
            userRepository,
            featureFlags);

        return new AccessEvaluator(
            membershipRepository,
            capabilityRepository,
            systemPermissionRepository,
            accessRules,
            cache);
    }

    [Fact]
    public async Task HasCapabilityAsync_ReturnsTrue_WhenCapabilityExists()
    {
        var sharedStore = new InMemorySharedStore();
        var evaluator = CreateEvaluator(sharedStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(sharedStore);
        var capabilityRepository = new InMemoryMembershipCapabilityRepository(sharedStore);

        // Usar um UserId diferente para evitar conflito com dados pré-populados
        var testUserId = Guid.NewGuid();
        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            testUserId,
            TerritoryId,
            MembershipRole.Resident,
            ResidencyVerification.GeoVerified,
            DateTime.UtcNow,
            null,
            DateTime.UtcNow);

        await membershipRepository.AddAsync(membership, CancellationToken.None);

        var capability = new MembershipCapability(
            Guid.NewGuid(),
            membership.Id,
            MembershipCapabilityType.Curator,
            DateTime.UtcNow,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test");

        await capabilityRepository.AddAsync(capability, CancellationToken.None);

        var hasCapability = await evaluator.HasCapabilityAsync(testUserId, TerritoryId, MembershipCapabilityType.Curator, CancellationToken.None);
        Assert.True(hasCapability);
    }

    [Fact]
    public async Task HasCapabilityAsync_ReturnsFalse_WhenCapabilityRevoked()
    {
        var sharedStore = new InMemorySharedStore();
        var evaluator = CreateEvaluator(sharedStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(sharedStore);
        var capabilityRepository = new InMemoryMembershipCapabilityRepository(sharedStore);

        // Usar um UserId diferente para evitar conflito com dados pré-populados
        var testUserId = Guid.NewGuid();
        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            testUserId,
            TerritoryId,
            MembershipRole.Resident,
            ResidencyVerification.GeoVerified,
            DateTime.UtcNow,
            null,
            DateTime.UtcNow);

        await membershipRepository.AddAsync(membership, CancellationToken.None);

        var capability = new MembershipCapability(
            Guid.NewGuid(),
            membership.Id,
            MembershipCapabilityType.Curator,
            DateTime.UtcNow,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test");

        capability.Revoke(DateTime.UtcNow);
        await capabilityRepository.AddAsync(capability, CancellationToken.None);

        var hasCapability = await evaluator.HasCapabilityAsync(testUserId, TerritoryId, MembershipCapabilityType.Curator, CancellationToken.None);
        Assert.False(hasCapability);
    }

    [Fact]
    public async Task HasCapabilityAsync_ReturnsFalse_WhenNoMembership()
    {
        var sharedStore = new InMemorySharedStore();
        var evaluator = CreateEvaluator(sharedStore);

        var hasCapability = await evaluator.HasCapabilityAsync(OtherUserId, TerritoryId, MembershipCapabilityType.Curator, CancellationToken.None);
        Assert.False(hasCapability);
    }

    [Fact]
    public async Task HasSystemPermissionAsync_ReturnsTrue_WhenPermissionActive()
    {
        var sharedStore = new InMemorySharedStore();
        var evaluator = CreateEvaluator(sharedStore);
        var systemPermissionRepository = new InMemorySystemPermissionRepository(sharedStore);

        var permission = new SystemPermission(
            Guid.NewGuid(),
            UserId,
            SystemPermissionType.SystemAdmin,
            DateTime.UtcNow,
            Guid.NewGuid(),
            null,
            null);

        await systemPermissionRepository.AddAsync(permission, CancellationToken.None);

        var hasPermission = await evaluator.HasSystemPermissionAsync(UserId, SystemPermissionType.SystemAdmin, CancellationToken.None);
        Assert.True(hasPermission);
    }

    [Fact]
    public async Task HasSystemPermissionAsync_ReturnsFalse_WhenPermissionRevoked()
    {
        var sharedStore = new InMemorySharedStore();
        var evaluator = CreateEvaluator(sharedStore);
        var systemPermissionRepository = new InMemorySystemPermissionRepository(sharedStore);

        var permission = new SystemPermission(
            Guid.NewGuid(),
            UserId,
            SystemPermissionType.SystemAdmin,
            DateTime.UtcNow,
            Guid.NewGuid(),
            DateTime.UtcNow,
            Guid.NewGuid());

        await systemPermissionRepository.AddAsync(permission, CancellationToken.None);

        var hasPermission = await evaluator.HasSystemPermissionAsync(UserId, SystemPermissionType.SystemAdmin, CancellationToken.None);
        Assert.False(hasPermission);
    }

    [Fact]
    public async Task HasSystemPermissionAsync_ReturnsFalse_WhenNoPermission()
    {
        var sharedStore = new InMemorySharedStore();
        var evaluator = CreateEvaluator(sharedStore);

        var hasPermission = await evaluator.HasSystemPermissionAsync(OtherUserId, SystemPermissionType.SystemAdmin, CancellationToken.None);
        Assert.False(hasPermission);
    }

    [Fact]
    public async Task IsResidentAsync_ReturnsTrue_ForVerifiedResident()
    {
        var sharedStore = new InMemorySharedStore();
        var evaluator = CreateEvaluator(sharedStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(sharedStore);

        // Usar um UserId diferente para evitar conflito com dados pré-populados
        var testUserId = Guid.NewGuid();
        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            testUserId,
            TerritoryId,
            MembershipRole.Resident,
            ResidencyVerification.GeoVerified,
            DateTime.UtcNow,
            null,
            DateTime.UtcNow);

        await membershipRepository.AddAsync(membership, CancellationToken.None);

        var isResident = await evaluator.IsResidentAsync(testUserId, TerritoryId, CancellationToken.None);
        Assert.True(isResident);
    }

    [Fact]
    public async Task IsResidentAsync_ReturnsFalse_ForUnverifiedResident()
    {
        var sharedStore = new InMemorySharedStore();
        var evaluator = CreateEvaluator(sharedStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(sharedStore);

        // Usar um UserId diferente para evitar conflito com dados pré-populados
        var testUserId = Guid.NewGuid();
        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            testUserId,
            TerritoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            DateTime.UtcNow);

        await membershipRepository.AddAsync(membership, CancellationToken.None);

        var isResident = await evaluator.IsResidentAsync(testUserId, TerritoryId, CancellationToken.None);
        Assert.False(isResident);
    }

    [Fact]
    public async Task IsResidentAsync_ReturnsFalse_ForVisitor()
    {
        var sharedStore = new InMemorySharedStore();
        var evaluator = CreateEvaluator(sharedStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(sharedStore);

        // Usar um UserId diferente para evitar conflito com dados pré-populados
        var testUserId = Guid.NewGuid();
        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            testUserId,
            TerritoryId,
            MembershipRole.Visitor,
            ResidencyVerification.None,
            null,
            null,
            DateTime.UtcNow);

        await membershipRepository.AddAsync(membership, CancellationToken.None);

        var isResident = await evaluator.IsResidentAsync(testUserId, TerritoryId, CancellationToken.None);
        Assert.False(isResident);
    }

    [Fact]
    public async Task GetRoleAsync_ReturnsCorrectRole()
    {
        var sharedStore = new InMemorySharedStore();
        var evaluator = CreateEvaluator(sharedStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(sharedStore);

        // Usar um UserId diferente para evitar conflito com dados pré-populados
        var testUserId = Guid.NewGuid();
        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            testUserId,
            TerritoryId,
            MembershipRole.Resident,
            ResidencyVerification.GeoVerified,
            DateTime.UtcNow,
            null,
            DateTime.UtcNow);

        await membershipRepository.AddAsync(membership, CancellationToken.None);

        var role = await evaluator.GetRoleAsync(testUserId, TerritoryId, CancellationToken.None);
        Assert.Equal(MembershipRole.Resident, role);
    }

    [Fact]
    public async Task GetRoleAsync_ReturnsNull_WhenNoMembership()
    {
        var sharedStore = new InMemorySharedStore();
        var evaluator = CreateEvaluator(sharedStore);

        var role = await evaluator.GetRoleAsync(OtherUserId, TerritoryId, CancellationToken.None);
        Assert.Null(role);
    }

    [Fact]
    public async Task HasCapabilityAsync_ReturnsTrue_WhenUserIsSystemAdmin()
    {
        var sharedStore = new InMemorySharedStore();
        var evaluator = CreateEvaluator(sharedStore);
        var systemPermissionRepository = new InMemorySystemPermissionRepository(sharedStore);

        // Usar um UserId diferente para evitar conflito com dados pré-populados
        var testUserId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();

        // Criar SystemPermission de SystemAdmin
        var permission = new SystemPermission(
            Guid.NewGuid(),
            testUserId,
            SystemPermissionType.SystemAdmin,
            DateTime.UtcNow,
            Guid.NewGuid(),
            null,
            null);

        await systemPermissionRepository.AddAsync(permission, CancellationToken.None);

        // SystemAdmin deve ter implicitamente todas as capabilities em todos os territórios
        var hasCurator = await evaluator.HasCapabilityAsync(testUserId, territoryId, MembershipCapabilityType.Curator, CancellationToken.None);
        var hasModerator = await evaluator.HasCapabilityAsync(testUserId, territoryId, MembershipCapabilityType.Moderator, CancellationToken.None);

        Assert.True(hasCurator);
        Assert.True(hasModerator);
    }

    [Fact]
    public async Task HasCapabilityAsync_ReturnsTrue_WhenSystemAdmin_EvenWithoutMembership()
    {
        var sharedStore = new InMemorySharedStore();
        var evaluator = CreateEvaluator(sharedStore);
        var systemPermissionRepository = new InMemorySystemPermissionRepository(sharedStore);

        // Usar um UserId diferente para evitar conflito com dados pré-populados
        var testUserId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();

        // Criar SystemPermission de SystemAdmin
        var permission = new SystemPermission(
            Guid.NewGuid(),
            testUserId,
            SystemPermissionType.SystemAdmin,
            DateTime.UtcNow,
            Guid.NewGuid(),
            null,
            null);

        await systemPermissionRepository.AddAsync(permission, CancellationToken.None);

        // SystemAdmin deve ter implicitamente todas as capabilities mesmo sem membership no território
        var hasCurator = await evaluator.HasCapabilityAsync(testUserId, territoryId, MembershipCapabilityType.Curator, CancellationToken.None);
        Assert.True(hasCurator);
    }

    [Fact]
    public async Task HasCapabilityAsync_ReturnsFalse_WhenNotSystemAdminAndNoCapability()
    {
        var sharedStore = new InMemorySharedStore();
        var evaluator = CreateEvaluator(sharedStore);
        var membershipRepository = new InMemoryTerritoryMembershipRepository(sharedStore);

        // Usar um UserId diferente para evitar conflito com dados pré-populados
        var testUserId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();

        // Criar membership sem capability
        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            testUserId,
            territoryId,
            MembershipRole.Resident,
            ResidencyVerification.GeoVerified,
            DateTime.UtcNow,
            null,
            DateTime.UtcNow);

        await membershipRepository.AddAsync(membership, CancellationToken.None);

        // Sem SystemAdmin e sem capability, deve retornar false
        var hasCurator = await evaluator.HasCapabilityAsync(testUserId, territoryId, MembershipCapabilityType.Curator, CancellationToken.None);
        Assert.False(hasCurator);
    }
}
