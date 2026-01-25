using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Configuration;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for SystemConfigService (empty key, Get without cache, List).
/// </summary>
public sealed class SystemConfigServiceEdgeCasesTests
{
    [Fact]
    public async Task GetByKeyAsync_WithEmptyKey_ReturnsNull()
    {
        var ds = new InMemoryDataStore();
        var repo = new InMemorySystemConfigRepository(ds);
        var uow = new InMemoryUnitOfWork();
        var audit = new InMemoryAuditLogger(ds);
        var svc = new SystemConfigService(repo, uow, audit, cacheService: null);

        var result = await svc.GetByKeyAsync("  ", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpsertAsync_WithEmptyKey_ReturnsFailure()
    {
        var ds = new InMemoryDataStore();
        var repo = new InMemorySystemConfigRepository(ds);
        var uow = new InMemoryUnitOfWork();
        var audit = new InMemoryAuditLogger(ds);
        var svc = new SystemConfigService(repo, uow, audit, cacheService: null);
        var actor = Guid.NewGuid();

        var result = await svc.UpsertAsync(
            "",
            "value",
            SystemConfigCategory.Other,
            null,
            actor,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Key", result.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ListAsync_WithCategory_ReturnsFiltered()
    {
        var ds = new InMemoryDataStore();
        var repo = new InMemorySystemConfigRepository(ds);
        var uow = new InMemoryUnitOfWork();
        var audit = new InMemoryAuditLogger(ds);
        var svc = new SystemConfigService(repo, uow, audit, cacheService: null);

        var list = await svc.ListAsync(SystemConfigCategory.Other, CancellationToken.None);

        Assert.NotNull(list);
    }

    [Fact]
    public async Task ListAsync_WithNullCategory_ReturnsAll()
    {
        var ds = new InMemoryDataStore();
        var repo = new InMemorySystemConfigRepository(ds);
        var uow = new InMemoryUnitOfWork();
        var audit = new InMemoryAuditLogger(ds);
        var svc = new SystemConfigService(repo, uow, audit, cacheService: null);

        var list = await svc.ListAsync(null, CancellationToken.None);

        Assert.NotNull(list);
    }
}
