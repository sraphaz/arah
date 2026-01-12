using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresNotificationInboxRepository : INotificationInboxRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresNotificationInboxRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(UserNotification notification, CancellationToken cancellationToken)
    {
        _dbContext.UserNotifications.Add(new UserNotificationRecord
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Title = notification.Title,
            Body = notification.Body,
            Kind = notification.Kind,
            DataJson = notification.DataJson,
            CreatedAtUtc = notification.CreatedAtUtc,
            ReadAtUtc = notification.ReadAtUtc,
            SourceOutboxId = notification.SourceOutboxId
        });

        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<UserNotification>> ListByUserAsync(
        Guid userId,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var records = await _dbContext.UserNotifications
            .AsNoTracking()
            .Where(notification => notification.UserId == userId)
            .OrderByDescending(notification => notification.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return records.Select(record => new UserNotification(
            record.Id,
            record.UserId,
            record.Title,
            record.Body,
            record.Kind,
            record.DataJson,
            record.CreatedAtUtc,
            record.ReadAtUtc,
            record.SourceOutboxId)).ToList();
    }

    public async Task<bool> MarkAsReadAsync(
        Guid notificationId,
        Guid userId,
        DateTime readAtUtc,
        CancellationToken cancellationToken)
    {
        var record = await _dbContext.UserNotifications
            .FirstOrDefaultAsync(
                notification => notification.Id == notificationId && notification.UserId == userId,
                cancellationToken);

        if (record is null)
        {
            return false;
        }

        if (record.ReadAtUtc is null)
        {
            record.ReadAtUtc = readAtUtc;
        }

        return true;
    }
}
