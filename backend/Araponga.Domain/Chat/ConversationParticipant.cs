namespace Araponga.Domain.Chat;

public sealed class ConversationParticipant
{
    public ConversationParticipant(
        Guid conversationId,
        Guid userId,
        ConversationParticipantRole role,
        DateTime joinedAtUtc,
        DateTime? leftAtUtc,
        DateTime? mutedUntilUtc,
        Guid? lastReadMessageId,
        DateTime? lastReadAtUtc)
    {
        if (conversationId == Guid.Empty)
        {
            throw new ArgumentException("ConversationId is required.", nameof(conversationId));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId is required.", nameof(userId));
        }

        if (leftAtUtc is not null && leftAtUtc < joinedAtUtc)
        {
            throw new ArgumentException("LeftAtUtc cannot be earlier than JoinedAtUtc.", nameof(leftAtUtc));
        }

        ConversationId = conversationId;
        UserId = userId;
        Role = role;
        JoinedAtUtc = joinedAtUtc;
        LeftAtUtc = leftAtUtc;
        MutedUntilUtc = mutedUntilUtc;
        LastReadMessageId = lastReadMessageId;
        LastReadAtUtc = lastReadAtUtc;
    }

    public Guid ConversationId { get; }
    public Guid UserId { get; }
    public ConversationParticipantRole Role { get; private set; }
    public DateTime JoinedAtUtc { get; }
    public DateTime? LeftAtUtc { get; private set; }
    public DateTime? MutedUntilUtc { get; private set; }

    public Guid? LastReadMessageId { get; private set; }
    public DateTime? LastReadAtUtc { get; private set; }

    public bool IsActive => LeftAtUtc is null;

    public void PromoteToOwner()
    {
        Role = ConversationParticipantRole.Owner;
    }

    public void DemoteToMember()
    {
        Role = ConversationParticipantRole.Member;
    }

    public void Leave(DateTime leftAtUtc)
    {
        if (LeftAtUtc is not null)
        {
            return;
        }

        if (leftAtUtc < JoinedAtUtc)
        {
            throw new ArgumentException("LeftAtUtc cannot be earlier than JoinedAtUtc.", nameof(leftAtUtc));
        }

        LeftAtUtc = leftAtUtc;
    }

    public void MuteUntil(DateTime? mutedUntilUtc)
    {
        MutedUntilUtc = mutedUntilUtc;
    }

    public void MarkRead(Guid messageId, DateTime readAtUtc)
    {
        if (messageId == Guid.Empty)
        {
            throw new ArgumentException("MessageId is required.", nameof(messageId));
        }

        LastReadMessageId = messageId;
        LastReadAtUtc = readAtUtc;
    }
}

