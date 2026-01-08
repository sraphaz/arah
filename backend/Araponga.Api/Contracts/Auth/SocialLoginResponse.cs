namespace Araponga.Api.Contracts.Auth;

/// <summary>
/// Response payload for social login.
/// </summary>
public sealed record SocialLoginResponse(
    /// <summary>
    /// Authenticated user data.
    /// </summary>
    UserResponse User,
    /// <summary>
    /// Authentication token.
    /// </summary>
    string Token
);
