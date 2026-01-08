namespace Araponga.Api.Contracts.Memberships;

public sealed record MembershipResponse(
    Guid Id,
    Guid UserId,
    Guid TerritoryId,
    string Status,
    DateTime CreatedAtUtc
);
