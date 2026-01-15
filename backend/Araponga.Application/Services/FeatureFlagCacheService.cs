using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Araponga.Application.Services;

/// <summary>
/// Service for caching feature flags to reduce database queries.
/// </summary>
public sealed class FeatureFlagCacheService
{
    private readonly IFeatureFlagService _featureFlagRepository;
    private readonly IMemoryCache _cache;
    private readonly CacheMetricsService? _metrics;

    public FeatureFlagCacheService(
        IFeatureFlagService featureFlagRepository, 
        IMemoryCache cache,
        CacheMetricsService? metrics = null)
    {
        _featureFlagRepository = featureFlagRepository;
        _cache = cache;
        _metrics = metrics;
    }

    /// <summary>
    /// Gets enabled flags for a territory from cache or repository.
    /// </summary>
    public IReadOnlyList<FeatureFlag> GetEnabledFlags(Guid territoryId)
    {
        var cacheKey = $"featureflags:{territoryId}";
        if (_cache.TryGetValue<IReadOnlyList<FeatureFlag>>(cacheKey, out var cached))
        {
            _metrics?.RecordCacheAccess(cacheKey, hit: true);
            return cached ?? Array.Empty<FeatureFlag>();
        }

        _metrics?.RecordCacheAccess(cacheKey, hit: false);

        var flags = _featureFlagRepository.GetEnabledFlags(territoryId);
        _cache.Set(cacheKey, flags, Constants.Cache.FeatureFlagExpiration);

        return flags;
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
    public void InvalidateFlags(Guid territoryId)
    {
        _cache.Remove($"featureflags:{territoryId}");
    }
}
