using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Subscriptions;
using Moq;
using Xunit;

namespace Araponga.Tests.Application;

public sealed class SubscriptionAnalyticsServiceTests
{
    private readonly Mock<ISubscriptionRepository> _subscriptionRepositoryMock;
    private readonly Mock<ISubscriptionPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<ISubscriptionPlanRepository> _planRepositoryMock;
    private readonly SubscriptionAnalyticsService _service;

    public SubscriptionAnalyticsServiceTests()
    {
        _subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
        _paymentRepositoryMock = new Mock<ISubscriptionPaymentRepository>();
        _planRepositoryMock = new Mock<ISubscriptionPlanRepository>();

        _service = new SubscriptionAnalyticsService(
            _subscriptionRepositoryMock.Object,
            _paymentRepositoryMock.Object,
            _planRepositoryMock.Object,
            Microsoft.Extensions.Logging.Abstractions.NullLogger<SubscriptionAnalyticsService>.Instance);
    }

    [Fact]
    public async Task GetMRRAsync_ReturnsCorrectMRR_WhenSubscriptionsExist()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var plan = CreateMonthlyPlan(planId, 29.90m);
        var subscription = CreateActiveSubscription(planId);

        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.ACTIVE, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription> { subscription });

        _planRepositoryMock
            .Setup(r => r.GetByIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        // Act
        var mrr = await _service.GetMRRAsync(cancellationToken: CancellationToken.None);

        // Assert
        Assert.Equal(29.90m, mrr);
    }

    [Fact]
    public async Task GetMRRAsync_ReturnsZero_WhenNoSubscriptions()
    {
        // Arrange
        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.ACTIVE, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription>());

        // Act
        var mrr = await _service.GetMRRAsync(cancellationToken: CancellationToken.None);

        // Assert
        Assert.Equal(0m, mrr);
    }

    [Fact]
    public async Task GetMRRAsync_FiltersByDateRange_Correctly()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var plan = CreateMonthlyPlan(planId, 29.90m);
        var subscription1 = CreateActiveSubscription(planId, DateTime.UtcNow.AddMonths(-2));
        var subscription2 = CreateActiveSubscription(planId, DateTime.UtcNow);

        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.ACTIVE, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription> { subscription1, subscription2 });

        _planRepositoryMock
            .Setup(r => r.GetByIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        var startDate = DateTime.UtcNow.AddMonths(-1);
        var endDate = DateTime.UtcNow;

        // Act
        var mrr = await _service.GetMRRAsync(startDate, endDate, CancellationToken.None);

        // Assert
        // Apenas subscription2 está no período
        Assert.Equal(29.90m, mrr);
    }

    [Fact]
    public async Task GetMRRAsync_ExcludesFreePlans()
    {
        // Arrange
        var freePlanId = Guid.NewGuid();
        var paidPlanId = Guid.NewGuid();
        var freePlan = CreateFreePlan(freePlanId);
        var paidPlan = CreateMonthlyPlan(paidPlanId, 29.90m);
        var freeSubscription = CreateActiveSubscription(freePlanId);
        var paidSubscription = CreateActiveSubscription(paidPlanId);

        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.ACTIVE, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription> { freeSubscription, paidSubscription });

        _planRepositoryMock
            .Setup(r => r.GetByIdAsync(freePlanId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(freePlan);
        _planRepositoryMock
            .Setup(r => r.GetByIdAsync(paidPlanId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(paidPlan);

        // Act
        var mrr = await _service.GetMRRAsync(cancellationToken: CancellationToken.None);

        // Assert
        Assert.Equal(29.90m, mrr); // Apenas plano pago conta
    }

    [Fact]
    public async Task GetMRRAsync_ConvertsQuarterlyToMonthly()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var plan = CreateQuarterlyPlan(planId, 90m); // 90/3 = 30 por mês
        var subscription = CreateActiveSubscription(planId);

        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.ACTIVE, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription> { subscription });

        _planRepositoryMock
            .Setup(r => r.GetByIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        // Act
        var mrr = await _service.GetMRRAsync(cancellationToken: CancellationToken.None);

        // Assert
        Assert.Equal(30m, mrr);
    }

    [Fact]
    public async Task GetMRRAsync_ConvertsYearlyToMonthly()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var plan = CreateYearlyPlan(planId, 360m); // 360/12 = 30 por mês
        var subscription = CreateActiveSubscription(planId);

        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.ACTIVE, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription> { subscription });

        _planRepositoryMock
            .Setup(r => r.GetByIdAsync(planId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        // Act
        var mrr = await _service.GetMRRAsync(cancellationToken: CancellationToken.None);

        // Assert
        Assert.Equal(30m, mrr);
    }

    [Fact]
    public async Task GetChurnRateAsync_ReturnsCorrectRate_WhenCancellationsExist()
    {
        // Arrange
        var activeSubscription = CreateActiveSubscription(Guid.NewGuid(), DateTime.UtcNow.AddMonths(-2));
        var canceledSubscription = CreateCanceledSubscription(Guid.NewGuid(), DateTime.UtcNow.AddDays(-5));

        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.ACTIVE, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription> { activeSubscription });

        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.CANCELED, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription> { canceledSubscription });

        var startDate = DateTime.UtcNow.AddMonths(-1);
        var endDate = DateTime.UtcNow;

        // Act
        var churnRate = await _service.GetChurnRateAsync(startDate, endDate, CancellationToken.None);

        // Assert
        // 1 cancelamento / 1 ativa = 100%
        Assert.Equal(100m, churnRate);
    }

    [Fact]
    public async Task GetChurnRateAsync_ReturnsZero_WhenNoCancellations()
    {
        // Arrange
        var activeSubscription = CreateActiveSubscription(Guid.NewGuid());

        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.ACTIVE, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription> { activeSubscription });

        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.CANCELED, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription>());

        // Act
        var churnRate = await _service.GetChurnRateAsync(cancellationToken: CancellationToken.None);

        // Assert
        Assert.Equal(0m, churnRate);
    }

    [Fact]
    public async Task GetChurnRateAsync_FiltersByDateRange_Correctly()
    {
        // Arrange
        var activeSubscription = CreateActiveSubscription(Guid.NewGuid(), DateTime.UtcNow.AddMonths(-2));
        var canceledInRange = CreateCanceledSubscription(Guid.NewGuid(), DateTime.UtcNow.AddDays(-5));
        var canceledOutOfRange = CreateCanceledSubscription(Guid.NewGuid(), DateTime.UtcNow.AddMonths(-2));

        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.ACTIVE, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription> { activeSubscription });

        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.CANCELED, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription> { canceledInRange, canceledOutOfRange });

        var startDate = DateTime.UtcNow.AddMonths(-1);
        var endDate = DateTime.UtcNow;

        // Act
        var churnRate = await _service.GetChurnRateAsync(startDate, endDate, CancellationToken.None);

        // Assert
        // Apenas canceledInRange está no período
        Assert.Equal(100m, churnRate);
    }

    [Fact]
    public async Task GetActiveSubscriptionsCountAsync_ReturnsCorrectCount()
    {
        // Arrange
        var subscription1 = CreateActiveSubscription(Guid.NewGuid());
        var subscription2 = CreateActiveSubscription(Guid.NewGuid());
        var canceledSubscription = CreateCanceledSubscription(Guid.NewGuid());

        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.ACTIVE, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription> { subscription1, subscription2 });

        // Act
        var count = await _service.GetActiveSubscriptionsCountAsync(CancellationToken.None);

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task GetNewSubscriptionsCountAsync_ReturnsCorrectCount_ForDateRange()
    {
        // Arrange
        var subscription1 = CreateActiveSubscription(Guid.NewGuid(), DateTime.UtcNow.AddDays(-5));
        var subscription2 = CreateActiveSubscription(Guid.NewGuid(), DateTime.UtcNow.AddDays(-10));
        var subscription3 = CreateActiveSubscription(Guid.NewGuid(), DateTime.UtcNow.AddMonths(-2));

        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription> { subscription1, subscription2, subscription3 });

        var startDate = DateTime.UtcNow.AddMonths(-1);
        var endDate = DateTime.UtcNow;

        // Act
        var count = await _service.GetNewSubscriptionsCountAsync(startDate, endDate, CancellationToken.None);

        // Assert
        // Apenas subscription1 e subscription2 estão no período
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task GetCanceledSubscriptionsCountAsync_ReturnsCorrectCount_ForDateRange()
    {
        // Arrange
        var canceled1 = CreateCanceledSubscription(Guid.NewGuid(), DateTime.UtcNow.AddDays(-5));
        var canceled2 = CreateCanceledSubscription(Guid.NewGuid(), DateTime.UtcNow.AddDays(-10));
        var canceled3 = CreateCanceledSubscription(Guid.NewGuid(), DateTime.UtcNow.AddMonths(-2));

        _subscriptionRepositoryMock
            .Setup(r => r.ListAsync(null, null, SubscriptionStatus.CANCELED, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Subscription> { canceled1, canceled2, canceled3 });

        var startDate = DateTime.UtcNow.AddMonths(-1);
        var endDate = DateTime.UtcNow;

        // Act
        var count = await _service.GetCanceledSubscriptionsCountAsync(startDate, endDate, CancellationToken.None);

        // Assert
        // Apenas canceled1 e canceled2 estão no período
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task GetRevenueByPlanAsync_ReturnsCorrectRevenue_GroupedByPlan()
    {
        // Arrange
        var planId1 = Guid.NewGuid();
        var planId2 = Guid.NewGuid();
        var subscription1 = CreateActiveSubscription(planId1);
        var subscription2 = CreateActiveSubscription(planId2);

        var payment1 = CreatePayment(subscription1.Id, 29.90m, DateTime.UtcNow.AddDays(-5));
        var payment2 = CreatePayment(subscription1.Id, 29.90m, DateTime.UtcNow.AddDays(-10));
        var payment3 = CreatePayment(subscription2.Id, 49.90m, DateTime.UtcNow.AddDays(-5));

        _paymentRepositoryMock
            .Setup(r => r.GetByUserIdAsync(Guid.Empty, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SubscriptionPayment> { payment1, payment2, payment3 });

        _subscriptionRepositoryMock
            .Setup(r => r.GetByIdAsync(subscription1.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription1);
        _subscriptionRepositoryMock
            .Setup(r => r.GetByIdAsync(subscription2.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription2);

        var startDate = DateTime.UtcNow.AddMonths(-1);
        var endDate = DateTime.UtcNow;

        // Act
        var revenue = await _service.GetRevenueByPlanAsync(startDate, endDate, CancellationToken.None);

        // Assert
        Assert.Equal(2, revenue.Count);
        Assert.Equal(59.80m, revenue[planId1]); // 29.90 + 29.90
        Assert.Equal(49.90m, revenue[planId2]);
    }

    [Fact]
    public async Task GetRevenueByPlanAsync_ReturnsEmpty_WhenNoSubscriptions()
    {
        // Arrange
        _paymentRepositoryMock
            .Setup(r => r.GetByUserIdAsync(Guid.Empty, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SubscriptionPayment>());

        // Act
        var revenue = await _service.GetRevenueByPlanAsync(cancellationToken: CancellationToken.None);

        // Assert
        Assert.Empty(revenue);
    }

    [Fact]
    public async Task GetRevenueByPlanAsync_FiltersByDateRange_Correctly()
    {
        // Arrange
        var planId = Guid.NewGuid();
        var subscription = CreateActiveSubscription(planId);
        var paymentInRange = CreatePayment(subscription.Id, 29.90m, DateTime.UtcNow.AddDays(-5));
        var paymentOutOfRange = CreatePayment(subscription.Id, 29.90m, DateTime.UtcNow.AddMonths(-2));

        _paymentRepositoryMock
            .Setup(r => r.GetByUserIdAsync(Guid.Empty, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<SubscriptionPayment> { paymentInRange, paymentOutOfRange });

        _subscriptionRepositoryMock
            .Setup(r => r.GetByIdAsync(subscription.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);

        var startDate = DateTime.UtcNow.AddMonths(-1);
        var endDate = DateTime.UtcNow;

        // Act
        var revenue = await _service.GetRevenueByPlanAsync(startDate, endDate, CancellationToken.None);

        // Assert
        // Apenas paymentInRange está no período
        Assert.Equal(29.90m, revenue[planId]);
    }

    private static SubscriptionPlan CreateMonthlyPlan(Guid id, decimal price)
    {
        return new SubscriptionPlan(
            id,
            "BASIC",
            "Basic plan",
            SubscriptionPlanTier.BASIC,
            PlanScope.Global,
            null,
            price,
            SubscriptionBillingCycle.MONTHLY,
            new List<FeatureCapability> { FeatureCapability.FeedBasic },
            new Dictionary<string, object>(),
            isDefault: false,
            trialDays: null,
            Guid.Empty);
    }

    private static SubscriptionPlan CreateQuarterlyPlan(Guid id, decimal price)
    {
        return new SubscriptionPlan(
            id,
            "BASIC",
            "Basic plan",
            SubscriptionPlanTier.BASIC,
            PlanScope.Global,
            null,
            price,
            SubscriptionBillingCycle.QUARTERLY,
            new List<FeatureCapability> { FeatureCapability.FeedBasic },
            new Dictionary<string, object>(),
            isDefault: false,
            trialDays: null,
            Guid.Empty);
    }

    private static SubscriptionPlan CreateYearlyPlan(Guid id, decimal price)
    {
        return new SubscriptionPlan(
            id,
            "BASIC",
            "Basic plan",
            SubscriptionPlanTier.BASIC,
            PlanScope.Global,
            null,
            price,
            SubscriptionBillingCycle.YEARLY,
            new List<FeatureCapability> { FeatureCapability.FeedBasic },
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

    private static Subscription CreateActiveSubscription(Guid planId, DateTime? createdAt = null)
    {
        return new Subscription(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            planId,
            SubscriptionStatus.ACTIVE,
            createdAt ?? DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(1),
            null,
            null,
            null,
            null);
    }

    private static Subscription CreateCanceledSubscription(Guid planId, DateTime? canceledAt = null)
    {
        return new Subscription(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            planId,
            SubscriptionStatus.CANCELED,
            DateTime.UtcNow.AddMonths(-2),
            DateTime.UtcNow.AddMonths(-1),
            canceledAt ?? DateTime.UtcNow.AddDays(-5),
            null,
            null,
            null);
    }

    private static SubscriptionPayment CreatePayment(Guid subscriptionId, decimal amount, DateTime paymentDate)
    {
        return new SubscriptionPayment(
            Guid.NewGuid(),
            subscriptionId,
            amount,
            "BRL",
            SubscriptionPaymentStatus.Succeeded,
            paymentDate,
            paymentDate,
            paymentDate.AddMonths(1),
            null,
            null);
    }
}
