namespace Araponga.Api.Contracts.Governance;

public sealed record VotingResponse(
    Guid Id,
    Guid TerritoryId,
    Guid CreatedByUserId,
    string Type,
    string Title,
    string Description,
    IReadOnlyList<string> Options,
    string Visibility,
    string Status,
    DateTime? StartsAtUtc,
    DateTime? EndsAtUtc,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
