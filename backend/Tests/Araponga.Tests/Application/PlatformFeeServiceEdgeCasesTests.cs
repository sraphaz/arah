using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Modules.Marketplace.Domain;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for PlatformFeeService,
/// focusing on null/empty inputs, invalid values, and edge cases.
/// </summary>
public sealed class PlatformFeeServiceEdgeCasesTests
{
    private readonly InMemoryDataStore _dataStore;
    private readonly InMemoryPlatformFeeConfigRepository _configRepository;
    private readonly InMemoryUnitOfWork _unitOfWork;
    private readonly PlatformFeeService _service;

    public PlatformFeeServiceEdgeCasesTests()
    {
        _dataStore = new InMemoryDataStore();
        _configRepository = new InMemoryPlatformFeeConfigRepository(_dataStore);
        _unitOfWork = new InMemoryUnitOfWork();
        _service = new PlatformFeeService(_configRepository, _unitOfWork);
    }

    [Fact]
    public async Task GetActiveFeeConfigAsync_WithEmptyTerritoryId_ReturnsNull()
    {
        var result = await _service.GetActiveFeeConfigAsync(
            Guid.Empty,
            ItemType.Product,
            CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetActiveFeeConfigAsync_WithNonExistentConfig_ReturnsNull()
    {
        var result = await _service.GetActiveFeeConfigAsync(
            Guid.NewGuid(),
            ItemType.Product,
            CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpsertFeeConfigAsync_WithEmptyTerritoryId_ThrowsArgumentException()
    {
        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _service.UpsertFeeConfigAsync(
                Guid.Empty,
                ItemType.Product,
                PlatformFeeMode.Percentage,
                5.0m,
                "BRL",
                true,
                CancellationToken.None));
    }

    [Fact]
    public async Task UpsertFeeConfigAsync_WithNegativeFeeValue_ThrowsArgumentOutOfRangeException()
    {
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            await _service.UpsertFeeConfigAsync(
                Guid.NewGuid(),
                ItemType.Product,
                PlatformFeeMode.Percentage,
                -1.0m,
                "BRL",
                true,
                CancellationToken.None));
    }

    [Fact]
    public async Task UpsertFeeConfigAsync_WithZeroFeeValue_HandlesCorrectly()
    {
        var territoryId = Guid.NewGuid();
        var result = await _service.UpsertFeeConfigAsync(
            territoryId,
            ItemType.Product,
            PlatformFeeMode.Percentage,
            0m,
            "BRL",
            true,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(0m, result.FeeValue);
    }

    [Fact]
    public async Task UpsertFeeConfigAsync_WithNullCurrency_HandlesCorrectly()
    {
        var territoryId = Guid.NewGuid();
        var result = await _service.UpsertFeeConfigAsync(
            territoryId,
            ItemType.Product,
            PlatformFeeMode.Percentage,
            5.0m,
            null,
            true,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Null(result.Currency);
    }

    [Fact]
    public async Task UpsertFeeConfigAsync_WithExistingConfig_UpdatesConfig()
    {
        var territoryId = Guid.NewGuid();
        
        // Criar configuração inicial
        var result1 = await _service.UpsertFeeConfigAsync(
            territoryId,
            ItemType.Product,
            PlatformFeeMode.Percentage,
            5.0m,
            "BRL",
            true,
            CancellationToken.None);

        Assert.NotNull(result1);
        Assert.Equal(5.0m, result1.FeeValue);

        // Atualizar configuração
        var result2 = await _service.UpsertFeeConfigAsync(
            territoryId,
            ItemType.Product,
            PlatformFeeMode.Fixed,
            10.0m,
            "USD",
            false,
            CancellationToken.None);

        Assert.NotNull(result2);
        Assert.Equal(result1.Id, result2.Id); // Mesmo ID
        Assert.Equal(PlatformFeeMode.Fixed, result2.FeeMode);
        Assert.Equal(10.0m, result2.FeeValue);
        Assert.Equal("USD", result2.Currency);
        Assert.False(result2.IsActive);
    }

    [Fact]
    public async Task UpsertFeeConfigAsync_WithDifferentItemTypes_CreatesSeparateConfigs()
    {
        var territoryId = Guid.NewGuid();
        
        var productConfig = await _service.UpsertFeeConfigAsync(
            territoryId,
            ItemType.Product,
            PlatformFeeMode.Percentage,
            5.0m,
            "BRL",
            true,
            CancellationToken.None);

        var serviceConfig = await _service.UpsertFeeConfigAsync(
            territoryId,
            ItemType.Service,
            PlatformFeeMode.Percentage,
            10.0m,
            "BRL",
            true,
            CancellationToken.None);

        Assert.NotEqual(productConfig.Id, serviceConfig.Id);
        Assert.Equal(ItemType.Product, productConfig.ItemType);
        Assert.Equal(ItemType.Service, serviceConfig.ItemType);
    }

    [Fact]
    public async Task ListActiveAsync_WithNonExistentTerritory_ReturnsEmpty()
    {
        var result = await _service.ListActiveAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ListActivePagedAsync_WithInvalidPagination_HandlesGracefully()
    {
        var territoryId = Guid.NewGuid();
        var pagination = new PaginationParameters(0, 0);

        var result = await _service.ListActivePagedAsync(
            territoryId,
            pagination,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public async Task UpsertFeeConfigAsync_WithVeryLargeFeeValue_HandlesCorrectly()
    {
        var territoryId = Guid.NewGuid();
        var result = await _service.UpsertFeeConfigAsync(
            territoryId,
            ItemType.Product,
            PlatformFeeMode.Percentage,
            1000.0m,
            "BRL",
            true,
            CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(1000.0m, result.FeeValue);
    }
}
