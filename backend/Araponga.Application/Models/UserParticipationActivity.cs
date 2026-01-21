namespace Araponga.Application.Models;

public sealed record UserParticipationActivity(
    Guid EventId,
    Guid TerritoryId,
    string EventTitle,
    DateTime StartsAtUtc,
    string Status,
    DateTime CreatedAtUtc);
