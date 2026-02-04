using Araponga.Modules.Moderation.Domain.Work;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Infrastructure;

public sealed class WorkItemRepositoryTests
{
    [Fact]
    public async Task WorkItemRepository_AddAndList()
    {
        var ds = new InMemoryDataStore();
        var repo = new InMemoryWorkItemRepository(ds);

        var territoryId = Guid.NewGuid();
        var item = new WorkItem(
            Guid.NewGuid(),
            WorkItemType.AssetCuration,
            WorkItemStatus.Pending,
            territoryId,
            Guid.NewGuid(),
            DateTime.UtcNow,
            requiredSystemPermission: null,
            requiredCapability: null,
            subjectType: "ASSET",
            subjectId: Guid.NewGuid(),
            payloadJson: "{}",
            outcome: WorkItemOutcome.None,
            completedAtUtc: null,
            completedByUserId: null,
            completionNotes: null);

        await repo.AddAsync(item, CancellationToken.None);

        var list = await repo.ListAsync(WorkItemType.AssetCuration, WorkItemStatus.Pending, territoryId, CancellationToken.None);
        Assert.Single(list);
        Assert.Equal(item.Id, list[0].Id);
    }

    [Fact]
    public async Task WorkItemRepository_GetLatestOpenBySubjectAsync_ReturnsMostRecentOpen()
    {
        var ds = new InMemoryDataStore();
        var repo = new InMemoryWorkItemRepository(ds);

        var territoryId = Guid.NewGuid();
        var subjectId = Guid.NewGuid();

        var older = new WorkItem(
            Guid.NewGuid(),
            WorkItemType.AssetCuration,
            WorkItemStatus.RequiresHumanReview,
            territoryId,
            Guid.NewGuid(),
            DateTime.UtcNow.AddMinutes(-10),
            requiredSystemPermission: null,
            requiredCapability: null,
            subjectType: "ASSET",
            subjectId: subjectId,
            payloadJson: null,
            outcome: WorkItemOutcome.None,
            completedAtUtc: null,
            completedByUserId: null,
            completionNotes: null);

        var newer = new WorkItem(
            Guid.NewGuid(),
            WorkItemType.AssetCuration,
            WorkItemStatus.RequiresHumanReview,
            territoryId,
            Guid.NewGuid(),
            DateTime.UtcNow,
            requiredSystemPermission: null,
            requiredCapability: null,
            subjectType: "ASSET",
            subjectId: subjectId,
            payloadJson: null,
            outcome: WorkItemOutcome.None,
            completedAtUtc: null,
            completedByUserId: null,
            completionNotes: null);

        await repo.AddAsync(older, CancellationToken.None);
        await repo.AddAsync(newer, CancellationToken.None);

        var found = await repo.GetLatestOpenBySubjectAsync(WorkItemType.AssetCuration, "asset", subjectId, CancellationToken.None);
        Assert.NotNull(found);
        Assert.Equal(newer.Id, found!.Id);
    }
}

