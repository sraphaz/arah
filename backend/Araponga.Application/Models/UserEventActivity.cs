namespace Araponga.Application.Models;

public sealed record UserEventActivity(
    Guid Id,
    Guid TerritoryId,
    string Title,
    DateTime StartsAtUtc,
    string Status,
    DateTime CreatedAtUtc);
