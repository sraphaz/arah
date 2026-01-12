using Araponga.Application.Models;

namespace Araponga.Application.Interfaces;

public interface INotificationInboxRepository
{
    Task AddAsync(UserNotification notification, CancellationToken cancellationToken);
    Task<IReadOnlyList<UserNotification>> ListByUserAsync(
        Guid userId,
        int skip,
        int take,
        CancellationToken cancellationToken);
    Task<bool> MarkAsReadAsync(Guid notificationId, Guid userId, DateTime readAtUtc, CancellationToken cancellationToken);
}
