namespace Araponga.Application.Events;

/// <summary>
/// Evento disparado quando uma MembershipCapability é revogada.
/// Usado para invalidar cache de membership do AccessEvaluator.
/// </summary>
public sealed record MembershipCapabilityRevokedEvent(
    Guid MembershipId,
    Guid UserId,
    Guid TerritoryId,
    DateTime OccurredAtUtc) : IAppEvent;
