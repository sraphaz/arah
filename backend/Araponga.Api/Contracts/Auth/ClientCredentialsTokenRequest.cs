namespace Araponga.Api.Contracts.Auth;

/// <summary>
/// Request para obter token de sistema (workers) via client credentials.
/// </summary>
public sealed record ClientCredentialsTokenRequest
{
    public string GrantType { get; init; } = "client_credentials";
    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
}
