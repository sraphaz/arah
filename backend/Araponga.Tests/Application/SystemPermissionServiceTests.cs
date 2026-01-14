using Araponga.Application.Events;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Domain.Users;
using Araponga.Infrastructure.Eventing;
using Araponga.Infrastructure.InMemory;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class SystemPermissionServiceTests
{
    [Fact]
    public async Task GrantAsync_CreatesPermission_WhenValid()
    {
        var dataStore = new InMemoryDataStore();
        var services = new ServiceCollection();
        services.AddSingleton<InMemoryDataStore>(dataStore);
        services.AddScoped<ISystemPermissionRepository, InMemorySystemPermissionRepository>();
        services.AddScoped<IEventBus, InMemoryEventBus>();
        services.AddScoped<IAuditLogger, InMemoryAuditLogger>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
        services.AddScoped<SystemPermissionService>();

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<SystemPermissionService>();
        var repository = serviceProvider.GetRequiredService<ISystemPermissionRepository>();
        var auditLogger = serviceProvider.GetRequiredService<IAuditLogger>();

        var userId = Guid.NewGuid();
        var grantedByUserId = Guid.NewGuid();
        var permissionType = SystemPermissionType.SystemAdmin;

        var result = await service.GrantAsync(userId, permissionType, grantedByUserId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(userId, result.Value.UserId);
        Assert.Equal(permissionType, result.Value.PermissionType);
        Assert.True(result.Value.IsActive());

        // Verificar que foi salvo no repositório
        var saved = await repository.GetByIdAsync(result.Value.Id, CancellationToken.None);
        Assert.NotNull(saved);

        // Verificar que foi registrado auditoria
        var auditEntries = dataStore.AuditEntries;
        Assert.Contains(auditEntries, e => 
            e.Action == "system_permission.granted" &&
            e.ActorUserId == grantedByUserId &&
            e.TargetId == result.Value.Id);
    }

    [Fact]
    public async Task GrantAsync_ReturnsFailure_WhenPermissionAlreadyExists()
    {
        var dataStore = new InMemoryDataStore();
        var services = new ServiceCollection();
        services.AddSingleton<InMemoryDataStore>(dataStore);
        services.AddScoped<ISystemPermissionRepository, InMemorySystemPermissionRepository>();
        services.AddScoped<IEventBus, InMemoryEventBus>();
        services.AddScoped<IAuditLogger, InMemoryAuditLogger>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
        services.AddScoped<SystemPermissionService>();

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<SystemPermissionService>();
        var repository = serviceProvider.GetRequiredService<ISystemPermissionRepository>();

        var userId = Guid.NewGuid();
        var grantedByUserId = Guid.NewGuid();
        var permissionType = SystemPermissionType.SystemAdmin;

        // Criar primeira permissão
        var firstResult = await service.GrantAsync(userId, permissionType, grantedByUserId, CancellationToken.None);
        Assert.True(firstResult.IsSuccess);

        // Tentar criar segunda permissão do mesmo tipo
        var secondResult = await service.GrantAsync(userId, permissionType, grantedByUserId, CancellationToken.None);
        Assert.False(secondResult.IsSuccess);
        Assert.Contains("already has this active permission", secondResult.Error ?? string.Empty);
    }

    [Fact]
    public async Task RevokeAsync_RevokesPermission_WhenValid()
    {
        var dataStore = new InMemoryDataStore();
        var services = new ServiceCollection();
        services.AddSingleton<InMemoryDataStore>(dataStore);
        services.AddScoped<ISystemPermissionRepository, InMemorySystemPermissionRepository>();
        services.AddScoped<IEventBus, InMemoryEventBus>();
        services.AddScoped<IAuditLogger, InMemoryAuditLogger>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
        services.AddScoped<SystemPermissionService>();

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<SystemPermissionService>();
        var repository = serviceProvider.GetRequiredService<ISystemPermissionRepository>();

        var userId = Guid.NewGuid();
        var grantedByUserId = Guid.NewGuid();
        var revokedByUserId = Guid.NewGuid();
        var permissionType = SystemPermissionType.SystemAdmin;

        // Criar permissão
        var grantResult = await service.GrantAsync(userId, permissionType, grantedByUserId, CancellationToken.None);
        Assert.True(grantResult.IsSuccess);

        var permissionId = grantResult.Value!.Id;

        // Revogar permissão
        var revokeResult = await service.RevokeAsync(permissionId, revokedByUserId, CancellationToken.None);
        Assert.True(revokeResult.IsSuccess);

        // Verificar que foi revogada
        var revoked = await repository.GetByIdAsync(permissionId, CancellationToken.None);
        Assert.NotNull(revoked);
        Assert.False(revoked.IsActive());
        Assert.NotNull(revoked.RevokedAtUtc);
        Assert.Equal(revokedByUserId, revoked.RevokedByUserId);

        // Verificar que foi registrado auditoria
        var auditEntries = dataStore.AuditEntries;
        Assert.Contains(auditEntries, e => 
            e.Action == "system_permission.revoked" &&
            e.ActorUserId == revokedByUserId &&
            e.TargetId == permissionId);
    }

    [Fact]
    public async Task RevokeAsync_ReturnsFailure_WhenPermissionNotFound()
    {
        var dataStore = new InMemoryDataStore();
        var services = new ServiceCollection();
        services.AddSingleton<InMemoryDataStore>(dataStore);
        services.AddScoped<ISystemPermissionRepository, InMemorySystemPermissionRepository>();
        services.AddScoped<IEventBus, InMemoryEventBus>();
        services.AddScoped<IAuditLogger, InMemoryAuditLogger>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
        services.AddScoped<SystemPermissionService>();

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<SystemPermissionService>();

        var result = await service.RevokeAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);
        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.Error ?? string.Empty);
    }

    [Fact]
    public async Task RevokeAsync_ReturnsFailure_WhenPermissionAlreadyRevoked()
    {
        var dataStore = new InMemoryDataStore();
        var services = new ServiceCollection();
        services.AddSingleton<InMemoryDataStore>(dataStore);
        services.AddScoped<ISystemPermissionRepository, InMemorySystemPermissionRepository>();
        services.AddScoped<IEventBus, InMemoryEventBus>();
        services.AddScoped<IAuditLogger, InMemoryAuditLogger>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
        services.AddScoped<SystemPermissionService>();

        var serviceProvider = services.BuildServiceProvider();
        var service = serviceProvider.GetRequiredService<SystemPermissionService>();

        var userId = Guid.NewGuid();
        var grantedByUserId = Guid.NewGuid();
        var revokedByUserId = Guid.NewGuid();
        var permissionType = SystemPermissionType.SystemAdmin;

        // Criar e revogar permissão
        var grantResult = await service.GrantAsync(userId, permissionType, grantedByUserId, CancellationToken.None);
        var revokeResult = await service.RevokeAsync(grantResult.Value!.Id, revokedByUserId, CancellationToken.None);
        Assert.True(revokeResult.IsSuccess);

        // Tentar revogar novamente
        var secondRevokeResult = await service.RevokeAsync(grantResult.Value!.Id, revokedByUserId, CancellationToken.None);
        Assert.False(secondRevokeResult.IsSuccess);
        Assert.Contains("already revoked", secondRevokeResult.Error ?? string.Empty);
    }
}
