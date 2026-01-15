using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Health;
using Microsoft.Extensions.Caching.Memory;

namespace Araponga.Application.Services;

/// <summary>
/// Service for caching health alert data to reduce database queries.
/// </summary>
public sealed class AlertCacheService
{
    private readonly IHealthAlertRepository _alertRepository;
    private readonly IMemoryCache _cache;
    private readonly CacheMetricsService? _metrics;

    public AlertCacheService(
        IHealthAlertRepository alertRepository, 
        IMemoryCache cache,
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
        if (_cache.TryGetValue<IReadOnlyList<HealthAlert>>(cacheKey, out var cached))
        {
            _metrics?.RecordCacheAccess(cacheKey, hit: true);
            return cached ?? Array.Empty<HealthAlert>();
        }

        _metrics?.RecordCacheAccess(cacheKey, hit: false);

        var alerts = await _alertRepository.ListByTerritoryAsync(territoryId, cancellationToken);
        _cache.Set(cacheKey, alerts, Constants.Cache.AlertExpiration);

        return alerts;
    }

    /// <summary>
    /// Invalidates cache for a territory's alerts.
    /// </summary>
    public void InvalidateTerritoryAlerts(Guid territoryId)
    {
        _cache.Remove($"alerts:{territoryId}");
    }
}
