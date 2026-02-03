using Araponga.Domain.Social.JoinRequests;

namespace Araponga.Application.Interfaces;

public interface ITerritoryJoinRequestRepository
{
    Task<TerritoryJoinRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<TerritoryJoinRequest?> GetPendingByRequesterAsync(Guid territoryId, Guid requesterUserId, CancellationToken cancellationToken);
    Task<IReadOnlyList<TerritoryJoinRequestRecipient>> ListRecipientsAsync(Guid joinRequestId, CancellationToken cancellationToken);
    Task<bool> IsRecipientAsync(Guid joinRequestId, Guid recipientUserId, CancellationToken cancellationToken);
    Task<IReadOnlyList<TerritoryJoinRequest>> ListIncomingAsync(
        Guid recipientUserId,
        TerritoryJoinRequestStatus status,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Lists incoming join requests with pagination.
    /// </summary>
    Task<IReadOnlyList<TerritoryJoinRequest>> ListIncomingPagedAsync(
        Guid recipientUserId,
        TerritoryJoinRequestStatus status,
        int skip,
        int take,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Counts incoming join requests.
    /// </summary>
    Task<int> CountIncomingAsync(
        Guid recipientUserId,
        TerritoryJoinRequestStatus status,
        CancellationToken cancellationToken);
    Task AddAsync(
        TerritoryJoinRequest request,
        IReadOnlyList<TerritoryJoinRequestRecipient> recipients,
        CancellationToken cancellationToken);
    Task UpdateStatusAsync(
        Guid joinRequestId,
        TerritoryJoinRequestStatus status,
        DateTime? decidedAtUtc,
        Guid? decidedByUserId,
        CancellationToken cancellationToken);
    Task SetRecipientRespondedAtAsync(
        Guid joinRequestId,
        Guid recipientUserId,
        DateTime respondedAtUtc,
        CancellationToken cancellationToken);
}
