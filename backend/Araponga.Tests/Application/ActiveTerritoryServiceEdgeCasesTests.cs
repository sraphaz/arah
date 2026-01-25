using Araponga.Application.Services;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for ActiveTerritoryService (SetActive invalid/valid, GetActive).
/// </summary>
public sealed class ActiveTerritoryServiceEdgeCasesTests
{
    [Fact]
    public async Task SetActiveAsync_WhenTerritoryNotFound_ReturnsFalse()
    {
        var ds = new InMemoryDataStore();
        var store = new InMemoryActiveTerritoryStore(ds);
        var territoryRepo = new InMemoryTerritoryRepository(ds);
        var uow = new InMemoryUnitOfWork();
        var svc = new ActiveTerritoryService(store, territoryRepo, uow);

        var ok = await svc.SetActiveAsync("session-1", Guid.NewGuid(), CancellationToken.None);

        Assert.False(ok);
    }

    [Fact]
    public async Task SetActiveAsync_WhenTerritoryExists_ReturnsTrue()
    {
        var ds = new InMemoryDataStore();
        var store = new InMemoryActiveTerritoryStore(ds);
        var territoryRepo = new InMemoryTerritoryRepository(ds);
        var uow = new InMemoryUnitOfWork();
        var svc = new ActiveTerritoryService(store, territoryRepo, uow);
        var territoryId = ds.Territories[0].Id;

        var ok = await svc.SetActiveAsync("session-1", territoryId, CancellationToken.None);

        Assert.True(ok);
    }

    [Fact]
    public async Task GetActiveAsync_WhenNotSet_ReturnsNull()
    {
        var ds = new InMemoryDataStore();
        var store = new InMemoryActiveTerritoryStore(ds);
        var territoryRepo = new InMemoryTerritoryRepository(ds);
        var uow = new InMemoryUnitOfWork();
        var svc = new ActiveTerritoryService(store, territoryRepo, uow);

        var active = await svc.GetActiveAsync("session-unknown", CancellationToken.None);

        Assert.Null(active);
    }

    [Fact]
    public async Task SetActiveAsync_ThenGetActiveAsync_ReturnsTerritoryId()
    {
        var ds = new InMemoryDataStore();
        var store = new InMemoryActiveTerritoryStore(ds);
        var territoryRepo = new InMemoryTerritoryRepository(ds);
        var uow = new InMemoryUnitOfWork();
        var svc = new ActiveTerritoryService(store, territoryRepo, uow);
        var territoryId = ds.Territories[0].Id;

        await svc.SetActiveAsync("s1", territoryId, CancellationToken.None);
        var active = await svc.GetActiveAsync("s1", CancellationToken.None);

        Assert.NotNull(active);
        Assert.Equal(territoryId, active);
    }
}
