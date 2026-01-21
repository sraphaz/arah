using Araponga.Application.Interfaces;
using Araponga.Domain.Social.JoinRequests;
using System.Linq;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryTerritoryJoinRequestRepository : ITerritoryJoinRequestRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryTerritoryJoinRequestRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<TerritoryJoinRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_dataStore.TerritoryJoinRequests.FirstOrDefault(request => request.Id == id));
    }

    public Task<TerritoryJoinRequest?> GetPendingByRequesterAsync(
        Guid territoryId,
        Guid requesterUserId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(_dataStore.TerritoryJoinRequests.FirstOrDefault(request =>
            request.TerritoryId == territoryId &&
            request.RequesterUserId == requesterUserId &&
            request.Status == TerritoryJoinRequestStatus.Pending));
    }

    public Task<IReadOnlyList<TerritoryJoinRequestRecipient>> ListRecipientsAsync(
        Guid joinRequestId,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<TerritoryJoinRequestRecipient> recipients = _dataStore.TerritoryJoinRequestRecipients
            .Where(recipient => recipient.JoinRequestId == joinRequestId)
            .ToList();

        return Task.FromResult(recipients);
    }

    public Task<bool> IsRecipientAsync(
        Guid joinRequestId,
        Guid recipientUserId,
        CancellationToken cancellationToken)
    {
        var isRecipient = _dataStore.TerritoryJoinRequestRecipients.Any(recipient =>
            recipient.JoinRequestId == joinRequestId && recipient.RecipientUserId == recipientUserId);

        return Task.FromResult(isRecipient);
    }

    public Task<IReadOnlyList<TerritoryJoinRequest>> ListIncomingAsync(
        Guid recipientUserId,
        TerritoryJoinRequestStatus status,
        CancellationToken cancellationToken)
    {
        var requestIds = _dataStore.TerritoryJoinRequestRecipients
            .Where(recipient => recipient.RecipientUserId == recipientUserId)
            .Select(recipient => recipient.JoinRequestId)
            .ToHashSet();

        IReadOnlyList<TerritoryJoinRequest> requests = _dataStore.TerritoryJoinRequests
            .Where(request => requestIds.Contains(request.Id) && request.Status == status)
            .OrderByDescending(request => request.CreatedAtUtc)
            .ToList();

        return Task.FromResult(requests);
    }

    public Task AddAsync(
        TerritoryJoinRequest request,
        IReadOnlyList<TerritoryJoinRequestRecipient> recipients,
        CancellationToken cancellationToken)
    {
        _dataStore.TerritoryJoinRequests.Add(request);
        _dataStore.TerritoryJoinRequestRecipients.AddRange(recipients);
        return Task.CompletedTask;
    }

    public Task UpdateStatusAsync(
        Guid joinRequestId,
        TerritoryJoinRequestStatus status,
        DateTime? decidedAtUtc,
        Guid? decidedByUserId,
        CancellationToken cancellationToken)
    {
        var request = _dataStore.TerritoryJoinRequests.FirstOrDefault(r => r.Id == joinRequestId);
        request?.UpdateDecision(status, decidedAtUtc, decidedByUserId);
        return Task.CompletedTask;
    }

    public Task SetRecipientRespondedAtAsync(
        Guid joinRequestId,
        Guid recipientUserId,
        DateTime respondedAtUtc,
        CancellationToken cancellationToken)
    {
        var recipient = _dataStore.TerritoryJoinRequestRecipients.FirstOrDefault(r =>
            r.JoinRequestId == joinRequestId && r.RecipientUserId == recipientUserId);
        recipient?.MarkResponded(respondedAtUtc);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<TerritoryJoinRequest>> ListIncomingPagedAsync(
        Guid recipientUserId,
        TerritoryJoinRequestStatus status,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var requestIds = _dataStore.TerritoryJoinRequestRecipients
            .Where(recipient => recipient.RecipientUserId == recipientUserId)
            .Select(recipient => recipient.JoinRequestId)
            .ToHashSet();

        var requests = _dataStore.TerritoryJoinRequests
            .Where(request => requestIds.Contains(request.Id) && request.Status == status)
            .OrderByDescending(request => request.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToList();

        return Task.FromResult<IReadOnlyList<TerritoryJoinRequest>>(requests);
    }

    public Task<int> CountIncomingAsync(
        Guid recipientUserId,
        TerritoryJoinRequestStatus status,
        CancellationToken cancellationToken)
    {
        var requestIds = _dataStore.TerritoryJoinRequestRecipients
            .Where(recipient => recipient.RecipientUserId == recipientUserId)
            .Select(recipient => recipient.JoinRequestId)
            .ToHashSet();

        const int maxInt32 = int.MaxValue;
        var count = _dataStore.TerritoryJoinRequests
            .Count(request => requestIds.Contains(request.Id) && request.Status == status);

        return Task.FromResult(count > maxInt32 ? maxInt32 : count);
    }
}
