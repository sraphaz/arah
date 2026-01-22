namespace Araponga.Api.Contracts.Policies;

/// <summary>
/// Request para aceitar política de privacidade.
/// </summary>
public sealed record AcceptPrivacyPolicyRequest
{
    /// <summary>
    /// ID da política a ser aceita.
    /// </summary>
    public Guid PolicyId { get; init; }

    /// <summary>
    /// Endereço IP do usuário (opcional, para auditoria).
    /// </summary>
    public string? IpAddress { get; init; }

    /// <summary>
    /// User Agent do navegador/aplicativo (opcional, para auditoria).
    /// </summary>
    public string? UserAgent { get; init; }
}
