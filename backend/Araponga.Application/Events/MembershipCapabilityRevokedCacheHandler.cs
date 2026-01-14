using Araponga.Application.Services;

namespace Araponga.Application.Events;

/// <summary>
/// Handler que invalida o cache de membership quando uma capability é revogada.
/// </summary>
public sealed class MembershipCapabilityRevokedCacheHandler : IEventHandler<MembershipCapabilityRevokedEvent>
{
    private readonly AccessEvaluator _accessEvaluator;

    public MembershipCapabilityRevokedCacheHandler(AccessEvaluator accessEvaluator)
    {
        _accessEvaluator = accessEvaluator;
    }

    public Task HandleAsync(MembershipCapabilityRevokedEvent appEvent, CancellationToken cancellationToken)
    {
        // Invalidar cache de membership para o usuário no território
        _accessEvaluator.InvalidateMembershipCache(
            appEvent.UserId,
            appEvent.TerritoryId);

        return Task.CompletedTask;
    }
}
