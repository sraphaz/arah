using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Araponga.Api.HealthChecks;

public sealed class CacheHealthCheck : IHealthCheck
{
    private readonly IMemoryCache _memoryCache;
    private readonly IDistributedCache _distributedCache;

    public CacheHealthCheck(IMemoryCache memoryCache, IDistributedCache distributedCache)
    {
        _memoryCache = memoryCache;
        _distributedCache = distributedCache;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var cacheKey = $"healthcheck:{Guid.NewGuid():N}";
            _memoryCache.Set(cacheKey, "ok", TimeSpan.FromSeconds(5));

            if (!_memoryCache.TryGetValue(cacheKey, out _))
            {
                return HealthCheckResult.Unhealthy("Memory cache não respondeu ao set/get.");
            }

            await _distributedCache.SetStringAsync(
                cacheKey,
                "ok",
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
                },
                cancellationToken);

            var value = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.Equals(value, "ok", StringComparison.Ordinal))
            {
                return HealthCheckResult.Unhealthy("Distributed cache não respondeu ao set/get.");
            }

            return HealthCheckResult.Healthy("Caches respondendo.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Falha ao verificar caches.", ex);
        }
    }
}
