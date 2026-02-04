using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Modules.Map.Domain;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class MapEntityCacheServiceEdgeCasesTests
{
    private readonly InMemoryDataStore _dataStore;
    private readonly InMemoryMapRepository _mapRepository;
    private readonly IDistributedCacheService _cache;
    private readonly Mock<ILogger<CacheMetricsService>> _loggerMock;
    private readonly CacheMetricsService _metrics;
    private readonly MapEntityCacheService _service;

    public MapEntityCacheServiceEdgeCasesTests()
    {
        _dataStore = new InMemoryDataStore();
        _mapRepository = new InMemoryMapRepository(_dataStore);
        _cache = CacheTestHelper.CreateDistributedCacheService();
        _loggerMock = new Mock<ILogger<CacheMetricsService>>();
        _metrics = new CacheMetricsService(_loggerMock.Object);
        _service = new MapEntityCacheService(_mapRepository, _cache, _metrics);
    }

    [Fact]
    public async Task GetEntitiesByTerritoryAsync_WithEmptyTerritoryId_ReturnsEmptyList()
    {
        var result = await _service.GetEntitiesByTerritoryAsync(Guid.Empty, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetEntitiesByTerritoryAsync_WithNonExistentTerritory_ReturnsEmptyList()
    {
        var result = await _service.GetEntitiesByTerritoryAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetEntitiesByTerritoryAsync_WithCachedData_ReturnsCachedData()
    {
        var territoryId = Guid.NewGuid();
        var entity1 = new MapEntity(
            Guid.NewGuid(),
            territoryId,
            Guid.NewGuid(),
            "Entity 1",
            "estabelecimento",
            0.0,
            0.0,
            MapEntityStatus.Validated,
            MapEntityVisibility.Public,
            0,
            DateTime.UtcNow);
        var entity2 = new MapEntity(
            Guid.NewGuid(),
            territoryId,
            Guid.NewGuid(),
            "Entity 2",
            "órgão do governo",
            0.0,
            0.0,
            MapEntityStatus.Validated,
            MapEntityVisibility.Public,
            0,
            DateTime.UtcNow);
        var entities = new[] { entity1, entity2 };

        // Popular cache diretamente
        var cacheKey = $"mapentities:{territoryId}";
        await _cache.SetAsync(cacheKey, entities, TimeSpan.FromMinutes(5), CancellationToken.None);

        var result = await _service.GetEntitiesByTerritoryAsync(territoryId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(entity1, result);
        Assert.Contains(entity2, result);
    }

    [Fact]
    public async Task GetEntitiesByTerritoryAsync_WithoutCache_LoadsFromRepository()
    {
        var territoryId = Guid.NewGuid();
        var entity = new MapEntity(
            Guid.NewGuid(),
            territoryId,
            Guid.NewGuid(),
            "Entity",
            "estabelecimento",
            0.0,
            0.0,
            MapEntityStatus.Validated,
            MapEntityVisibility.Public,
            0,
            DateTime.UtcNow);

        await _mapRepository.AddAsync(entity, CancellationToken.None);

        var result = await _service.GetEntitiesByTerritoryAsync(territoryId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(entity, result);
    }

    [Fact]
    public async Task GetEntitiesByTerritoryAsync_RecordsCacheHit_WhenCached()
    {
        var territoryId = Guid.NewGuid();
        var entities = new[] { new MapEntity(
            Guid.NewGuid(),
            territoryId,
            Guid.NewGuid(),
            "Entity",
            "estabelecimento",
            0.0,
            0.0,
            MapEntityStatus.Validated,
            MapEntityVisibility.Public,
            0,
            DateTime.UtcNow) };

        var cacheKey = $"mapentities:{territoryId}";
        await _cache.SetAsync(cacheKey, entities, TimeSpan.FromMinutes(5), CancellationToken.None);

        await _service.GetEntitiesByTerritoryAsync(territoryId, CancellationToken.None);

        var metrics = _metrics.GetMetrics();
        Assert.True(metrics.CacheHits > 0);
    }

    [Fact]
    public async Task GetEntitiesByTerritoryAsync_RecordsCacheMiss_WhenNotCached()
    {
        var territoryId = Guid.NewGuid();

        await _service.GetEntitiesByTerritoryAsync(territoryId, CancellationToken.None);

        var metrics = _metrics.GetMetrics();
        Assert.True(metrics.CacheMisses > 0);
    }

    [Fact]
    public async Task InvalidateTerritoryEntitiesAsync_WithEmptyTerritoryId_DoesNotThrow()
    {
        await _service.InvalidateTerritoryEntitiesAsync(Guid.Empty, CancellationToken.None);
        // Não deve lançar exceção
    }

    [Fact]
    public async Task InvalidateTerritoryEntitiesAsync_WithNonExistentTerritory_DoesNotThrow()
    {
        await _service.InvalidateTerritoryEntitiesAsync(Guid.NewGuid(), CancellationToken.None);
        // Não deve lançar exceção
    }

    [Fact]
    public async Task InvalidateTerritoryEntitiesAsync_RemovesCache()
    {
        var territoryId = Guid.NewGuid();
        var entities = new[] { new MapEntity(
            Guid.NewGuid(),
            territoryId,
            Guid.NewGuid(),
            "Entity",
            "estabelecimento",
            0.0,
            0.0,
            MapEntityStatus.Validated,
            MapEntityVisibility.Public,
            0,
            DateTime.UtcNow) };

        var cacheKey = $"mapentities:{territoryId}";
        await _cache.SetAsync(cacheKey, entities, TimeSpan.FromMinutes(5), CancellationToken.None);

        // Verificar que está em cache
        var exists = await _cache.ExistsAsync(cacheKey, CancellationToken.None);
        Assert.True(exists);

        // Invalidar
        await _service.InvalidateTerritoryEntitiesAsync(territoryId, CancellationToken.None);

        // Verificar que foi removido
        var stillExists = await _cache.ExistsAsync(cacheKey, CancellationToken.None);
        Assert.False(stillExists);
    }

    [Fact]
    public void InvalidateTerritoryEntities_WithEmptyTerritoryId_DoesNotThrow()
    {
        _service.InvalidateTerritoryEntities(Guid.Empty);
        // Não deve lançar exceção
    }

    [Fact]
    public async Task GetEntitiesByTerritoryAsync_WithMultipleEntities_ReturnsAll()
    {
        var territoryId = Guid.NewGuid();
        var entity1 = new MapEntity(
            Guid.NewGuid(),
            territoryId,
            Guid.NewGuid(),
            "Entity 1",
            "estabelecimento",
            0.0,
            0.0,
            MapEntityStatus.Validated,
            MapEntityVisibility.Public,
            0,
            DateTime.UtcNow);
        var entity2 = new MapEntity(
            Guid.NewGuid(),
            territoryId,
            Guid.NewGuid(),
            "Entity 2",
            "órgão do governo",
            0.0,
            0.0,
            MapEntityStatus.Validated,
            MapEntityVisibility.Public,
            0,
            DateTime.UtcNow);
        var entity3 = new MapEntity(
            Guid.NewGuid(),
            territoryId,
            Guid.NewGuid(),
            "Entity 3",
            "espaço público",
            0.0,
            0.0,
            MapEntityStatus.Validated,
            MapEntityVisibility.Public,
            0,
            DateTime.UtcNow);

        await _mapRepository.AddAsync(entity1, CancellationToken.None);
        await _mapRepository.AddAsync(entity2, CancellationToken.None);
        await _mapRepository.AddAsync(entity3, CancellationToken.None);

        var result = await _service.GetEntitiesByTerritoryAsync(territoryId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(entity1, result);
        Assert.Contains(entity2, result);
        Assert.Contains(entity3, result);
    }

    [Fact]
    public async Task GetEntitiesByTerritoryAsync_AfterInvalidation_ReloadsFromRepository()
    {
        var territoryId = Guid.NewGuid();
        var entity1 = new MapEntity(
            Guid.NewGuid(),
            territoryId,
            Guid.NewGuid(),
            "Entity 1",
            "estabelecimento",
            0.0,
            0.0,
            MapEntityStatus.Validated,
            MapEntityVisibility.Public,
            0,
            DateTime.UtcNow);

        // Criar primeira entidade
        await _mapRepository.AddAsync(entity1, CancellationToken.None);

        // Popular cache
        var result1 = await _service.GetEntitiesByTerritoryAsync(territoryId, CancellationToken.None);
        Assert.Single(result1);

        // Invalidar cache
        await _service.InvalidateTerritoryEntitiesAsync(territoryId, CancellationToken.None);

        // Adicionar nova entidade
        var entity2 = new MapEntity(
            Guid.NewGuid(),
            territoryId,
            Guid.NewGuid(),
            "Entity 2",
            "órgão do governo",
            0.0,
            0.0,
            MapEntityStatus.Validated,
            MapEntityVisibility.Public,
            0,
            DateTime.UtcNow);
        await _mapRepository.AddAsync(entity2, CancellationToken.None);

        // Buscar novamente (deve recarregar do repositório)
        var result2 = await _service.GetEntitiesByTerritoryAsync(territoryId, CancellationToken.None);
        Assert.Equal(2, result2.Count);
        Assert.Contains(entity1, result2);
        Assert.Contains(entity2, result2);
    }

    [Fact]
    public async Task GetEntitiesByTerritoryAsync_WithDifferentTerritories_ReturnsSeparateCaches()
    {
        var territoryId1 = Guid.NewGuid();
        var territoryId2 = Guid.NewGuid();

        var entity1 = new MapEntity(
            Guid.NewGuid(),
            territoryId1,
            Guid.NewGuid(),
            "Entity 1",
            "estabelecimento",
            0.0,
            0.0,
            MapEntityStatus.Validated,
            MapEntityVisibility.Public,
            0,
            DateTime.UtcNow);
        var entity2 = new MapEntity(
            Guid.NewGuid(),
            territoryId2,
            Guid.NewGuid(),
            "Entity 2",
            "órgão do governo",
            0.0,
            0.0,
            MapEntityStatus.Validated,
            MapEntityVisibility.Public,
            0,
            DateTime.UtcNow);

        await _mapRepository.AddAsync(entity1, CancellationToken.None);
        await _mapRepository.AddAsync(entity2, CancellationToken.None);

        var result1 = await _service.GetEntitiesByTerritoryAsync(territoryId1, CancellationToken.None);
        var result2 = await _service.GetEntitiesByTerritoryAsync(territoryId2, CancellationToken.None);

        Assert.Single(result1);
        Assert.Single(result2);
        Assert.Contains(entity1, result1);
        Assert.Contains(entity2, result2);
        Assert.DoesNotContain(entity1, result2);
        Assert.DoesNotContain(entity2, result1);
    }
}
