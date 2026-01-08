namespace Araponga.Api.Contracts.Auth;

/// <summary>
/// Request payload for social login.
/// </summary>
public sealed record SocialLoginRequest(
    /// <summary>
    /// Social provider identifier.
    /// </summary>
    string Provider,
    /// <summary>
    /// External provider user identifier.
    /// </summary>
    string ExternalId,
    /// <summary>
    /// Display name to register or update.
    /// </summary>
    string DisplayName,
    /// <summary>
    /// User email address.
    /// </summary>
    string Email
);
