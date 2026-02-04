namespace Araponga.Api.Contracts.Auth;

/// <summary>
/// Response payload for social login and refresh. BFF/frontend guarda o access token no estado e usa refresh_token para renovar quando expirar.
/// </summary>
public sealed record SocialLoginResponse(
    UserResponse User,
    string Token,
    string RefreshToken,
    int ExpiresInSeconds
);
