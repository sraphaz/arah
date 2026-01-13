namespace Araponga.Api.Contracts.Users;

public sealed record UpdateContactInfoRequest(
    string? Email,
    string? PhoneNumber,
    string? Address);
