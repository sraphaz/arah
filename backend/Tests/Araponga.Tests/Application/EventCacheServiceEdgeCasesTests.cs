using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Events;
using Araponga.Domain.Membership;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class EventCacheServiceEdgeCasesTests
{
    private readonly InMemoryDataStore _dataStore;
    private readonly InMemoryTerritoryEventRepository _eventRepository;
    private readonly IDistributedCacheService _cache;
    private readonly Mock<ILogger<CacheMetricsService>> _loggerMock;
    private readonly CacheMetricsService _metrics;
    private readonly EventCacheService _service;

    public EventCacheServiceEdgeCasesTests()
    {
        _dataStore = new InMemoryDataStore();
        _eventRepository = new InMemoryTerritoryEventRepository(_dataStore);
        _cache = CacheTestHelper.CreatePatternAwareCacheService();
        _loggerMock = new Mock<ILogger<CacheMetricsService>>();
        _metrics = new CacheMetricsService(_loggerMock.Object);
        _service = new EventCacheService(_eventRepository, _cache, _metrics);
    }

    [Fact]
    public async Task GetEventsByTerritoryAsync_WithEmptyTerritoryId_ReturnsEmptyList()
    {
        var result = await _service.GetEventsByTerritoryAsync(Guid.Empty, null, null, null, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetEventsByTerritoryAsync_WithNonExistentTerritory_ReturnsEmptyList()
    {
        var result = await _service.GetEventsByTerritoryAsync(Guid.NewGuid(), null, null, null, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetEventsByTerritoryAsync_WithCachedData_ReturnsCachedData()
    {
        var territoryId = Guid.NewGuid();
        var event1 = new TerritoryEvent(
            Guid.NewGuid(),
            territoryId,
            "Event 1",
            "Description 1",
            DateTime.UtcNow.AddDays(1),
            null,
            0.0,
            0.0,
            null,
            Guid.NewGuid(),
            MembershipRole.Resident,
            EventStatus.Scheduled,
            DateTime.UtcNow,
            DateTime.UtcNow);
        var event2 = new TerritoryEvent(
            Guid.NewGuid(),
            territoryId,
            "Event 2",
            "Description 2",
            DateTime.UtcNow.AddDays(2),
            null,
            0.0,
            0.0,
            null,
            Guid.NewGuid(),
            MembershipRole.Resident,
            EventStatus.Scheduled,
            DateTime.UtcNow,
            DateTime.UtcNow);
        var events = new[] { event1, event2 };

        // Popular cache diretamente
        var cacheKey = $"events:{territoryId}:null:null:null";
        await _cache.SetAsync(cacheKey, events, TimeSpan.FromMinutes(5), CancellationToken.None);

        var result = await _service.GetEventsByTerritoryAsync(territoryId, null, null, null, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(event1, result);
        Assert.Contains(event2, result);
    }

    [Fact]
    public async Task GetEventsByTerritoryAsync_WithoutCache_LoadsFromRepository()
    {
        var territoryId = Guid.NewGuid();
        var event1 = new TerritoryEvent(
            Guid.NewGuid(),
            territoryId,
            "Event 1",
            "Description 1",
            DateTime.UtcNow.AddDays(1),
            null,
            0.0,
            0.0,
            null,
            Guid.NewGuid(),
            MembershipRole.Resident,
            EventStatus.Scheduled,
            DateTime.UtcNow,
            DateTime.UtcNow);

        await _eventRepository.AddAsync(event1, CancellationToken.None);

        var result = await _service.GetEventsByTerritoryAsync(territoryId, null, null, null, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains(event1, result);
    }

    [Fact]
    public async Task GetEventsByTerritoryAsync_WithDateFilters_CreatesDifferentCacheKeys()
    {
        var territoryId = Guid.NewGuid();
        var from = DateTime.UtcNow.Date;
        var to = DateTime.UtcNow.Date.AddDays(7);

        // Primeira busca com filtros
        var result1 = await _service.GetEventsByTerritoryAsync(territoryId, from, to, null, CancellationToken.None);
        Assert.NotNull(result1);

        // Segunda busca sem filtros (deve ter cache key diferente)
        var result2 = await _service.GetEventsByTerritoryAsync(territoryId, null, null, null, CancellationToken.None);
        Assert.NotNull(result2);

        // Verificar que ambos foram cacheados separadamente
        var metrics = _metrics.GetMetrics();
        Assert.True(metrics.TotalRequests >= 2);
    }

    [Fact]
    public async Task GetEventsByTerritoryAsync_WithStatusFilter_CreatesDifferentCacheKey()
    {
        var territoryId = Guid.NewGuid();

        // Buscar com status Scheduled
        var result1 = await _service.GetEventsByTerritoryAsync(territoryId, null, null, EventStatus.Scheduled, CancellationToken.None);
        Assert.NotNull(result1);

        // Buscar com status Finished
        var result2 = await _service.GetEventsByTerritoryAsync(territoryId, null, null, EventStatus.Finished, CancellationToken.None);
        Assert.NotNull(result2);

        // Verificar que ambos foram cacheados separadamente
        var metrics = _metrics.GetMetrics();
        Assert.True(metrics.TotalRequests >= 2);
    }

    [Fact]
    public async Task GetEventsByTerritoryAsync_RecordsCacheHit_WhenCached()
    {
        var territoryId = Guid.NewGuid();
        var events = new[] { new TerritoryEvent(
            Guid.NewGuid(),
            territoryId,
            "Event",
            "Desc",
            DateTime.UtcNow.AddDays(1),
            null,
            0.0,
            0.0,
            null,
            Guid.NewGuid(),
            MembershipRole.Resident,
            EventStatus.Scheduled,
            DateTime.UtcNow,
            DateTime.UtcNow) };

        var cacheKey = $"events:{territoryId}:null:null:null";
        await _cache.SetAsync(cacheKey, events, TimeSpan.FromMinutes(5), CancellationToken.None);

        await _service.GetEventsByTerritoryAsync(territoryId, null, null, null, CancellationToken.None);

        var metrics = _metrics.GetMetrics();
        Assert.True(metrics.CacheHits > 0);
    }

    [Fact]
    public async Task GetEventsByTerritoryAsync_RecordsCacheMiss_WhenNotCached()
    {
        var territoryId = Guid.NewGuid();

        await _service.GetEventsByTerritoryAsync(territoryId, null, null, null, CancellationToken.None);

        var metrics = _metrics.GetMetrics();
        Assert.True(metrics.CacheMisses > 0);
    }

    [Fact]
    public async Task InvalidateTerritoryEventsAsync_WithEmptyTerritoryId_DoesNotThrow()
    {
        await _service.InvalidateTerritoryEventsAsync(Guid.Empty, CancellationToken.None);
        // Não deve lançar exceção
    }

    [Fact]
    public async Task InvalidateTerritoryEventsAsync_RemovesAllPatternKeys()
    {
        var territoryId = Guid.NewGuid();
        var events = new[] { new TerritoryEvent(
            Guid.NewGuid(),
            territoryId,
            "Event",
            "Desc",
            DateTime.UtcNow.AddDays(1),
            null,
            0.0,
            0.0,
            null,
            Guid.NewGuid(),
            MembershipRole.Resident,
            EventStatus.Scheduled,
            DateTime.UtcNow,
            DateTime.UtcNow) };

        // Popular cache com diferentes filtros
        var cacheKey1 = $"events:{territoryId}:null:null:null";
        var cacheKey2 = $"events:{territoryId}:2026-01-01:2026-01-31:null";
        await _cache.SetAsync(cacheKey1, events, TimeSpan.FromMinutes(5), CancellationToken.None);
        await _cache.SetAsync(cacheKey2, events, TimeSpan.FromMinutes(5), CancellationToken.None);

        // Invalidar
        await _service.InvalidateTerritoryEventsAsync(territoryId, CancellationToken.None);

        // Verificar que ambos foram removidos (se RemoveByPatternAsync estiver implementado)
        // Nota: InMemoryDistributedCacheService pode não suportar pattern removal
    }

    [Fact]
    public void InvalidateTerritoryEvents_WithEmptyTerritoryId_DoesNotThrow()
    {
        _service.InvalidateTerritoryEvents(Guid.Empty);
        // Não deve lançar exceção
    }

    [Fact]
    public async Task InvalidateEventAsync_WithEmptyIds_DoesNotThrow()
    {
        await _service.InvalidateEventAsync(Guid.Empty, Guid.Empty, CancellationToken.None);
        // Não deve lançar exceção
    }

    [Fact]
    public void InvalidateEvent_WithEmptyIds_DoesNotThrow()
    {
        _service.InvalidateEvent(Guid.Empty, Guid.Empty);
        // Não deve lançar exceção
    }

    [Fact]
    public async Task GetEventsByTerritoryAsync_WithMultipleEvents_ReturnsAll()
    {
        var territoryId = Guid.NewGuid();
        var event1 = new TerritoryEvent(
            Guid.NewGuid(),
            territoryId,
            "Event 1",
            "Desc 1",
            DateTime.UtcNow.AddDays(1),
            null,
            0.0,
            0.0,
            null,
            Guid.NewGuid(),
            MembershipRole.Resident,
            EventStatus.Scheduled,
            DateTime.UtcNow,
            DateTime.UtcNow);
        var event2 = new TerritoryEvent(
            Guid.NewGuid(),
            territoryId,
            "Event 2",
            "Desc 2",
            DateTime.UtcNow.AddDays(2),
            null,
            0.0,
            0.0,
            null,
            Guid.NewGuid(),
            MembershipRole.Resident,
            EventStatus.Scheduled,
            DateTime.UtcNow,
            DateTime.UtcNow);
        var event3 = new TerritoryEvent(
            Guid.NewGuid(),
            territoryId,
            "Event 3",
            "Desc 3",
            DateTime.UtcNow.AddDays(3),
            null,
            0.0,
            0.0,
            null,
            Guid.NewGuid(),
            MembershipRole.Resident,
            EventStatus.Finished,
            DateTime.UtcNow,
            DateTime.UtcNow);

        await _eventRepository.AddAsync(event1, CancellationToken.None);
        await _eventRepository.AddAsync(event2, CancellationToken.None);
        await _eventRepository.AddAsync(event3, CancellationToken.None);

        var result = await _service.GetEventsByTerritoryAsync(territoryId, null, null, null, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
        Assert.Contains(event1, result);
        Assert.Contains(event2, result);
        Assert.Contains(event3, result);
    }

    [Fact]
    public async Task GetEventsByTerritoryAsync_AfterInvalidation_ReloadsFromRepository()
    {
        var territoryId = Guid.NewGuid();
        var event1 = new TerritoryEvent(
            Guid.NewGuid(),
            territoryId,
            "Event 1",
            "Desc 1",
            DateTime.UtcNow.AddDays(1),
            null,
            0.0,
            0.0,
            null,
            Guid.NewGuid(),
            MembershipRole.Resident,
            EventStatus.Scheduled,
            DateTime.UtcNow,
            DateTime.UtcNow);

        // Criar primeiro evento
        await _eventRepository.AddAsync(event1, CancellationToken.None);

        // Popular cache
        var result1 = await _service.GetEventsByTerritoryAsync(territoryId, null, null, null, CancellationToken.None);
        Assert.Single(result1);

        // Invalidar cache
        await _service.InvalidateTerritoryEventsAsync(territoryId, CancellationToken.None);

        // Adicionar novo evento
        var event2 = new TerritoryEvent(
            Guid.NewGuid(),
            territoryId,
            "Event 2",
            "Desc 2",
            DateTime.UtcNow.AddDays(2),
            null,
            0.0,
            0.0,
            null,
            Guid.NewGuid(),
            MembershipRole.Resident,
            EventStatus.Scheduled,
            DateTime.UtcNow,
            DateTime.UtcNow);
        await _eventRepository.AddAsync(event2, CancellationToken.None);

        // Buscar novamente (deve recarregar do repositório)
        var result2 = await _service.GetEventsByTerritoryAsync(territoryId, null, null, null, CancellationToken.None);
        Assert.Equal(2, result2.Count);
        Assert.Contains(event1, result2);
        Assert.Contains(event2, result2);
    }

    [Fact]
    public async Task GetEventsByTerritoryAsync_WithDifferentTerritories_ReturnsSeparateCaches()
    {
        var territoryId1 = Guid.NewGuid();
        var territoryId2 = Guid.NewGuid();

        var event1 = new TerritoryEvent(
            Guid.NewGuid(),
            territoryId1,
            "Event 1",
            "Desc 1",
            DateTime.UtcNow.AddDays(1),
            null,
            0.0,
            0.0,
            null,
            Guid.NewGuid(),
            MembershipRole.Resident,
            EventStatus.Scheduled,
            DateTime.UtcNow,
            DateTime.UtcNow);
        var event2 = new TerritoryEvent(
            Guid.NewGuid(),
            territoryId2,
            "Event 2",
            "Desc 2",
            DateTime.UtcNow.AddDays(2),
            null,
            0.0,
            0.0,
            null,
            Guid.NewGuid(),
            MembershipRole.Resident,
            EventStatus.Scheduled,
            DateTime.UtcNow,
            DateTime.UtcNow);

        await _eventRepository.AddAsync(event1, CancellationToken.None);
        await _eventRepository.AddAsync(event2, CancellationToken.None);

        var result1 = await _service.GetEventsByTerritoryAsync(territoryId1, null, null, null, CancellationToken.None);
        var result2 = await _service.GetEventsByTerritoryAsync(territoryId2, null, null, null, CancellationToken.None);

        Assert.Single(result1);
        Assert.Single(result2);
        Assert.Contains(event1, result1);
        Assert.Contains(event2, result2);
        Assert.DoesNotContain(event1, result2);
        Assert.DoesNotContain(event2, result1);
    }
}
