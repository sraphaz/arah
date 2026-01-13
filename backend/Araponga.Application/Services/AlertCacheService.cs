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
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);

    public AlertCacheService(IHealthAlertRepository alertRepository, IMemoryCache cache)
    {
        _alertRepository = alertRepository;
        _cache = cache;
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
            return cached ?? Array.Empty<HealthAlert>();
        }

        var alerts = await _alertRepository.ListByTerritoryAsync(territoryId, cancellationToken);
        _cache.Set(cacheKey, alerts, CacheExpiration);

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
