using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Health;

namespace Araponga.Application.Services;

/// <summary>
/// Service for caching health alert data to reduce database queries.
/// Uses distributed cache (Redis) with automatic fallback to memory cache.
/// </summary>
public sealed class AlertCacheService
{
    private readonly IHealthAlertRepository _alertRepository;
    private readonly IDistributedCacheService _cache;
    private readonly CacheMetricsService? _metrics;

    public AlertCacheService(
        IHealthAlertRepository alertRepository, 
        IDistributedCacheService cache,
        CacheMetricsService? metrics = null)
    {
        _alertRepository = alertRepository;
        _cache = cache;
        _metrics = metrics;
    }

    /// <summary>
    /// Gets alerts for a territory from cache or repository.
    /// </summary>
    public async Task<IReadOnlyList<HealthAlert>> GetAlertsByTerritoryAsync(
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"alerts:{territoryId}";
        var cached = await _cache.GetAsync<IReadOnlyList<HealthAlert>>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            _metrics?.RecordCacheAccess(cacheKey, hit: true);
            return cached;
        }

        _metrics?.RecordCacheAccess(cacheKey, hit: false);

        var alerts = await _alertRepository.ListByTerritoryAsync(territoryId, cancellationToken);
        await _cache.SetAsync(cacheKey, alerts, Constants.Cache.AlertExpiration, cancellationToken);

        return alerts;
    }

    /// <summary>
    /// Invalidates cache for a territory's alerts.
    /// </summary>
    public async Task InvalidateTerritoryAlertsAsync(Guid territoryId, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync($"alerts:{territoryId}", cancellationToken);
    }

    /// <summary>
    /// Invalidates cache for a territory's alerts (synchronous version for backward compatibility).
    /// </summary>
    public void InvalidateTerritoryAlerts(Guid territoryId)
    {
        InvalidateTerritoryAlertsAsync(territoryId).GetAwaiter().GetResult();
    }
}
