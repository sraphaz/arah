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

public sealed class AnalyticsServiceTests
{
    private readonly Mock<ITerritoryRepository> _territoryRepositoryMock;
    private readonly Mock<ITerritoryMembershipRepository> _membershipRepositoryMock;
    private readonly Mock<IFeedRepository> _feedRepositoryMock;
    private readonly Mock<ITerritoryEventRepository> _eventRepositoryMock;
    private readonly Mock<ICheckoutRepository> _checkoutRepositoryMock;
    private readonly Mock<ISellerTransactionRepository> _sellerTransactionRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IStoreRepository> _storeRepositoryMock;
    private readonly Mock<IStoreItemRepository> _storeItemRepositoryMock;
    private readonly AnalyticsService _service;

    public AnalyticsServiceTests()
    {
        _territoryRepositoryMock = new Mock<ITerritoryRepository>();
        _membershipRepositoryMock = new Mock<ITerritoryMembershipRepository>();
        _feedRepositoryMock = new Mock<IFeedRepository>();
        _eventRepositoryMock = new Mock<ITerritoryEventRepository>();
        _checkoutRepositoryMock = new Mock<ICheckoutRepository>();
        _sellerTransactionRepositoryMock = new Mock<ISellerTransactionRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _storeRepositoryMock = new Mock<IStoreRepository>();
        _storeItemRepositoryMock = new Mock<IStoreItemRepository>();

        _service = new AnalyticsService(
            _territoryRepositoryMock.Object,
            _membershipRepositoryMock.Object,
            _feedRepositoryMock.Object,
            _eventRepositoryMock.Object,
            _checkoutRepositoryMock.Object,
            _sellerTransactionRepositoryMock.Object,
            _userRepositoryMock.Object,
            _storeRepositoryMock.Object,
            _storeItemRepositoryMock.Object);
    }

    [Fact]
    public async Task GetTerritoryStatsAsync_WhenTerritoryNotFound_ReturnsNull()
    {
        // Arrange
        var territoryId = Guid.NewGuid();
        _territoryRepositoryMock.Setup(r => r.GetByIdAsync(territoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Territory?)null);

        // Act
        var result = await _service.GetTerritoryStatsAsync(territoryId, null, null, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetTerritoryStatsAsync_WhenTerritoryExists_ReturnsStats()
    {
        // Arrange
        var territoryId = Guid.NewGuid();
        var territory = new Territory(
            territoryId,
            null,
            "Test Territory",
            null,
            TerritoryStatus.Active,
            "City",
            "State",
            0.0,
            0.0,
            DateTime.UtcNow);

        _territoryRepositoryMock.Setup(r => r.GetByIdAsync(territoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(territory);
        _feedRepositoryMock.Setup(r => r.ListByTerritoryAsync(territoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<CommunityPost>());
        _eventRepositoryMock.Setup(r => r.CountByTerritoryAsync(territoryId, null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        _membershipRepositoryMock.Setup(r => r.ListResidentUserIdsAsync(territoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Guid>());
        _sellerTransactionRepositoryMock.Setup(r => r.GetByTerritoryIdAsync(territoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SellerTransaction>());

        // Act
        var result = await _service.GetTerritoryStatsAsync(territoryId, null, null, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(territoryId, result.TerritoryId);
        Assert.Equal("Test Territory", result.TerritoryName);
        Assert.Equal(0, result.TotalPosts);
        Assert.Equal(0, result.TotalEvents);
    }

    [Fact]
    public async Task GetPlatformStatsAsync_ReturnsStats()
    {
        // Arrange
        _territoryRepositoryMock.Setup(r => r.ListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Territory>());
        _checkoutRepositoryMock.Setup(r => r.ListAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Checkout>());

        // Act
        var result = await _service.GetPlatformStatsAsync(null, null, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalTerritories);
    }

    [Fact]
    public async Task GetMarketplaceStatsAsync_ReturnsStats()
    {
        // Arrange
        _checkoutRepositoryMock.Setup(r => r.ListAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Checkout>());

        // Act
        var result = await _service.GetMarketplaceStatsAsync(null, null, null, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.TotalSalesAmount);
        Assert.Equal(0, result.TotalSalesCount);
    }
}
