using Araponga.Application.Services;

namespace Araponga.Application.Events;

/// <summary>
/// Handler que invalida o cache de SystemPermission quando uma permissão é revogada.
/// </summary>
public sealed class SystemPermissionRevokedCacheHandler : IEventHandler<SystemPermissionRevokedEvent>
{
    private readonly AccessEvaluator _accessEvaluator;

    public SystemPermissionRevokedCacheHandler(AccessEvaluator accessEvaluator)
    {
        _accessEvaluator = accessEvaluator;
    }

    public Task HandleAsync(SystemPermissionRevokedEvent appEvent, CancellationToken cancellationToken)
    {
        _accessEvaluator.InvalidateSystemPermissionCache(
            appEvent.UserId,
            appEvent.PermissionType);

        return Task.CompletedTask;
    }
}
