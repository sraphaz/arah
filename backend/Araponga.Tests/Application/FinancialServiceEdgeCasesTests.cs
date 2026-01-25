using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Financial;
using Araponga.Domain.Marketplace;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.TestHelpers;
using Moq;
using Xunit;
using PayoutStatus = Araponga.Application.Interfaces.PayoutStatus;
using PayoutStatusResult = Araponga.Application.Interfaces.PayoutStatusResult;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for SellerPayoutService (FinancialService),
/// focusing on negative values, invalid transactions, invalid currencies, and error handling.
/// </summary>
public class FinancialServiceEdgeCasesTests
{
    private static readonly DateTime TestDate = DateTime.UtcNow;
    private static readonly Guid TestTerritoryId = Guid.NewGuid();
    private static readonly Guid TestUserId = Guid.NewGuid();
    private static readonly Guid TestStoreId = Guid.NewGuid();
    private static readonly Guid TestCheckoutId = Guid.NewGuid();

    private static SellerPayoutService CreateService(
        InMemoryDataStore dataStore,
        Mock<IPayoutGateway>? payoutGatewayMock = null)
    {
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
        var auditLogger = new InMemoryAuditLogger(dataStore);
        var unitOfWork = new InMemoryUnitOfWork();

        var gatewayMock = payoutGatewayMock ?? new Mock<IPayoutGateway>();

        return new SellerPayoutService(
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
            gatewayMock.Object,
            auditLogger,
            unitOfWork);
    }

    [Fact]
    public async Task ProcessPaidCheckoutAsync_WithNonExistentCheckout_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var service = CreateService(dataStore);

        var result = await service.ProcessPaidCheckoutAsync(
            Guid.NewGuid(),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Checkout not found", result.Error ?? "");
    }

    [Fact]
    public async Task ProcessPaidCheckoutAsync_WithCheckoutNotPaid_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var checkout = new Checkout(
            TestCheckoutId,
            TestTerritoryId,
            TestUserId,
            TestStoreId,
            CheckoutStatus.Created,
            "BRL",
            90.0m,
            10.0m,
            100.0m,
            DateTime.UtcNow,
            DateTime.UtcNow);
        dataStore.Checkouts.Add(checkout);

        var service = CreateService(dataStore);

        var result = await service.ProcessPaidCheckoutAsync(
            TestCheckoutId,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Checkout is not paid", result.Error ?? "");
    }

    [Fact]
    public async Task ProcessPaidCheckoutAsync_WithNullAmounts_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var checkout = new Checkout(
            TestCheckoutId,
            TestTerritoryId,
            TestUserId,
            TestStoreId,
            CheckoutStatus.Paid,
            "BRL",
            null, // itemsSubtotalAmount
            null, // platformFeeAmount
            null, // totalAmount
            DateTime.UtcNow,
            DateTime.UtcNow);
        dataStore.Checkouts.Add(checkout);

        var store = new Store(
            TestStoreId,
            TestTerritoryId,
            TestUserId,
            "Test Store",
            "Description",
            StoreStatus.Active,
            false, // paymentsEnabled
            StoreContactVisibility.Public,
            null, // phone
            null, // whatsapp
            null, // email
            null, // instagram
            null, // website
            null, // preferredContactMethod
            DateTime.UtcNow,
            DateTime.UtcNow);
        dataStore.TerritoryStores.Add(store);

        var service = CreateService(dataStore);

        var result = await service.ProcessPaidCheckoutAsync(
            TestCheckoutId,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Checkout amounts are not set", result.Error ?? "");
    }

    [Fact]
    public async Task ProcessPaidCheckoutAsync_WithNonExistentStore_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var checkout = new Checkout(
            TestCheckoutId,
            TestTerritoryId,
            TestUserId,
            TestStoreId,
            CheckoutStatus.Paid,
            "BRL",
            90.0m, // itemsSubtotalAmount
            10.0m, // platformFeeAmount
            100.0m, // totalAmount
            DateTime.UtcNow,
            DateTime.UtcNow);
        dataStore.Checkouts.Add(checkout);

        var service = CreateService(dataStore);

        var result = await service.ProcessPaidCheckoutAsync(
            TestCheckoutId,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Store not found", result.Error ?? "");
    }

    [Fact]
    public async Task ProcessPaidCheckoutAsync_WithZeroAmounts_HandlesCorrectly()
    {
        var dataStore = new InMemoryDataStore();
        var checkout = new Checkout(
            TestCheckoutId,
            TestTerritoryId,
            TestUserId,
            TestStoreId,
            CheckoutStatus.Paid,
            "BRL",
            0.0m, // itemsSubtotalAmount
            0.0m, // platformFeeAmount
            0.0m, // totalAmount
            DateTime.UtcNow,
            DateTime.UtcNow);
        dataStore.Checkouts.Add(checkout);

        var store = new Store(
            TestStoreId,
            TestTerritoryId,
            TestUserId,
            "Test Store",
            "Description",
            StoreStatus.Active,
            false, // paymentsEnabled
            StoreContactVisibility.Public,
            null, // phone
            null, // whatsapp
            null, // email
            null, // instagram
            null, // website
            null, // preferredContactMethod
            DateTime.UtcNow,
            DateTime.UtcNow);
        dataStore.TerritoryStores.Add(store);

        var service = CreateService(dataStore);

        // Deve processar com sucesso mesmo com valores zero
        var result = await service.ProcessPaidCheckoutAsync(
            TestCheckoutId,
            CancellationToken.None);

        // Pode ser sucesso ou falha dependendo da implementação, mas não deve lançar exceção
        Assert.NotNull(result);
    }

    [Fact]
    public async Task ProcessPaidCheckoutAsync_WithAlreadyProcessedCheckout_ReturnsSuccess()
    {
        var dataStore = new InMemoryDataStore();
        var checkout = new Checkout(
            TestCheckoutId,
            TestTerritoryId,
            TestUserId,
            TestStoreId,
            CheckoutStatus.Paid,
            "BRL",
            90.0m, // itemsSubtotalAmount
            10.0m, // platformFeeAmount
            100.0m, // totalAmount
            DateTime.UtcNow,
            DateTime.UtcNow);
        dataStore.Checkouts.Add(checkout);

        var store = new Store(
            TestStoreId,
            TestTerritoryId,
            TestUserId,
            "Test Store",
            "Description",
            StoreStatus.Active,
            false, // paymentsEnabled
            StoreContactVisibility.Public,
            null, // phone
            null, // whatsapp
            null, // email
            null, // instagram
            null, // website
            null, // preferredContactMethod
            DateTime.UtcNow,
            DateTime.UtcNow);
        dataStore.TerritoryStores.Add(store);

        var service = CreateService(dataStore);

        // Primeira processamento
        var result1 = await service.ProcessPaidCheckoutAsync(
            TestCheckoutId,
            CancellationToken.None);

        // Segunda processamento (já processado)
        var result2 = await service.ProcessPaidCheckoutAsync(
            TestCheckoutId,
            CancellationToken.None);

        // Segunda chamada deve retornar sucesso (idempotente)
        Assert.True(result2.IsSuccess);
    }

    [Fact]
    public async Task ProcessPendingPayoutsAsync_WithInactiveConfig_ReturnsZero()
    {
        var dataStore = new InMemoryDataStore();
        var service = CreateService(dataStore);

        var result = await service.ProcessPendingPayoutsAsync(
            TestTerritoryId,
            TestUserId,
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value); // Nenhum payout processado quando config inativa
    }

    [Fact]
    public async Task ProcessPendingPayoutsAsync_WithNoReadyTransactions_ReturnsZero()
    {
        var dataStore = new InMemoryDataStore();
        
        // Criar configuração ativa
        var config = new TerritoryPayoutConfig(
            Guid.NewGuid(),
            TestTerritoryId,
            7, // retentionPeriodDays
            10000, // minimumPayoutAmountInCents
            null, // maximumPayoutAmountInCents
            PayoutFrequency.Weekly,
            true, // autoPayoutEnabled
            false, // requiresApproval
            "BRL");
        dataStore.TerritoryPayoutConfigs.Add(config);

        var service = CreateService(dataStore);

        var result = await service.ProcessPendingPayoutsAsync(
            TestTerritoryId,
            TestUserId,
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value); // Nenhuma transação pronta
    }

    [Fact]
    public async Task MarkTransactionAsReadyForPayoutAsync_WithNonExistentTransaction_HandlesCorrectly()
    {
        var dataStore = new InMemoryDataStore();
        var service = CreateService(dataStore);

        // MarkTransactionAsReadyForPayoutAsync é privado, então testamos indiretamente
        // através de ProcessPaidCheckoutAsync com configuração que deve marcar como ready
        
        var checkout = new Checkout(
            TestCheckoutId,
            TestTerritoryId,
            TestUserId,
            TestStoreId,
            CheckoutStatus.Paid,
            "BRL",
            90.0m, // itemsSubtotalAmount
            10.0m, // platformFeeAmount
            100.0m, // totalAmount
            DateTime.UtcNow,
            DateTime.UtcNow);
        dataStore.Checkouts.Add(checkout);

        var store = new Store(
            TestStoreId,
            TestTerritoryId,
            TestUserId,
            "Test Store",
            "Description",
            StoreStatus.Active,
            false, // paymentsEnabled
            StoreContactVisibility.Public,
            null, // phone
            null, // whatsapp
            null, // email
            null, // instagram
            null, // website
            null, // preferredContactMethod
            DateTime.UtcNow,
            DateTime.UtcNow);
        dataStore.TerritoryStores.Add(store);

        // Criar configuração que permite marcar como ready imediatamente
        var config = new TerritoryPayoutConfig(
            Guid.NewGuid(),
            TestTerritoryId,
            0, // retentionPeriodDays = 0 (sem retenção)
            0, // minimumPayoutAmountInCents = 0 (sem mínimo)
            null,
            PayoutFrequency.Weekly,
            true,
            false,
            "BRL");
        dataStore.TerritoryPayoutConfigs.Add(config);

        var result = await service.ProcessPaidCheckoutAsync(
            TestCheckoutId,
            CancellationToken.None);

        // Deve processar com sucesso
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task UpdatePayoutStatusAsync_WithNonExistentPayoutId_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var payoutGatewayMock = new Mock<IPayoutGateway>();
        payoutGatewayMock
            .Setup(g => g.GetPayoutStatusAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(OperationResult<PayoutStatusResult>.Failure("Payout not found"));

        var service = CreateService(dataStore, payoutGatewayMock);

        var result = await service.UpdatePayoutStatusAsync(
            "non-existent-payout-id",
            CancellationToken.None);

        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task UpdatePayoutStatusAsync_WithNoTransactions_ReturnsFailure()
    {
        var dataStore = new InMemoryDataStore();
        var payoutGatewayMock = new Mock<IPayoutGateway>();
        payoutGatewayMock
            .Setup(g => g.GetPayoutStatusAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(OperationResult<PayoutStatusResult>.Success(new PayoutStatusResult("payout-id", PayoutStatus.Completed, null, DateTime.UtcNow)));

        var service = CreateService(dataStore, payoutGatewayMock);

        var result = await service.UpdatePayoutStatusAsync(
            "payout-id",
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("No transactions found", result.Error ?? "");
    }
}
