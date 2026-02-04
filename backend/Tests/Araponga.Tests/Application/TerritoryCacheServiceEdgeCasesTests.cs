using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Territories;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
using Araponga.Tests.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for TerritoryCacheService.
/// Tests are isolated - each test cleans up data and cache before execution.
/// </summary>
public sealed class TerritoryCacheServiceEdgeCasesTests
{
    private readonly InMemorySharedStore _sharedStore;
    private readonly InMemoryTerritoryRepository _territoryRepository;
    private readonly IDistributedCacheService _cache;
    private readonly Mock<ILogger<CacheMetricsService>> _loggerMock;
    private readonly CacheMetricsService _metrics;
    private readonly TerritoryCacheService _service;

    public TerritoryCacheServiceEdgeCasesTests()
    {
        _sharedStore = new InMemorySharedStore();
        _territoryRepository = new InMemoryTerritoryRepository(_sharedStore);
        _cache = CacheTestHelper.CreateDistributedCacheService();
        _loggerMock = new Mock<ILogger<CacheMetricsService>>();
        _metrics = new CacheMetricsService(_loggerMock.Object);
        _service = new TerritoryCacheService(_territoryRepository, _cache, _metrics);
    }

    /// <summary>
    /// Cleans up test data before each test to ensure isolation.
    /// Removes all territories and clears the cache.
    /// </summary>
    private async Task CleanupAsync()
    {
        // Clear all territories from data store
        _sharedStore.Territories.Clear();

        // Clear cache
        await _cache.RemoveAsync(Constants.CacheKeys.ActiveTerritories, CancellationToken.None);
        
        // Reset metrics
        _metrics.Reset();
    }

    [Fact]
    public async Task GetActiveTerritoriesAsync_WithNoTerritories_ReturnsEmptyList()
    {
        // Arrange
        await CleanupAsync();

        // Act
        var result = await _service.GetActiveTerritoriesAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetActiveTerritoriesAsync_WithCachedData_ReturnsCachedData()
    {
        // Arrange
        await CleanupAsync();
        var territory1 = new Territory(
            Guid.NewGuid(),
            null,
            "Territory 1",
            "Description 1",
            TerritoryStatus.Active,
            "City 1",
            "ST",
            0.0,
            0.0,
            DateTime.UtcNow);
        var territory2 = new Territory(
            Guid.NewGuid(),
            null,
            "Territory 2",
            "Description 2",
            TerritoryStatus.Active,
            "City 2",
            "ST",
            0.0,
            0.0,
            DateTime.UtcNow);
        var territories = new[] { territory1, territory2 };

        // Popular cache diretamente
        var cacheKey = Constants.CacheKeys.ActiveTerritories;
        await _cache.SetAsync(cacheKey, territories, TimeSpan.FromMinutes(5), CancellationToken.None);

        // Act
        var result = await _service.GetActiveTerritoriesAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(territory1, result);
        Assert.Contains(territory2, result);
    }

    [Fact]
    public async Task GetActiveTerritoriesAsync_WithoutCache_LoadsFromRepository()
    {
        // Arrange
        await CleanupAsync();
        var territory = new Territory(
            Guid.NewGuid(),
            null,
            "Active Territory",
            "Description",
            TerritoryStatus.Active,
            "City",
            "ST",
            0.0,
            0.0,
            DateTime.UtcNow);

        await _territoryRepository.AddAsync(territory, CancellationToken.None);

        // Act
        var result = await _service.GetActiveTerritoriesAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(territory, result);
    }

    [Fact]
    public async Task GetActiveTerritoriesAsync_FiltersOnlyActiveTerritories()
    {
        // Arrange
        await CleanupAsync();
        var activeTerritory = new Territory(
            Guid.NewGuid(),
            null,
            "Active",
            "Desc",
            TerritoryStatus.Active,
            "City",
            "ST",
            0.0,
            0.0,
            DateTime.UtcNow);
        var inactiveTerritory = new Territory(
            Guid.NewGuid(),
            null,
            "Inactive",
            "Desc",
            TerritoryStatus.Inactive,
            "City",
            "ST",
            0.0,
            0.0,
            DateTime.UtcNow);

        await _territoryRepository.AddAsync(activeTerritory, CancellationToken.None);
        await _territoryRepository.AddAsync(inactiveTerritory, CancellationToken.None);

        // Act
        var result = await _service.GetActiveTerritoriesAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(activeTerritory, result);
        Assert.DoesNotContain(inactiveTerritory, result);
    }

    [Fact]
    public async Task GetActiveTerritoriesAsync_SortsByName()
    {
        // Arrange
        await CleanupAsync();
        var territory1 = new Territory(
            Guid.NewGuid(),
            null,
            "Zebra Territory",
            "Desc",
            TerritoryStatus.Active,
            "City",
            "ST",
            0.0,
            0.0,
            DateTime.UtcNow);
        var territory2 = new Territory(
            Guid.NewGuid(),
            null,
            "Alpha Territory",
            "Desc",
            TerritoryStatus.Active,
            "City",
            "ST",
            0.0,
            0.0,
            DateTime.UtcNow);
        var territory3 = new Territory(
            Guid.NewGuid(),
            null,
            "Beta Territory",
            "Desc",
            TerritoryStatus.Active,
            "City",
            "ST",
            0.0,
            0.0,
            DateTime.UtcNow);

        await _territoryRepository.AddAsync(territory1, CancellationToken.None);
        await _territoryRepository.AddAsync(territory2, CancellationToken.None);
        await _territoryRepository.AddAsync(territory3, CancellationToken.None);

        // Act
        var result = await _service.GetActiveTerritoriesAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Equal("Alpha Territory", result[0].Name);
        Assert.Equal("Beta Territory", result[1].Name);
        Assert.Equal("Zebra Territory", result[2].Name);
    }

    [Fact]
    public async Task GetActiveTerritoriesAsync_RecordsCacheHit_WhenCached()
    {
        // Arrange
        await CleanupAsync();
        var territories = new[] { new Territory(
            Guid.NewGuid(),
            null,
            "Test",
            "Desc",
            TerritoryStatus.Active,
            "City",
            "ST",
            0.0,
            0.0,
            DateTime.UtcNow) };

        var cacheKey = Constants.CacheKeys.ActiveTerritories;
        await _cache.SetAsync(cacheKey, territories, TimeSpan.FromMinutes(5), CancellationToken.None);

        // Act
        await _service.GetActiveTerritoriesAsync(CancellationToken.None);

        // Assert
        var metrics = _metrics.GetMetrics();
        Assert.True(metrics.CacheHits > 0);
    }

    [Fact]
    public async Task GetActiveTerritoriesAsync_RecordsCacheMiss_WhenNotCached()
    {
        await _service.GetActiveTerritoriesAsync(CancellationToken.None);

        var metrics = _metrics.GetMetrics();
        Assert.True(metrics.CacheMisses > 0);
    }

    [Fact]
    public async Task InvalidateActiveTerritoriesAsync_RemovesCache()
    {
        // Arrange
        await CleanupAsync();
        var territories = new[] { new Territory(
            Guid.NewGuid(),
            null,
            "Test",
            "Desc",
            TerritoryStatus.Active,
            "City",
            "ST",
            0.0,
            0.0,
            DateTime.UtcNow) };

        var cacheKey = Constants.CacheKeys.ActiveTerritories;
        await _cache.SetAsync(cacheKey, territories, TimeSpan.FromMinutes(5), CancellationToken.None);

        // Verificar que está em cache
        var exists = await _cache.ExistsAsync(cacheKey, CancellationToken.None);
        Assert.True(exists);

        // Act - Invalidar
        await _service.InvalidateActiveTerritoriesAsync(CancellationToken.None);

        // Assert - Verificar que foi removido
        var stillExists = await _cache.ExistsAsync(cacheKey, CancellationToken.None);
        Assert.False(stillExists);
    }

    [Fact]
    public void InvalidateActiveTerritories_DoesNotThrow()
    {
        _service.InvalidateActiveTerritories();
        // Não deve lançar exceção
    }

    [Fact]
    public async Task GetByIdAsync_WithEmptyId_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(Guid.Empty, CancellationToken.None, useCache: false);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None, useCache: false);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithoutCache_LoadsFromRepository()
    {
        // Arrange
        await CleanupAsync();
        var territory = new Territory(
            Guid.NewGuid(),
            null,
            "Test Territory",
            "Description",
            TerritoryStatus.Active,
            "City",
            "ST",
            0.0,
            0.0,
            DateTime.UtcNow);

        await _territoryRepository.AddAsync(territory, CancellationToken.None);

        // Act
        var result = await _service.GetByIdAsync(territory.Id, CancellationToken.None, useCache: false);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(territory.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithCache_ReturnsCachedData()
    {
        // Arrange
        await CleanupAsync();
        var territory = new Territory(
            Guid.NewGuid(),
            null,
            "Test Territory",
            "Description",
            TerritoryStatus.Active,
            "City",
            "ST",
            0.0,
            0.0,
            DateTime.UtcNow);

        // Popular cache diretamente
        var cacheKey = Constants.CacheKeys.Territory(territory.Id);
        await _cache.SetAsync(cacheKey, territory, TimeSpan.FromMinutes(5), CancellationToken.None);

        // Act
        var result = await _service.GetByIdAsync(territory.Id, CancellationToken.None, useCache: true);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(territory.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithCache_LoadsFromRepositoryIfNotCached()
    {
        // Arrange
        await CleanupAsync();
        var territory = new Territory(
            Guid.NewGuid(),
            null,
            "Test Territory",
            "Description",
            TerritoryStatus.Active,
            "City",
            "ST",
            0.0,
            0.0,
            DateTime.UtcNow);

        await _territoryRepository.AddAsync(territory, CancellationToken.None);

        // Act
        var result = await _service.GetByIdAsync(territory.Id, CancellationToken.None, useCache: true);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(territory.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WithCache_RecordsCacheHit_WhenCached()
    {
        // Arrange
        await CleanupAsync();
        var territory = new Territory(
            Guid.NewGuid(),
            null,
            "Test",
            "Desc",
            TerritoryStatus.Active,
            "City",
            "ST",
            0.0,
            0.0,
            DateTime.UtcNow);

        var cacheKey = Constants.CacheKeys.Territory(territory.Id);
        await _cache.SetAsync(cacheKey, territory, TimeSpan.FromMinutes(5), CancellationToken.None);

        // Act
        await _service.GetByIdAsync(territory.Id, CancellationToken.None, useCache: true);

        // Assert
        var metrics = _metrics.GetMetrics();
        Assert.True(metrics.CacheHits > 0);
    }

    [Fact]
    public async Task GetByIdAsync_WithCache_RecordsCacheMiss_WhenNotCached()
    {
        // Arrange
        await CleanupAsync();
        var territory = new Territory(
            Guid.NewGuid(),
            null,
            "Test",
            "Desc",
            TerritoryStatus.Active,
            "City",
            "ST",
            0.0,
            0.0,
            DateTime.UtcNow);

        await _territoryRepository.AddAsync(territory, CancellationToken.None);

        // Act
        await _service.GetByIdAsync(territory.Id, CancellationToken.None, useCache: true);

        // Assert
        var metrics = _metrics.GetMetrics();
        Assert.True(metrics.CacheMisses > 0);
    }

    [Fact]
    public async Task InvalidateTerritoryAsync_WithEmptyTerritoryId_DoesNotThrow()
    {
        await _service.InvalidateTerritoryAsync(Guid.Empty, CancellationToken.None);
        // Não deve lançar exceção
    }

    [Fact]
    public async Task InvalidateTerritoryAsync_RemovesTerritoryAndActiveListCache()
    {
        // Arrange
        await CleanupAsync();
        var territory = new Territory(
            Guid.NewGuid(),
            null,
            "Test",
            "Desc",
            TerritoryStatus.Active,
            "City",
            "ST",
            0.0,
            0.0,
            DateTime.UtcNow);

        var territoryCacheKey = Constants.CacheKeys.Territory(territory.Id);
        var activeListCacheKey = Constants.CacheKeys.ActiveTerritories;

        await _cache.SetAsync(territoryCacheKey, territory, TimeSpan.FromMinutes(5), CancellationToken.None);
        await _cache.SetAsync(activeListCacheKey, new[] { territory }, TimeSpan.FromMinutes(5), CancellationToken.None);

        // Act - Invalidar
        await _service.InvalidateTerritoryAsync(territory.Id, CancellationToken.None);

        // Assert - Verificar que ambos foram removidos
        Assert.False(await _cache.ExistsAsync(territoryCacheKey, CancellationToken.None));
        Assert.False(await _cache.ExistsAsync(activeListCacheKey, CancellationToken.None));
    }

    [Fact]
    public void InvalidateTerritory_WithEmptyTerritoryId_DoesNotThrow()
    {
        _service.InvalidateTerritory(Guid.Empty);
        // Não deve lançar exceção
    }

    [Fact]
    public async Task GetActiveTerritoriesAsync_AfterInvalidation_ReloadsFromRepository()
    {
        // Arrange
        await CleanupAsync();
        var territory1 = new Territory(
            Guid.NewGuid(),
            null,
            "Territory 1",
            "Desc",
            TerritoryStatus.Active,
            "City",
            "ST",
            0.0,
            0.0,
            DateTime.UtcNow);

        // Criar primeiro território
        await _territoryRepository.AddAsync(territory1, CancellationToken.None);

        // Popular cache
        var result1 = await _service.GetActiveTerritoriesAsync(CancellationToken.None);
        Assert.Single(result1);

        // Invalidar cache
        await _service.InvalidateActiveTerritoriesAsync(CancellationToken.None);

        // Adicionar novo território
        var territory2 = new Territory(
            Guid.NewGuid(),
            null,
            "Territory 2",
            "Desc",
            TerritoryStatus.Active,
            "City",
            "ST",
            0.0,
            0.0,
            DateTime.UtcNow);
        await _territoryRepository.AddAsync(territory2, CancellationToken.None);

        // Act - Buscar novamente (deve recarregar do repositório)
        var result2 = await _service.GetActiveTerritoriesAsync(CancellationToken.None);

        // Assert
        Assert.Equal(2, result2.Count);
        Assert.Contains(territory1, result2);
        Assert.Contains(territory2, result2);
    }
}
