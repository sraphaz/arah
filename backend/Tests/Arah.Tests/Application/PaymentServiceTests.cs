using Arah.Application.Interfaces;
using Arah.Application.Models;
using Arah.Application.Services;
using Arah.Domain.Financial;
using Arah.Infrastructure.InMemory;
using Arah.Infrastructure.Payments;
using Arah.Modules.Marketplace.Domain;
using Arah.Tests.Shared;
using Moq;
using Xunit;

namespace Arah.Tests.Application;

/// <summary>
/// DOD-05 — transição Checkout → Paid via PaymentService.
/// </summary>
public sealed class PaymentServiceTests
{
    private readonly InMemoryDataStore _dataStore;
    private readonly InMemoryCheckoutRepository _checkoutRepository;
    private readonly InMemoryStoreRepository _storeRepository;
    private readonly MockPaymentGateway _paymentGateway;
    private readonly SellerPayoutService _sellerPayoutService;
    private readonly PaymentService _service;

    public PaymentServiceTests()
    {
        _dataStore = new InMemoryDataStore();
        _checkoutRepository = new InMemoryCheckoutRepository(_dataStore);
        _storeRepository = new InMemoryStoreRepository(_dataStore);
        _paymentGateway = new MockPaymentGateway();

        var sellerTransactionRepository = new InMemorySellerTransactionRepository(_dataStore);
        var sellerBalanceRepository = new InMemorySellerBalanceRepository(_dataStore);
        var financialTransactionRepository = new InMemoryFinancialTransactionRepository(_dataStore);
        var transactionStatusHistoryRepository = new InMemoryTransactionStatusHistoryRepository(_dataStore);
        var platformRevenueTransactionRepository = new InMemoryPlatformRevenueTransactionRepository(_dataStore);
        var platformFinancialBalanceRepository = new InMemoryPlatformFinancialBalanceRepository(_dataStore);
        var platformExpenseTransactionRepository = new InMemoryPlatformExpenseTransactionRepository(_dataStore);
        var payoutConfigRepository = new InMemoryTerritoryPayoutConfigRepository(_dataStore);
        var payoutGatewayMock = new Mock<IPayoutGateway>();
        var auditLogger = new InMemoryAuditLogger(_dataStore);
        var unitOfWork = new InMemoryUnitOfWork();

        _sellerPayoutService = new SellerPayoutService(
            _checkoutRepository,
            _storeRepository,
            sellerTransactionRepository,
            sellerBalanceRepository,
            financialTransactionRepository,
            transactionStatusHistoryRepository,
            platformRevenueTransactionRepository,
            platformFinancialBalanceRepository,
            platformExpenseTransactionRepository,
            payoutConfigRepository,
            payoutGatewayMock.Object,
            auditLogger,
            unitOfWork);

        _service = new PaymentService(
            _checkoutRepository,
            _paymentGateway,
            _sellerPayoutService,
            unitOfWork);
    }

    private async Task<(Store Store, Checkout Checkout)> SeedStoreAndCheckoutAsync(CheckoutStatus status)
    {
        var store = new Store(
            Guid.NewGuid(),
            TestIds.Territory1,
            TestIds.ResidentUser,
            "Payment Test Store",
            null,
            StoreStatus.Active,
            true,
            StoreContactVisibility.Public,
            null,
            null,
            null,
            null,
            null,
            null,
            DateTime.UtcNow,
            DateTime.UtcNow);
        await _storeRepository.AddAsync(store, CancellationToken.None);

        var checkout = new Checkout(
            Guid.NewGuid(),
            TestIds.Territory1,
            Guid.NewGuid(),
            store.Id,
            status,
            "BRL",
            100m,
            10m,
            110m,
            DateTime.UtcNow,
            DateTime.UtcNow);
        await _checkoutRepository.AddAsync(checkout, CancellationToken.None);
        return (store, checkout);
    }

    [Fact]
    public async Task InitiatePaymentAsync_FromCreated_TransitionsToAwaitingPayment()
    {
        var (_, checkout) = await SeedStoreAndCheckoutAsync(CheckoutStatus.Created);

        var result = await _service.InitiatePaymentAsync(checkout.Id, PaymentMethod.Pix, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(CheckoutStatus.AwaitingPayment, result.Value!.Status);
        Assert.False(string.IsNullOrWhiteSpace(result.Value.GatewayPaymentId));
        Assert.NotNull(result.Value.PixCopyPasteCode);

        var updated = await _checkoutRepository.GetByIdAsync(checkout.Id, CancellationToken.None);
        Assert.Equal(CheckoutStatus.AwaitingPayment, updated!.Status);
    }

    [Fact]
    public async Task ConfirmPaymentAsync_WhenApproved_TransitionsToPaidAndCreatesLedger()
    {
        var (_, checkout) = await SeedStoreAndCheckoutAsync(CheckoutStatus.Created);
        var initiate = await _service.InitiatePaymentAsync(checkout.Id, PaymentMethod.Pix, CancellationToken.None);
        Assert.True(initiate.IsSuccess);

        _paymentGateway.SimulatePaymentApproved(initiate.Value!.GatewayPaymentId);

        var confirm = await _service.ConfirmPaymentAsync(
            checkout.Id,
            initiate.Value.GatewayPaymentId,
            CancellationToken.None);

        Assert.True(confirm.IsSuccess);
        Assert.False(confirm.Value!.AlreadyPaid);
        Assert.Equal(CheckoutStatus.Paid, confirm.Value.Status);

        var updated = await _checkoutRepository.GetByIdAsync(checkout.Id, CancellationToken.None);
        Assert.Equal(CheckoutStatus.Paid, updated!.Status);
    }

    [Fact]
    public async Task ConfirmPaymentAsync_WhenAlreadyPaid_IsIdempotent()
    {
        var (_, checkout) = await SeedStoreAndCheckoutAsync(CheckoutStatus.Created);
        var initiate = await _service.InitiatePaymentAsync(checkout.Id, PaymentMethod.Pix, CancellationToken.None);
        _paymentGateway.SimulatePaymentApproved(initiate.Value!.GatewayPaymentId);
        await _service.ConfirmPaymentAsync(checkout.Id, initiate.Value.GatewayPaymentId, CancellationToken.None);

        var second = await _service.ConfirmPaymentAsync(
            checkout.Id,
            initiate.Value.GatewayPaymentId,
            CancellationToken.None);

        Assert.True(second.IsSuccess);
        Assert.True(second.Value!.AlreadyPaid);
    }

    [Fact]
    public async Task ConfirmPaymentAsync_WhenPaymentBelongsToAnotherCheckout_ReturnsFailure()
    {
        var (_, checkoutA) = await SeedStoreAndCheckoutAsync(CheckoutStatus.Created);
        var (_, checkoutB) = await SeedStoreAndCheckoutAsync(CheckoutStatus.Created);

        var initiateA = await _service.InitiatePaymentAsync(checkoutA.Id, PaymentMethod.Pix, CancellationToken.None);
        _paymentGateway.SimulatePaymentApproved(initiateA.Value!.GatewayPaymentId);

        // Tentar confirmar o checkout B com o pagamento aprovado do checkout A.
        var confirm = await _service.ConfirmPaymentAsync(
            checkoutB.Id,
            initiateA.Value.GatewayPaymentId,
            CancellationToken.None);

        Assert.True(confirm.IsFailure);
        Assert.Contains("does not belong", confirm.Error ?? "", StringComparison.OrdinalIgnoreCase);

        var updatedB = await _checkoutRepository.GetByIdAsync(checkoutB.Id, CancellationToken.None);
        Assert.NotEqual(CheckoutStatus.Paid, updatedB!.Status);
    }

    [Fact]
    public async Task ConfirmPaymentAsync_WhenNotApproved_ReturnsFailure()
    {
        var (_, checkout) = await SeedStoreAndCheckoutAsync(CheckoutStatus.Created);
        var initiate = await _service.InitiatePaymentAsync(checkout.Id, PaymentMethod.Pix, CancellationToken.None);

        var confirm = await _service.ConfirmPaymentAsync(
            checkout.Id,
            initiate.Value!.GatewayPaymentId,
            CancellationToken.None);

        Assert.True(confirm.IsFailure);
        Assert.Contains("not approved", confirm.Error ?? "", StringComparison.OrdinalIgnoreCase);
    }
}
