namespace Araponga.Domain.Social;

public sealed class TerritoryMembership
{
    public TerritoryMembership(
        Guid id,
        Guid userId,
        Guid territoryId,
        MembershipRole role,
        ResidencyVerification residencyVerification,
        DateTime? lastGeoVerifiedAtUtc,
        DateTime? lastDocumentVerifiedAtUtc,
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
        ResidencyVerification = residencyVerification;
        LastGeoVerifiedAtUtc = lastGeoVerifiedAtUtc;
        LastDocumentVerifiedAtUtc = lastDocumentVerifiedAtUtc;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid TerritoryId { get; }
    public MembershipRole Role { get; private set; }
    public ResidencyVerification ResidencyVerification { get; private set; }
    public DateTime? LastGeoVerifiedAtUtc { get; private set; }
    public DateTime? LastDocumentVerifiedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; }

    public void UpdateRole(MembershipRole role)
    {
        Role = role;
    }

    public void UpdateResidencyVerification(ResidencyVerification verification)
    {
        ResidencyVerification = verification;
    }

    public void UpdateGeoVerification(DateTime verifiedAtUtc)
    {
        ResidencyVerification = ResidencyVerification.GeoVerified;
        LastGeoVerifiedAtUtc = verifiedAtUtc;
    }

    public void UpdateDocumentVerification(DateTime verifiedAtUtc)
    {
        ResidencyVerification = ResidencyVerification.DocumentVerified;
        LastDocumentVerifiedAtUtc = verifiedAtUtc;
    }
}