using System.Collections.Concurrent;
using Araponga.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Araponga.Tests.TestHelpers;

/// <summary>
/// Cache de teste que implementa RemoveByPatternAsync via prefix matching.
/// Usado em testes que validam invalidação por pattern (ex.: EventCacheService).
/// </summary>
public sealed class PatternAwareTestCacheService : IDistributedCacheService
{
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private readonly ConcurrentDictionary<string, byte> _keys = new();

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(key, out var value) && value is T typed)
            return Task.FromResult<T?>(typed);
        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken cancellationToken = default)
    {
        _keys.TryAdd(key, 0);
        _cache.Set(key, value, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration });
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _keys.TryRemove(key, out _);
        _cache.Remove(key);
        return Task.CompletedTask;
    }

    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        var prefix = pattern.EndsWith("*", StringComparison.Ordinal)
            ? pattern[..^1]
            : pattern;
        var toRemove = _keys.Keys.Where(k => k.StartsWith(prefix, StringComparison.Ordinal)).ToList();
        foreach (var k in toRemove)
        {
            _keys.TryRemove(k, out _);
            _cache.Remove(k);
        }
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_cache.TryGetValue(key, out _));
    }
}
