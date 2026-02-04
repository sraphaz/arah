using Araponga.Application.Interfaces;
using Araponga.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Araponga.Tests.TestHelpers;

/// <summary>
/// Helper para criar serviços de cache nos testes.
/// </summary>
public static class CacheTestHelper
{
    /// <summary>
    /// Cria um IDistributedCacheService para testes usando IMemoryCache como fallback.
    /// </summary>
    public static IDistributedCacheService CreateDistributedCacheService()
    {
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var logger = NullLogger<RedisCacheService>.Instance;
        return new RedisCacheService(null, memoryCache, logger);
    }

    /// <summary>
    /// Cria um cache que implementa RemoveByPatternAsync (prefix matching).
    /// Use em testes que validam invalidação por pattern (ex.: EventCacheService).
    /// </summary>
    public static IDistributedCacheService CreatePatternAwareCacheService()
    {
        return new PatternAwareTestCacheService();
    }
}
