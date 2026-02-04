using Araponga.Modules.Moderation.Application.Interfaces;
using Araponga.Modules.Moderation.Domain.Work;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryWorkItemRepository : IWorkItemRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryWorkItemRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<WorkItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var found = _dataStore.WorkItems.FirstOrDefault(w => w.Id == id);
        return Task.FromResult<WorkItem?>(found);
    }

    public Task<IReadOnlyList<WorkItem>> ListAsync(
        WorkItemType? type,
        WorkItemStatus? status,
        Guid? territoryId,
        CancellationToken cancellationToken)
    {
        var query = _dataStore.WorkItems.AsEnumerable();
        if (type.HasValue)
        {
            query = query.Where(w => w.Type == type.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(w => w.Status == status.Value);
        }

        if (territoryId.HasValue)
        {
            query = query.Where(w => w.TerritoryId == territoryId.Value);
        }

        var items = query
            .OrderByDescending(w => w.CreatedAtUtc)
            .ToList();

        return Task.FromResult<IReadOnlyList<WorkItem>>(items);
    }

    public Task<WorkItem?> GetLatestOpenBySubjectAsync(
        WorkItemType type,
        string subjectType,
        Guid subjectId,
        CancellationToken cancellationToken)
    {
        var normalizedSubjectType = string.IsNullOrWhiteSpace(subjectType)
            ? string.Empty
            : subjectType.Trim().ToUpperInvariant();

        var found = _dataStore.WorkItems
            .Where(w =>
                w.Type == type &&
                w.SubjectType == normalizedSubjectType &&
                w.SubjectId == subjectId &&
                w.Status != WorkItemStatus.Completed &&
                w.Status != WorkItemStatus.Cancelled)
            .OrderByDescending(w => w.CreatedAtUtc)
            .FirstOrDefault();

        return Task.FromResult<WorkItem?>(found);
    }

    public Task AddAsync(WorkItem item, CancellationToken cancellationToken)
    {
        _dataStore.WorkItems.Add(item);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(WorkItem item, CancellationToken cancellationToken)
    {
        var idx = _dataStore.WorkItems.FindIndex(w => w.Id == item.Id);
        if (idx >= 0)
        {
            _dataStore.WorkItems[idx] = item;
        }
        return Task.CompletedTask;
    }
}

