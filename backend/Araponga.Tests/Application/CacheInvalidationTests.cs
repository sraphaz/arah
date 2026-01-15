using Araponga.Application.Events;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Membership;
using Araponga.Domain.Users;
using Araponga.Infrastructure.Eventing;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class CacheInvalidationTests
{
    [Fact]
    public async Task SystemPermissionRevokedEvent_InvalidatesCache()
    {
        var dataStore = new InMemoryDataStore();
        var services = new ServiceCollection();
        var cacheService = CacheTestHelper.CreateDistributedCacheService();
        services.AddSingleton<IDistributedCacheService>(_ => cacheService);
        services.AddSingleton<InMemoryDataStore>(dataStore);
        services.AddScoped<ISystemPermissionRepository, InMemorySystemPermissionRepository>();
        services.AddScoped<AccessEvaluator>(sp =>
        {
            var ds = sp.GetRequiredService<InMemoryDataStore>();
            var membershipRepository = new InMemoryTerritoryMembershipRepository(ds);
            var settingsRepository = new InMemoryMembershipSettingsRepository(ds);
            var userRepository = new InMemoryUserRepository(ds);
            var capabilityRepository = new InMemoryMembershipCapabilityRepository(ds);
            var systemPermissionRepository = new InMemorySystemPermissionRepository(ds);
            var featureFlags = new InMemoryFeatureFlagService();
            var cache = sp.GetRequiredService<IDistributedCacheService>();

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
        });
        services.AddScoped<IEventBus, InMemoryEventBus>();
        services.AddScoped<IEventHandler<SystemPermissionRevokedEvent>, SystemPermissionRevokedCacheHandler>();

        var serviceProvider = services.BuildServiceProvider();
        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        var accessEvaluator = serviceProvider.GetRequiredService<AccessEvaluator>();
        var systemPermissionRepository = serviceProvider.GetRequiredService<ISystemPermissionRepository>();

        var userId = Guid.NewGuid();
        var permissionType = SystemPermissionType.SystemAdmin;

        // Criar permissão
        var permission = new SystemPermission(
            Guid.NewGuid(),
            userId,
            permissionType,
            DateTime.UtcNow,
            Guid.NewGuid(),
            null,
            null);

        await systemPermissionRepository.AddAsync(permission, CancellationToken.None);

        // Popular cache
        var hasPermission = await accessEvaluator.HasSystemPermissionAsync(userId, permissionType, CancellationToken.None);
        Assert.True(hasPermission);

        // Verificar que está em cache
        var cache = serviceProvider.GetRequiredService<IDistributedCacheService>();
        var cacheKey = $"system:permission:{userId}:{permissionType}";
        var exists = await cache.ExistsAsync(cacheKey, CancellationToken.None);
        Assert.True(exists);

        // Revogar permissão
        permission.Revoke(Guid.NewGuid(), DateTime.UtcNow);
        await systemPermissionRepository.UpdateAsync(permission, CancellationToken.None);

        // Publicar evento
        var revokedEvent = new SystemPermissionRevokedEvent(
            userId,
            permissionType,
            DateTime.UtcNow);

        await eventBus.PublishAsync(revokedEvent, CancellationToken.None);

        // Verificar que cache foi invalidado
        var stillExists = await cache.ExistsAsync(cacheKey, CancellationToken.None);
        Assert.False(stillExists);
    }

    [Fact]
    public async Task MembershipCapabilityRevokedEvent_InvalidatesCache()
    {
        var dataStore = new InMemoryDataStore();
        var services = new ServiceCollection();
        var cacheService = CacheTestHelper.CreateDistributedCacheService();
        services.AddSingleton<IDistributedCacheService>(_ => cacheService);
        services.AddSingleton<InMemoryDataStore>(dataStore);
        services.AddScoped<ITerritoryMembershipRepository, InMemoryTerritoryMembershipRepository>();
        services.AddScoped<IMembershipCapabilityRepository, InMemoryMembershipCapabilityRepository>();
        services.AddScoped<AccessEvaluator>(sp =>
        {
            var ds = sp.GetRequiredService<InMemoryDataStore>();
            var membershipRepository = new InMemoryTerritoryMembershipRepository(ds);
            var settingsRepository = new InMemoryMembershipSettingsRepository(ds);
            var userRepository = new InMemoryUserRepository(ds);
            var capabilityRepository = new InMemoryMembershipCapabilityRepository(ds);
            var systemPermissionRepository = new InMemorySystemPermissionRepository(ds);
            var featureFlags = new InMemoryFeatureFlagService();
            var cache = sp.GetRequiredService<IDistributedCacheService>();

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
        });
        services.AddScoped<IEventBus, InMemoryEventBus>();
        services.AddScoped<IEventHandler<MembershipCapabilityRevokedEvent>, MembershipCapabilityRevokedCacheHandler>();

        var serviceProvider = services.BuildServiceProvider();
        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        var accessEvaluator = serviceProvider.GetRequiredService<AccessEvaluator>();
        var membershipRepository = serviceProvider.GetRequiredService<ITerritoryMembershipRepository>();
        var capabilityRepository = serviceProvider.GetRequiredService<IMembershipCapabilityRepository>();

        var userId = Guid.NewGuid();
        var territoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        // Criar membership
        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            userId,
            territoryId,
            MembershipRole.Resident,
            ResidencyVerification.GeoVerified,
            DateTime.UtcNow,
            null,
            DateTime.UtcNow);

        await membershipRepository.AddAsync(membership, CancellationToken.None);

        // Criar capability
        var capability = new MembershipCapability(
            Guid.NewGuid(),
            membership.Id,
            MembershipCapabilityType.Curator,
            DateTime.UtcNow,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test");

        await capabilityRepository.AddAsync(capability, CancellationToken.None);

        // Popular cache de membership (usar GetRoleAsync que sempre popula cache)
        var role = await accessEvaluator.GetRoleAsync(userId, territoryId, CancellationToken.None);
        Assert.Equal(MembershipRole.Resident, role);

        // Verificar que está em cache
        var cache = serviceProvider.GetRequiredService<IDistributedCacheService>();
        var residentCacheKey = $"membership:resident:{userId}:{territoryId}";
        var roleCacheKey = $"membership:role:{userId}:{territoryId}";
        
        // Popular cache de resident também
        var isResident = await accessEvaluator.IsResidentAsync(userId, territoryId, CancellationToken.None);
        
        // Verificar que ambos estão em cache
        Assert.True(await cache.ExistsAsync(residentCacheKey, CancellationToken.None));
        Assert.True(await cache.ExistsAsync(roleCacheKey, CancellationToken.None));

        // Revogar capability
        capability.Revoke(DateTime.UtcNow);
        await capabilityRepository.UpdateAsync(capability, CancellationToken.None);

        // Publicar evento
        var revokedEvent = new MembershipCapabilityRevokedEvent(
            capability.MembershipId,
            userId,
            territoryId,
            DateTime.UtcNow);

        await eventBus.PublishAsync(revokedEvent, CancellationToken.None);

        // Verificar que cache foi invalidado
        Assert.False(await cache.ExistsAsync(residentCacheKey, CancellationToken.None));
        Assert.False(await cache.ExistsAsync(roleCacheKey, CancellationToken.None));
    }

    [Fact]
    public async Task SystemPermissionService_RevokeAsync_PublishesEvent()
    {
        var dataStore = new InMemoryDataStore();
        var services = new ServiceCollection();
        var cacheService = CacheTestHelper.CreateDistributedCacheService();
        services.AddSingleton<IDistributedCacheService>(_ => cacheService);
        services.AddSingleton<InMemoryDataStore>(dataStore);
        services.AddScoped<ISystemPermissionRepository, InMemorySystemPermissionRepository>();
        services.AddScoped<AccessEvaluator>(sp =>
        {
            var ds = sp.GetRequiredService<InMemoryDataStore>();
            var membershipRepository = new InMemoryTerritoryMembershipRepository(ds);
            var settingsRepository = new InMemoryMembershipSettingsRepository(ds);
            var userRepository = new InMemoryUserRepository(ds);
            var capabilityRepository = new InMemoryMembershipCapabilityRepository(ds);
            var systemPermissionRepository = new InMemorySystemPermissionRepository(ds);
            var featureFlags = new InMemoryFeatureFlagService();
            var cache = sp.GetRequiredService<IDistributedCacheService>();

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
        });
        services.AddScoped<IEventBus, InMemoryEventBus>();
        services.AddScoped<IEventHandler<SystemPermissionRevokedEvent>, SystemPermissionRevokedCacheHandler>();
        services.AddScoped<IAuditLogger, InMemoryAuditLogger>();
        services.AddScoped<SystemPermissionService>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<SystemPermissionService>();
        var accessEvaluator = serviceProvider.GetRequiredService<AccessEvaluator>();
        var systemPermissionRepository = serviceProvider.GetRequiredService<ISystemPermissionRepository>();
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

        var userId = Guid.NewGuid();
        var permissionType = SystemPermissionType.SystemAdmin;

        // Criar permissão
        var permission = new SystemPermission(
            Guid.NewGuid(),
            userId,
            permissionType,
            DateTime.UtcNow,
            Guid.NewGuid(),
            null,
            null);

        await systemPermissionRepository.AddAsync(permission, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        // Popular cache
        var hasPermission = await accessEvaluator.HasSystemPermissionAsync(userId, permissionType, CancellationToken.None);
        Assert.True(hasPermission);

        // Verificar que está em cache
        var cache = serviceProvider.GetRequiredService<IDistributedCacheService>();
        var cacheKey = $"system:permission:{userId}:{permissionType}";
        var exists = await cache.ExistsAsync(cacheKey, CancellationToken.None);
        Assert.True(exists);

        // Revogar usando serviço
        var result = await service.RevokeAsync(permission.Id, Guid.NewGuid(), CancellationToken.None);
        Assert.True(result.IsSuccess);

        // Verificar que cache foi invalidado
        var stillExists = await cache.ExistsAsync(cacheKey, CancellationToken.None);
        Assert.False(stillExists);
    }

    [Fact]
    public async Task MembershipCapabilityService_RevokeAsync_PublishesEvent()
    {
        var dataStore = new InMemoryDataStore();
        var services = new ServiceCollection();
        var cacheService = CacheTestHelper.CreateDistributedCacheService();
        services.AddSingleton<IDistributedCacheService>(_ => cacheService);
        services.AddSingleton<InMemoryDataStore>(dataStore);
        services.AddScoped<IMembershipCapabilityRepository, InMemoryMembershipCapabilityRepository>();
        services.AddScoped<ITerritoryMembershipRepository, InMemoryTerritoryMembershipRepository>();
        services.AddScoped<AccessEvaluator>(sp =>
        {
            var ds = sp.GetRequiredService<InMemoryDataStore>();
            var membershipRepository = new InMemoryTerritoryMembershipRepository(ds);
            var settingsRepository = new InMemoryMembershipSettingsRepository(ds);
            var userRepository = new InMemoryUserRepository(ds);
            var capabilityRepository = new InMemoryMembershipCapabilityRepository(ds);
            var systemPermissionRepository = new InMemorySystemPermissionRepository(ds);
            var featureFlags = new InMemoryFeatureFlagService();
            var cache = sp.GetRequiredService<IDistributedCacheService>();

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
        });
        services.AddScoped<IEventBus, InMemoryEventBus>();
        services.AddScoped<IEventHandler<MembershipCapabilityRevokedEvent>, MembershipCapabilityRevokedCacheHandler>();
        services.AddScoped<IAuditLogger, InMemoryAuditLogger>();
        services.AddScoped<MembershipCapabilityService>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<MembershipCapabilityService>();
        var accessEvaluator = serviceProvider.GetRequiredService<AccessEvaluator>();
        var membershipRepository = serviceProvider.GetRequiredService<ITerritoryMembershipRepository>();
        var capabilityRepository = serviceProvider.GetRequiredService<IMembershipCapabilityRepository>();
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

        var userId = Guid.NewGuid();
        var territoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        // Criar membership
        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            userId,
            territoryId,
            MembershipRole.Resident,
            ResidencyVerification.GeoVerified,
            DateTime.UtcNow,
            null,
            DateTime.UtcNow);

        await membershipRepository.AddAsync(membership, CancellationToken.None);

        // Criar capability
        var capability = new MembershipCapability(
            Guid.NewGuid(),
            membership.Id,
            MembershipCapabilityType.Curator,
            DateTime.UtcNow,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test");

        await capabilityRepository.AddAsync(capability, CancellationToken.None);
        await unitOfWork.CommitAsync(CancellationToken.None);

        // Popular cache (usar GetRoleAsync que sempre popula cache)
        var role = await accessEvaluator.GetRoleAsync(userId, territoryId, CancellationToken.None);
        Assert.Equal(MembershipRole.Resident, role);

        // Popular cache de resident também
        var isResident = await accessEvaluator.IsResidentAsync(userId, territoryId, CancellationToken.None);

        // Verificar que está em cache
        var cache = serviceProvider.GetRequiredService<IDistributedCacheService>();
        var residentCacheKey = $"membership:resident:{userId}:{territoryId}";
        var roleCacheKey = $"membership:role:{userId}:{territoryId}";
        Assert.True(await cache.ExistsAsync(residentCacheKey, CancellationToken.None));
        Assert.True(await cache.ExistsAsync(roleCacheKey, CancellationToken.None));

        // Revogar usando serviço
        var result = await service.RevokeAsync(capability.Id, Guid.NewGuid(), CancellationToken.None);
        Assert.True(result.IsSuccess);

        // Verificar que cache foi invalidado
        Assert.False(await cache.ExistsAsync(residentCacheKey, CancellationToken.None));
        Assert.False(await cache.ExistsAsync(roleCacheKey, CancellationToken.None));
    }
}
