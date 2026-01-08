namespace Araponga.Api.Contracts.Auth;

public sealed record SocialLoginResponse(
    UserResponse User,
    string Token
);
