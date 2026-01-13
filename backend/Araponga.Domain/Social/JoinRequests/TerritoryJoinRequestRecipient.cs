namespace Araponga.Domain.Social.JoinRequests;

public sealed class TerritoryJoinRequestRecipient
{
    public TerritoryJoinRequestRecipient(
        Guid joinRequestId,
        Guid recipientUserId,
        DateTime createdAtUtc,
        DateTime? respondedAtUtc)
    {
        if (joinRequestId == Guid.Empty)
        {
            throw new ArgumentException("Join request ID is required.", nameof(joinRequestId));
        }

        if (recipientUserId == Guid.Empty)
        {
            throw new ArgumentException("Recipient user ID is required.", nameof(recipientUserId));
        }

        JoinRequestId = joinRequestId;
        RecipientUserId = recipientUserId;
        CreatedAtUtc = createdAtUtc;
        RespondedAtUtc = respondedAtUtc;
    }

    public Guid JoinRequestId { get; }
    public Guid RecipientUserId { get; }
    public DateTime CreatedAtUtc { get; }
    public DateTime? RespondedAtUtc { get; private set; }

    public void MarkResponded(DateTime respondedAtUtc)
    {
        RespondedAtUtc = respondedAtUtc;
    }
}
