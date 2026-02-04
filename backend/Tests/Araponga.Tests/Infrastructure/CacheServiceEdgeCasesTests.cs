using Araponga.Infrastructure.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Araponga.Tests.Infrastructure;

/// <summary>
/// Edge case tests for RedisCacheService,
/// focusing on fallback behavior, null handling, expiration, and error scenarios.
/// </summary>
public class CacheServiceEdgeCasesTests
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<RedisCacheService> _logger;

    public CacheServiceEdgeCasesTests()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _logger = new Mock<ILogger<RedisCacheService>>().Object;
    }

    // GetAsync edge cases
    [Fact]
    public async Task RedisCacheService_GetAsync_WithEmptyKey_ReturnsDefault()
    {
        var cacheService = new RedisCacheService(null, _memoryCache, _logger);

        var result = await cacheService.GetAsync<string>("", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task RedisCacheService_GetAsync_WithNonExistentKey_ReturnsDefault()
    {
        var cacheService = new RedisCacheService(null, _memoryCache, _logger);

        var result = await cacheService.GetAsync<string>("nonexistent-key", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task RedisCacheService_GetAsync_WithMemoryCacheFallback_ReturnsValue()
    {
        var cacheService = new RedisCacheService(null, _memoryCache, _logger);
        var key = "test-key";
        var value = "test-value";

        await cacheService.SetAsync(key, value, TimeSpan.FromMinutes(1), CancellationToken.None);
        var result = await cacheService.GetAsync<string>(key, CancellationToken.None);

        Assert.Equal(value, result);
    }

    [Fact]
    public async Task RedisCacheService_GetAsync_WithExpiredValue_ReturnsDefault()
    {
        var cacheService = new RedisCacheService(null, _memoryCache, _logger);
        var key = "expired-key";
        var value = "test-value";

        await cacheService.SetAsync(key, value, TimeSpan.FromMilliseconds(10), CancellationToken.None);
        await Task.Delay(50); // Wait for expiration
        var result = await cacheService.GetAsync<string>(key, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task RedisCacheService_GetAsync_WithComplexObject_DeserializesCorrectly()
    {
        var cacheService = new RedisCacheService(null, _memoryCache, _logger);
        var key = "complex-key";
        var value = new TestObject { Id = Guid.NewGuid(), Name = "Test", Count = 42 };

        await cacheService.SetAsync(key, value, TimeSpan.FromMinutes(1), CancellationToken.None);
        var result = await cacheService.GetAsync<TestObject>(key, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(value.Id, result!.Id);
        Assert.Equal(value.Name, result.Name);
        Assert.Equal(value.Count, result.Count);
    }

    [Fact]
    public async Task RedisCacheService_GetAsync_WithValueType_DeserializesCorrectly()
    {
        var cacheService = new RedisCacheService(null, _memoryCache, _logger);
        var key = "int-key";
        var value = 42;

        await cacheService.SetAsync(key, value, TimeSpan.FromMinutes(1), CancellationToken.None);
        var result = await cacheService.GetAsync<int>(key, CancellationToken.None);

        Assert.Equal(value, result);
    }

    [Fact]
    public async Task RedisCacheService_GetAsync_WithNullValue_ReturnsNull()
    {
        var cacheService = new RedisCacheService(null, _memoryCache, _logger);
        var key = "null-key";

        // Set null value (should serialize as null)
        await cacheService.SetAsync<string?>(key, null, TimeSpan.FromMinutes(1), CancellationToken.None);
        var result = await cacheService.GetAsync<string>(key, CancellationToken.None);

        Assert.Null(result);
    }

    // SetAsync edge cases
    [Fact]
    public async Task RedisCacheService_SetAsync_WithEmptyKey_DoesNotThrow()
    {
        var cacheService = new RedisCacheService(null, _memoryCache, _logger);

        await cacheService.SetAsync("", "value", TimeSpan.FromMinutes(1), CancellationToken.None);

        // Should not throw
        Assert.True(true);
    }

    [Fact]
    public async Task RedisCacheService_SetAsync_WithMinimalExpiration_StoresCorrectly()
    {
        var cacheService = new RedisCacheService(null, _memoryCache, _logger);
        var key = "minimal-expiration-key";
        var value = "test-value";

        await cacheService.SetAsync(key, value, TimeSpan.FromMilliseconds(1), CancellationToken.None);
        var result = await cacheService.GetAsync<string>(key, CancellationToken.None);

        Assert.Equal(value, result);
    }

    [Fact]
    public async Task RedisCacheService_SetAsync_WithVeryLongExpiration_StoresCorrectly()
    {
        var cacheService = new RedisCacheService(null, _memoryCache, _logger);
        var key = "long-expiration-key";
        var value = "test-value";

        await cacheService.SetAsync(key, value, TimeSpan.FromDays(365), CancellationToken.None);
        var result = await cacheService.GetAsync<string>(key, CancellationToken.None);

        Assert.Equal(value, result);
    }

    [Fact]
    public async Task RedisCacheService_SetAsync_OverwritesExistingValue()
    {
        var cacheService = new RedisCacheService(null, _memoryCache, _logger);
        var key = "overwrite-key";
        var value1 = "value1";
        var value2 = "value2";

        await cacheService.SetAsync(key, value1, TimeSpan.FromMinutes(1), CancellationToken.None);
        await cacheService.SetAsync(key, value2, TimeSpan.FromMinutes(1), CancellationToken.None);
        var result = await cacheService.GetAsync<string>(key, CancellationToken.None);

        Assert.Equal(value2, result);
    }

    [Fact]
    public async Task RedisCacheService_SetAsync_WithUnicodeValue_StoresCorrectly()
    {
        var cacheService = new RedisCacheService(null, _memoryCache, _logger);
        var key = "unicode-key";
        var value = "café naïve résumé 文字 🏪";

        await cacheService.SetAsync(key, value, TimeSpan.FromMinutes(1), CancellationToken.None);
        var result = await cacheService.GetAsync<string>(key, CancellationToken.None);

        Assert.Equal(value, result);
    }

    // RemoveAsync edge cases
    [Fact]
    public async Task RedisCacheService_RemoveAsync_WithEmptyKey_DoesNotThrow()
    {
        var cacheService = new RedisCacheService(null, _memoryCache, _logger);

        await cacheService.RemoveAsync("", CancellationToken.None);

        // Should not throw
        Assert.True(true);
    }

    [Fact]
    public async Task RedisCacheService_RemoveAsync_WithNonExistentKey_DoesNotThrow()
    {
        var cacheService = new RedisCacheService(null, _memoryCache, _logger);

        await cacheService.RemoveAsync("nonexistent-key", CancellationToken.None);

        // Should not throw
        Assert.True(true);
    }

    [Fact]
    public async Task RedisCacheService_RemoveAsync_RemovesExistingValue()
    {
        var cacheService = new RedisCacheService(null, _memoryCache, _logger);
        var key = "remove-key";
        var value = "test-value";

        await cacheService.SetAsync(key, value, TimeSpan.FromMinutes(1), CancellationToken.None);
        await cacheService.RemoveAsync(key, CancellationToken.None);
        var result = await cacheService.GetAsync<string>(key, CancellationToken.None);

        Assert.Null(result);
    }

    // ExistsAsync edge cases
    [Fact]
    public async Task RedisCacheService_ExistsAsync_WithEmptyKey_ReturnsFalse()
    {
        var cacheService = new RedisCacheService(null, _memoryCache, _logger);

        var result = await cacheService.ExistsAsync("", CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task RedisCacheService_ExistsAsync_WithNonExistentKey_ReturnsFalse()
    {
        var cacheService = new RedisCacheService(null, _memoryCache, _logger);

        var result = await cacheService.ExistsAsync("nonexistent-key", CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task RedisCacheService_ExistsAsync_WithExistingKey_ReturnsTrue()
    {
        var cacheService = new RedisCacheService(null, _memoryCache, _logger);
        var key = "exists-key";
        var value = "test-value";

        await cacheService.SetAsync(key, value, TimeSpan.FromMinutes(1), CancellationToken.None);
        var result = await cacheService.ExistsAsync(key, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task RedisCacheService_ExistsAsync_WithExpiredKey_ReturnsFalse()
    {
        var cacheService = new RedisCacheService(null, _memoryCache, _logger);
        var key = "expired-exists-key";
        var value = "test-value";

        await cacheService.SetAsync(key, value, TimeSpan.FromMilliseconds(10), CancellationToken.None);
        await Task.Delay(50); // Wait for expiration
        var result = await cacheService.ExistsAsync(key, CancellationToken.None);

        Assert.False(result);
    }

    // RemoveByPatternAsync edge cases
    [Fact]
    public async Task RedisCacheService_RemoveByPatternAsync_WithEmptyPattern_DoesNotThrow()
    {
        var cacheService = new RedisCacheService(null, _memoryCache, _logger);

        await cacheService.RemoveByPatternAsync("", CancellationToken.None);

        // Should not throw (currently not fully implemented)
        Assert.True(true);
    }

    [Fact]
    public async Task RedisCacheService_RemoveByPatternAsync_WithPattern_DoesNotThrow()
    {
        var cacheService = new RedisCacheService(null, _memoryCache, _logger);

        await cacheService.RemoveByPatternAsync("pattern:*", CancellationToken.None);

        // Should not throw (currently not fully implemented)
        Assert.True(true);
    }

    // Distributed cache fallback tests
    [Fact]
    public async Task RedisCacheService_WithNullDistributedCache_UsesMemoryCache()
    {
        var cacheService = new RedisCacheService(null, _memoryCache, _logger);
        var key = "memory-only-key";
        var value = "test-value";

        // Set should use memory cache
        await cacheService.SetAsync(key, value, TimeSpan.FromMinutes(1), CancellationToken.None);

        // Get should retrieve from memory cache
        var result = await cacheService.GetAsync<string>(key, CancellationToken.None);

        Assert.Equal(value, result);
    }

    [Fact]
    public async Task RedisCacheService_WithMemoryCacheOnly_WorksCorrectly()
    {
        var cacheService = new RedisCacheService(null, _memoryCache, _logger);
        var key = "memory-cache-key";
        var value = "test-value";

        await cacheService.SetAsync(key, value, TimeSpan.FromMinutes(1), CancellationToken.None);
        
        var exists = await cacheService.ExistsAsync(key, CancellationToken.None);
        Assert.True(exists);

        var retrieved = await cacheService.GetAsync<string>(key, CancellationToken.None);
        Assert.Equal(value, retrieved);

        await cacheService.RemoveAsync(key, CancellationToken.None);
        
        var existsAfterRemove = await cacheService.ExistsAsync(key, CancellationToken.None);
        Assert.False(existsAfterRemove);
    }

    // Helper class for complex object tests
    private class TestObject
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
