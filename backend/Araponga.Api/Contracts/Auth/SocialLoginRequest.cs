namespace Araponga.Api.Contracts.Auth;

public sealed record SocialLoginRequest(
    string Provider,
    string ExternalId,
    string DisplayName,
    string Email
);
