using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Araponga.Application.Services;

/// <summary>
/// Service for caching user block data to reduce database queries.
/// </summary>
public sealed class UserBlockCacheService
{
    private readonly IUserBlockRepository _blockRepository;
    private readonly IMemoryCache _cache;
    private readonly CacheMetricsService? _metrics;

    public UserBlockCacheService(
        IUserBlockRepository blockRepository, 
        IMemoryCache cache,
        CacheMetricsService? metrics = null)
    {
        _blockRepository = blockRepository;
        _cache = cache;
        _metrics = metrics;
    }

    /// <summary>
    /// Gets blocked user IDs for a user from cache or repository.
    /// </summary>
    public async Task<IReadOnlyCollection<Guid>> GetBlockedUserIdsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var cacheKey = $"userblocks:{userId}";
        if (_cache.TryGetValue<IReadOnlyCollection<Guid>>(cacheKey, out var cached))
        {
            _metrics?.RecordCacheAccess(cacheKey, hit: true);
            return cached ?? Array.Empty<Guid>();
        }

        _metrics?.RecordCacheAccess(cacheKey, hit: false);

        var blockedIds = await _blockRepository.GetBlockedUserIdsAsync(userId, cancellationToken);
        _cache.Set(cacheKey, blockedIds, Constants.Cache.UserBlockExpiration);

        return blockedIds;
    }

    /// <summary>
    /// Invalidates cache for a user's blocked list.
    /// </summary>
    public void InvalidateUserBlocks(Guid userId)
    {
        _cache.Remove($"userblocks:{userId}");
    }

    /// <summary>
    /// Invalidates cache for both blocker and blocked user (when a block is created/removed).
    /// </summary>
    public void InvalidateBlock(Guid blockerUserId, Guid blockedUserId)
    {
        InvalidateUserBlocks(blockerUserId);
        // Also invalidate reverse lookup if needed
        _cache.Remove($"userblocks:{blockedUserId}");
    }
}
