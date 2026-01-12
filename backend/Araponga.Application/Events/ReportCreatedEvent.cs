namespace Araponga.Application.Events;

public sealed record ReportCreatedEvent(
    Guid ReportId,
    Guid TerritoryId,
    Guid ReporterUserId,
    DateTime OccurredAtUtc) : IAppEvent;
