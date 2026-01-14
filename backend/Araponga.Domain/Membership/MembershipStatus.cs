namespace Araponga.Domain.Membership;

/// <summary>
/// Status de um membership durante o processo de validação.
/// </summary>
public enum MembershipStatus
{
    /// <summary>
    /// Pendente de validação.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Validado e ativo.
    /// </summary>
    Validated = 2,

    /// <summary>
    /// Rejeitado.
    /// </summary>
    Rejected = 3
}
