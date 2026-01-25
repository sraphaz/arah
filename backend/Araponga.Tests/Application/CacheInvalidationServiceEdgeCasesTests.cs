using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Tests.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for CacheInvalidationService,
/// focusing on invalidation methods not throwing and InvalidateKey async/sync.
/// </summary>
public sealed class CacheInvalidationServiceEdgeCasesTests
{
    private readonly IDistributedCacheService _cache;
    private readonly Mock<ILogger<CacheInvalidationService>> _loggerMock;
    private readonly CacheInvalidationService _service;

    public CacheInvalidationServiceEdgeCasesTests()
    {
        _cache = CacheTestHelper.CreateDistributedCacheService();
        _loggerMock = new Mock<ILogger<CacheInvalidationService>>();
        _service = new CacheInvalidationService(_cache, _loggerMock.Object);
    }

    [Fact]
    public void InvalidateMembershipCache_WithValidIds_DoesNotThrow()
    {
        _service.InvalidateMembershipCache(Guid.NewGuid(), Guid.NewGuid());
    }

    [Fact]
    public void InvalidateCapabilityCache_WithMembershipIdOnly_DoesNotThrow()
    {
        _service.InvalidateCapabilityCache(Guid.NewGuid(), userId: null, territoryId: null);
    }

    [Fact]
    public void InvalidateCapabilityCache_WithUserIdAndTerritoryId_DoesNotThrow()
    {
        _service.InvalidateCapabilityCache(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
    }

    [Fact]
    public void InvalidateSystemPermissionCache_WithValidUserId_DoesNotThrow()
    {
        _service.InvalidateSystemPermissionCache(Guid.NewGuid());
    }

    [Fact]
    public void InvalidateTerritoryCache_WithValidTerritoryId_DoesNotThrow()
    {
        _service.InvalidateTerritoryCache(Guid.NewGuid());
    }

    [Fact]
    public void InvalidateAssetCache_WithTerritoryIdOnly_DoesNotThrow()
    {
        _service.InvalidateAssetCache(Guid.NewGuid(), assetId: null);
    }

    [Fact]
    public void InvalidateAssetCache_WithAssetId_DoesNotThrow()
    {
        _service.InvalidateAssetCache(Guid.NewGuid(), Guid.NewGuid());
    }

    [Fact]
    public async Task InvalidateKeyAsync_WithValidKey_DoesNotThrow()
    {
        await _service.InvalidateKeyAsync("test:key:123", CancellationToken.None);
    }

    [Fact]
    public void InvalidateKey_WithValidKey_DoesNotThrow()
    {
        _service.InvalidateKey("test:key:sync");
    }
}
