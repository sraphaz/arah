using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Subscriptions;
using Moq;
using System.Text.Json;
using Xunit;

namespace Araponga.Tests.Modules.Subscriptions.Application;

public sealed class MercadoPagoWebhookServiceTests
{
    private readonly Mock<ISubscriptionRepository> _subscriptionRepositoryMock;
    private readonly Mock<ISubscriptionPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<ISubscriptionPlanRepository> _planRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly MercadoPagoWebhookService _service;

    public MercadoPagoWebhookServiceTests()
    {
        _subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
        _paymentRepositoryMock = new Mock<ISubscriptionPaymentRepository>();
        _planRepositoryMock = new Mock<ISubscriptionPlanRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _service = new MercadoPagoWebhookService(
            _subscriptionRepositoryMock.Object,
            _paymentRepositoryMock.Object,
            _planRepositoryMock.Object,
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object,
            Microsoft.Extensions.Logging.Abstractions.NullLogger<MercadoPagoWebhookService>.Instance);
    }

    [Fact]
    public async Task ProcessEventAsync_ProcessesSubscriptionCreated_WhenValidEvent()
    {
        // Arrange
        var eventType = "subscription.created";
        var eventData = CreateMercadoPagoSubscriptionCreatedEvent("mp_sub_123", "mp_cus_456", "active");

        var subscription = CreateActiveSubscription(Guid.NewGuid());
        // MercadoPago usa o mesmo campo StripeSubscriptionId para armazenar o ID
        _subscriptionRepositoryMock
            .Setup(r => r.GetByStripeSubscriptionIdAsync("mp_sub_123", It.IsAny<CancellationToken>()))
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
        var eventType = "subscription.updated";
        var eventData = CreateMercadoPagoSubscriptionUpdatedEvent("mp_sub_123", "active");

        var subscription = CreateActiveSubscription(Guid.NewGuid());
        subscription.UpdateStripeIds("mp_sub_123", "mp_cus_456");

        _subscriptionRepositoryMock
            .Setup(r => r.GetByStripeSubscriptionIdAsync("mp_sub_123", It.IsAny<CancellationToken>()))
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
    public async Task ProcessEventAsync_ProcessesPaymentApproved_WhenValidEvent()
    {
        // Arrange
        var eventType = "payment.approved";
        var eventData = CreateMercadoPagoPaymentApprovedEvent("mp_pay_123", "mp_sub_123", 29.90m);

        var subscription = CreateActiveSubscription(Guid.NewGuid());
        subscription.UpdateStripeIds("mp_sub_123", "mp_cus_456");

        _subscriptionRepositoryMock
            .Setup(r => r.GetByStripeSubscriptionIdAsync("mp_sub_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        _paymentRepositoryMock
            .Setup(r => r.GetByStripeInvoiceIdAsync("mp_pay_123", It.IsAny<CancellationToken>()))
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
    public async Task ProcessEventAsync_ProcessesPaymentRejected_WhenValidEvent()
    {
        // Arrange
        var eventType = "payment.rejected";
        var eventData = CreateMercadoPagoPaymentRejectedEvent("mp_pay_123", "mp_sub_123");

        var subscription = CreateActiveSubscription(Guid.NewGuid());
        subscription.UpdateStripeIds("mp_sub_123", "mp_cus_456");

        _subscriptionRepositoryMock
            .Setup(r => r.GetByStripeSubscriptionIdAsync("mp_sub_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        _paymentRepositoryMock
            .Setup(r => r.GetByStripeInvoiceIdAsync("123", It.IsAny<CancellationToken>())) // ID numérico do Mercado Pago
            .ReturnsAsync((SubscriptionPayment?)null); // Pagamento não existe ainda
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
        _subscriptionRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Subscription>(), It.IsAny<CancellationToken>()), Times.Once);
        _paymentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SubscriptionPayment>(), It.IsAny<CancellationToken>()), Times.Once);
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
        var eventType = "payment.approved";
        var eventData = CreateMercadoPagoPaymentApprovedEvent("mp_pay_123", "mp_sub_123", 29.90m);

        var subscription = CreateActiveSubscription(Guid.NewGuid());
        subscription.UpdateStripeIds("mp_sub_123", "mp_cus_456");

        var existingPayment = CreatePayment(subscription.Id, 29.90m, "123"); // ID numérico do Mercado Pago

        _subscriptionRepositoryMock
            .Setup(r => r.GetByStripeSubscriptionIdAsync("mp_sub_123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(subscription);
        _paymentRepositoryMock
            .Setup(r => r.GetByStripeInvoiceIdAsync("123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPayment); // Já existe pagamento
        _paymentRepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<SubscriptionPayment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock
            .Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ProcessEventAsync(eventType, eventData, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _paymentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<SubscriptionPayment>(), It.IsAny<CancellationToken>()), Times.Never);
        _paymentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<SubscriptionPayment>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private static JsonElement CreateMercadoPagoSubscriptionCreatedEvent(string subscriptionId, string payerId, string status)
    {
        var json = $$"""
        {
            "id": "{{subscriptionId}}",
            "payer_id": "{{payerId}}",
            "status": "{{status}}"
        }
        """;
        return JsonDocument.Parse(json).RootElement;
    }

    private static JsonElement CreateMercadoPagoSubscriptionUpdatedEvent(string subscriptionId, string status)
    {
        var json = $$"""
        {
            "id": "{{subscriptionId}}",
            "status": "{{status}}"
        }
        """;
        return JsonDocument.Parse(json).RootElement;
    }

    private static JsonElement CreateMercadoPagoPaymentApprovedEvent(string paymentId, string subscriptionId, decimal amount)
    {
        var dateCreated = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
        // Mercado Pago usa ID numérico para pagamentos
        var paymentIdNum = long.TryParse(paymentId.Replace("mp_pay_", ""), out var num) ? num : 123;
        var amountStr = amount.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var json = $$"""
        {
            "id": {{paymentIdNum}},
            "subscription_id": "{{subscriptionId}}",
            "transaction_amount": {{amountStr}},
            "status": "approved",
            "date_created": "{{dateCreated}}"
        }
        """;
        return JsonDocument.Parse(json).RootElement;
    }

    private static JsonElement CreateMercadoPagoPaymentRejectedEvent(string paymentId, string subscriptionId)
    {
        // Mercado Pago usa ID numérico para pagamentos
        var paymentIdNum = long.TryParse(paymentId.Replace("mp_pay_", ""), out var num) ? num : 123;
        var json = $$"""
        {
            "id": {{paymentIdNum}},
            "subscription_id": "{{subscriptionId}}",
            "status": "rejected",
            "status_detail": "cc_rejected_insufficient_amount"
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

    private static SubscriptionPayment CreatePayment(Guid subscriptionId, decimal amount, string? externalId = null)
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
            externalId,
            null,
            null);
    }
}
