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
/// DOD-06 — estorno persiste reversão append-only no ledger.
/// </summary>
public sealed class RefundLedgerIntegrationTests
{
    [Fact]
    public async Task RefundAsync_AfterPaidCheckout_CreatesRefundFinancialTransactions()
    {
        var dataStore = new InMemoryDataStore();
        var checkoutRepository = new InMemoryCheckoutRepository(dataStore);
        var storeRepository = new InMemoryStoreRepository(dataStore);
        var sellerTransactionRepository = new InMemorySellerTransactionRepository(dataStore);
        var sellerBalanceRepository = new InMemorySellerBalanceRepository(dataStore);
        var financialTransactionRepository = new InMemoryFinancialTransactionRepository(dataStore);
        var transactionStatusHistoryRepository = new InMemoryTransactionStatusHistoryRepository(dataStore);
        var platformRevenueTransactionRepository = new InMemoryPlatformRevenueTransactionRepository(dataStore);
        var platformFinancialBalanceRepository = new InMemoryPlatformFinancialBalanceRepository(dataStore);
        var platformExpenseTransactionRepository = new InMemoryPlatformExpenseTransactionRepository(dataStore);
        var payoutConfigRepository = new InMemoryTerritoryPayoutConfigRepository(dataStore);
        var feeSplitRules = new InMemoryFeeSplitRuleRepository(dataStore);
        var payoutGatewayMock = new Mock<IPayoutGateway>();
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();
        var paymentGateway = new MockPaymentGateway();

        var sellerPayoutService = new SellerPayoutService(
            checkoutRepository,
            storeRepository,
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

        var paymentService = new PaymentService(
            checkoutRepository,
            paymentGateway,
            sellerPayoutService,
            unitOfWork);

        var refundService = new RefundService(
            checkoutRepository,
            feeSplitRules,
            sellerPayoutService);

        var store = new Store(
            Guid.NewGuid(),
            TestIds.Territory1,
            TestIds.ResidentUser,
            "Refund Ledger Store",
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
        await storeRepository.AddAsync(store, CancellationToken.None);

        var checkout = new Checkout(
            Guid.NewGuid(),
            TestIds.Territory1,
            Guid.NewGuid(),
            store.Id,
            CheckoutStatus.Created,
            "BRL",
            100m,
            10m,
            110m,
            DateTime.UtcNow,
            DateTime.UtcNow);
        await checkoutRepository.AddAsync(checkout, CancellationToken.None);

        var initiate = await paymentService.InitiatePaymentAsync(checkout.Id, PaymentMethod.Pix, CancellationToken.None);
        paymentGateway.SimulatePaymentApproved(initiate.Value!.GatewayPaymentId);
        await paymentService.ConfirmPaymentAsync(checkout.Id, initiate.Value.GatewayPaymentId, CancellationToken.None);

        var refund = await refundService.RefundAsync(checkout.Id, CancellationToken.None);

        Assert.True(refund.IsSuccess);
        Assert.Equal(CheckoutStatus.Refunded, refund.Value!.Status);

        var ledger = await financialTransactionRepository.GetByRelatedEntityIdAsync(
            checkout.Id,
            "Checkout",
            CancellationToken.None);

        Assert.Contains(ledger, t => t.Type == TransactionType.Refund && t.AmountInCents < 0);
        Assert.Equal(2, ledger.Count(t => t.Type == TransactionType.Refund));

        var sellerTx = await sellerTransactionRepository.GetByCheckoutIdAsync(checkout.Id, CancellationToken.None);
        Assert.NotNull(sellerTx);
        Assert.Equal(SellerTransactionStatus.Canceled, sellerTx!.Status);
    }
}
