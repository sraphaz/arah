using Araponga.Application.Interfaces;
using Araponga.Domain.Social.JoinRequests;

namespace Araponga.Infrastructure.Shared.InMemory;

/// <summary>Implementação InMemory de ITerritoryJoinRequestRepository usando InMemorySharedStore.</summary>
public sealed class InMemoryTerritoryJoinRequestRepository : ITerritoryJoinRequestRepository
{
    private readonly InMemorySharedStore _store;

    public InMemoryTerritoryJoinRequestRepository(InMemorySharedStore store) => _store = store;

    public Task<TerritoryJoinRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => Task.FromResult(_store.TerritoryJoinRequests.FirstOrDefault(r => r.Id == id));

    public Task<TerritoryJoinRequest?> GetPendingByRequesterAsync(Guid territoryId, Guid requesterUserId, CancellationToken cancellationToken)
        => Task.FromResult(_store.TerritoryJoinRequests.FirstOrDefault(r =>
            r.TerritoryId == territoryId && r.RequesterUserId == requesterUserId && r.Status == TerritoryJoinRequestStatus.Pending));

    public Task<IReadOnlyList<TerritoryJoinRequestRecipient>> ListRecipientsAsync(Guid joinRequestId, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<TerritoryJoinRequestRecipient>>(
            _store.TerritoryJoinRequestRecipients.Where(r => r.JoinRequestId == joinRequestId).ToList());

    public Task<bool> IsRecipientAsync(Guid joinRequestId, Guid recipientUserId, CancellationToken cancellationToken)
        => Task.FromResult(_store.TerritoryJoinRequestRecipients.Any(r => r.JoinRequestId == joinRequestId && r.RecipientUserId == recipientUserId));

    public Task<IReadOnlyList<TerritoryJoinRequest>> ListIncomingAsync(Guid recipientUserId, TerritoryJoinRequestStatus status, CancellationToken cancellationToken)
    {
        var ids = _store.TerritoryJoinRequestRecipients.Where(r => r.RecipientUserId == recipientUserId).Select(r => r.JoinRequestId).ToHashSet();
        var list = _store.TerritoryJoinRequests.Where(r => ids.Contains(r.Id) && r.Status == status).OrderByDescending(r => r.CreatedAtUtc).ToList();
        return Task.FromResult<IReadOnlyList<TerritoryJoinRequest>>(list);
    }

    public Task AddAsync(TerritoryJoinRequest request, IReadOnlyList<TerritoryJoinRequestRecipient> recipients, CancellationToken cancellationToken)
    {
        _store.TerritoryJoinRequests.Add(request);
        _store.TerritoryJoinRequestRecipients.AddRange(recipients);
        return Task.CompletedTask;
    }

    public Task UpdateStatusAsync(Guid joinRequestId, TerritoryJoinRequestStatus status, DateTime? decidedAtUtc, Guid? decidedByUserId, CancellationToken cancellationToken)
    {
        var r = _store.TerritoryJoinRequests.FirstOrDefault(x => x.Id == joinRequestId);
        r?.UpdateDecision(status, decidedAtUtc, decidedByUserId);
        return Task.CompletedTask;
    }

    public Task SetRecipientRespondedAtAsync(Guid joinRequestId, Guid recipientUserId, DateTime respondedAtUtc, CancellationToken cancellationToken)
    {
        var r = _store.TerritoryJoinRequestRecipients.FirstOrDefault(x => x.JoinRequestId == joinRequestId && x.RecipientUserId == recipientUserId);
        r?.MarkResponded(respondedAtUtc);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<TerritoryJoinRequest>> ListIncomingPagedAsync(Guid recipientUserId, TerritoryJoinRequestStatus status, int skip, int take, CancellationToken cancellationToken)
    {
        var ids = _store.TerritoryJoinRequestRecipients.Where(r => r.RecipientUserId == recipientUserId).Select(r => r.JoinRequestId).ToHashSet();
        var list = _store.TerritoryJoinRequests.Where(r => ids.Contains(r.Id) && r.Status == status).OrderByDescending(r => r.CreatedAtUtc).Skip(skip).Take(take).ToList();
        return Task.FromResult<IReadOnlyList<TerritoryJoinRequest>>(list);
    }

    public Task<int> CountIncomingAsync(Guid recipientUserId, TerritoryJoinRequestStatus status, CancellationToken cancellationToken)
    {
        var ids = _store.TerritoryJoinRequestRecipients.Where(r => r.RecipientUserId == recipientUserId).Select(r => r.JoinRequestId).ToHashSet();
        var c = _store.TerritoryJoinRequests.Count(r => ids.Contains(r.Id) && r.Status == status);
        return Task.FromResult(c > int.MaxValue ? int.MaxValue : (int)c);
    }
}
