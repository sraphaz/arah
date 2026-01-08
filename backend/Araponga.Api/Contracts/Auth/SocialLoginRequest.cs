namespace Araponga.Api.Contracts.Auth;

/// <summary>
/// Request payload for social login.
/// </summary>
public sealed record SocialLoginRequest(
    string Provider,
    string ExternalId,
    string DisplayName,
    string Email
);
