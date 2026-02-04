using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Subscriptions;
using Araponga.Domain.Users;
using Moq;
using System.Text.Json;
using Xunit;

namespace Araponga.Tests.Modules.Subscriptions.Application;

public sealed class StripeWebhookServiceTests
{
    private readonly Mock<ISubscriptionRepository> _subscriptionRepositoryMock;
    private readonly Mock<ISubscriptionPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<ISubscriptionPlanRepository> _planRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IOutbox> _outboxMock;
    private readonly StripeWebhookService _service;

    public StripeWebhookServiceTests()
    {
        _subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
        _paymentRepositoryMock = new Mock<ISubscriptionPaymentRepository>();
        _planRepositoryMock = new Mock<ISubscriptionPlanRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _outboxMock = new Mock<IOutbox>();

        _service = new StripeWebhookService(
            _subscriptionRepositoryMock.Object,
            _paymentRepositoryMock.Object,
            _planRepositoryMock.Object,
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _outboxMock.Object,
            Microsoft.Extensions.Logging.Abstractions.NullLogger<StripeWebhookService>.Instance);
    }

    [Fact]
    public async Task ProcessEventAsync_ProcessesSubscriptionCreated_WhenValidEvent()
    {
        // Arrange
        var eventType = "customer.subscription.created";
        var eventData = CreateSubscriptionCreatedEvent("sub_123", "cus_456", "active");

        var subscription = CreateActiveSubscription(Guid.NewGuid());
        _subscriptionRepositoryMock
            .Setup(r => r.GetByStripeSubscriptionIdAsync("sub_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        _subscriptionRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ProcessEventAsync(eventType, eventData, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _subscriptionRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessEventAsync_ProcessesSubscriptionUpdated_WhenValidEvent()
    {
        // Arrange
        var eventType = "customer.subscription.updated";
        var eventData = CreateSubscriptionUpdatedEvent("sub_123", "active");

        var subscription = CreateActiveSubscription(Guid.NewGuid());
        subscription.UpdateStripeIds("sub_123", "cus_456");

        _subscriptionRepositoryMock
            .Setup(r => r.GetByStripeSubscriptionIdAsync("sub_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        _subscriptionRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ProcessEventAsync(eventType, eventData, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _subscriptionRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessEventAsync_ProcessesSubscriptionDeleted_WhenValidEvent()
    {
        // Arrange
        var eventType = "customer.subscription.deleted";
        var eventData = CreateSubscriptionDeletedEvent("sub_123");

        var subscription = CreateActiveSubscription(Guid.NewGuid());
        subscription.UpdateStripeIds("sub_123", "cus_456");

        _subscriptionRepositoryMock
            .Setup(r => r.GetByStripeSubscriptionIdAsync("sub_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        _subscriptionRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ProcessEventAsync(eventType, eventData, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(SubscriptionStatus.CANCELED, subscription.Status);
    }

    [Fact]
    public async Task ProcessEventAsync_ProcessesInvoicePaymentSucceeded_WhenValidEvent()
    {
        // Arrange
        var eventType = "invoice.payment_succeeded";
        var eventData = CreateInvoicePaymentSucceededEvent("in_123", "sub_123", 2990);

        var subscription = CreateActiveSubscription(Guid.NewGuid());
        subscription.UpdateStripeIds("sub_123", "cus_456");

        _subscriptionRepositoryMock
            .Setup(r => r.GetByStripeSubscriptionIdAsync("sub_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        _paymentRepositoryMock
            .Setup(r => r.GetByStripeInvoiceIdAsync("in_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync((SubscriptionPayment?)null);
        _paymentRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<SubscriptionPayment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _subscriptionRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ProcessEventAsync(eventType, eventData, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _paymentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SubscriptionPayment>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessEventAsync_ProcessesInvoicePaymentFailed_WhenValidEvent()
    {
        // Arrange
        var eventType = "invoice.payment_failed";
        var eventData = CreateInvoicePaymentFailedEvent("in_123", "sub_123");

        var subscription = CreateActiveSubscription(Guid.NewGuid());
        subscription.UpdateStripeIds("sub_123", "cus_456");

        _subscriptionRepositoryMock
            .Setup(r => r.GetByStripeSubscriptionIdAsync("sub_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        _subscriptionRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ProcessEventAsync(eventType, eventData, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task ProcessEventAsync_ProcessesTrialWillEnd_WhenValidEvent()
    {
        // Arrange
        var eventType = "customer.subscription.trial_will_end";
        var eventData = CreateTrialWillEndEvent("sub_123");

        var subscription = CreateTrialSubscription(Guid.NewGuid());
        subscription.UpdateStripeIds("sub_123", "cus_456");

        _subscriptionRepositoryMock
            .Setup(r => r.GetByStripeSubscriptionIdAsync("sub_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        _outboxMock
            .Setup(o => o.EnqueueAsync(It.IsAny<Araponga.Application.Models.OutboxMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ProcessEventAsync(eventType, eventData, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _outboxMock.Verify(o => o.EnqueueAsync(It.IsAny<Araponga.Application.Models.OutboxMessage>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessEventAsync_ReturnsSuccess_WhenInvalidEvent()
    {
        // Arrange
        var eventType = "unknown.event";
        var eventData = JsonDocument.Parse("{}").RootElement;

        // Act
        var result = await _service.ProcessEventAsync(eventType, eventData, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess); // Ignora eventos desconhecidos
    }

    [Fact]
    public async Task ProcessEventAsync_HandlesIdempotency_Correctly()
    {
        // Arrange
        var eventType = "invoice.payment_succeeded";
        var eventData = CreateInvoicePaymentSucceededEvent("in_123", "sub_123", 2990);

        var subscription = CreateActiveSubscription(Guid.NewGuid());
        subscription.UpdateStripeIds("sub_123", "cus_456");

        var existingPayment = CreatePayment(subscription.Id, 29.90m);

        _subscriptionRepositoryMock
            .Setup(r => r.GetByStripeSubscriptionIdAsync("sub_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        _paymentRepositoryMock
            .Setup(r => r.GetByStripeInvoiceIdAsync("in_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPayment); // Já existe pagamento

        // Act
        var result = await _service.ProcessEventAsync(eventType, eventData, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _paymentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SubscriptionPayment>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ProcessEventAsync_UpdatesSubscriptionStatus_Correctly()
    {
        // Arrange
        var eventType = "customer.subscription.updated";
        var eventData = CreateSubscriptionUpdatedEvent("sub_123", "canceled");

        var subscription = CreateActiveSubscription(Guid.NewGuid());
        subscription.UpdateStripeIds("sub_123", "cus_456");

        _subscriptionRepositoryMock
            .Setup(r => r.GetByStripeSubscriptionIdAsync("sub_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        _subscriptionRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ProcessEventAsync(eventType, eventData, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(SubscriptionStatus.CANCELED, subscription.Status);
    }

    [Fact]
    public async Task ProcessEventAsync_CreatesPaymentRecord_WhenPaymentSucceeded()
    {
        // Arrange
        var eventType = "invoice.payment_succeeded";
        var eventData = CreateInvoicePaymentSucceededEvent("in_123", "sub_123", 2990);

        var subscription = CreateActiveSubscription(Guid.NewGuid());
        subscription.UpdateStripeIds("sub_123", "cus_456");

        _subscriptionRepositoryMock
            .Setup(r => r.GetByStripeSubscriptionIdAsync("sub_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        _paymentRepositoryMock
            .Setup(r => r.GetByStripeInvoiceIdAsync("in_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync((SubscriptionPayment?)null);
        _paymentRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<SubscriptionPayment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _subscriptionRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ProcessEventAsync(eventType, eventData, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _paymentRepositoryMock.Verify(r => r.AddAsync(
            It.Is<SubscriptionPayment>(p => p.Amount == 29.90m && p.Status == SubscriptionPaymentStatus.Succeeded),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private static JsonElement CreateSubscriptionCreatedEvent(string subscriptionId, string customerId, string status)
    {
        var json = $$"""
        {
            "object": {
                "id": "{{subscriptionId}}",
                "customer": "{{customerId}}",
                "status": "{{status}}",
                "current_period_start": {{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}},
                "current_period_end": {{DateTimeOffset.UtcNow.AddMonths(1).ToUnixTimeSeconds()}}
            }
        }
        """;
        return JsonDocument.Parse(json).RootElement;
    }

    private static JsonElement CreateSubscriptionUpdatedEvent(string subscriptionId, string status)
    {
        var json = $$"""
        {
            "object": {
                "id": "{{subscriptionId}}",
                "status": "{{status}}",
                "current_period_start": {{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}},
                "current_period_end": {{DateTimeOffset.UtcNow.AddMonths(1).ToUnixTimeSeconds()}},
                "cancel_at_period_end": false
            }
        }
        """;
        return JsonDocument.Parse(json).RootElement;
    }

    private static JsonElement CreateSubscriptionDeletedEvent(string subscriptionId)
    {
        var json = $$"""
        {
            "object": {
                "id": "{{subscriptionId}}"
            }
        }
        """;
        return JsonDocument.Parse(json).RootElement;
    }

    private static JsonElement CreateInvoicePaymentSucceededEvent(string invoiceId, string subscriptionId, int amountPaid)
    {
        var json = $$"""
        {
            "object": {
                "id": "{{invoiceId}}",
                "subscription": "{{subscriptionId}}",
                "amount_paid": {{amountPaid}},
                "currency": "brl",
                "created": {{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}}
            }
        }
        """;
        return JsonDocument.Parse(json).RootElement;
    }

    private static JsonElement CreateInvoicePaymentFailedEvent(string invoiceId, string subscriptionId)
    {
        var json = $$"""
        {
            "object": {
                "id": "{{invoiceId}}",
                "subscription": "{{subscriptionId}}"
            }
        }
        """;
        return JsonDocument.Parse(json).RootElement;
    }

    private static JsonElement CreateTrialWillEndEvent(string subscriptionId)
    {
        var json = $$"""
        {
            "object": {
                "id": "{{subscriptionId}}",
                "trial_end": {{DateTimeOffset.UtcNow.AddDays(3).ToUnixTimeSeconds()}}
            }
        }
        """;
        return JsonDocument.Parse(json).RootElement;
    }

    private static Subscription CreateActiveSubscription(Guid planId)
    {
        return new Subscription(
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
    }

    private static Subscription CreateTrialSubscription(Guid planId)
    {
        return new Subscription(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            planId,
            SubscriptionStatus.TRIALING,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(1),
            DateTime.UtcNow.AddDays(3),
            null,
            null,
            null);
    }

    private static SubscriptionPayment CreatePayment(Guid subscriptionId, decimal amount)
    {
        var now = DateTime.UtcNow;
        return new SubscriptionPayment(
            Guid.NewGuid(),
            subscriptionId,
            amount,
            "BRL",
            SubscriptionPaymentStatus.Succeeded,
            now,
            now,
            now.AddMonths(1),
            "in_123",
            null,
            null);
    }
}
