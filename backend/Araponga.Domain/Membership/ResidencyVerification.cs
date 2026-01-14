namespace Araponga.Domain.Membership;

/// <summary>
/// Define os tipos de verificação do vínculo residencial de um Membership.
/// Usa Flags para permitir acumulação de verificações (ex: GeoVerified | DocumentVerified).
/// Separado do MembershipRole para eliminar ambiguidades entre papel e verificação.
/// </summary>
[Flags]
public enum ResidencyVerification
{
    /// <summary>
    /// Sem verificação de residência.
    /// </summary>
    None = 0,

    /// <summary>
    /// Verificado por geolocalização.
    /// </summary>
    GeoVerified = 1,

    /// <summary>
    /// Verificado por comprovante documental.
    /// </summary>
    DocumentVerified = 2
}
