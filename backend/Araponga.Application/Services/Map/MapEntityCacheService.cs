using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Modules.Map.Application.Interfaces;
using Araponga.Modules.Map.Domain;

namespace Araponga.Application.Services;

/// <summary>
/// Service for caching map entity data to reduce database queries.
/// Uses distributed cache (Redis) with automatic fallback to memory cache.
/// </summary>
public sealed class MapEntityCacheService
{
    private readonly IMapRepository _mapRepository;
    private readonly IDistributedCacheService _cache;
    private readonly CacheMetricsService? _metrics;

    public MapEntityCacheService(
        IMapRepository mapRepository, 
        IDistributedCacheService cache,
        CacheMetricsService? metrics = null)
    {
        _mapRepository = mapRepository;
        _cache = cache;
        _metrics = metrics;
    }

    /// <summary>
    /// Gets map entities for a territory from cache or repository.
    /// </summary>
    public async Task<IReadOnlyList<MapEntity>> GetEntitiesByTerritoryAsync(
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"mapentities:{territoryId}";
        var cached = await _cache.GetAsync<IReadOnlyList<MapEntity>>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            _metrics?.RecordCacheAccess(cacheKey, hit: true);
            return cached;
        }

        _metrics?.RecordCacheAccess(cacheKey, hit: false);

        var entities = await _mapRepository.ListByTerritoryAsync(territoryId, cancellationToken);
        await _cache.SetAsync(cacheKey, entities, Constants.Cache.MapEntityExpiration, cancellationToken);

        return entities;
    }

    /// <summary>
    /// Invalidates cache for a territory's map entities.
    /// </summary>
    public async Task InvalidateTerritoryEntitiesAsync(Guid territoryId, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync($"mapentities:{territoryId}", cancellationToken);
    }

    /// <summary>
    /// Invalidates cache for a territory's map entities (synchronous version for backward compatibility).
    /// </summary>
    public void InvalidateTerritoryEntities(Guid territoryId)
    {
        InvalidateTerritoryEntitiesAsync(territoryId).GetAwaiter().GetResult();
    }
}
