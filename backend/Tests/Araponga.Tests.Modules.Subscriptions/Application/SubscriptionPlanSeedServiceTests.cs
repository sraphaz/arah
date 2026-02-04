using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Subscriptions;
using Moq;
using Xunit;

namespace Araponga.Tests.Modules.Subscriptions.Application;

public sealed class SubscriptionPlanSeedServiceTests
{
    private readonly Mock<ISubscriptionPlanRepository> _planRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly SubscriptionPlanSeedService _service;

    public SubscriptionPlanSeedServiceTests()
    {
        _planRepositoryMock = new Mock<ISubscriptionPlanRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _service = new SubscriptionPlanSeedService(
            _planRepositoryMock.Object,
            _unitOfWorkMock.Object,
            Microsoft.Extensions.Logging.Abstractions.NullLogger<SubscriptionPlanSeedService>.Instance);
    }

    [Fact]
    public async Task SeedDefaultFreePlanAsync_CreatesPlan_WhenNotExists()
    {
        // Arrange
        _planRepositoryMock
            .Setup(r => r.GetDefaultPlanAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((SubscriptionPlan?)null);
        _planRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.SeedDefaultFreePlanAsync(CancellationToken.None);

        // Assert
        _planRepositoryMock.Verify(r => r.AddAsync(
            It.Is<SubscriptionPlan>(p => 
                p.Tier == SubscriptionPlanTier.FREE &&
                p.Scope == PlanScope.Global &&
                p.IsDefault &&
                p.Capabilities.Contains(FeatureCapability.FeedBasic)),
            It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SeedDefaultFreePlanAsync_ReturnsSuccess_WhenAlreadyExists()
    {
        // Arrange
        var existingPlan = new SubscriptionPlan(
            Guid.NewGuid(),
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

        _planRepositoryMock
            .Setup(r => r.GetDefaultPlanAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPlan);

        // Act
        await _service.SeedDefaultFreePlanAsync(CancellationToken.None);

        // Assert
        _planRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SeedDefaultFreePlanAsync_ValidatesBasicCapabilities()
    {
        // Arrange
        _planRepositoryMock
            .Setup(r => r.GetDefaultPlanAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((SubscriptionPlan?)null);
        _planRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.SeedDefaultFreePlanAsync(CancellationToken.None);

        // Assert
        _planRepositoryMock.Verify(r => r.AddAsync(
            It.Is<SubscriptionPlan>(p => 
                p.Capabilities.Contains(FeatureCapability.FeedBasic) &&
                p.Capabilities.Contains(FeatureCapability.PostsBasic) &&
                p.Capabilities.Contains(FeatureCapability.EventsBasic) &&
                p.Capabilities.Contains(FeatureCapability.MarketplaceBasic) &&
                p.Capabilities.Contains(FeatureCapability.ChatBasic)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SeedDefaultFreePlanAsync_SetsCorrectLimits()
    {
        // Arrange
        _planRepositoryMock
            .Setup(r => r.GetDefaultPlanAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((SubscriptionPlan?)null);
        _planRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<SubscriptionPlan>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.SeedDefaultFreePlanAsync(CancellationToken.None);

        // Assert
        _planRepositoryMock.Verify(r => r.AddAsync(
            It.Is<SubscriptionPlan>(p => 
                p.Limits != null &&
                p.Limits.ContainsKey("maxPosts") &&
                p.Limits.ContainsKey("maxEvents") &&
                p.Limits.ContainsKey("maxMarketplaceItems") &&
                p.Limits.ContainsKey("maxStorageMB") &&
                Convert.ToInt32(p.Limits["maxPosts"]) == 10 &&
                Convert.ToInt32(p.Limits["maxEvents"]) == 3 &&
                Convert.ToInt32(p.Limits["maxMarketplaceItems"]) == 5 &&
                Convert.ToInt32(p.Limits["maxStorageMB"]) == 100),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
