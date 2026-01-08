namespace Araponga.Api.Contracts.Auth;

public sealed record UserResponse(
    Guid Id,
    string DisplayName,
    string Email
);
