namespace Araponga.Api.Contracts.Auth;

/// <summary>
/// Request para verificar se um e-mail já está cadastrado (fluxo login/criar conta).
/// </summary>
public sealed record CheckEmailRequest(string Email);
