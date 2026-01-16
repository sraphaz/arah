namespace Araponga.Api.Contracts.Events;

public sealed record CreateEventRequest(
    Guid TerritoryId,
    string Title,
    string? Description,
    DateTime StartsAtUtc,
    DateTime? EndsAtUtc,
    double Latitude,
    double Longitude,
    string? LocationLabel,
    Guid? CoverMediaId = null,
    IReadOnlyCollection<Guid>? AdditionalMediaIds = null);
