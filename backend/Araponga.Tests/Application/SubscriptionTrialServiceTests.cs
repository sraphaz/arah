using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Domain.Email;
using Araponga.Domain.Subscriptions;
using Moq;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class SubscriptionTrialServiceTests
{
    private readonly Mock<ISubscriptionRepository> _subscriptionRepositoryMock;
    private readonly Mock<ISubscriptionPlanRepository> _planRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IOutbox> _outboxMock;
    private readonly SubscriptionTrialService _service;

    public SubscriptionTrialServiceTests()
    {
        _subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
        _planRepositoryMock = new Mock<ISubscriptionPlanRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _outboxMock = new Mock<IOutbox>();

        _service = new SubscriptionTrialService(
            _subscriptionRepositoryMock.Object,
            _planRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _outboxMock.Object,
            Microsoft.Extensions.Logging.Abstractions.NullLogger<SubscriptionTrialService>.Instance);
    }

    [Fact]
    public async Task GetTrialsExpiringSoonAsync_ReturnsTrials_WhenExpiringSoon()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var subscription1 = CreateTrialEndingSoonSubscription(planId, daysUntilExpiration: 2);
        var subscription2 = CreateTrialEndingSoonSubscription(planId, daysUntilExpiration: 5);
        var subscription3 = CreateTrialEndingSoonSubscription(planId, daysUntilExpiration: 10);

        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.TRIALING, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription> { subscription1, subscription2, subscription3 });

        // Act
        var trials = await _service.GetTrialsExpiringSoonAsync(3, CancellationToken.None);

        // Assert
        // Apenas subscription1 deve estar nos próximos 3 dias
        Assert.Single(trials);
        Assert.Equal(subscription1.Id, trials[0].Id);
    }

    [Fact]
    public async Task ProcessExpiredTrialsAsync_EndsTrial_WhenExpired()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var subscription = CreateExpiredTrialSubscription(planId);

        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.TRIALING, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription> { subscription });
        _subscriptionRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ProcessExpiredTrialsAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(SubscriptionStatus.ACTIVE, subscription.Status);
    }

    [Fact]
    public async Task ProcessExpiredTrialsAsync_ActivatesSubscription_WhenTrialEnds()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var subscription = CreateExpiredTrialSubscription(planId);

        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.TRIALING, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription> { subscription });
        _subscriptionRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ProcessExpiredTrialsAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(SubscriptionStatus.ACTIVE, subscription.Status);
    }

    [Fact]
    public async Task ProcessExpiredTrialsAsync_SendsNotification_WhenTrialEnded()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var plan = CreatePlanWithTrial(planId, trialDays: 7);
        var subscription = CreateExpiredTrialSubscription(planId);

        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.TRIALING, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription> { subscription });
        _planRepositoryMock
            .Setup(r => r.GetByIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);
        _subscriptionRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _outboxMock
            .Setup(o => o.EnqueueAsync(It.IsAny<Araponga.Application.Models.OutboxMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ProcessExpiredTrialsAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _outboxMock.Verify(o => o.EnqueueAsync(It.IsAny<Araponga.Application.Models.OutboxMessage>(), It.IsAny<CancellationToken>()), Times.Once);
    }


    private static SubscriptionPlan CreatePlanWithTrial(Guid id, int trialDays)
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
            new List<FeatureCapability> { FeatureCapability.FeedBasic },
            new Dictionary<string, object>(),
            isDefault: false,
            trialDays: trialDays,
            Guid.Empty);
    }

    private static SubscriptionPlan CreatePlanWithoutTrial(Guid id)
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
            new List<FeatureCapability> { FeatureCapability.FeedBasic },
            new Dictionary<string, object>(),
            isDefault: false,
            trialDays: null,
            Guid.Empty);
    }

    private static Subscription CreateActiveSubscription(Guid planId, Guid userId)
    {
        return new Subscription(
            Guid.NewGuid(),
            userId,
            null,
            planId,
            SubscriptionStatus.ACTIVE,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(1),
            null,
            null,
            null,
            null);
    }

    private static Subscription CreateExpiredTrialSubscription(Guid planId)
    {
        return new Subscription(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            planId,
            SubscriptionStatus.TRIALING,
            DateTime.UtcNow.AddDays(-8),
            DateTime.UtcNow.AddMonths(1),
            DateTime.UtcNow.AddDays(-1), // Trial expirou há 1 dia
            null,
            null,
            null);
    }

    private static Subscription CreateTrialEndingSoonSubscription(Guid planId, int daysUntilExpiration)
    {
        return new Subscription(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            planId,
            SubscriptionStatus.TRIALING,
            DateTime.UtcNow.AddDays(-5),
            DateTime.UtcNow.AddMonths(1),
            DateTime.UtcNow.AddDays(daysUntilExpiration),
            null,
            null,
            null);
    }
}
