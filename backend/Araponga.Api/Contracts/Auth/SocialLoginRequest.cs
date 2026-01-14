namespace Araponga.Api.Contracts.Auth;

/// <summary>
/// Request payload for social login.
/// </summary>
/// <param name="AuthProvider">Provedor de autenticação social (ex: "google", "apple", "facebook").</param>
/// <param name="ExternalId">ID único do usuário no provedor de autenticação.</param>
/// <param name="DisplayName">Nome de exibição do usuário.</param>
/// <param name="Cpf">CPF brasileiro (opcional, mutuamente exclusivo com ForeignDocument).</param>
/// <param name="ForeignDocument">Documento de identificação estrangeiro (opcional, mutuamente exclusivo com Cpf).</param>
/// <param name="PhoneNumber">Número de telefone (opcional).</param>
/// <param name="Address">Endereço físico (opcional).</param>
/// <param name="Email">Endereço de e-mail (opcional).</param>
public sealed record SocialLoginRequest(
    string AuthProvider,
    string ExternalId,
    string DisplayName,
    string? Cpf,
    string? ForeignDocument,
    string? PhoneNumber,
    string? Address,
    string? Email
);
