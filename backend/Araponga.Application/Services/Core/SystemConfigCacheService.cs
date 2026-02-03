using Araponga.Application.Interfaces;
using Araponga.Domain.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace Araponga.Application.Services;

/// <summary>
/// Cache simples de SystemConfig (por key).
/// Invalidação explícita após upsert.
/// </summary>
public sealed class SystemConfigCacheService
{
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(5);

    private readonly ISystemConfigRepository _repository;
    private readonly IMemoryCache _cache;

    public SystemConfigCacheService(ISystemConfigRepository repository, IMemoryCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<SystemConfig?> GetByKeyAsync(string key, CancellationToken cancellationToken)
    {
        var normalizedKey = string.IsNullOrWhiteSpace(key) ? string.Empty : key.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalizedKey))
        {
            return null;
        }

        var cacheKey = $"system_config:{normalizedKey}";
        if (_cache.TryGetValue<SystemConfig?>(cacheKey, out var cached))
        {
            return cached;
        }

        var config = await _repository.GetByKeyAsync(normalizedKey, cancellationToken);
        _cache.Set(cacheKey, config, CacheExpiration);
        return config;
    }

    public void Invalidate(string key)
    {
        var normalizedKey = string.IsNullOrWhiteSpace(key) ? string.Empty : key.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(normalizedKey))
        {
            return;
        }

        _cache.Remove($"system_config:{normalizedKey}");
    }
}

