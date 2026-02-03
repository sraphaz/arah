using Araponga.Domain.Work;

namespace Araponga.Application.Interfaces;

public interface IWorkItemRepository
{
    Task<WorkItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<WorkItem>> ListAsync(
        WorkItemType? type,
        WorkItemStatus? status,
        Guid? territoryId,
        CancellationToken cancellationToken);
    Task<WorkItem?> GetLatestOpenBySubjectAsync(
        WorkItemType type,
        string subjectType,
        Guid subjectId,
        CancellationToken cancellationToken);
    Task AddAsync(WorkItem item, CancellationToken cancellationToken);
    Task UpdateAsync(WorkItem item, CancellationToken cancellationToken);
}

