using Araponga.Application.Interfaces;
using Araponga.Application.Models;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryNotificationInboxRepository : INotificationInboxRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryNotificationInboxRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task AddAsync(UserNotification notification, CancellationToken cancellationToken)
    {
        _dataStore.UserNotifications.Add(notification);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<UserNotification>> ListByUserAsync(
        Guid userId,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var notifications = _dataStore.UserNotifications
            .Where(notification => notification.UserId == userId)
            .OrderByDescending(notification => notification.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToList();

        return Task.FromResult<IReadOnlyList<UserNotification>>(notifications);
    }

    public Task<bool> MarkAsReadAsync(
        Guid notificationId,
        Guid userId,
        DateTime readAtUtc,
        CancellationToken cancellationToken)
    {
        var notification = _dataStore.UserNotifications
            .FirstOrDefault(item => item.Id == notificationId && item.UserId == userId);

        if (notification is null)
        {
            return Task.FromResult(false);
        }

        if (notification.ReadAtUtc is null)
        {
            var updated = notification with { ReadAtUtc = readAtUtc };
            var index = _dataStore.UserNotifications.IndexOf(notification);
            if (index >= 0)
            {
                _dataStore.UserNotifications[index] = updated;
            }
        }

        return Task.FromResult(true);
    }
}
