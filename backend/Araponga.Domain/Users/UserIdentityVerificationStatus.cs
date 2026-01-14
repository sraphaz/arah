namespace Araponga.Domain.Users;

/// <summary>
/// Status de verificação de identidade global do usuário.
/// Usado para operações que requerem verificação de identidade (ex: marketplace).
/// </summary>
public enum UserIdentityVerificationStatus
{
    /// <summary>
    /// Identidade não verificada.
    /// </summary>
    Unverified = 1,

    /// <summary>
    /// Verificação pendente (em análise).
    /// </summary>
    Pending = 2,

    /// <summary>
    /// Identidade verificada e aprovada.
    /// </summary>
    Verified = 3,

    /// <summary>
    /// Verificação rejeitada.
    /// </summary>
    Rejected = 4
}
