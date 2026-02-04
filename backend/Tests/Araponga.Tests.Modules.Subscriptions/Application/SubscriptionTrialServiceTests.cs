using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Domain.Email;
using Araponga.Domain.Subscriptions;
using Moq;
using Xunit;

namespace Araponga.Tests.Modules.Subscriptions.Application;

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
        // Criar subscriptions com TrialEnd nos próximos 2, 5 e 10 dias
        var subscription1 = CreateTrialEndingSoonSubscription(planId, daysUntilExpiration: 2);
        var subscription2 = CreateTrialEndingSoonSubscription(planId, daysUntilExpiration: 5);
        var subscription3 = CreateTrialEndingSoonSubscription(planId, daysUntilExpiration: 10);

        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.TRIALING, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription> { subscription1, subscription2, subscription3 });

        // Act
        // Buscar trials que expiram nos próximos 3 dias
        var trials = await _service.GetTrialsExpiringSoonAsync(3, CancellationToken.None);

        // Assert
        // Apenas subscription1 (expira em 2 dias) deve estar nos próximos 3 dias
        // subscription2 expira em 5 dias (> 3), subscription3 expira em 10 dias (> 3)
        Assert.Single(trials);
        Assert.Equal(subscription1.Id, trials[0].Id);
        Assert.True(trials[0].TrialEnd.HasValue);
        var trialEnd = trials[0].TrialEnd!.Value;
        Assert.True(trialEnd > DateTime.UtcNow);
        Assert.True(trialEnd <= DateTime.UtcNow.AddDays(3));
    }

    [Fact]
    public async Task ProcessExpiredTrialsAsync_EndsTrial_WhenExpired()
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
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ProcessExpiredTrialsAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        // Verificar que UpdateAsync foi chamado com uma subscription com status ACTIVE
        _subscriptionRepositoryMock.Verify(r => r.UpdateAsync(
            It.Is<Subscription>(s => s.Status == SubscriptionStatus.ACTIVE),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessExpiredTrialsAsync_ActivatesSubscription_WhenTrialEnds()
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
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ProcessExpiredTrialsAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        // Verificar que UpdateAsync foi chamado com uma subscription com status ACTIVE
        _subscriptionRepositoryMock.Verify(r => r.UpdateAsync(
            It.Is<Subscription>(s => s.Status == SubscriptionStatus.ACTIVE),
            It.IsAny<CancellationToken>()), Times.Once);
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

    [Fact]
    public async Task GetTrialsExpiringSoonAsync_ReturnsEmpty_WhenNoTrialsExpiring()
    {
        // Arrange
        var planId = Guid.NewGuid();
        // Criar subscription que expira em 10 dias (fora do range de 3 dias)
        var subscription = CreateTrialEndingSoonSubscription(planId, daysUntilExpiration: 10);

        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.TRIALING, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription> { subscription });

        // Act
        // Buscar trials que expiram nos próximos 3 dias
        var trials = await _service.GetTrialsExpiringSoonAsync(3, CancellationToken.None);

        // Assert
        Assert.Empty(trials);
    }

    [Fact]
    public async Task GetTrialsExpiringSoonAsync_ReturnsMultipleTrials_WhenMultipleExpiring()
    {
        // Arrange
        var planId = Guid.NewGuid();
        // Criar subscriptions que expiram em 1, 2 e 3 dias (todas dentro do range de 3 dias)
        var subscription1 = CreateTrialEndingSoonSubscription(planId, daysUntilExpiration: 1);
        var subscription2 = CreateTrialEndingSoonSubscription(planId, daysUntilExpiration: 2);
        var subscription3 = CreateTrialEndingSoonSubscription(planId, daysUntilExpiration: 3);
        // Esta expira em 5 dias (fora do range)
        var subscription4 = CreateTrialEndingSoonSubscription(planId, daysUntilExpiration: 5);

        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.TRIALING, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription> { subscription1, subscription2, subscription3, subscription4 });

        // Act
        // Buscar trials que expiram nos próximos 3 dias
        var trials = await _service.GetTrialsExpiringSoonAsync(3, CancellationToken.None);

        // Assert
        Assert.Equal(3, trials.Count);
        Assert.Contains(trials, t => t.Id == subscription1.Id);
        Assert.Contains(trials, t => t.Id == subscription2.Id);
        Assert.Contains(trials, t => t.Id == subscription3.Id);
        Assert.DoesNotContain(trials, t => t.Id == subscription4.Id);
    }

    [Fact]
    public async Task ProcessExpiredTrialsAsync_HandlesMultipleExpiredTrials()
    {
        // Arrange
        var planId1 = Guid.NewGuid();
        var planId2 = Guid.NewGuid();
        var plan1 = CreatePlanWithTrial(planId1, trialDays: 7);
        var plan2 = CreatePlanWithTrial(planId2, trialDays: 14);
        var subscription1 = CreateExpiredTrialSubscription(planId1);
        var subscription2 = CreateExpiredTrialSubscription(planId2);

        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.TRIALING, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription> { subscription1, subscription2 });
        _planRepositoryMock
            .Setup(r => r.GetByIdAsync(planId1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan1);
        _planRepositoryMock
            .Setup(r => r.GetByIdAsync(planId2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan2);
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
        // Verificar que UpdateAsync foi chamado duas vezes (uma para cada subscription)
        _subscriptionRepositoryMock.Verify(r => r.UpdateAsync(
            It.Is<Subscription>(s => s.Status == SubscriptionStatus.ACTIVE),
            It.IsAny<CancellationToken>()), Times.Exactly(2));
        // Verificar que notificações foram enviadas para ambas
        _outboxMock.Verify(o => o.EnqueueAsync(It.IsAny<Araponga.Application.Models.OutboxMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
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
            trialStart: DateTime.UtcNow.AddDays(-8), // Trial começou há 8 dias
            trialEnd: DateTime.UtcNow.AddDays(-1), // Trial expirou há 1 dia
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
            trialStart: DateTime.UtcNow.AddDays(-7), // Trial começou há 7 dias
            trialEnd: DateTime.UtcNow.AddDays(daysUntilExpiration), // Trial expira em X dias
            null,
            null);
    }
}
