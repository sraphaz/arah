namespace Araponga.Api.Contracts.Auth;

/// <summary>
/// Resposta do endpoint de token para client credentials (workers).
/// </summary>
public sealed record ClientCredentialsTokenResponse(
    string AccessToken,
    string TokenType,
    int ExpiresInSeconds
);
