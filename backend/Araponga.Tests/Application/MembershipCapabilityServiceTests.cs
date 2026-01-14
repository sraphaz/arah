using Araponga.Application.Events;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Domain.Membership;
using Araponga.Infrastructure.Eventing;
using Araponga.Infrastructure.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class MembershipCapabilityServiceTests
{
    [Fact]
    public async Task GrantAsync_CreatesCapability_WhenValid()
    {
        var dataStore = new InMemoryDataStore();
        var services = new ServiceCollection();
        services.AddSingleton<InMemoryDataStore>(dataStore);
        services.AddScoped<IMembershipCapabilityRepository, InMemoryMembershipCapabilityRepository>();
        services.AddScoped<ITerritoryMembershipRepository, InMemoryTerritoryMembershipRepository>();
        services.AddScoped<IEventBus, InMemoryEventBus>();
        services.AddScoped<IAuditLogger, InMemoryAuditLogger>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
        services.AddScoped<MembershipCapabilityService>();

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<MembershipCapabilityService>();
        var membershipRepository = serviceProvider.GetRequiredService<ITerritoryMembershipRepository>();
        var capabilityRepository = serviceProvider.GetRequiredService<IMembershipCapabilityRepository>();

        // Criar membership
        var userId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();
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

        var grantedByUserId = Guid.NewGuid();
        var grantedByMembershipId = Guid.NewGuid();
        var capabilityType = MembershipCapabilityType.Curator;
        var reason = "Test reason";

        var result = await service.GrantAsync(
            membership.Id,
            capabilityType,
            grantedByUserId,
            grantedByMembershipId,
            reason,
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(membership.Id, result.Value.MembershipId);
        Assert.Equal(capabilityType, result.Value.CapabilityType);
        Assert.Equal(reason, result.Value.Reason);
        Assert.True(result.Value.IsActive());

        // Verificar que foi salvo no repositório
        var saved = await capabilityRepository.GetByIdAsync(result.Value.Id, CancellationToken.None);
        Assert.NotNull(saved);

        // Verificar que foi registrado auditoria
        var auditEntries = dataStore.AuditEntries;
        Assert.Contains(auditEntries, e =>
            e.Action == "membership_capability.granted" &&
            e.ActorUserId == grantedByUserId &&
            e.TerritoryId == territoryId &&
            e.TargetId == result.Value.Id);
    }

    [Fact]
    public async Task GrantAsync_ReturnsFailure_WhenMembershipNotFound()
    {
        var dataStore = new InMemoryDataStore();
        var services = new ServiceCollection();
        services.AddSingleton<InMemoryDataStore>(dataStore);
        services.AddScoped<IMembershipCapabilityRepository, InMemoryMembershipCapabilityRepository>();
        services.AddScoped<ITerritoryMembershipRepository, InMemoryTerritoryMembershipRepository>();
        services.AddScoped<IEventBus, InMemoryEventBus>();
        services.AddScoped<IAuditLogger, InMemoryAuditLogger>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
        services.AddScoped<MembershipCapabilityService>();

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<MembershipCapabilityService>();

        var result = await service.GrantAsync(
            Guid.NewGuid(),
            MembershipCapabilityType.Curator,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test",
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Membership not found", result.Error ?? string.Empty);
    }

    [Fact]
    public async Task GrantAsync_ReturnsFailure_WhenCapabilityAlreadyExists()
    {
        var dataStore = new InMemoryDataStore();
        var services = new ServiceCollection();
        services.AddSingleton<InMemoryDataStore>(dataStore);
        services.AddScoped<IMembershipCapabilityRepository, InMemoryMembershipCapabilityRepository>();
        services.AddScoped<ITerritoryMembershipRepository, InMemoryTerritoryMembershipRepository>();
        services.AddScoped<IEventBus, InMemoryEventBus>();
        services.AddScoped<IAuditLogger, InMemoryAuditLogger>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
        services.AddScoped<MembershipCapabilityService>();

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<MembershipCapabilityService>();
        var membershipRepository = serviceProvider.GetRequiredService<ITerritoryMembershipRepository>();

        // Criar membership
        var userId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();
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

        var grantedByUserId = Guid.NewGuid();
        var capabilityType = MembershipCapabilityType.Curator;

        // Criar primeira capability
        var firstResult = await service.GrantAsync(
            membership.Id,
            capabilityType,
            grantedByUserId,
            null,
            "First",
            CancellationToken.None);
        Assert.True(firstResult.IsSuccess);

        // Tentar criar segunda capability do mesmo tipo
        var secondResult = await service.GrantAsync(
            membership.Id,
            capabilityType,
            grantedByUserId,
            null,
            "Second",
            CancellationToken.None);
        Assert.False(secondResult.IsSuccess);
        Assert.Contains("already has this active capability", secondResult.Error ?? string.Empty);
    }

    [Fact]
    public async Task RevokeAsync_RevokesCapability_WhenValid()
    {
        var dataStore = new InMemoryDataStore();
        var services = new ServiceCollection();
        services.AddSingleton<InMemoryDataStore>(dataStore);
        services.AddScoped<IMembershipCapabilityRepository, InMemoryMembershipCapabilityRepository>();
        services.AddScoped<ITerritoryMembershipRepository, InMemoryTerritoryMembershipRepository>();
        services.AddScoped<IEventBus, InMemoryEventBus>();
        services.AddScoped<IAuditLogger, InMemoryAuditLogger>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
        services.AddScoped<MembershipCapabilityService>();

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<MembershipCapabilityService>();
        var membershipRepository = serviceProvider.GetRequiredService<ITerritoryMembershipRepository>();
        var capabilityRepository = serviceProvider.GetRequiredService<IMembershipCapabilityRepository>();

        // Criar membership
        var userId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();
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
        var grantResult = await service.GrantAsync(
            membership.Id,
            MembershipCapabilityType.Curator,
            Guid.NewGuid(),
            null,
            "Test",
            CancellationToken.None);
        Assert.True(grantResult.IsSuccess);

        var capabilityId = grantResult.Value!.Id;
        var revokedByUserId = Guid.NewGuid();

        // Revogar capability
        var revokeResult = await service.RevokeAsync(capabilityId, revokedByUserId, CancellationToken.None);
        Assert.True(revokeResult.IsSuccess);

        // Verificar que foi revogada
        var revoked = await capabilityRepository.GetByIdAsync(capabilityId, CancellationToken.None);
        Assert.NotNull(revoked);
        Assert.False(revoked.IsActive());
        Assert.NotNull(revoked.RevokedAtUtc);

        // Verificar que foi registrado auditoria
        var auditEntries = dataStore.AuditEntries;
        Assert.Contains(auditEntries, e =>
            e.Action == "membership_capability.revoked" &&
            e.ActorUserId == revokedByUserId &&
            e.TerritoryId == territoryId &&
            e.TargetId == capabilityId);
    }

    [Fact]
    public async Task RevokeAsync_ReturnsFailure_WhenCapabilityNotFound()
    {
        var dataStore = new InMemoryDataStore();
        var services = new ServiceCollection();
        services.AddSingleton<InMemoryDataStore>(dataStore);
        services.AddScoped<IMembershipCapabilityRepository, InMemoryMembershipCapabilityRepository>();
        services.AddScoped<ITerritoryMembershipRepository, InMemoryTerritoryMembershipRepository>();
        services.AddScoped<IEventBus, InMemoryEventBus>();
        services.AddScoped<IAuditLogger, InMemoryAuditLogger>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
        services.AddScoped<MembershipCapabilityService>();

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<MembershipCapabilityService>();

        var result = await service.RevokeAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error ?? string.Empty);
    }

    [Fact]
    public async Task RevokeAsync_ReturnsFailure_WhenCapabilityAlreadyRevoked()
    {
        var dataStore = new InMemoryDataStore();
        var services = new ServiceCollection();
        services.AddSingleton<InMemoryDataStore>(dataStore);
        services.AddScoped<IMembershipCapabilityRepository, InMemoryMembershipCapabilityRepository>();
        services.AddScoped<ITerritoryMembershipRepository, InMemoryTerritoryMembershipRepository>();
        services.AddScoped<IEventBus, InMemoryEventBus>();
        services.AddScoped<IAuditLogger, InMemoryAuditLogger>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
        services.AddScoped<MembershipCapabilityService>();

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<MembershipCapabilityService>();
        var membershipRepository = serviceProvider.GetRequiredService<ITerritoryMembershipRepository>();

        // Criar membership
        var userId = Guid.NewGuid();
        var territoryId = Guid.NewGuid();
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

        // Criar e revogar capability
        var grantResult = await service.GrantAsync(
            membership.Id,
            MembershipCapabilityType.Curator,
            Guid.NewGuid(),
            null,
            "Test",
            CancellationToken.None);
        var revokeResult = await service.RevokeAsync(grantResult.Value!.Id, Guid.NewGuid(), CancellationToken.None);
        Assert.True(revokeResult.IsSuccess);

        // Tentar revogar novamente
        var secondRevokeResult = await service.RevokeAsync(grantResult.Value!.Id, Guid.NewGuid(), CancellationToken.None);
        Assert.False(secondRevokeResult.IsSuccess);
        Assert.Contains("already revoked", secondRevokeResult.Error ?? string.Empty);
    }
}
