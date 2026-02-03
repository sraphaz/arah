using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;

namespace Araponga.Application.Services;

/// <summary>
/// Service for caching feature flags to reduce database queries.
/// Uses distributed cache (Redis) with automatic fallback to memory cache.
/// </summary>
public sealed class FeatureFlagCacheService
{
    private readonly IFeatureFlagService _featureFlagRepository;
    private readonly IDistributedCacheService _cache;
    private readonly CacheMetricsService? _metrics;

    public FeatureFlagCacheService(
        IFeatureFlagService featureFlagRepository, 
        IDistributedCacheService cache,
        CacheMetricsService? metrics = null)
    {
        _featureFlagRepository = featureFlagRepository;
        _cache = cache;
        _metrics = metrics;
    }

    /// <summary>
    /// Gets enabled flags for a territory from cache or repository.
    /// </summary>
    public async Task<IReadOnlyList<FeatureFlag>> GetEnabledFlagsAsync(Guid territoryId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"featureflags:{territoryId}";
        var cached = await _cache.GetAsync<IReadOnlyList<FeatureFlag>>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            _metrics?.RecordCacheAccess(cacheKey, hit: true);
            return cached;
        }

        _metrics?.RecordCacheAccess(cacheKey, hit: false);

        var flags = _featureFlagRepository.GetEnabledFlags(territoryId);
        await _cache.SetAsync(cacheKey, flags, Constants.Cache.FeatureFlagExpiration, cancellationToken);

        return flags;
    }

    /// <summary>
    /// Gets enabled flags for a territory from cache or repository (synchronous version for backward compatibility).
    /// </summary>
    public IReadOnlyList<FeatureFlag> GetEnabledFlags(Guid territoryId)
    {
        return GetEnabledFlagsAsync(territoryId).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Checks if a specific flag is enabled for a territory.
    /// </summary>
    public bool IsEnabled(Guid territoryId, FeatureFlag flag)
    {
        var flags = GetEnabledFlags(territoryId);
        return flags.Contains(flag);
    }

    /// <summary>
    /// Invalidates cache for a territory's feature flags.
    /// </summary>
    public async Task InvalidateFlagsAsync(Guid territoryId, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync($"featureflags:{territoryId}", cancellationToken);
    }

    /// <summary>
    /// Invalidates cache for a territory's feature flags (synchronous version for backward compatibility).
    /// </summary>
    public void InvalidateFlags(Guid territoryId)
    {
        InvalidateFlagsAsync(territoryId).GetAwaiter().GetResult();
    }
}
