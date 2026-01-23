using Araponga.Application.Interfaces;
using Araponga.Domain.Email;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryEmailQueueRepository : IEmailQueueRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryEmailQueueRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<EmailQueueItem> AddAsync(EmailQueueItem item, CancellationToken cancellationToken)
    {
        _dataStore.EmailQueueItems.Add(item);
        return Task.FromResult(item);
    }

    public Task<IReadOnlyList<EmailQueueItem>> GetPendingItemsAsync(
        int limit,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var items = _dataStore.EmailQueueItems
            .Where(item => item.Status == EmailQueueStatus.Pending &&
                          (item.ScheduledFor == null || item.ScheduledFor <= now) &&
                          (item.NextRetryAtUtc == null || item.NextRetryAtUtc <= now))
            .OrderByDescending(item => item.Priority)
            .ThenBy(item => item.CreatedAtUtc)
            .Take(limit)
            .ToList();

        return Task.FromResult<IReadOnlyList<EmailQueueItem>>(items);
    }

    public Task<EmailQueueItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var item = _dataStore.EmailQueueItems.FirstOrDefault(i => i.Id == id);
        return Task.FromResult(item);
    }

    public Task UpdateAsync(EmailQueueItem item, CancellationToken cancellationToken)
    {
        // InMemory já atualiza por referência
        return Task.CompletedTask;
    }

    public Task<int> CountPendingAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var count = _dataStore.EmailQueueItems
            .Count(item => item.Status == EmailQueueStatus.Pending &&
                          (item.ScheduledFor == null || item.ScheduledFor <= now) &&
                          (item.NextRetryAtUtc == null || item.NextRetryAtUtc <= now));

        return Task.FromResult(count);
    }
}
