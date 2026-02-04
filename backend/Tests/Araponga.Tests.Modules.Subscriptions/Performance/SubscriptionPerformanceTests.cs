using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Subscriptions;
using Moq;
using Xunit;

namespace Araponga.Tests.Modules.Subscriptions.Performance;

/// <summary>
/// Testes de performance básicos para sistema de assinaturas.
/// </summary>
public sealed class SubscriptionPerformanceTests
{
    [Fact(Skip = "Performance test - may fail in CI environments with limited resources")]
    public async Task GetMRRAsync_CompletesWithinTimeLimit()
    {
        // Arrange
        var subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
        var paymentRepositoryMock = new Mock<ISubscriptionPaymentRepository>();
        var planRepositoryMock = new Mock<ISubscriptionPlanRepository>();

        var subscriptions = Enumerable.Range(0, 1000)
            .Select(i => new Subscription(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null,
                Guid.NewGuid(),
                SubscriptionStatus.ACTIVE,
                DateTime.UtcNow.AddMonths(-i % 12),
                DateTime.UtcNow.AddMonths(1),
                null,
                null,
                null,
                null))
            .ToList();

        subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.ACTIVE, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscriptions);

        planRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SubscriptionPlan(
                Guid.NewGuid(),
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
                Guid.Empty));

        var service = new SubscriptionAnalyticsService(
            subscriptionRepositoryMock.Object,
            paymentRepositoryMock.Object,
            planRepositoryMock.Object,
            Microsoft.Extensions.Logging.Abstractions.NullLogger<SubscriptionAnalyticsService>.Instance);

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var mrr = await service.GetMRRAsync(cancellationToken: CancellationToken.None);
        stopwatch.Stop();

        // Assert
        Assert.True(stopwatch.ElapsedMilliseconds < 5000, $"GetMRRAsync took {stopwatch.ElapsedMilliseconds}ms, expected < 5000ms");
    }

    [Fact(Skip = "Performance test - may fail in CI environments with limited resources")]
    public async Task GetChurnRateAsync_CompletesWithinTimeLimit()
    {
        // Arrange
        var subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
        var paymentRepositoryMock = new Mock<ISubscriptionPaymentRepository>();
        var planRepositoryMock = new Mock<ISubscriptionPlanRepository>();

        var activeSubscriptions = Enumerable.Range(0, 1000)
            .Select(i => new Subscription(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null,
                Guid.NewGuid(),
                SubscriptionStatus.ACTIVE,
                DateTime.UtcNow.AddMonths(-i % 12),
                DateTime.UtcNow.AddMonths(1),
                null,
                null,
                null,
                null))
            .ToList();

        var canceledSubscriptions = Enumerable.Range(0, 100)
            .Select(i => new Subscription(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null,
                Guid.NewGuid(),
                SubscriptionStatus.CANCELED,
                DateTime.UtcNow.AddMonths(-2),
                DateTime.UtcNow.AddMonths(-1),
                DateTime.UtcNow.AddDays(-i),
                null,
                null,
                null))
            .ToList();

        subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.ACTIVE, It.IsAny<CancellationToken>()))
            .ReturnsAsync(activeSubscriptions);
        subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.CANCELED, It.IsAny<CancellationToken>()))
            .ReturnsAsync(canceledSubscriptions);

        var service = new SubscriptionAnalyticsService(
            subscriptionRepositoryMock.Object,
            paymentRepositoryMock.Object,
            planRepositoryMock.Object,
            Microsoft.Extensions.Logging.Abstractions.NullLogger<SubscriptionAnalyticsService>.Instance);

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var churnRate = await service.GetChurnRateAsync(cancellationToken: CancellationToken.None);
        stopwatch.Stop();

        // Assert
        Assert.True(stopwatch.ElapsedMilliseconds < 5000, $"GetChurnRateAsync took {stopwatch.ElapsedMilliseconds}ms, expected < 5000ms");
    }

    [Fact(Skip = "Performance test - may fail in CI environments with limited resources")]
    public async Task ProcessRenewalsAsync_HandlesLargeVolume()
    {
        // Arrange
        var subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
        var paymentRepositoryMock = new Mock<ISubscriptionPaymentRepository>();
        var planRepositoryMock = new Mock<ISubscriptionPlanRepository>();
        var gatewayFactoryMock = new Mock<ISubscriptionGatewayFactory>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var subscriptions = Enumerable.Range(0, 500)
            .Select(i => new Subscription(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null,
                Guid.NewGuid(),
                SubscriptionStatus.ACTIVE,
                DateTime.UtcNow.AddMonths(-1),
                DateTime.UtcNow.AddDays(2), // Expira em 2 dias
                null,
                null,
                $"sub_{i}",
                $"cus_{i}"))
            .ToList();

        subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.ACTIVE, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscriptions);

        planRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SubscriptionPlan(
                Guid.NewGuid(),
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
                Guid.Empty));

        var gatewayMock = new Mock<ISubscriptionGateway>();
        gatewayMock
            .Setup(g => g.GetSubscriptionAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SubscriptionGatewayInfo?)null);

        gatewayFactoryMock
            .Setup(f => f.GetGateway())
            .Returns(gatewayMock.Object);

        subscriptionRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new SubscriptionRenewalService(
            subscriptionRepositoryMock.Object,
            paymentRepositoryMock.Object,
            planRepositoryMock.Object,
            gatewayFactoryMock.Object,
            unitOfWorkMock.Object,
            Microsoft.Extensions.Logging.Abstractions.NullLogger<SubscriptionRenewalService>.Instance);

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await service.ProcessRenewalsAsync(CancellationToken.None);
        stopwatch.Stop();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(stopwatch.ElapsedMilliseconds < 30000, $"ProcessRenewalsAsync took {stopwatch.ElapsedMilliseconds}ms, expected < 30000ms");
    }
}
