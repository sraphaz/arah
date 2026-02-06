namespace Araponga.Api.Contracts.Auth;

/// <summary>
/// Resposta de check-email: indica se o e-mail já está cadastrado.
/// </summary>
public sealed record CheckEmailResponse(bool Exists);
