using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Modules.Moderation.Application.Interfaces;

namespace Araponga.Application.Services;

/// <summary>
/// Service for caching user block data to reduce database queries.
/// Uses distributed cache (Redis) with automatic fallback to memory cache.
/// </summary>
public sealed class UserBlockCacheService
{
    private readonly IUserBlockRepository _blockRepository;
    private readonly IDistributedCacheService _cache;
    private readonly CacheMetricsService? _metrics;

    public UserBlockCacheService(
        IUserBlockRepository blockRepository, 
        IDistributedCacheService cache,
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
        var cached = await _cache.GetAsync<IReadOnlyCollection<Guid>>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            _metrics?.RecordCacheAccess(cacheKey, hit: true);
            return cached;
        }

        _metrics?.RecordCacheAccess(cacheKey, hit: false);

        var blockedIds = await _blockRepository.GetBlockedUserIdsAsync(userId, cancellationToken);
        await _cache.SetAsync(cacheKey, blockedIds, Constants.Cache.UserBlockExpiration, cancellationToken);

        return blockedIds;
    }

    /// <summary>
    /// Invalidates cache for a user's blocked list.
    /// </summary>
    public async Task InvalidateUserBlocksAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync($"userblocks:{userId}", cancellationToken);
    }

    /// <summary>
    /// Invalidates cache for a user's blocked list (synchronous version for backward compatibility).
    /// </summary>
    public void InvalidateUserBlocks(Guid userId)
    {
        InvalidateUserBlocksAsync(userId).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Invalidates cache for both blocker and blocked user (when a block is created/removed).
    /// </summary>
    public async Task InvalidateBlockAsync(Guid blockerUserId, Guid blockedUserId, CancellationToken cancellationToken = default)
    {
        await InvalidateUserBlocksAsync(blockerUserId, cancellationToken);
        // Also invalidate reverse lookup if needed
        await _cache.RemoveAsync($"userblocks:{blockedUserId}", cancellationToken);
    }

    /// <summary>
    /// Invalidates cache for both blocker and blocked user (synchronous version for backward compatibility).
    /// </summary>
    public void InvalidateBlock(Guid blockerUserId, Guid blockedUserId)
    {
        InvalidateBlockAsync(blockerUserId, blockedUserId).GetAwaiter().GetResult();
    }
}
