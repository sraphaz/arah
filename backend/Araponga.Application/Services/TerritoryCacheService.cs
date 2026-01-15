using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Territories;

namespace Araponga.Application.Services;

/// <summary>
/// Service for caching territory data to reduce database queries.
/// Uses distributed cache (Redis) with automatic fallback to memory cache.
/// </summary>
public sealed class TerritoryCacheService
{
    private readonly ITerritoryRepository _territoryRepository;
    private readonly IDistributedCacheService _cache;
    private readonly CacheMetricsService? _metrics;

    public TerritoryCacheService(
        ITerritoryRepository territoryRepository, 
        IDistributedCacheService cache,
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
        var cached = await _cache.GetAsync<IReadOnlyList<Territory>>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            _metrics?.RecordCacheAccess(cacheKey, hit: true);
            return cached;
        }

        _metrics?.RecordCacheAccess(cacheKey, hit: false);

        var territories = await _territoryRepository.ListAsync(cancellationToken);
        var activeTerritories = territories
            .Where(t => t.Status == TerritoryStatus.Active)
            .OrderBy(t => t.Name)
            .ToList();

        await _cache.SetAsync(cacheKey, activeTerritories, Constants.Cache.TerritoryExpiration, cancellationToken);

        return activeTerritories;
    }

    /// <summary>
    /// Invalidates the active territories cache.
    /// </summary>
    public async Task InvalidateActiveTerritoriesAsync(CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(Constants.CacheKeys.ActiveTerritories, cancellationToken);
    }

    /// <summary>
    /// Invalidates the active territories cache (synchronous version for backward compatibility).
    /// </summary>
    public void InvalidateActiveTerritories()
    {
        InvalidateActiveTerritoriesAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// Gets a territory by ID with optional caching.
    /// </summary>
    public async Task<Territory?> GetByIdAsync(Guid id, CancellationToken cancellationToken, bool useCache = false)
    {
        if (useCache)
        {
            var cacheKey = Constants.CacheKeys.Territory(id);
            var cached = await _cache.GetAsync<Territory>(cacheKey, cancellationToken);
            if (cached is not null)
            {
                _metrics?.RecordCacheAccess(cacheKey, hit: true);
                return cached;
            }

            _metrics?.RecordCacheAccess(cacheKey, hit: false);

            var territory = await _territoryRepository.GetByIdAsync(id, cancellationToken);
            if (territory is not null)
            {
                await _cache.SetAsync(cacheKey, territory, Constants.Cache.TerritoryDetailExpiration, cancellationToken);
            }

            return territory;
        }

        return await _territoryRepository.GetByIdAsync(id, cancellationToken);
    }

    /// <summary>
    /// Invalidates cache for a specific territory.
    /// </summary>
    public async Task InvalidateTerritoryAsync(Guid territoryId, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(Constants.CacheKeys.Territory(territoryId), cancellationToken);
        // Also invalidate active list since it may have changed
        await InvalidateActiveTerritoriesAsync(cancellationToken);
    }

    /// <summary>
    /// Invalidates cache for a specific territory (synchronous version for backward compatibility).
    /// </summary>
    public void InvalidateTerritory(Guid territoryId)
    {
        InvalidateTerritoryAsync(territoryId).GetAwaiter().GetResult();
    }
}
