using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Configuration;
using Araponga.Infrastructure.InMemory;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class SystemConfigCacheServiceEdgeCasesTests
{
    private readonly InMemoryDataStore _dataStore;
    private readonly InMemorySystemConfigRepository _repository;
    private readonly IMemoryCache _cache;
    private readonly SystemConfigCacheService _service;

    public SystemConfigCacheServiceEdgeCasesTests()
    {
        _dataStore = new InMemoryDataStore();
        _repository = new InMemorySystemConfigRepository(_dataStore);
        _cache = new MemoryCache(new MemoryCacheOptions());
        _service = new SystemConfigCacheService(_repository, _cache);
    }

    [Fact]
    public async Task GetByKeyAsync_WithNullKey_ReturnsNull()
    {
        var result = await _service.GetByKeyAsync(null!, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByKeyAsync_WithEmptyKey_ReturnsNull()
    {
        var result = await _service.GetByKeyAsync("", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByKeyAsync_WithWhitespaceKey_ReturnsNull()
    {
        var result = await _service.GetByKeyAsync("   ", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByKeyAsync_WithNonExistentKey_ReturnsNull()
    {
        var result = await _service.GetByKeyAsync("nonexistent.key", CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByKeyAsync_WithExistingKey_ReturnsConfig()
    {
        var config = new SystemConfig(
            Guid.NewGuid(),
            "test.key",
            "value",
            SystemConfigCategory.Moderation,
            null,
            DateTime.UtcNow,
            Guid.NewGuid(),
            null,
            null);

        await _repository.UpsertAsync(config, CancellationToken.None);

        var result = await _service.GetByKeyAsync("test.key", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("test.key", result.Key);
        Assert.Equal("value", result.Value);
    }

    [Fact]
    public async Task GetByKeyAsync_WithUppercaseKey_NormalizesToLowercase()
    {
        var config = new SystemConfig(
            Guid.NewGuid(),
            "test.key",
            "value",
            SystemConfigCategory.Moderation,
            null,
            DateTime.UtcNow,
            Guid.NewGuid(),
            null,
            null);

        await _repository.UpsertAsync(config, CancellationToken.None);

        var result = await _service.GetByKeyAsync("TEST.KEY", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("test.key", result.Key);
    }

    [Fact]
    public async Task GetByKeyAsync_WithWhitespaceInKey_TrimsKey()
    {
        var config = new SystemConfig(
            Guid.NewGuid(),
            "test.key",
            "value",
            SystemConfigCategory.Moderation,
            null,
            DateTime.UtcNow,
            Guid.NewGuid(),
            null,
            null);

        await _repository.UpsertAsync(config, CancellationToken.None);

        var result = await _service.GetByKeyAsync("  test.key  ", CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("test.key", result.Key);
    }

    [Fact]
    public async Task GetByKeyAsync_CachesResult_OnSecondCall()
    {
        var config = new SystemConfig(
            Guid.NewGuid(),
            "test.key",
            "value",
            SystemConfigCategory.Moderation,
            null,
            DateTime.UtcNow,
            Guid.NewGuid(),
            null,
            null);

        await _repository.UpsertAsync(config, CancellationToken.None);

        var result1 = await _service.GetByKeyAsync("test.key", CancellationToken.None);
        Assert.NotNull(result1);

        // Update config to verify cache is used
        var updatedConfig = new SystemConfig(
            config.Id,
            "test.key",
            "newvalue",
            SystemConfigCategory.Moderation,
            null,
            DateTime.UtcNow,
            Guid.NewGuid(),
            null,
            null);
        await _repository.UpsertAsync(updatedConfig, CancellationToken.None);

        var result2 = await _service.GetByKeyAsync("test.key", CancellationToken.None);
        Assert.NotNull(result2); // Still returns from cache (old value)
        Assert.Equal("value", result2.Value); // Old cached value
    }

    [Fact]
    public void Invalidate_WithNullKey_DoesNotThrow()
    {
        _service.Invalidate(null!);
        // Should not throw
    }

    [Fact]
    public void Invalidate_WithEmptyKey_DoesNotThrow()
    {
        _service.Invalidate("");
        // Should not throw
    }

    [Fact]
    public void Invalidate_WithWhitespaceKey_DoesNotThrow()
    {
        _service.Invalidate("   ");
        // Should not throw
    }

    [Fact]
    public async Task Invalidate_RemovesFromCache()
    {
        var config = new SystemConfig(
            Guid.NewGuid(),
            "test.key",
            "value",
            SystemConfigCategory.Moderation,
            null,
            DateTime.UtcNow,
            Guid.NewGuid(),
            null,
            null);

        await _repository.UpsertAsync(config, CancellationToken.None);

        // First call caches
        var result1 = await _service.GetByKeyAsync("test.key", CancellationToken.None);
        Assert.NotNull(result1);

        // Invalidate
        _service.Invalidate("test.key");

        // Update config in repository
        var updatedConfig = new SystemConfig(
            config.Id,
            "test.key",
            "newvalue",
            SystemConfigCategory.Moderation,
            null,
            DateTime.UtcNow,
            Guid.NewGuid(),
            null,
            null);
        await _repository.UpsertAsync(updatedConfig, CancellationToken.None);

        // Second call should return new value (cache invalidated, reloaded from repository)
        var result2 = await _service.GetByKeyAsync("test.key", CancellationToken.None);
        Assert.NotNull(result2);
        Assert.Equal("newvalue", result2.Value);
    }

    [Fact]
    public async Task Invalidate_WithUppercaseKey_NormalizesToLowercase()
    {
        var config = new SystemConfig(
            Guid.NewGuid(),
            "test.key",
            "value",
            SystemConfigCategory.Moderation,
            null,
            DateTime.UtcNow,
            Guid.NewGuid(),
            null,
            null);

        await _repository.UpsertAsync(config, CancellationToken.None);

        // Cache it
        await _service.GetByKeyAsync("test.key", CancellationToken.None);

        // Invalidate with uppercase
        _service.Invalidate("TEST.KEY");

        // Update config in repository
        var updatedConfig = new SystemConfig(
            config.Id,
            "test.key",
            "newvalue",
            SystemConfigCategory.Moderation,
            null,
            DateTime.UtcNow,
            Guid.NewGuid(),
            null,
            null);
        await _repository.UpsertAsync(updatedConfig, CancellationToken.None);

        // Should return new value (cache invalidated)
        var result = await _service.GetByKeyAsync("test.key", CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal("newvalue", result.Value);
    }
}
