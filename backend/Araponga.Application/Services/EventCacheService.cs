using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Events;

namespace Araponga.Application.Services;

/// <summary>
/// Service for caching event data to reduce database queries.
/// Uses distributed cache (Redis) with automatic fallback to memory cache.
/// </summary>
public sealed class EventCacheService
{
    private readonly ITerritoryEventRepository _eventRepository;
    private readonly IDistributedCacheService _cache;
    private readonly CacheMetricsService? _metrics;

    public EventCacheService(
        ITerritoryEventRepository eventRepository, 
        IDistributedCacheService cache,
        CacheMetricsService? metrics = null)
    {
        _eventRepository = eventRepository;
        _cache = cache;
        _metrics = metrics;
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
        var cached = await _cache.GetAsync<IReadOnlyList<TerritoryEvent>>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            _metrics?.RecordCacheAccess(cacheKey, hit: true);
            return cached;
        }

        _metrics?.RecordCacheAccess(cacheKey, hit: false);

        var events = await _eventRepository.ListByTerritoryAsync(territoryId, from, to, status, cancellationToken);
        await _cache.SetAsync(cacheKey, events, Constants.Cache.EventExpiration, cancellationToken);

        return events;
    }

    /// <summary>
    /// Invalidates all event caches for a territory.
    /// </summary>
    public async Task InvalidateTerritoryEventsAsync(Guid territoryId, CancellationToken cancellationToken = default)
    {
        // Since we use composite keys, we need to clear all patterns
        // For simplicity, we'll use a pattern-based invalidation
        // In production, consider using a more sophisticated cache invalidation strategy
        await _cache.RemoveByPatternAsync($"events:{territoryId}:*", cancellationToken);
    }

    /// <summary>
    /// Invalidates all event caches for a territory (synchronous version for backward compatibility).
    /// </summary>
    public void InvalidateTerritoryEvents(Guid territoryId)
    {
        InvalidateTerritoryEventsAsync(territoryId).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Invalidates cache for a specific event.
    /// </summary>
    public async Task InvalidateEventAsync(Guid territoryId, Guid eventId, CancellationToken cancellationToken = default)
    {
        await InvalidateTerritoryEventsAsync(territoryId, cancellationToken);
    }

    /// <summary>
    /// Invalidates cache for a specific event (synchronous version for backward compatibility).
    /// </summary>
    public void InvalidateEvent(Guid territoryId, Guid eventId)
    {
        InvalidateEventAsync(territoryId, eventId).GetAwaiter().GetResult();
    }
}
