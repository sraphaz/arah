using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Arah.Bff.Services;

/// <summary>
/// Cache de respostas HTTP das jornadas (GET 2xx) para reduzir chamadas à API.
/// Chave inclui path, query e autorização para respostas por usuário.
/// </summary>
public interface IJourneyResponseCache
{
    bool TryGet(string key, out CachedJourneyResponse? cached);
    void Set(string key, CachedJourneyResponse response, TimeSpan ttl);
    bool ShouldCache(string method, string pathAndQuery, int statusCode);
    int GetTtlSeconds(string pathAndQuery);
}

public sealed record CachedJourneyResponse(
    int StatusCode,
    IReadOnlyDictionary<string, string[]> Headers,
    string? ContentType,
    byte[] Body);

public sealed class JourneyResponseCache : IJourneyResponseCache
{
    private const string CacheKeyPrefix = "bff:journey:";
    private readonly IMemoryCache _cache;
    private readonly IOptions<BffOptions> _options;

    public JourneyResponseCache(IMemoryCache cache, IOptions<BffOptions> options)
    {
        _cache = cache;
        _options = options;
    }

    public bool TryGet(string key, out CachedJourneyResponse? cached)
    {
        var fullKey = CacheKeyPrefix + key;
        if (_cache.TryGetValue(fullKey, out CachedJourneyResponse? value) && value is not null)
        {
            cached = value;
            return true;
        }
        cached = null;
        return false;
    }

    public void Set(string key, CachedJourneyResponse response, TimeSpan ttl)
    {
        var fullKey = CacheKeyPrefix + key;
        _cache.Set(fullKey, response, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl,
            Size = 1
        });
    }

    public bool ShouldCache(string method, string pathAndQuery, int statusCode)
    {
        if (!_options.Value.EnableCache)
            return false;
        if (!string.Equals(method, "GET", StringComparison.OrdinalIgnoreCase))
            return false;
        if (statusCode < 200 || statusCode >= 300)
            return false;
        var normalized = pathAndQuery.TrimStart('/');
        if (normalized.StartsWith("auth/", StringComparison.OrdinalIgnoreCase) || normalized.Equals("auth", StringComparison.OrdinalIgnoreCase))
            return false;
        return true;
    }

    public int GetTtlSeconds(string pathAndQuery)
    {
        var byPath = _options.Value.CacheTtlByPath;
        if (byPath is not null && byPath.Count > 0)
        {
            var normalized = pathAndQuery.TrimStart('/');
            var bestTtl = _options.Value.CacheTtlSeconds;
            var bestLength = -1;
            foreach (var (prefix, ttl) in byPath)
            {
                var p = prefix.TrimStart('/');
                if (p.Length > bestLength && normalized.StartsWith(p, StringComparison.OrdinalIgnoreCase))
                {
                    bestLength = p.Length;
                    bestTtl = ttl;
                }
            }
            if (bestLength >= 0)
                return bestTtl;
        }
        return _options.Value.CacheTtlSeconds;
    }
}
