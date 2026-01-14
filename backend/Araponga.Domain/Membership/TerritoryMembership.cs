namespace Araponga.Domain.Membership;

/// <summary>
/// Representa o vínculo entre um User e um Territory.
/// Define o papel (Role) e o status de verificação de residência (ResidencyVerification).
/// </summary>
public sealed class TerritoryMembership
{
    /// <summary>
    /// Inicializa uma nova instância de TerritoryMembership.
    /// </summary>
    /// <param name="id">Identificador único do membership</param>
    /// <param name="userId">Identificador do usuário</param>
    /// <param name="territoryId">Identificador do território</param>
    /// <param name="role">Papel do membro no território (Visitor ou Resident)</param>
    /// <param name="residencyVerification">Status de verificação de residência (acumulável)</param>
    /// <param name="lastGeoVerifiedAtUtc">Data da última verificação por geolocalização</param>
    /// <param name="lastDocumentVerifiedAtUtc">Data da última verificação por documento</param>
    /// <param name="createdAtUtc">Data de criação do membership</param>
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

    /// <summary>
    /// Identificador único do membership.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Identificador do usuário associado ao membership.
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Identificador do território associado ao membership.
    /// </summary>
    public Guid TerritoryId { get; }

    /// <summary>
    /// Papel do membro no território (Visitor ou Resident).
    /// </summary>
    public MembershipRole Role { get; private set; }

    /// <summary>
    /// Status de verificação de residência (acumulável via Flags).
    /// </summary>
    public ResidencyVerification ResidencyVerification { get; private set; }

    /// <summary>
    /// Data da última verificação por geolocalização.
    /// </summary>
    public DateTime? LastGeoVerifiedAtUtc { get; private set; }

    /// <summary>
    /// Data da última verificação por comprovante documental.
    /// </summary>
    public DateTime? LastDocumentVerifiedAtUtc { get; private set; }

    /// <summary>
    /// Data de criação do membership.
    /// </summary>
    public DateTime CreatedAtUtc { get; }

    /// <summary>
    /// Atualiza o papel do membro no território.
    /// </summary>
    /// <param name="role">Novo papel (Visitor ou Resident)</param>
    public void UpdateRole(MembershipRole role)
    {
        Role = role;
    }

    /// <summary>
    /// Atualiza o status de verificação de residência.
    /// </summary>
    /// <param name="verification">Novo status de verificação</param>
    public void UpdateResidencyVerification(ResidencyVerification verification)
    {
        ResidencyVerification = verification;
    }

    /// <summary>
    /// Adiciona verificação por geolocalização (acumulável).
    /// </summary>
    /// <param name="verifiedAtUtc">Data da verificação</param>
    public void AddGeoVerification(DateTime verifiedAtUtc)
    {
        ResidencyVerification |= ResidencyVerification.GeoVerified;
        LastGeoVerifiedAtUtc = verifiedAtUtc;
    }

    /// <summary>
    /// Adiciona verificação por comprovante documental (acumulável).
    /// </summary>
    /// <param name="verifiedAtUtc">Data da verificação</param>
    public void AddDocumentVerification(DateTime verifiedAtUtc)
    {
        ResidencyVerification |= ResidencyVerification.DocumentVerified;
        LastDocumentVerifiedAtUtc = verifiedAtUtc;
    }

    /// <summary>
    /// Remove verificação por geolocalização.
    /// </summary>
    public void RemoveGeoVerification()
    {
        ResidencyVerification &= ~ResidencyVerification.GeoVerified;
        LastGeoVerifiedAtUtc = null;
    }

    /// <summary>
    /// Remove verificação por comprovante documental.
    /// </summary>
    public void RemoveDocumentVerification()
    {
        ResidencyVerification &= ~ResidencyVerification.DocumentVerified;
        LastDocumentVerifiedAtUtc = null;
    }

    /// <summary>
    /// Verifica se tem qualquer tipo de verificação.
    /// </summary>
    /// <returns>True se possui pelo menos uma verificação</returns>
    public bool HasAnyVerification() => ResidencyVerification != ResidencyVerification.None;

    /// <summary>
    /// Verifica se tem verificação por geolocalização.
    /// </summary>
    /// <returns>True se possui verificação geográfica</returns>
    public bool IsGeoVerified() => (ResidencyVerification & ResidencyVerification.GeoVerified) != 0;

    /// <summary>
    /// Verifica se tem verificação por comprovante documental.
    /// </summary>
    /// <returns>True se possui verificação documental</returns>
    public bool IsDocumentVerified() => (ResidencyVerification & ResidencyVerification.DocumentVerified) != 0;

    // Métodos de compatibilidade (manter temporariamente para transição suave)
    /// <summary>
    /// [Obsoleto] Use AddGeoVerification ao invés disso.
    /// </summary>
    [Obsolete("Use AddGeoVerification instead")]
    public void UpdateGeoVerification(DateTime verifiedAtUtc)
    {
        AddGeoVerification(verifiedAtUtc);
    }

    /// <summary>
    /// [Obsoleto] Use AddDocumentVerification ao invés disso.
    /// </summary>
    [Obsolete("Use AddDocumentVerification instead")]
    public void UpdateDocumentVerification(DateTime verifiedAtUtc)
    {
        AddDocumentVerification(verifiedAtUtc);
    }
}
