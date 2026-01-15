namespace Araponga.Api.Contracts.Chat;

public sealed record ConversationResponse(
    Guid Id,
    Guid? TerritoryId,
    string Kind,
    string Status,
    string? Name,
    Guid CreatedByUserId,
    DateTime CreatedAtUtc,
    Guid? ApprovedByUserId,
    DateTime? ApprovedAtUtc,
    Guid? LockedByUserId,
    DateTime? LockedAtUtc,
    Guid? DisabledByUserId,
    DateTime? DisabledAtUtc);

