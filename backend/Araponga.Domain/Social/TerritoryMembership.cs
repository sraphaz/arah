namespace Araponga.Domain.Social;

public sealed class TerritoryMembership
{
    public TerritoryMembership(
        Guid id,
        Guid userId,
        Guid territoryId,
        MembershipRole role,
        VerificationStatus verificationStatus,
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
        Role = role;
        VerificationStatus = verificationStatus;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid TerritoryId { get; }
    public MembershipRole Role { get; private set; }
    public VerificationStatus VerificationStatus { get; private set; }
    public DateTime CreatedAtUtc { get; }

    public void UpdateVerificationStatus(VerificationStatus status)
    {
        VerificationStatus = status;
    }

    public void UpdateRole(MembershipRole role)
    {
        Role = role;
    }
}
