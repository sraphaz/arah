namespace Araponga.Domain.Social.JoinRequests;

public sealed class TerritoryJoinRequest
{
    public TerritoryJoinRequest(
        Guid id,
        Guid territoryId,
        Guid requesterUserId,
        string? message,
        TerritoryJoinRequestStatus status,
        DateTime createdAtUtc,
        DateTime? expiresAtUtc,
        DateTime? decidedAtUtc,
        Guid? decidedByUserId)
    {
        if (territoryId == Guid.Empty)
        {
            throw new ArgumentException("Territory ID is required.", nameof(territoryId));
        }

        if (requesterUserId == Guid.Empty)
        {
            throw new ArgumentException("Requester user ID is required.", nameof(requesterUserId));
        }

        Id = id;
        TerritoryId = territoryId;
        RequesterUserId = requesterUserId;
        Message = string.IsNullOrWhiteSpace(message) ? null : message.Trim();
        Status = status;
        CreatedAtUtc = createdAtUtc;
        ExpiresAtUtc = expiresAtUtc;
        DecidedAtUtc = decidedAtUtc;
        DecidedByUserId = decidedByUserId;
    }

    public Guid Id { get; }
    public Guid TerritoryId { get; }
    public Guid RequesterUserId { get; }
    public string? Message { get; }
    public TerritoryJoinRequestStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; }
    public DateTime? ExpiresAtUtc { get; }
    public DateTime? DecidedAtUtc { get; private set; }
    public Guid? DecidedByUserId { get; private set; }

    public void UpdateDecision(
        TerritoryJoinRequestStatus status,
        DateTime? decidedAtUtc,
        Guid? decidedByUserId)
    {
        Status = status;
        DecidedAtUtc = decidedAtUtc;
        DecidedByUserId = decidedByUserId;
    }
}
