using Araponga.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Araponga.Infrastructure.Caching;

/// <summary>
/// Serviço de cache distribuído com fallback para IMemoryCache se Redis estiver indisponível.
/// </summary>
public sealed class RedisCacheService : IDistributedCacheService
{
    private readonly IDistributedCache? _distributedCache;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly bool _useDistributedCache;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        MaxDepth = 64,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    public RedisCacheService(
        IDistributedCache? distributedCache,
        IMemoryCache memoryCache,
        ILogger<RedisCacheService> logger)
    {
        _distributedCache = distributedCache;
        _memoryCache = memoryCache;
        _logger = logger;
        _useDistributedCache = distributedCache is not null;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_useDistributedCache && _distributedCache is not null)
        {
            try
            {
                var cached = await _distributedCache.GetStringAsync(key, cancellationToken);
                if (cached is not null)
                {
                    return JsonSerializer.Deserialize<T>(cached, JsonOptions);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error reading from Redis cache, falling back to memory cache for key {Key}", key);
                // Fallback para memory cache
            }
        }

        // Fallback para memory cache
        if (_memoryCache.TryGetValue(key, out var value))
        {
            if (value is T typedValue)
            {
                return typedValue;
            }
            // Tentar deserializar se for string (para value types)
            if (value is string strValue)
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(strValue, JsonOptions);
                }
                catch
                {
                    // Ignorar erro de deserialização
                }
            }
        }

        return default(T);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        var serialized = JsonSerializer.Serialize(value, JsonOptions);
        var cacheOptions = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };

        if (_useDistributedCache && _distributedCache is not null)
        {
            try
            {
                await _distributedCache.SetStringAsync(key, serialized, cacheOptions, cancellationToken);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error writing to Redis cache, falling back to memory cache for key {Key}", key);
                // Fallback para memory cache
            }
        }

        // Fallback para memory cache
        var memoryOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration
        };
        _memoryCache.Set(key, value, memoryOptions);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (_useDistributedCache && _distributedCache is not null)
        {
            try
            {
                await _distributedCache.RemoveAsync(key, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error removing from Redis cache, falling back to memory cache for key {Key}", key);
            }
        }

        _memoryCache.Remove(key);
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        // Redis suporta padrões via SCAN, mas IMemoryCache não
        // Para IMemoryCache, não há suporte nativo a padrões
        // Em produção com Redis, implementar SCAN aqui
        _logger.LogDebug("RemoveByPatternAsync called for pattern {Pattern} - pattern removal not fully implemented", pattern);
        
        // Por enquanto, apenas logamos - em produção com Redis, implementar SCAN
        // Para IMemoryCache, manter lista de chaves ou invalidar manualmente
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        if (_useDistributedCache && _distributedCache is not null)
        {
            try
            {
                var value = await _distributedCache.GetStringAsync(key, cancellationToken);
                return value is not null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error checking Redis cache, falling back to memory cache for key {Key}", key);
            }
        }

        return _memoryCache.TryGetValue(key, out _);
    }
}
