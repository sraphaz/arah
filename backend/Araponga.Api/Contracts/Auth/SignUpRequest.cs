namespace Araponga.Api.Contracts.Auth;

/// <summary>
/// Request para cadastro com e-mail e senha.
/// </summary>
/// <param name="Email">E-mail do usuário.</param>
/// <param name="DisplayName">Nome de exibição.</param>
/// <param name="Password">Senha (mín. 6 caracteres).</param>
public sealed record SignUpRequest(string Email, string DisplayName, string Password);
