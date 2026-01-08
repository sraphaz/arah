namespace Araponga.Domain.Users;

public sealed class UserTerritory
{
    public UserTerritory(
        Guid id,
        Guid userId,
        Guid territoryId,
        MembershipStatus status,
        DateTime createdAtUtc)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID is required.", nameof(userId));
        }

        if (territoryId == Guid.Empty)
        {
            throw new ArgumentException("Territory ID is required.", nameof(territoryId));
        }

        Id = id;
        UserId = userId;
        TerritoryId = territoryId;
        Status = status;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid TerritoryId { get; }
    public MembershipStatus Status { get; }
    public DateTime CreatedAtUtc { get; }
}
