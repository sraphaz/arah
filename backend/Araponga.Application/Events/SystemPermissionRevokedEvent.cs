namespace Araponga.Application.Events;

/// <summary>
/// Evento disparado quando uma SystemPermission é revogada.
/// Usado para invalidar cache de permissões do AccessEvaluator.
/// </summary>
public sealed record SystemPermissionRevokedEvent(
    Guid UserId,
    Domain.Users.SystemPermissionType PermissionType,
    DateTime OccurredAtUtc) : IAppEvent;
