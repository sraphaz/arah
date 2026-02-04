namespace Araponga.Api.Contracts.Auth;

/// <summary>
/// Request para renovar o access token usando o refresh token (padrão OAuth: rotação de tokens).
/// </summary>
public sealed record RefreshTokenRequest
{
    public string RefreshToken { get; init; } = string.Empty;
}
