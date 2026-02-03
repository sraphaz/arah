using Araponga.Application.Metrics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para rastrear métricas de cache (hit/miss rates).
/// </summary>
public sealed class CacheMetricsService
{
    private readonly ILogger<CacheMetricsService> _logger;
    private long _totalRequests;
    private long _cacheHits;
    private long _cacheMisses;

    public CacheMetricsService(ILogger<CacheMetricsService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Registra uma tentativa de acesso ao cache.
    /// </summary>
    public void RecordCacheAccess(string key, bool hit)
    {
        Interlocked.Increment(ref _totalRequests);
        
        if (hit)
        {
            Interlocked.Increment(ref _cacheHits);
            ArapongaMetrics.CacheHits.Add(1);
        }
        else
        {
            Interlocked.Increment(ref _cacheMisses);
            ArapongaMetrics.CacheMisses.Add(1);
        }
    }

    /// <summary>
    /// Obtém as métricas atuais de cache.
    /// </summary>
    public CacheMetrics GetMetrics()
    {
        var total = _totalRequests;
        var hits = _cacheHits;
        var misses = _cacheMisses;
        
        var hitRate = total > 0 ? (double)hits / total : 0.0;
        var missRate = total > 0 ? (double)misses / total : 0.0;

        return new CacheMetrics
        {
            TotalRequests = total,
            CacheHits = hits,
            CacheMisses = misses,
            HitRate = hitRate,
            MissRate = missRate
        };
    }

    /// <summary>
    /// Reseta as métricas (útil para testes ou reset periódico).
    /// </summary>
    public void Reset()
    {
        Interlocked.Exchange(ref _totalRequests, 0);
        Interlocked.Exchange(ref _cacheHits, 0);
        Interlocked.Exchange(ref _cacheMisses, 0);
        _logger.LogInformation("Cache metrics reset");
    }

    /// <summary>
    /// Loga as métricas atuais.
    /// </summary>
    public void LogMetrics()
    {
        var metrics = GetMetrics();
        _logger.LogInformation(
            "Cache Metrics - Total: {Total}, Hits: {Hits}, Misses: {Misses}, Hit Rate: {HitRate:P2}, Miss Rate: {MissRate:P2}",
            metrics.TotalRequests,
            metrics.CacheHits,
            metrics.CacheMisses,
            metrics.HitRate,
            metrics.MissRate);
    }
}

/// <summary>
/// Métricas de cache.
/// </summary>
public sealed class CacheMetrics
{
    public long TotalRequests { get; set; }
    public long CacheHits { get; set; }
    public long CacheMisses { get; set; }
    public double HitRate { get; set; }
    public double MissRate { get; set; }
}
