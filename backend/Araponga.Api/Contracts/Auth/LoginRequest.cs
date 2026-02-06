namespace Araponga.Api.Contracts.Auth;

/// <summary>
/// Request para login com e-mail e senha.
/// </summary>
/// <param name="Email">E-mail do usuário.</param>
/// <param name="Password">Senha.</param>
public sealed record LoginRequest(string Email, string Password);
