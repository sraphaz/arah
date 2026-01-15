namespace Araponga.Api.Contracts.Chat;

public sealed record ConversationSummaryResponse(
    Guid Id,
    Guid? TerritoryId,
    string Kind,
    string Status,
    string? Name,
    DateTime CreatedAtUtc);

