namespace Araponga.Domain.Membership;

/// <summary>
/// Tipos de capacidades operacionais que podem ser atribuídas a um Membership.
/// Capacidades são territoriais e empilháveis.
/// </summary>
public enum MembershipCapabilityType
{
    /// <summary>
    /// Capacidade de curadoria - pode validar entidades e conteúdo do território.
    /// </summary>
    Curator = 1,

    /// <summary>
    /// Capacidade de moderação - pode moderar conteúdo e reports do território.
    /// </summary>
    Moderator = 2
}
