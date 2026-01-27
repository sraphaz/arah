using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Domain.Events;
using Araponga.Domain.Feed;
using Araponga.Domain.Marketplace;
using Araponga.Domain.Membership;
using Araponga.Domain.Territories;
using Moq;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for AnalyticsService,
/// focusing on date filters, null territory, marketplace stats by territory vs platform.
/// </summary>
public sealed class AnalyticsServiceEdgeCasesTests
{
    private readonly Mock<ITerritoryRepository> _territoryRepo;
    private readonly Mock<ITerritoryMembershipRepository> _membershipRepo;
    private readonly Mock<IFeedRepository> _feedRepo;
    private readonly Mock<ITerritoryEventRepository> _eventRepo;
    private readonly Mock<ICheckoutRepository> _checkoutRepo;
    private readonly Mock<ISellerTransactionRepository> _sellerTxRepo;
    private readonly Mock<IUserRepository> _userRepo;
    private readonly Mock<IStoreRepository> _storeRepo;
    private readonly Mock<IStoreItemRepository> _storeItemRepo;
    private readonly AnalyticsService _service;

    public AnalyticsServiceEdgeCasesTests()
    {
        _territoryRepo = new Mock<ITerritoryRepository>();
        _membershipRepo = new Mock<ITerritoryMembershipRepository>();
        _feedRepo = new Mock<IFeedRepository>();
        _eventRepo = new Mock<ITerritoryEventRepository>();
        _checkoutRepo = new Mock<ICheckoutRepository>();
        _sellerTxRepo = new Mock<ISellerTransactionRepository>();
        _userRepo = new Mock<IUserRepository>();
        _storeRepo = new Mock<IStoreRepository>();
        _storeItemRepo = new Mock<IStoreItemRepository>();
        _service = new AnalyticsService(
            _territoryRepo.Object,
            _membershipRepo.Object,
            _feedRepo.Object,
            _eventRepo.Object,
            _checkoutRepo.Object,
            _sellerTxRepo.Object,
            _userRepo.Object,
            _storeRepo.Object,
            _storeItemRepo.Object);
    }

    [Fact]
    public async Task GetTerritoryStatsAsync_WithDateFilters_AppliesPeriod()
    {
        var territoryId = Guid.NewGuid();
        var from = DateTime.UtcNow.AddDays(-30);
        var to = DateTime.UtcNow;
        var territory = new Territory(
            territoryId,
            null,
            "Test",
            null,
            TerritoryStatus.Active,
            "City",
            "ST",
            0,
            0,
            DateTime.UtcNow);

        _territoryRepo.Setup(r => r.GetByIdAsync(territoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(territory);
        _feedRepo.Setup(r => r.ListByTerritoryAsync(territoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<CommunityPost>());
        _eventRepo.Setup(r => r.CountByTerritoryAsync(
                territoryId,
                from,
                to,
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _membershipRepo.Setup(r => r.ListResidentUserIdsAsync(territoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Guid>());
        _sellerTxRepo.Setup(r => r.GetByTerritoryIdAsync(territoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SellerTransaction>());

        var result = await _service.GetTerritoryStatsAsync(territoryId, from, to, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(from, result.PeriodStart);
        Assert.Equal(to, result.PeriodEnd);
    }

    [Fact]
    public async Task GetPlatformStatsAsync_WithDateFilters_AppliesPeriod()
    {
        _territoryRepo.Setup(r => r.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Territory>());
        _checkoutRepo.Setup(r => r.ListAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Checkout>());

        var from = DateTime.UtcNow.AddDays(-7);
        var to = DateTime.UtcNow;
        var result = await _service.GetPlatformStatsAsync(from, to, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(from, result.PeriodStart);
        Assert.Equal(to, result.PeriodEnd);
    }

    [Fact]
    public async Task GetMarketplaceStatsAsync_WithTerritoryId_FiltersByTerritory()
    {
        var territoryId = Guid.NewGuid();
        _checkoutRepo.Setup(r => r.ListAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Checkout>());
        _sellerTxRepo.Setup(r => r.GetByTerritoryIdAsync(territoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SellerTransaction>());

        var result = await _service.GetMarketplaceStatsAsync(territoryId, null, null, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(territoryId, result.TerritoryId);
    }

    [Fact]
    public async Task GetMarketplaceStatsAsync_WithoutTerritoryId_ReturnsPlatformWide()
    {
        _checkoutRepo.Setup(r => r.ListAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Checkout>());

        var result = await _service.GetMarketplaceStatsAsync(null, null, null, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Null(result.TerritoryId);
    }

    [Fact]
    public async Task GetTerritoryStatsAsync_WhenTerritoryExists_ReturnsNonNegativeCounts()
    {
        var territoryId = Guid.NewGuid();
        var territory = new Territory(
            territoryId,
            null,
            "T",
            null,
            TerritoryStatus.Active,
            "C",
            "S",
            0,
            0,
            DateTime.UtcNow);

        _territoryRepo.Setup(r => r.GetByIdAsync(territoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(territory);
        _feedRepo.Setup(r => r.ListByTerritoryAsync(territoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<CommunityPost>());
        _eventRepo.Setup(r => r.CountByTerritoryAsync(territoryId, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _membershipRepo.Setup(r => r.ListResidentUserIdsAsync(territoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Guid>());
        _sellerTxRepo.Setup(r => r.GetByTerritoryIdAsync(territoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SellerTransaction>());

        var result = await _service.GetTerritoryStatsAsync(territoryId, null, null, CancellationToken.None);

        Assert.NotNull(result);
        Assert.True(result.TotalPosts >= 0);
        Assert.True(result.TotalEvents >= 0);
        Assert.True(result.TotalMembers >= 0);
        Assert.True(result.TotalSalesAmount >= 0);
        Assert.True(result.TotalSalesCount >= 0);
    }
}
