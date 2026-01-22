using Araponga.Application.Interfaces;
using Araponga.Domain.Email;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresEmailQueueRepository : IEmailQueueRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresEmailQueueRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<EmailQueueItem> AddAsync(EmailQueueItem item, CancellationToken cancellationToken)
    {
        var record = item.ToRecord();
        _dbContext.EmailQueueItems.Add(record);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task<IReadOnlyList<EmailQueueItem>> GetPendingItemsAsync(
        int limit,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var records = await _dbContext.EmailQueueItems
            .AsNoTracking()
            .Where(item => item.Status == (int)EmailQueueStatus.Pending &&
                          (item.ScheduledFor == null || item.ScheduledFor <= now) &&
                          (item.NextRetryAtUtc == null || item.NextRetryAtUtc <= now))
            .OrderByDescending(item => item.Priority)
            .ThenBy(item => item.CreatedAtUtc)
            .Take(limit)
            .ToListAsync(cancellationToken);

        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<EmailQueueItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.EmailQueueItems
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return record?.ToDomain();
    }

    public async Task UpdateAsync(EmailQueueItem item, CancellationToken cancellationToken)
    {
        var record = await _dbContext.EmailQueueItems
            .FirstOrDefaultAsync(r => r.Id == item.Id, cancellationToken);

        if (record == null)
            return;

        record.To = item.To;
        record.Subject = item.Subject;
        record.Body = item.Body;
        record.IsHtml = item.IsHtml;
        record.TemplateName = item.TemplateName;
        record.TemplateDataJson = item.TemplateDataJson;
        record.Priority = (int)item.Priority;
        record.ScheduledFor = item.ScheduledFor;
        record.Attempts = item.Attempts;
        record.Status = (int)item.Status;
        record.ProcessedAtUtc = item.ProcessedAtUtc;
        record.ErrorMessage = item.ErrorMessage;
        record.NextRetryAtUtc = item.NextRetryAtUtc;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CountPendingAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        return await _dbContext.EmailQueueItems
            .AsNoTracking()
            .CountAsync(item => item.Status == (int)EmailQueueStatus.Pending &&
                               (item.ScheduledFor == null || item.ScheduledFor <= now) &&
                               (item.NextRetryAtUtc == null || item.NextRetryAtUtc <= now),
                       cancellationToken);
    }
}
