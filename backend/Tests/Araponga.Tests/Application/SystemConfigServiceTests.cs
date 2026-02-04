using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Configuration;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class SystemConfigServiceTests
{
    [Fact]
    public async Task UpsertAsync_CreatesConfig_AndWritesAudit()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var services = new ServiceCollection();
        services.AddSingleton<InMemoryDataStore>(dataStore);
        services.AddSingleton(sharedStore);
        services.AddMemoryCache();
        services.AddScoped<ISystemConfigRepository, InMemorySystemConfigRepository>();
        services.AddScoped<IAuditLogger, InMemoryAuditLogger>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
        services.AddScoped<SystemConfigCacheService>();
        services.AddScoped<SystemConfigService>();

        var sp = services.BuildServiceProvider();
        var svc = sp.GetRequiredService<SystemConfigService>();

        var actor = Guid.NewGuid();
        var result = await svc.UpsertAsync(
            "moderation.reports.threshold",
            "3",
            SystemConfigCategory.Moderation,
            "Threshold de reports",
            actor,
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);

        Assert.Contains(dataStore.AuditEntries, e =>
            e.Action == "system_config.created" &&
            e.ActorUserId == actor &&
            e.TargetId == result.Value!.Id);
    }

    [Fact]
    public async Task GetByKeyAsync_UsesCache_AndInvalidatesOnUpdate()
    {
        var dataStore = new InMemoryDataStore();
        var sharedStore = new InMemorySharedStore();
        var services = new ServiceCollection();
        services.AddSingleton<InMemoryDataStore>(dataStore);
        services.AddSingleton(sharedStore);
        services.AddMemoryCache();
        services.AddScoped<ISystemConfigRepository, InMemorySystemConfigRepository>();
        services.AddScoped<IAuditLogger, InMemoryAuditLogger>();
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
        services.AddScoped<SystemConfigCacheService>();
        services.AddScoped<SystemConfigService>();

        var sp = services.BuildServiceProvider();
        var svc = sp.GetRequiredService<SystemConfigService>();

        var actor = Guid.NewGuid();
        await svc.UpsertAsync(
            "providers.email.enabled",
            "false",
            SystemConfigCategory.Providers,
            null,
            actor,
            CancellationToken.None);

        var first = await svc.GetByKeyAsync("providers.email.enabled", CancellationToken.None);
        Assert.NotNull(first);
        Assert.Equal("false", first!.Value);

        await svc.UpsertAsync(
            "providers.email.enabled",
            "true",
            SystemConfigCategory.Providers,
            null,
            actor,
            CancellationToken.None);

        var second = await svc.GetByKeyAsync("providers.email.enabled", CancellationToken.None);
        Assert.NotNull(second);
        Assert.Equal("true", second!.Value);
    }
}

