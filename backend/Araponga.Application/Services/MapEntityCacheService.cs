using Araponga.Application.Interfaces;
using Araponga.Domain.Map;
using Microsoft.Extensions.Caching.Memory;

namespace Araponga.Application.Services;

/// <summary>
/// Service for caching map entity data to reduce database queries.
/// </summary>
public sealed class MapEntityCacheService
{
    private readonly IMapRepository _mapRepository;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);

    public MapEntityCacheService(IMapRepository mapRepository, IMemoryCache cache)
    {
        _mapRepository = mapRepository;
        _cache = cache;
    }

    /// <summary>
    /// Gets map entities for a territory from cache or repository.
    /// </summary>
    public async Task<IReadOnlyList<MapEntity>> GetEntitiesByTerritoryAsync(
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"mapentities:{territoryId}";
        if (_cache.TryGetValue<IReadOnlyList<MapEntity>>(cacheKey, out var cached))
        {
            return cached ?? Array.Empty<MapEntity>();
        }

        var entities = await _mapRepository.ListByTerritoryAsync(territoryId, cancellationToken);
        _cache.Set(cacheKey, entities, CacheExpiration);

        return entities;
    }

    /// <summary>
    /// Invalidates cache for a territory's map entities.
    /// </summary>
    public void InvalidateTerritoryEntities(Guid territoryId)
    {
        _cache.Remove($"mapentities:{territoryId}");
    }
}
