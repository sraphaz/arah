using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Subscriptions;
using Araponga.Domain.Territories;
using Moq;
using Xunit;

namespace Araponga.Tests.Modules.Subscriptions.Application;

public sealed class SubscriptionPlanAdminServiceTests
{
    private readonly Mock<ISubscriptionPlanRepository> _planRepositoryMock;
    private readonly Mock<ISubscriptionPlanHistoryRepository> _historyRepositoryMock;
    private readonly Mock<ITerritoryRepository> _territoryRepositoryMock;
    private readonly Mock<ISubscriptionRepository> _subscriptionRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly SubscriptionPlanAdminService _service;

    public SubscriptionPlanAdminServiceTests()
    {
        _planRepositoryMock = new Mock<ISubscriptionPlanRepository>();
        _historyRepositoryMock = new Mock<ISubscriptionPlanHistoryRepository>();
        _territoryRepositoryMock = new Mock<ITerritoryRepository>();
        _subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _service = new SubscriptionPlanAdminService(
            _planRepositoryMock.Object,
            _historyRepositoryMock.Object,
            _territoryRepositoryMock.Object,
            _subscriptionRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateGlobalPlanAsync_CreatesPlan_WhenValidData()
    {
        // Arrange
        var adminUserId = Guid.NewGuid();
        var capabilities = new List<FeatureCapability> { FeatureCapability.FeedBasic, FeatureCapability.PostsUnlimited };

        _planRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _historyRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<SubscriptionPlanHistory>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateGlobalPlanAsync(
            adminUserId,
            "BASIC",
            "Basic plan",
            SubscriptionPlanTier.BASIC,
            29.90m,
            SubscriptionBillingCycle.MONTHLY,
            capabilities,
            new Dictionary<string, object>(),
            null,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("BASIC", result.Value.Name);
        _planRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()), Times.Once);
        _historyRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SubscriptionPlanHistory>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateGlobalPlanAsync_ReturnsFailure_WhenInvalidData()
    {
        // Arrange
        var adminUserId = Guid.NewGuid();
        var capabilities = new List<FeatureCapability>(); // Sem funcionalidades básicas para FREE

        // Act
        var result = await _service.CreateGlobalPlanAsync(
            adminUserId,
            "FREE",
            "Free plan",
            SubscriptionPlanTier.FREE,
            0m,
            null,
            capabilities, // Sem FeedBasic
            new Dictionary<string, object>(),
            null,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("FeedBasic", result.Error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateGlobalPlanAsync_ValidatesRequiredCapabilities_ForFreePlan()
    {
        // Arrange
        var adminUserId = Guid.NewGuid();
        var capabilities = new List<FeatureCapability> { FeatureCapability.PostsUnlimited }; // Sem FeedBasic

        // Act
        var result = await _service.CreateGlobalPlanAsync(
            adminUserId,
            "FREE",
            "Free plan",
            SubscriptionPlanTier.FREE,
            0m,
            null,
            capabilities,
            new Dictionary<string, object>(),
            null,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("FeedBasic", result.Error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UpdatePlanAsync_UpdatesPlan_WhenValidData()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingPlan = CreateBasicPlan(planId);

        _planRepositoryMock
            .Setup(r => r.GetByIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPlan);
        _planRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _historyRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<SubscriptionPlanHistory>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdatePlanAsync(
            planId,
            userId,
            "UPDATED_BASIC",
            "Updated description",
            39.90m,
            SubscriptionBillingCycle.MONTHLY,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("UPDATED_BASIC", result.Value.Name);
        _historyRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SubscriptionPlanHistory>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdatePlanAsync_ReturnsFailure_WhenPlanNotFound()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _planRepositoryMock
            .Setup(r => r.GetByIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SubscriptionPlan?)null);

        // Act
        var result = await _service.UpdatePlanAsync(
            planId,
            userId,
            "UPDATED",
            null,
            null,
            null,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UpdatePlanCapabilitiesAsync_PreventsRemovingBasicCapabilities()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingPlan = CreateFreePlan(planId);
        var newCapabilities = new List<FeatureCapability>(); // Tentando remover FeedBasic

        _planRepositoryMock
            .Setup(r => r.GetByIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPlan);

        // Act
        var result = await _service.UpdatePlanCapabilitiesAsync(
            planId,
            userId,
            newCapabilities,
            CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("FeedBasic", result.Error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task DeactivatePlanAsync_DeactivatesPlan_WhenNoActiveSubscriptions()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingPlan = CreateBasicPlan(planId);

        _planRepositoryMock
            .Setup(r => r.GetByIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPlan);
        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.ACTIVE, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription>()); // Nenhuma assinatura ativa
        _planRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _historyRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<SubscriptionPlanHistory>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeactivatePlanAsync(planId, userId, null, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _planRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeactivatePlanAsync_ReturnsFailure_WhenActiveSubscriptionsExist()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingPlan = CreateBasicPlan(planId);
        var activeSubscription = new Subscription(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            planId,
            SubscriptionStatus.ACTIVE,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(1),
            null,
            null,
            null,
            null);

        _planRepositoryMock
            .Setup(r => r.GetByIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPlan);
        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.ACTIVE, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription> { activeSubscription });

        // Act
        var result = await _service.DeactivatePlanAsync(planId, userId, null, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains("active subscriptions", result.Error!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetPlanHistoryAsync_ReturnsHistory_WhenChangesExist()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var history1 = new SubscriptionPlanHistory(
            Guid.NewGuid(),
            planId,
            Guid.NewGuid(),
            SubscriptionPlanHistoryChangeType.Created);
        var history2 = new SubscriptionPlanHistory(
            Guid.NewGuid(),
            planId,
            Guid.NewGuid(),
            SubscriptionPlanHistoryChangeType.Updated);

        _historyRepositoryMock
            .Setup(r => r.GetByPlanIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SubscriptionPlanHistory> { history1, history2 });

        // Act
        var history = await _service.GetPlanHistoryAsync(planId, CancellationToken.None);

        // Assert
        Assert.Equal(2, history.Count);
    }

    [Fact]
    public async Task GetPlanHistoryAsync_ReturnsEmpty_WhenNoHistory()
    {
        // Arrange
        var planId = Guid.NewGuid();

        _historyRepositoryMock
            .Setup(r => r.GetByPlanIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SubscriptionPlanHistory>());

        // Act
        var history = await _service.GetPlanHistoryAsync(planId, CancellationToken.None);

        // Assert
        Assert.Empty(history);
    }

    private static SubscriptionPlan CreateBasicPlan(Guid id)
    {
        return new SubscriptionPlan(
            id,
            "BASIC",
            "Basic plan",
            SubscriptionPlanTier.BASIC,
            PlanScope.Global,
            null,
            29.90m,
            SubscriptionBillingCycle.MONTHLY,
            new List<FeatureCapability> { FeatureCapability.FeedBasic, FeatureCapability.PostsUnlimited },
            new Dictionary<string, object>(),
            isDefault: false,
            trialDays: null,
            Guid.Empty);
    }

    private static SubscriptionPlan CreateFreePlan(Guid id)
    {
        return new SubscriptionPlan(
            id,
            "FREE",
            "Free plan",
            SubscriptionPlanTier.FREE,
            PlanScope.Global,
            null,
            0m,
            null,
            new List<FeatureCapability> { FeatureCapability.FeedBasic },
            new Dictionary<string, object>(),
            isDefault: true,
            trialDays: null,
            Guid.Empty);
    }
}
