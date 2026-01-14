namespace Araponga.Api.Contracts.Memberships;

public sealed record RequestResidencyResponse(
    Guid JoinRequestId,
    string Status,
    DateTime CreatedAtUtc);

