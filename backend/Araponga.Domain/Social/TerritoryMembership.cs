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
        : this(id, userId, territoryId, role, verificationStatus, null, null, null, createdAtUtc)
    {
    }

    public TerritoryMembership(
        Guid id,
        Guid userId,
        Guid territoryId,
        MembershipRole role,
        ResidencyVerification residencyVerification,
        DateTime? lastGeoVerifiedAtUtc,
        DateTime? lastDocumentVerifiedAtUtc,
        DateTime createdAtUtc)
        : this(id, userId, territoryId, role, null, residencyVerification, lastGeoVerifiedAtUtc, lastDocumentVerifiedAtUtc, createdAtUtc)
    {
    }

    private TerritoryMembership(
        Guid id,
        Guid userId,
        Guid territoryId,
        MembershipRole role,
        VerificationStatus? verificationStatus,
        ResidencyVerification? residencyVerification,
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
        CreatedAtUtc = createdAtUtc;

        // Para compatibilidade durante migração: se verificationStatus foi fornecido, converter para residencyVerification
        if (verificationStatus.HasValue)
        {
            VerificationStatus = verificationStatus.Value;
            ResidencyVerification = ConvertVerificationStatusToResidencyVerification(verificationStatus.Value, role);
        }
        else if (residencyVerification.HasValue)
        {
            ResidencyVerification = residencyVerification.Value;
            // Para compatibilidade: converter ResidencyVerification de volta para VerificationStatus obsoleto
            VerificationStatus = ConvertResidencyVerificationToVerificationStatus(residencyVerification.Value, role);
        }
        else
        {
            ResidencyVerification = Araponga.Domain.Social.ResidencyVerification.Unverified;
            VerificationStatus = VerificationStatus.Pending; // Default para compatibilidade
        }

        LastGeoVerifiedAtUtc = lastGeoVerifiedAtUtc;
        LastDocumentVerifiedAtUtc = lastDocumentVerifiedAtUtc;
    }

    public Guid Id { get; }
    public Guid UserId { get; }
    public Guid TerritoryId { get; }
    public MembershipRole Role { get; private set; }
    
    [Obsolete("Use ResidencyVerification instead. This property will be removed in a future version.")]
    public VerificationStatus VerificationStatus { get; private set; }
    
    public ResidencyVerification ResidencyVerification { get; private set; }
    public DateTime? LastGeoVerifiedAtUtc { get; private set; }
    public DateTime? LastDocumentVerifiedAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; }

    [Obsolete("Use UpdateResidencyVerification methods instead.")]
    public void UpdateVerificationStatus(VerificationStatus status)
    {
        VerificationStatus = status;
        ResidencyVerification = ConvertVerificationStatusToResidencyVerification(status, Role);
    }

    public void UpdateRole(MembershipRole role)
    {
        Role = role;
        // Atualizar VerificationStatus obsoleto para compatibilidade quando role muda
        VerificationStatus = ConvertResidencyVerificationToVerificationStatus(ResidencyVerification, role);
    }

    public void UpdateResidencyVerification(ResidencyVerification verification)
    {
        ResidencyVerification = verification;
        // Atualizar VerificationStatus obsoleto para compatibilidade
        VerificationStatus = ConvertResidencyVerificationToVerificationStatus(verification, Role);
    }

    public void UpdateGeoVerification(DateTime verifiedAtUtc)
    {
        ResidencyVerification = Araponga.Domain.Social.ResidencyVerification.GeoVerified;
        LastGeoVerifiedAtUtc = verifiedAtUtc;
        // Atualizar VerificationStatus obsoleto para compatibilidade
        VerificationStatus = VerificationStatus.Validated;
    }

    public void UpdateDocumentVerification(DateTime verifiedAtUtc)
    {
        ResidencyVerification = Araponga.Domain.Social.ResidencyVerification.DocumentVerified;
        LastDocumentVerifiedAtUtc = verifiedAtUtc;
        // Atualizar VerificationStatus obsoleto para compatibilidade
        VerificationStatus = VerificationStatus.Validated; // DocumentVerified é considerado Validated
    }

    private static ResidencyVerification ConvertVerificationStatusToResidencyVerification(
        VerificationStatus status,
        MembershipRole role)
    {
        if (role == MembershipRole.Visitor)
        {
            return Araponga.Domain.Social.ResidencyVerification.Unverified;
        }

        return status switch
        {
            VerificationStatus.Pending => Araponga.Domain.Social.ResidencyVerification.Unverified,
            VerificationStatus.Validated => Araponga.Domain.Social.ResidencyVerification.GeoVerified, // Assumir geo como padrão
            VerificationStatus.Rejected => Araponga.Domain.Social.ResidencyVerification.Unverified,
            _ => Araponga.Domain.Social.ResidencyVerification.Unverified
        };
    }

    private static VerificationStatus ConvertResidencyVerificationToVerificationStatus(
        ResidencyVerification verification,
        MembershipRole role)
    {
        if (role == MembershipRole.Visitor)
        {
            return VerificationStatus.Pending; // Visitor sempre Pending
        }

        return verification switch
        {
            Araponga.Domain.Social.ResidencyVerification.Unverified => VerificationStatus.Pending,
            Araponga.Domain.Social.ResidencyVerification.GeoVerified => VerificationStatus.Validated,
            Araponga.Domain.Social.ResidencyVerification.DocumentVerified => VerificationStatus.Validated,
            _ => VerificationStatus.Pending
        };
    }
}
