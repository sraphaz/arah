using Araponga.Application.Common;
using Araponga.Application.Services;
using Araponga.Domain.Territories;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for TerritoryService (Create validation, List, GetById, Search, Paged).
/// </summary>
public sealed class TerritoryServiceEdgeCasesTests
{
    [Fact]
    public async Task CreateAsync_WithEmptyName_ReturnsFailure()
    {
        var sharedStore = new InMemorySharedStore();
        var repo = new InMemoryTerritoryRepository(sharedStore);
        var uow = new InMemoryUnitOfWork();
        var svc = new TerritoryService(repo, uow, null, null);

        var result = await svc.CreateAsync("", "Desc", "City", "ST", 0, 0, CancellationToken.None);

        Assert.False(result.Success);
        Assert.NotNull(result.Error);
        Assert.Contains("Name", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_WithEmptyCity_ReturnsFailure()
    {
        var sharedStore = new InMemorySharedStore();
        var repo = new InMemoryTerritoryRepository(sharedStore);
        var uow = new InMemoryUnitOfWork();
        var svc = new TerritoryService(repo, uow, null, null);

        var result = await svc.CreateAsync("T", "D", "", "ST", 0, 0, CancellationToken.None);

        Assert.False(result.Success);
        Assert.NotNull(result.Error);
        Assert.Contains("City", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateAsync_WithEmptyState_ReturnsFailure()
    {
        var sharedStore = new InMemorySharedStore();
        var repo = new InMemoryTerritoryRepository(sharedStore);
        var uow = new InMemoryUnitOfWork();
        var svc = new TerritoryService(repo, uow, null, null);

        var result = await svc.CreateAsync("T", "D", "City", "  ", 0, 0, CancellationToken.None);

        Assert.False(result.Success);
        Assert.NotNull(result.Error);
        Assert.Contains("State", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ListAvailableAsync_WhenNoCache_ReturnsActiveTerritories()
    {
        var sharedStore = new InMemorySharedStore();
        var repo = new InMemoryTerritoryRepository(sharedStore);
        var uow = new InMemoryUnitOfWork();
        var svc = new TerritoryService(repo, uow, null, null);

        var list = await svc.ListAvailableAsync(CancellationToken.None);

        Assert.NotNull(list);
        Assert.All(list, t => Assert.Equal(TerritoryStatus.Active, t.Status));
    }

    [Fact]
    public async Task GetByIdAsync_WhenExists_ReturnsTerritory()
    {
        var sharedStore = new InMemorySharedStore();
        var repo = new InMemoryTerritoryRepository(sharedStore);
        var uow = new InMemoryUnitOfWork();
        var svc = new TerritoryService(repo, uow, null, null);
        var id = sharedStore.Territories[0].Id;

        var t = await svc.GetByIdAsync(id, CancellationToken.None);

        Assert.NotNull(t);
        Assert.Equal(id, t!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotExists_ReturnsNull()
    {
        var sharedStore = new InMemorySharedStore();
        var repo = new InMemoryTerritoryRepository(sharedStore);
        var uow = new InMemoryUnitOfWork();
        var svc = new TerritoryService(repo, uow, null, null);

        var t = await svc.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(t);
    }

    [Fact]
    public async Task SearchAsync_WithNullQuery_ReturnsResults()
    {
        var sharedStore = new InMemorySharedStore();
        var repo = new InMemoryTerritoryRepository(sharedStore);
        var uow = new InMemoryUnitOfWork();
        var svc = new TerritoryService(repo, uow, null, null);

        var list = await svc.SearchAsync(null, null, null, CancellationToken.None);

        Assert.NotNull(list);
    }

    [Fact]
    public async Task ListAvailablePagedAsync_ReturnsPagedResult()
    {
        var sharedStore = new InMemorySharedStore();
        var repo = new InMemoryTerritoryRepository(sharedStore);
        var uow = new InMemoryUnitOfWork();
        var svc = new TerritoryService(repo, uow, null, null);
        var paging = new PaginationParameters(1, 10);

        var result = await svc.ListAvailablePagedAsync(paging, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Items);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
    }
}
