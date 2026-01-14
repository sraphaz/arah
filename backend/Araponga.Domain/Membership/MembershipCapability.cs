namespace Araponga.Domain.Membership;

/// <summary>
/// Representa uma capacidade operacional atribuída a um Membership.
/// Capacidades são territoriais, empilháveis e não alteram o papel social do membro.
/// </summary>
public sealed class MembershipCapability
{
    private const int MaxReasonLength = 500;

    /// <summary>
    /// Inicializa uma nova instância de MembershipCapability.
    /// </summary>
    /// <param name="id">Identificador único da capability</param>
    /// <param name="membershipId">Identificador do membership ao qual a capability pertence</param>
    /// <param name="capabilityType">Tipo de capability (Curator, Moderator, etc.)</param>
    /// <param name="grantedAtUtc">Data em que a capability foi concedida</param>
    /// <param name="grantedByUserId">Identificador do usuário que concedeu a capability</param>
    /// <param name="grantedByMembershipId">Identificador do membership que concedeu a capability</param>
    /// <param name="reason">Motivo da concessão da capability</param>
    public MembershipCapability(
        Guid id,
        Guid membershipId,
        MembershipCapabilityType capabilityType,
        DateTime grantedAtUtc,
        Guid? grantedByUserId,
        Guid? grantedByMembershipId,
        string? reason)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID is required.", nameof(id));
        }

        if (membershipId == Guid.Empty)
        {
            throw new ArgumentException("Membership ID is required.", nameof(membershipId));
        }

        if (reason is not null && reason.Length > MaxReasonLength)
        {
            throw new ArgumentException($"Reason must not exceed {MaxReasonLength} characters.", nameof(reason));
        }

        Id = id;
        MembershipId = membershipId;
        CapabilityType = capabilityType;
        GrantedAtUtc = grantedAtUtc;
        GrantedByUserId = grantedByUserId;
        GrantedByMembershipId = grantedByMembershipId;
        Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
    }

    /// <summary>
    /// Identificador único da capability.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Identificador do membership ao qual a capability pertence.
    /// </summary>
    public Guid MembershipId { get; }

    /// <summary>
    /// Tipo de capability (Curator, Moderator, etc.).
    /// </summary>
    public MembershipCapabilityType CapabilityType { get; }

    /// <summary>
    /// Data em que a capability foi concedida.
    /// </summary>
    public DateTime GrantedAtUtc { get; }

    /// <summary>
    /// Data em que a capability foi revogada (null se ainda está ativa).
    /// </summary>
    public DateTime? RevokedAtUtc { get; private set; }

    /// <summary>
    /// Identificador do usuário que concedeu a capability.
    /// </summary>
    public Guid? GrantedByUserId { get; }

    /// <summary>
    /// Identificador do membership que concedeu a capability.
    /// </summary>
    public Guid? GrantedByMembershipId { get; }

    /// <summary>
    /// Motivo da concessão da capability.
    /// </summary>
    public string? Reason { get; }

    /// <summary>
    /// Revoga a capability.
    /// </summary>
    /// <param name="revokedAtUtc">Data da revogação</param>
    /// <exception cref="InvalidOperationException">Lançada se a capability já foi revogada</exception>
    public void Revoke(DateTime revokedAtUtc)
    {
        if (RevokedAtUtc.HasValue)
        {
            throw new InvalidOperationException("Capability is already revoked.");
        }

        RevokedAtUtc = revokedAtUtc;
    }

    /// <summary>
    /// Verifica se a capability está ativa.
    /// </summary>
    /// <returns>True se a capability não foi revogada</returns>
    public bool IsActive() => !RevokedAtUtc.HasValue;
}
