namespace Araponga.Api.Contracts.Users;

public sealed record UserProfileResponse(
    Guid Id,
    string DisplayName,
    string? Email,
    string? PhoneNumber,
    string? Address,
    DateTime CreatedAtUtc,
    IReadOnlyList<string> Interests);
