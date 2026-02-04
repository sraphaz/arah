namespace Araponga.Modules.Moderation.Domain.Moderation;

public sealed class UserBlock
{
    public UserBlock(Guid blockerUserId, Guid blockedUserId, DateTime createdAtUtc)
    {
        if (blockerUserId == Guid.Empty)
        {
            throw new ArgumentException("Blocker user ID is required.", nameof(blockerUserId));
        }

        if (blockedUserId == Guid.Empty)
        {
            throw new ArgumentException("Blocked user ID is required.", nameof(blockedUserId));
        }

        if (blockerUserId == blockedUserId)
        {
            throw new ArgumentException("A user cannot block themselves.", nameof(blockedUserId));
        }

        BlockerUserId = blockerUserId;
        BlockedUserId = blockedUserId;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid BlockerUserId { get; }
    public Guid BlockedUserId { get; }
    public DateTime CreatedAtUtc { get; }
}
