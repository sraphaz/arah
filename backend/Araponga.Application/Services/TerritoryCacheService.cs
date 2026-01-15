using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Territories;
using Microsoft.Extensions.Caching.Memory;

namespace Araponga.Application.Services;

/// <summary>
/// Service for caching territory data to reduce database queries.
/// </summary>
public sealed class TerritoryCacheService
{
    private readonly ITerritoryRepository _territoryRepository;
    private readonly IMemoryCache _cache;
    private readonly CacheMetricsService? _metrics;

    public TerritoryCacheService(
        ITerritoryRepository territoryRepository, 
        IMemoryCache cache,
        CacheMetricsService? metrics = null)
    {
        _territoryRepository = territoryRepository;
        _cache = cache;
        _metrics = metrics;
    }

    /// <summary>
    /// Gets the list of active territories from cache or database.
    /// </summary>
    public async Task<IReadOnlyList<Territory>> GetActiveTerritoriesAsync(CancellationToken cancellationToken)
    {
        var cacheKey = Constants.CacheKeys.ActiveTerritories;
        if (_cache.TryGetValue<IReadOnlyList<Territory>>(cacheKey, out var cached))
        {
            _metrics?.RecordCacheAccess(cacheKey, hit: true);
            return cached ?? Array.Empty<Territory>();
        }

        _metrics?.RecordCacheAccess(cacheKey, hit: false);

        var territories = await _territoryRepository.ListAsync(cancellationToken);
        var activeTerritories = territories
            .Where(t => t.Status == TerritoryStatus.Active)
            .OrderBy(t => t.Name)
            .ToList();

        _cache.Set(cacheKey, activeTerritories, Constants.Cache.TerritoryExpiration);

        return activeTerritories;
    }

    /// <summary>
    /// Invalidates the active territories cache.
    /// </summary>
    public void InvalidateActiveTerritories()
    {
        _cache.Remove(Constants.CacheKeys.ActiveTerritories);
    }

    /// <summary>
    /// Gets a territory by ID with optional caching.
    /// </summary>
    public async Task<Territory?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool useCache = false)
    {
        if (useCache)
        {
            var cacheKey = Constants.CacheKeys.Territory(id);
            if (_cache.TryGetValue<Territory>(cacheKey, out var cached))
            {
                _metrics?.RecordCacheAccess(cacheKey, hit: true);
                return cached;
            }

            _metrics?.RecordCacheAccess(cacheKey, hit: false);

            var territory = await _territoryRepository.GetByIdAsync(id, cancellationToken);
            if (territory is not null)
            {
                _cache.Set(cacheKey, territory, Constants.Cache.TerritoryDetailExpiration);
            }

            return territory;
        }

        return await _territoryRepository.GetByIdAsync(id, cancellationToken);
    }

    /// <summary>
    /// Invalidates cache for a specific territory.
    /// </summary>
    public void InvalidateTerritory(Guid territoryId)
    {
        _cache.Remove(Constants.CacheKeys.Territory(territoryId));
        // Also invalidate active list since it may have changed
        InvalidateActiveTerritories();
    }
}
