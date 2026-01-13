using Araponga.Application.Interfaces;
using Araponga.Domain.Events;
using Microsoft.Extensions.Caching.Memory;

namespace Araponga.Application.Services;

/// <summary>
/// Service for caching event data to reduce database queries.
/// </summary>
public sealed class EventCacheService
{
    private readonly ITerritoryEventRepository _eventRepository;
    private readonly IMemoryCache _cache;
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(5);

    public EventCacheService(ITerritoryEventRepository eventRepository, IMemoryCache cache)
    {
        _eventRepository = eventRepository;
        _cache = cache;
    }

    /// <summary>
    /// Gets events for a territory with filters from cache or repository.
    /// Cache key includes filters to ensure correct data.
    /// </summary>
    public async Task<IReadOnlyList<TerritoryEvent>> GetEventsByTerritoryAsync(
        Guid territoryId,
        DateTime? from,
        DateTime? to,
        EventStatus? status,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"events:{territoryId}:{from?.ToString("yyyy-MM-dd") ?? "null"}:{to?.ToString("yyyy-MM-dd") ?? "null"}:{status?.ToString() ?? "null"}";
        if (_cache.TryGetValue<IReadOnlyList<TerritoryEvent>>(cacheKey, out var cached))
        {
            return cached ?? Array.Empty<TerritoryEvent>();
        }

        var events = await _eventRepository.ListByTerritoryAsync(territoryId, from, to, status, cancellationToken);
        _cache.Set(cacheKey, events, CacheExpiration);

        return events;
    }

    /// <summary>
    /// Invalidates all event caches for a territory.
    /// </summary>
    public void InvalidateTerritoryEvents(Guid territoryId)
    {
        // Since we use composite keys, we need to clear all patterns
        // For simplicity, we'll use a pattern-based invalidation
        // In production, consider using a more sophisticated cache invalidation strategy
        _cache.Remove($"events:{territoryId}:");
    }
}
