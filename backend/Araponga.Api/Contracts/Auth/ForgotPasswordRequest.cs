namespace Araponga.Api.Contracts.Auth;

/// <summary>
/// Request para solicitar recuperação de senha.
/// </summary>
public sealed record ForgotPasswordRequest(string Email);
