namespace Araponga.Api.Contracts.Events;

public sealed record UpdateEventRequest(
    string? Title,
    string? Description,
    DateTime? StartsAtUtc,
    DateTime? EndsAtUtc,
    double? Latitude,
    double? Longitude,
    string? LocationLabel);
