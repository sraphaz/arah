using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Health;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class AlertCacheServiceEdgeCasesTests
{
    private readonly InMemoryDataStore _dataStore;
    private readonly InMemoryHealthAlertRepository _alertRepository;
    private readonly IDistributedCacheService _cache;
    private readonly Mock<ILogger<CacheMetricsService>> _loggerMock;
    private readonly CacheMetricsService _metrics;
    private readonly AlertCacheService _service;

    public AlertCacheServiceEdgeCasesTests()
    {
        _dataStore = new InMemoryDataStore();
        _alertRepository = new InMemoryHealthAlertRepository(_dataStore);
        _cache = CacheTestHelper.CreateDistributedCacheService();
        _loggerMock = new Mock<ILogger<CacheMetricsService>>();
        _metrics = new CacheMetricsService(_loggerMock.Object);
        _service = new AlertCacheService(_alertRepository, _cache, _metrics);
    }

    [Fact]
    public async Task GetAlertsByTerritoryAsync_WithEmptyTerritoryId_ReturnsEmptyList()
    {
        var result = await _service.GetAlertsByTerritoryAsync(Guid.Empty, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAlertsByTerritoryAsync_WithNonExistentTerritory_ReturnsEmptyList()
    {
        var result = await _service.GetAlertsByTerritoryAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAlertsByTerritoryAsync_WithCachedData_ReturnsCachedData()
    {
        var territoryId = Guid.NewGuid();
        var reporterUserId = Guid.NewGuid();
        var alert1 = new HealthAlert(
            Guid.NewGuid(),
            territoryId,
            reporterUserId,
            "Test Alert 1",
            "Description 1",
            HealthAlertStatus.Pending,
            DateTime.UtcNow);
        var alert2 = new HealthAlert(
            Guid.NewGuid(),
            territoryId,
            reporterUserId,
            "Test Alert 2",
            "Description 2",
            HealthAlertStatus.Pending,
            DateTime.UtcNow);
        var alerts = new[] { alert1, alert2 };

        // Popular cache diretamente
        var cacheKey = $"alerts:{territoryId}";
        await _cache.SetAsync(cacheKey, alerts, TimeSpan.FromMinutes(5), CancellationToken.None);

        var result = await _service.GetAlertsByTerritoryAsync(territoryId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(alert1, result);
        Assert.Contains(alert2, result);
    }

    [Fact]
    public async Task GetAlertsByTerritoryAsync_WithoutCache_LoadsFromRepository()
    {
        var territoryId = Guid.NewGuid();
        var reporterUserId = Guid.NewGuid();
        var alert = new HealthAlert(
            Guid.NewGuid(),
            territoryId,
            reporterUserId,
            "Test Alert",
            "Description",
            HealthAlertStatus.Pending,
            DateTime.UtcNow);

        await _alertRepository.AddAsync(alert, CancellationToken.None);

        var result = await _service.GetAlertsByTerritoryAsync(territoryId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(alert, result);
    }

    [Fact]
    public async Task GetAlertsByTerritoryAsync_RecordsCacheHit_WhenCached()
    {
        var territoryId = Guid.NewGuid();
        var reporterUserId = Guid.NewGuid();
        var alerts = new[] { new HealthAlert(
            Guid.NewGuid(),
            territoryId,
            reporterUserId,
            "Test",
            "Desc",
            HealthAlertStatus.Pending,
            DateTime.UtcNow) };

        var cacheKey = $"alerts:{territoryId}";
        await _cache.SetAsync(cacheKey, alerts, TimeSpan.FromMinutes(5), CancellationToken.None);

        await _service.GetAlertsByTerritoryAsync(territoryId, CancellationToken.None);

        var metrics = _metrics.GetMetrics();
        Assert.True(metrics.CacheHits > 0);
    }

    [Fact]
    public async Task GetAlertsByTerritoryAsync_RecordsCacheMiss_WhenNotCached()
    {
        var territoryId = Guid.NewGuid();

        await _service.GetAlertsByTerritoryAsync(territoryId, CancellationToken.None);

        var metrics = _metrics.GetMetrics();
        Assert.True(metrics.CacheMisses > 0);
    }

    [Fact]
    public async Task InvalidateTerritoryAlertsAsync_WithEmptyTerritoryId_DoesNotThrow()
    {
        await _service.InvalidateTerritoryAlertsAsync(Guid.Empty, CancellationToken.None);
        // Não deve lançar exceção
    }

    [Fact]
    public async Task InvalidateTerritoryAlertsAsync_WithNonExistentTerritory_DoesNotThrow()
    {
        await _service.InvalidateTerritoryAlertsAsync(Guid.NewGuid(), CancellationToken.None);
        // Não deve lançar exceção
    }

    [Fact]
    public async Task InvalidateTerritoryAlertsAsync_RemovesCache()
    {
        var territoryId = Guid.NewGuid();
        var reporterUserId = Guid.NewGuid();
        var alerts = new[] { new HealthAlert(
            Guid.NewGuid(),
            territoryId,
            reporterUserId,
            "Test",
            "Desc",
            HealthAlertStatus.Pending,
            DateTime.UtcNow) };

        var cacheKey = $"alerts:{territoryId}";
        await _cache.SetAsync(cacheKey, alerts, TimeSpan.FromMinutes(5), CancellationToken.None);

        // Verificar que está em cache
        var exists = await _cache.ExistsAsync(cacheKey, CancellationToken.None);
        Assert.True(exists);

        // Invalidar
        await _service.InvalidateTerritoryAlertsAsync(territoryId, CancellationToken.None);

        // Verificar que foi removido
        var stillExists = await _cache.ExistsAsync(cacheKey, CancellationToken.None);
        Assert.False(stillExists);
    }

    [Fact]
    public void InvalidateTerritoryAlerts_WithEmptyTerritoryId_DoesNotThrow()
    {
        _service.InvalidateTerritoryAlerts(Guid.Empty);
        // Não deve lançar exceção
    }

    [Fact]
    public async Task GetAlertsByTerritoryAsync_WithMultipleAlerts_ReturnsAll()
    {
        var territoryId = Guid.NewGuid();
        var reporterUserId = Guid.NewGuid();
        var alert1 = new HealthAlert(
            Guid.NewGuid(),
            territoryId,
            reporterUserId,
            "Alert 1",
            "Desc 1",
            HealthAlertStatus.Pending,
            DateTime.UtcNow);
        var alert2 = new HealthAlert(
            Guid.NewGuid(),
            territoryId,
            reporterUserId,
            "Alert 2",
            "Desc 2",
            HealthAlertStatus.Pending,
            DateTime.UtcNow);
        var alert3 = new HealthAlert(
            Guid.NewGuid(),
            territoryId,
            reporterUserId,
            "Alert 3",
            "Desc 3",
            HealthAlertStatus.Validated,
            DateTime.UtcNow);

        await _alertRepository.AddAsync(alert1, CancellationToken.None);
        await _alertRepository.AddAsync(alert2, CancellationToken.None);
        await _alertRepository.AddAsync(alert3, CancellationToken.None);

        var result = await _service.GetAlertsByTerritoryAsync(territoryId, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(alert1, result);
        Assert.Contains(alert2, result);
        Assert.Contains(alert3, result);
    }

    [Fact]
    public async Task GetAlertsByTerritoryAsync_AfterInvalidation_ReloadsFromRepository()
    {
        var territoryId = Guid.NewGuid();
        var reporterUserId = Guid.NewGuid();
        var alert1 = new HealthAlert(
            Guid.NewGuid(),
            territoryId,
            reporterUserId,
            "Alert 1",
            "Desc 1",
            HealthAlertStatus.Pending,
            DateTime.UtcNow);

        // Criar primeiro alerta
        await _alertRepository.AddAsync(alert1, CancellationToken.None);

        // Popular cache
        var result1 = await _service.GetAlertsByTerritoryAsync(territoryId, CancellationToken.None);
        Assert.Single(result1);

        // Invalidar cache
        await _service.InvalidateTerritoryAlertsAsync(territoryId, CancellationToken.None);

        // Adicionar novo alerta
        var alert2 = new HealthAlert(
            Guid.NewGuid(),
            territoryId,
            reporterUserId,
            "Alert 2",
            "Desc 2",
            HealthAlertStatus.Pending,
            DateTime.UtcNow);
        await _alertRepository.AddAsync(alert2, CancellationToken.None);

        // Buscar novamente (deve recarregar do repositório)
        var result2 = await _service.GetAlertsByTerritoryAsync(territoryId, CancellationToken.None);
        Assert.Equal(2, result2.Count);
        Assert.Contains(alert1, result2);
        Assert.Contains(alert2, result2);
    }

    [Fact]
    public async Task GetAlertsByTerritoryAsync_WithDifferentTerritories_ReturnsSeparateCaches()
    {
        var territoryId1 = Guid.NewGuid();
        var territoryId2 = Guid.NewGuid();

        var reporterUserId1 = Guid.NewGuid();
        var reporterUserId2 = Guid.NewGuid();
        var alert1 = new HealthAlert(
            Guid.NewGuid(),
            territoryId1,
            reporterUserId1,
            "Alert 1",
            "Desc 1",
            HealthAlertStatus.Pending,
            DateTime.UtcNow);
        var alert2 = new HealthAlert(
            Guid.NewGuid(),
            territoryId2,
            reporterUserId2,
            "Alert 2",
            "Desc 2",
            HealthAlertStatus.Pending,
            DateTime.UtcNow);

        await _alertRepository.AddAsync(alert1, CancellationToken.None);
        await _alertRepository.AddAsync(alert2, CancellationToken.None);

        var result1 = await _service.GetAlertsByTerritoryAsync(territoryId1, CancellationToken.None);
        var result2 = await _service.GetAlertsByTerritoryAsync(territoryId2, CancellationToken.None);

        Assert.Single(result1);
        Assert.Single(result2);
        Assert.Contains(alert1, result1);
        Assert.Contains(alert2, result2);
        Assert.DoesNotContain(alert1, result2);
        Assert.DoesNotContain(alert2, result1);
    }
}
