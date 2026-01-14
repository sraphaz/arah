using Araponga.Application.Interfaces;
using Araponga.Domain.Work;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresWorkItemRepository : IWorkItemRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresWorkItemRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<WorkItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.WorkItems
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<WorkItem>> ListAsync(
        WorkItemType? type,
        WorkItemStatus? status,
        Guid? territoryId,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.WorkItems.AsNoTracking();
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

        var records = await query
            .OrderByDescending(w => w.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<WorkItem?> GetLatestOpenBySubjectAsync(
        WorkItemType type,
        string subjectType,
        Guid subjectId,
        CancellationToken cancellationToken)
    {
        var normalizedSubjectType = string.IsNullOrWhiteSpace(subjectType)
            ? string.Empty
            : subjectType.Trim().ToUpperInvariant();

        if (string.IsNullOrWhiteSpace(normalizedSubjectType) || subjectId == Guid.Empty)
        {
            return null;
        }

        var record = await _dbContext.WorkItems
            .AsNoTracking()
            .Where(w =>
                w.Type == type &&
                w.SubjectType == normalizedSubjectType &&
                w.SubjectId == subjectId &&
                w.Status != WorkItemStatus.Completed &&
                w.Status != WorkItemStatus.Cancelled)
            .OrderByDescending(w => w.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        return record?.ToDomain();
    }

    public Task AddAsync(WorkItem item, CancellationToken cancellationToken)
    {
        _dbContext.WorkItems.Add(item.ToRecord());
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(WorkItem item, CancellationToken cancellationToken)
    {
        var record = await _dbContext.WorkItems
            .FirstOrDefaultAsync(w => w.Id == item.Id, cancellationToken);

        if (record is null)
        {
            return;
        }

        var updated = item.ToRecord();
        _dbContext.Entry(record).CurrentValues.SetValues(updated);
    }
}

