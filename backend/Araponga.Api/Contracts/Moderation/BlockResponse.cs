namespace Araponga.Api.Contracts.Moderation;

public sealed record BlockResponse(
    Guid BlockerUserId,
    Guid BlockedUserId,
    bool AlreadyBlocked,
    DateTime CreatedAtUtc);
