using Araponga.Application.Interfaces;
using Araponga.Domain.Social.JoinRequests;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresTerritoryJoinRequestRepository : ITerritoryJoinRequestRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresTerritoryJoinRequestRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TerritoryJoinRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryJoinRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(request => request.Id == id, cancellationToken);

        return record?.ToDomain();
    }

    public async Task<TerritoryJoinRequest?> GetPendingByRequesterAsync(
        Guid territoryId,
        Guid requesterUserId,
        CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryJoinRequests
            .AsNoTracking()
            .FirstOrDefaultAsync(request =>
                request.TerritoryId == territoryId &&
                request.RequesterUserId == requesterUserId &&
                request.Status == TerritoryJoinRequestStatus.Pending,
                cancellationToken);

        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<TerritoryJoinRequestRecipient>> ListRecipientsAsync(
        Guid joinRequestId,
        CancellationToken cancellationToken)
    {
        var records = await _dbContext.TerritoryJoinRequestRecipients
            .AsNoTracking()
            .Where(recipient => recipient.JoinRequestId == joinRequestId)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public Task<bool> IsRecipientAsync(Guid joinRequestId, Guid recipientUserId, CancellationToken cancellationToken)
    {
        return _dbContext.TerritoryJoinRequestRecipients
            .AsNoTracking()
            .AnyAsync(recipient =>
                recipient.JoinRequestId == joinRequestId && recipient.RecipientUserId == recipientUserId,
                cancellationToken);
    }

    public async Task<IReadOnlyList<TerritoryJoinRequest>> ListIncomingAsync(
        Guid recipientUserId,
        TerritoryJoinRequestStatus status,
        CancellationToken cancellationToken)
    {
        var requestIds = await _dbContext.TerritoryJoinRequestRecipients
            .AsNoTracking()
            .Where(recipient => recipient.RecipientUserId == recipientUserId)
            .Select(recipient => recipient.JoinRequestId)
            .ToListAsync(cancellationToken);

        var records = await _dbContext.TerritoryJoinRequests
            .AsNoTracking()
            .Where(request => requestIds.Contains(request.Id) && request.Status == status)
            .OrderByDescending(request => request.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public Task AddAsync(
        TerritoryJoinRequest request,
        IReadOnlyList<TerritoryJoinRequestRecipient> recipients,
        CancellationToken cancellationToken)
    {
        _dbContext.TerritoryJoinRequests.Add(request.ToRecord());
        _dbContext.TerritoryJoinRequestRecipients.AddRange(
            recipients.Select(recipient => recipient.ToRecord()));
        return Task.CompletedTask;
    }

    public async Task UpdateStatusAsync(
        Guid joinRequestId,
        TerritoryJoinRequestStatus status,
        DateTime? decidedAtUtc,
        Guid? decidedByUserId,
        CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryJoinRequests
            .FirstOrDefaultAsync(request => request.Id == joinRequestId, cancellationToken);

        if (record is null)
        {
            return;
        }

        record.Status = status;
        record.DecidedAtUtc = decidedAtUtc;
        record.DecidedByUserId = decidedByUserId;
    }

    public async Task SetRecipientRespondedAtAsync(
        Guid joinRequestId,
        Guid recipientUserId,
        DateTime respondedAtUtc,
        CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryJoinRequestRecipients
            .FirstOrDefaultAsync(recipient =>
                recipient.JoinRequestId == joinRequestId && recipient.RecipientUserId == recipientUserId,
                cancellationToken);

        if (record is null)
        {
            return;
        }

        record.RespondedAtUtc = respondedAtUtc;
    }

    public async Task<IReadOnlyList<TerritoryJoinRequest>> ListIncomingPagedAsync(
        Guid recipientUserId,
        TerritoryJoinRequestStatus status,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var requestIds = await _dbContext.TerritoryJoinRequestRecipients
            .AsNoTracking()
            .Where(recipient => recipient.RecipientUserId == recipientUserId)
            .Select(recipient => recipient.JoinRequestId)
            .ToListAsync(cancellationToken);

        var records = await _dbContext.TerritoryJoinRequests
            .AsNoTracking()
            .Where(request => requestIds.Contains(request.Id) && request.Status == status)
            .OrderByDescending(request => request.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<int> CountIncomingAsync(
        Guid recipientUserId,
        TerritoryJoinRequestStatus status,
        CancellationToken cancellationToken)
    {
        var requestIds = await _dbContext.TerritoryJoinRequestRecipients
            .AsNoTracking()
            .Where(recipient => recipient.RecipientUserId == recipientUserId)
            .Select(recipient => recipient.JoinRequestId)
            .ToListAsync(cancellationToken);

        const int maxInt32 = int.MaxValue;
        var count = await _dbContext.TerritoryJoinRequests
            .AsNoTracking()
            .Where(request => requestIds.Contains(request.Id) && request.Status == status)
            .CountAsync(cancellationToken);
        return count > maxInt32 ? maxInt32 : (int)count;
    }
}
