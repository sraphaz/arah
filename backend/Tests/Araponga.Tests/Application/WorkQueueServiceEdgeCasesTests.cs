using Araponga.Application.Services;
using Araponga.Modules.Moderation.Domain.Work;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for WorkQueueService (List empty, GetById not found, Enqueue success).
/// </summary>
public sealed class WorkQueueServiceEdgeCasesTests
{
    [Fact]
    public async Task ListAsync_WhenNoItems_ReturnsEmpty()
    {
        var ds = new InMemoryDataStore();
        ds.WorkItems.Clear();
        var repo = new InMemoryWorkItemRepository(ds);
        var audit = new InMemoryAuditLogger(ds);
        var uow = new InMemoryUnitOfWork();
        var svc = new WorkQueueService(repo, audit, uow);

        var list = await svc.ListAsync(null, null, null, CancellationToken.None);

        Assert.NotNull(list);
        Assert.Empty(list);
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        var ds = new InMemoryDataStore();
        var repo = new InMemoryWorkItemRepository(ds);
        var audit = new InMemoryAuditLogger(ds);
        var uow = new InMemoryUnitOfWork();
        var svc = new WorkQueueService(repo, audit, uow);

        var item = await svc.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(item);
    }

    [Fact]
    public async Task EnqueueAsync_WithValidArgs_ReturnsSuccess()
    {
        var ds = new InMemoryDataStore();
        var repo = new InMemoryWorkItemRepository(ds);
        var audit = new InMemoryAuditLogger(ds);
        var uow = new InMemoryUnitOfWork();
        var svc = new WorkQueueService(repo, audit, uow);
        var userId = Guid.NewGuid();
        var territoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        var result = await svc.EnqueueAsync(
            WorkItemType.IdentityVerification,
            territoryId,
            userId,
            null,
            null,
            "post",
            Guid.NewGuid(),
            null,
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(WorkItemStatus.Pending, result.Value.Status);
    }
}
