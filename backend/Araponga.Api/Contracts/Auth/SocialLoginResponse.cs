namespace Araponga.Api.Contracts.Auth;

/// <summary>
/// Response payload for social login.
/// </summary>
public sealed record SocialLoginResponse(
    UserResponse User,
    string Token
);
