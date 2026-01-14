namespace Araponga.Domain.Membership;

/// <summary>
/// Define os papéis que um usuário pode ter em um território.
/// </summary>
public enum MembershipRole
{
    /// <summary>
    /// Visitante - pode visualizar conteúdo público do território.
    /// </summary>
    Visitor = 1,

    /// <summary>
    /// Morador - pode criar conteúdo e participar ativamente do território.
    /// </summary>
    Resident = 2
}
