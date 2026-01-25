using Araponga.Application.Common;
using Araponga.Application.Services;
using Araponga.Domain.Map;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.TestHelpers;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for MapService (ListEntities userId null, ListEntitiesPaged, etc.).
/// </summary>
public sealed class MapServiceEdgeCasesTests
{
    private static readonly Guid TerritoryB = Guid.Parse("22222222-2222-2222-2222-222222222222");

    private static MapService CreateService(InMemoryDataStore ds)
    {
        var mapRepo = new InMemoryMapRepository(ds);
        var membershipRepo = new InMemoryTerritoryMembershipRepository(ds);
        var userRepo = new InMemoryUserRepository(ds);
        var cache = CacheTestHelper.CreateDistributedCacheService();
        var settingsRepo = new InMemoryMembershipSettingsRepository(ds);
        var capabilityRepo = new InMemoryMembershipCapabilityRepository(ds);
        var featureFlags = new InMemoryFeatureFlagService();
        var accessRules = new MembershipAccessRules(membershipRepo, settingsRepo, userRepo, featureFlags);
        var permissionRepo = new InMemorySystemPermissionRepository(ds);
        var accessEvaluator = new AccessEvaluator(
            membershipRepo,
            capabilityRepo,
            permissionRepo,
            accessRules,
            cache);
        var auditLogger = new InMemoryAuditLogger(ds);
        var blockRepo = new InMemoryUserBlockRepository(ds);
        var relationRepo = new InMemoryMapEntityRelationRepository(ds);
        var uow = new InMemoryUnitOfWork();
        return new MapService(
            mapRepo,
            accessEvaluator,
            auditLogger,
            blockRepo,
            relationRepo,
            uow,
            mapEntityCache: null,
            userBlockCache: null,
            cacheInvalidation: null);
    }

    [Fact]
    public async Task ListEntitiesAsync_WhenUserIdNull_ReturnsOnlyPublic()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);

        var list = await svc.ListEntitiesAsync(TerritoryB, null, CancellationToken.None);

        Assert.NotNull(list);
        Assert.All(list, e => Assert.Equal(MapEntityVisibility.Public, e.Visibility));
    }

    [Fact]
    public async Task ListEntitiesPagedAsync_ReturnsPagedResult()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        var paging = new PaginationParameters(1, 10);

        var result = await svc.ListEntitiesPagedAsync(TerritoryB, null, paging, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Items);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.All(result.Items, e => Assert.Equal(MapEntityVisibility.Public, e.Visibility));
    }

    [Fact]
    public async Task ListEntitiesAsync_WhenResident_ReturnsIncludingResidentsOnly()
    {
        var ds = new InMemoryDataStore();
        var svc = CreateService(ds);
        var residentId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        var list = await svc.ListEntitiesAsync(TerritoryB, residentId, CancellationToken.None);

        Assert.NotNull(list);
        var hasResidentsOnly = list.Any(e => e.Visibility == MapEntityVisibility.ResidentsOnly);
        Assert.True(hasResidentsOnly, "Resident should see ResidentsOnly entities.");
    }

    [Fact]
    public async Task ListEntitiesAsync_WhenTerritoryHasNoEntities_ReturnsEmpty()
    {
        var ds = new InMemoryDataStore();
        ds.MapEntities.Clear();
        var mapRepo = new InMemoryMapRepository(ds);
        var membershipRepo = new InMemoryTerritoryMembershipRepository(ds);
        var userRepo = new InMemoryUserRepository(ds);
        var cache = CacheTestHelper.CreateDistributedCacheService();
        var settingsRepo = new InMemoryMembershipSettingsRepository(ds);
        var capabilityRepo = new InMemoryMembershipCapabilityRepository(ds);
        var featureFlags = new InMemoryFeatureFlagService();
        var accessRules = new MembershipAccessRules(membershipRepo, settingsRepo, userRepo, featureFlags);
        var permissionRepo = new InMemorySystemPermissionRepository(ds);
        var accessEvaluator = new AccessEvaluator(
            membershipRepo,
            capabilityRepo,
            permissionRepo,
            accessRules,
            cache);
        var auditLogger = new InMemoryAuditLogger(ds);
        var blockRepo = new InMemoryUserBlockRepository(ds);
        var relationRepo = new InMemoryMapEntityRelationRepository(ds);
        var uow = new InMemoryUnitOfWork();
        var svc = new MapService(
            mapRepo,
            accessEvaluator,
            auditLogger,
            blockRepo,
            relationRepo,
            uow,
            null,
            null,
            null);

        var list = await svc.ListEntitiesAsync(TerritoryB, null, CancellationToken.None);

        Assert.NotNull(list);
        Assert.Empty(list);
    }
}
