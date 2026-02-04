using Araponga.Application.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class CacheMetricsServiceTests
{
    [Fact]
    public void RecordCacheAccess_IncrementsTotalRequests()
    {
        var logger = NullLogger<CacheMetricsService>.Instance;
        var service = new CacheMetricsService(logger);

        service.RecordCacheAccess("key1", hit: true);
        service.RecordCacheAccess("key2", hit: false);

        var metrics = service.GetMetrics();

        Assert.Equal(2, metrics.TotalRequests);
        Assert.Equal(1, metrics.CacheHits);
        Assert.Equal(1, metrics.CacheMisses);
    }

    [Fact]
    public void GetMetrics_CalculatesHitAndMissRates()
    {
        var logger = NullLogger<CacheMetricsService>.Instance;
        var service = new CacheMetricsService(logger);

        // 10 hits, 5 misses = 15 total
        for (int i = 0; i < 10; i++)
        {
            service.RecordCacheAccess($"key{i}", hit: true);
        }
        for (int i = 0; i < 5; i++)
        {
            service.RecordCacheAccess($"miss{i}", hit: false);
        }

        var metrics = service.GetMetrics();

        Assert.Equal(15, metrics.TotalRequests);
        Assert.Equal(10, metrics.CacheHits);
        Assert.Equal(5, metrics.CacheMisses);
        Assert.Equal(10.0 / 15.0, metrics.HitRate, 5);
        Assert.Equal(5.0 / 15.0, metrics.MissRate, 5);
    }

    [Fact]
    public void GetMetrics_ReturnsZeroRatesWhenNoRequests()
    {
        var logger = NullLogger<CacheMetricsService>.Instance;
        var service = new CacheMetricsService(logger);

        var metrics = service.GetMetrics();

        Assert.Equal(0, metrics.TotalRequests);
        Assert.Equal(0, metrics.CacheHits);
        Assert.Equal(0, metrics.CacheMisses);
        Assert.Equal(0.0, metrics.HitRate);
        Assert.Equal(0.0, metrics.MissRate);
    }

    [Fact]
    public void Reset_ClearsAllMetrics()
    {
        var logger = NullLogger<CacheMetricsService>.Instance;
        var service = new CacheMetricsService(logger);

        service.RecordCacheAccess("key1", hit: true);
        service.RecordCacheAccess("key2", hit: false);

        service.Reset();

        var metrics = service.GetMetrics();

        Assert.Equal(0, metrics.TotalRequests);
        Assert.Equal(0, metrics.CacheHits);
        Assert.Equal(0, metrics.CacheMisses);
    }

    [Fact]
    public async Task RecordCacheAccess_IsThreadSafe()
    {
        var logger = NullLogger<CacheMetricsService>.Instance;
        var service = new CacheMetricsService(logger);

        var tasks = new List<Task>();
        for (int i = 0; i < 100; i++)
        {
            int index = i;
            tasks.Add(Task.Run(() =>
            {
                service.RecordCacheAccess($"key{index}", hit: index % 2 == 0);
            }));
        }

        await Task.WhenAll(tasks);

        var metrics = service.GetMetrics();

        Assert.Equal(100, metrics.TotalRequests);
        Assert.Equal(50, metrics.CacheHits); // Even indices
        Assert.Equal(50, metrics.CacheMisses); // Odd indices
    }
}
