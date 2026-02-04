using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Modules.Moderation.Domain.Moderation;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class UserBlockCacheServiceEdgeCasesTests
{
    private readonly InMemoryDataStore _dataStore;
    private readonly InMemoryUserBlockRepository _blockRepository;
    private readonly IDistributedCacheService _cache;
    private readonly Mock<ILogger<CacheMetricsService>> _loggerMock;
    private readonly CacheMetricsService _metrics;
    private readonly UserBlockCacheService _service;

    public UserBlockCacheServiceEdgeCasesTests()
    {
        _dataStore = new InMemoryDataStore();
        _blockRepository = new InMemoryUserBlockRepository(_dataStore);
        _cache = CacheTestHelper.CreateDistributedCacheService();
        _loggerMock = new Mock<ILogger<CacheMetricsService>>();
        _metrics = new CacheMetricsService(_loggerMock.Object);
        _service = new UserBlockCacheService(_blockRepository, _cache, _metrics);
    }

    [Fact]
    public async Task GetBlockedUserIdsAsync_WithEmptyUserId_ReturnsEmptyList()
    {
        var result = await _service.GetBlockedUserIdsAsync(Guid.Empty, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBlockedUserIdsAsync_WithNonExistentUser_ReturnsEmptyList()
    {
        var result = await _service.GetBlockedUserIdsAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetBlockedUserIdsAsync_WithCachedData_ReturnsCachedData()
    {
        var userId = Guid.NewGuid();
        var blockedUserId1 = Guid.NewGuid();
        var blockedUserId2 = Guid.NewGuid();
        var blockedIds = new[] { blockedUserId1, blockedUserId2 };

        // Popular cache diretamente
        var cacheKey = $"userblocks:{userId}";
        await _cache.SetAsync(cacheKey, blockedIds, TimeSpan.FromMinutes(5), CancellationToken.None);

        var result = await _service.GetBlockedUserIdsAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(blockedUserId1, result);
        Assert.Contains(blockedUserId2, result);
    }

    [Fact]
    public async Task GetBlockedUserIdsAsync_WithoutCache_LoadsFromRepository()
    {
        var userId = Guid.NewGuid();
        var blockedUserId = Guid.NewGuid();

        var userBlock = new UserBlock(
            userId,
            blockedUserId,
            DateTime.UtcNow);

        await _blockRepository.AddAsync(userBlock, CancellationToken.None);

        var result = await _service.GetBlockedUserIdsAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(blockedUserId, result);
    }

    [Fact]
    public async Task GetBlockedUserIdsAsync_RecordsCacheHit_WhenCached()
    {
        var userId = Guid.NewGuid();
        var blockedIds = new[] { Guid.NewGuid() };

        var cacheKey = $"userblocks:{userId}";
        await _cache.SetAsync(cacheKey, blockedIds, TimeSpan.FromMinutes(5), CancellationToken.None);

        await _service.GetBlockedUserIdsAsync(userId, CancellationToken.None);

        var metrics = _metrics.GetMetrics();
        Assert.True(metrics.CacheHits > 0);
    }

    [Fact]
    public async Task GetBlockedUserIdsAsync_RecordsCacheMiss_WhenNotCached()
    {
        var userId = Guid.NewGuid();

        await _service.GetBlockedUserIdsAsync(userId, CancellationToken.None);

        var metrics = _metrics.GetMetrics();
        Assert.True(metrics.CacheMisses > 0);
    }

    [Fact]
    public async Task InvalidateUserBlocksAsync_WithEmptyUserId_DoesNotThrow()
    {
        await _service.InvalidateUserBlocksAsync(Guid.Empty, CancellationToken.None);
        // Não deve lançar exceção
    }

    [Fact]
    public async Task InvalidateUserBlocksAsync_WithNonExistentUser_DoesNotThrow()
    {
        await _service.InvalidateUserBlocksAsync(Guid.NewGuid(), CancellationToken.None);
        // Não deve lançar exceção
    }

    [Fact]
    public async Task InvalidateUserBlocksAsync_RemovesCache()
    {
        var userId = Guid.NewGuid();
        var blockedIds = new[] { Guid.NewGuid() };

        var cacheKey = $"userblocks:{userId}";
        await _cache.SetAsync(cacheKey, blockedIds, TimeSpan.FromMinutes(5), CancellationToken.None);

        // Verificar que está em cache
        var exists = await _cache.ExistsAsync(cacheKey, CancellationToken.None);
        Assert.True(exists);

        // Invalidar
        await _service.InvalidateUserBlocksAsync(userId, CancellationToken.None);

        // Verificar que foi removido
        var stillExists = await _cache.ExistsAsync(cacheKey, CancellationToken.None);
        Assert.False(stillExists);
    }

    [Fact]
    public void InvalidateUserBlocks_WithEmptyUserId_DoesNotThrow()
    {
        _service.InvalidateUserBlocks(Guid.Empty);
        // Não deve lançar exceção
    }

    [Fact]
    public async Task InvalidateBlockAsync_WithEmptyUserIds_DoesNotThrow()
    {
        await _service.InvalidateBlockAsync(Guid.Empty, Guid.Empty, CancellationToken.None);
        // Não deve lançar exceção
    }

    [Fact]
    public async Task InvalidateBlockAsync_InvalidatesBothUsers()
    {
        var blockerUserId = Guid.NewGuid();
        var blockedUserId = Guid.NewGuid();

        var cacheKey1 = $"userblocks:{blockerUserId}";
        var cacheKey2 = $"userblocks:{blockedUserId}";

        await _cache.SetAsync(cacheKey1, new[] { blockedUserId }, TimeSpan.FromMinutes(5), CancellationToken.None);
        await _cache.SetAsync(cacheKey2, new[] { blockerUserId }, TimeSpan.FromMinutes(5), CancellationToken.None);

        // Verificar que ambos estão em cache
        Assert.True(await _cache.ExistsAsync(cacheKey1, CancellationToken.None));
        Assert.True(await _cache.ExistsAsync(cacheKey2, CancellationToken.None));

        // Invalidar
        await _service.InvalidateBlockAsync(blockerUserId, blockedUserId, CancellationToken.None);

        // Verificar que ambos foram removidos
        Assert.False(await _cache.ExistsAsync(cacheKey1, CancellationToken.None));
        Assert.False(await _cache.ExistsAsync(cacheKey2, CancellationToken.None));
    }

    [Fact]
    public void InvalidateBlock_WithEmptyUserIds_DoesNotThrow()
    {
        _service.InvalidateBlock(Guid.Empty, Guid.Empty);
        // Não deve lançar exceção
    }

    [Fact]
    public async Task GetBlockedUserIdsAsync_WithMultipleBlocks_ReturnsAll()
    {
        var userId = Guid.NewGuid();
        var blockedUserId1 = Guid.NewGuid();
        var blockedUserId2 = Guid.NewGuid();
        var blockedUserId3 = Guid.NewGuid();

        await _blockRepository.AddAsync(new UserBlock(userId, blockedUserId1, DateTime.UtcNow), CancellationToken.None);
        await _blockRepository.AddAsync(new UserBlock(userId, blockedUserId2, DateTime.UtcNow), CancellationToken.None);
        await _blockRepository.AddAsync(new UserBlock(userId, blockedUserId3, DateTime.UtcNow), CancellationToken.None);

        var result = await _service.GetBlockedUserIdsAsync(userId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(blockedUserId1, result);
        Assert.Contains(blockedUserId2, result);
        Assert.Contains(blockedUserId3, result);
    }

    [Fact]
    public async Task GetBlockedUserIdsAsync_AfterInvalidation_ReloadsFromRepository()
    {
        var userId = Guid.NewGuid();
        var blockedUserId1 = Guid.NewGuid();
        var blockedUserId2 = Guid.NewGuid();

        // Criar primeiro bloco
        await _blockRepository.AddAsync(new UserBlock(userId, blockedUserId1, DateTime.UtcNow), CancellationToken.None);

        // Popular cache
        var result1 = await _service.GetBlockedUserIdsAsync(userId, CancellationToken.None);
        Assert.Single(result1);

        // Invalidar cache
        await _service.InvalidateUserBlocksAsync(userId, CancellationToken.None);

        // Adicionar novo bloco
        await _blockRepository.AddAsync(new UserBlock(userId, blockedUserId2, DateTime.UtcNow), CancellationToken.None);

        // Buscar novamente (deve recarregar do repositório)
        var result2 = await _service.GetBlockedUserIdsAsync(userId, CancellationToken.None);
        Assert.Equal(2, result2.Count);
        Assert.Contains(blockedUserId1, result2);
        Assert.Contains(blockedUserId2, result2);
    }
}
