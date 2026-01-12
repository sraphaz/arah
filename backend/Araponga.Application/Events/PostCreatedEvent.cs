namespace Araponga.Application.Events;

public sealed record PostCreatedEvent(
    Guid PostId,
    Guid TerritoryId,
    Guid AuthorUserId,
    DateTime OccurredAtUtc) : IAppEvent;
