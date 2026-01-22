namespace Araponga.Api.Contracts.Policies;

/// <summary>
/// Request para aceitar termos de serviço.
/// </summary>
public sealed record AcceptTermsRequest
{
    /// <summary>
    /// ID dos termos a serem aceitos.
    /// </summary>
    public Guid TermsId { get; init; }

    /// <summary>
    /// Endereço IP do usuário (opcional, para auditoria).
    /// </summary>
    public string? IpAddress { get; init; }

    /// <summary>
    /// User Agent do navegador/aplicativo (opcional, para auditoria).
    /// </summary>
    public string? UserAgent { get; init; }
}
