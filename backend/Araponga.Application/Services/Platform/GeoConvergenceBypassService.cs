using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Users;

namespace Araponga.Application.Services;

/// <summary>
/// Centraliza a decisão de ignorar a exigência de convergência geo: por território (feature flag)
/// ou por usuário (SystemAdmin ou RemoteAccessToTerritory). Sys admin tem bypass por padrão.
/// </summary>
public sealed class GeoConvergenceBypassService : IGeoConvergenceBypassService
{
    private readonly TerritoryFeatureFlagGuard _featureGuard;
    private readonly AccessEvaluator _accessEvaluator;

    public GeoConvergenceBypassService(
        TerritoryFeatureFlagGuard featureGuard,
        AccessEvaluator accessEvaluator)
    {
        _featureGuard = featureGuard;
        _accessEvaluator = accessEvaluator;
    }

    /// <inheritdoc />
    public async Task<bool> ShouldBypassGeoEnforcementAsync(
        Guid territoryId,
        Guid? userId,
        CancellationToken cancellationToken = default)
    {
        if (_featureGuard.IsEnabled(territoryId, FeatureFlag.RemoteAccessToTerritoryEnabled))
        {
            return true;
        }

        if (userId is null || userId.Value == Guid.Empty)
        {
            return false;
        }

        if (await _accessEvaluator.IsSystemAdminAsync(userId.Value, cancellationToken).ConfigureAwait(false))
        {
            return true;
        }

        if (await _accessEvaluator.HasSystemPermissionAsync(
                userId.Value,
                SystemPermissionType.RemoteAccessToTerritory,
                cancellationToken)
            .ConfigureAwait(false))
        {
            return true;
        }

        return false;
    }
}
