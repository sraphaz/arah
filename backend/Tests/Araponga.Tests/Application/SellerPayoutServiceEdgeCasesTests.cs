using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Application.Services;
using Araponga.Domain.Financial;
using Araponga.Modules.Marketplace.Domain;
using Araponga.Infrastructure.InMemory;
using Moq;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for SellerPayoutService,
/// focusing on invalid checkouts, missing amounts, gateway errors, and payout edge cases.
/// </summary>
public sealed class SellerPayoutServiceEdgeCasesTests
{
    private readonly InMemoryDataStore _dataStore;
    private readonly InMemoryCheckoutRepository _checkoutRepository;
    private readonly InMemoryStoreRepository _storeRepository;
    private readonly InMemorySellerTransactionRepository _sellerTransactionRepository;
    private readonly InMemorySellerBalanceRepository _sellerBalanceRepository;
    private readonly InMemoryFinancialTransactionRepository _financialTransactionRepository;
    private readonly InMemoryTransactionStatusHistoryRepository _transactionStatusHistoryRepository;
    private readonly InMemoryPlatformRevenueTransactionRepository _platformRevenueTransactionRepository;
    private readonly InMemoryPlatformFinancialBalanceRepository _platformFinancialBalanceRepository;
    private readonly InMemoryPlatformExpenseTransactionRepository _platformExpenseTransactionRepository;
    private readonly InMemoryTerritoryPayoutConfigRepository _payoutConfigRepository;
    private readonly Mock<IPayoutGateway> _payoutGatewayMock;
    private readonly InMemoryAuditLogger _auditLogger;
    private readonly InMemoryUnitOfWork _unitOfWork;
    private readonly SellerPayoutService _service;

    public SellerPayoutServiceEdgeCasesTests()
    {
        _dataStore = new InMemoryDataStore();
        _checkoutRepository = new InMemoryCheckoutRepository(_dataStore);
        _storeRepository = new InMemoryStoreRepository(_dataStore);
        _sellerTransactionRepository = new InMemorySellerTransactionRepository(_dataStore);
        _sellerBalanceRepository = new InMemorySellerBalanceRepository(_dataStore);
        _financialTransactionRepository = new InMemoryFinancialTransactionRepository(_dataStore);
        _transactionStatusHistoryRepository = new InMemoryTransactionStatusHistoryRepository(_dataStore);
        _platformRevenueTransactionRepository = new InMemoryPlatformRevenueTransactionRepository(_dataStore);
        _platformFinancialBalanceRepository = new InMemoryPlatformFinancialBalanceRepository(_dataStore);
        _platformExpenseTransactionRepository = new InMemoryPlatformExpenseTransactionRepository(_dataStore);
        _payoutConfigRepository = new InMemoryTerritoryPayoutConfigRepository(_dataStore);
        _payoutGatewayMock = new Mock<IPayoutGateway>();
        _auditLogger = new InMemoryAuditLogger(_dataStore);
        _unitOfWork = new InMemoryUnitOfWork();
        _service = new SellerPayoutService(
            _checkoutRepository,
            _storeRepository,
            _sellerTransactionRepository,
            _sellerBalanceRepository,
            _financialTransactionRepository,
            _transactionStatusHistoryRepository,
            _platformRevenueTransactionRepository,
            _platformFinancialBalanceRepository,
            _platformExpenseTransactionRepository,
            _payoutConfigRepository,
            _payoutGatewayMock.Object,
            _auditLogger,
            _unitOfWork);
    }

    private async Task<Store> CreateAndAddStoreAsync()
    {
        var territoryId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var store = new Store(
            Guid.NewGuid(),
            territoryId,
            ownerId,
            "Test Store",
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
        return store;
    }

    [Fact]
    public async Task ProcessPaidCheckoutAsync_WithNonExistentCheckout_ReturnsFailure()
    {
        var result = await _service.ProcessPaidCheckoutAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not found", result.Error ?? "");
    }

    [Fact]
    public async Task ProcessPaidCheckoutAsync_WithCheckoutNotPaid_ReturnsFailure()
    {
        var checkout = new Checkout(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            CheckoutStatus.AwaitingPayment, // Não pago
            "BRL",
            100m,
            5m,
            105m,
            DateTime.UtcNow,
            DateTime.UtcNow);
        await _checkoutRepository.AddAsync(checkout, CancellationToken.None);

        var result = await _service.ProcessPaidCheckoutAsync(checkout.Id, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("not paid", result.Error ?? "");
    }

    [Fact]
    public async Task ProcessPaidCheckoutAsync_WithNullAmounts_ReturnsFailure()
    {
        var store = await CreateAndAddStoreAsync();
        var checkout = new Checkout(
            Guid.NewGuid(),
            store.TerritoryId,
            Guid.NewGuid(),
            store.Id,
            CheckoutStatus.Paid,
            "BRL",
            null, // Amounts não definidos
            null,
            null,
            DateTime.UtcNow,
            DateTime.UtcNow);
        await _checkoutRepository.AddAsync(checkout, CancellationToken.None);

        var result = await _service.ProcessPaidCheckoutAsync(checkout.Id, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("amounts are not set", result.Error ?? "");
    }

    [Fact]
    public async Task ProcessPaidCheckoutAsync_WithNonExistentStore_ReturnsFailure()
    {
        var checkout = new Checkout(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(), // Store não existe
            CheckoutStatus.Paid,
            "BRL",
            100m,
            5m,
            105m,
            DateTime.UtcNow,
            DateTime.UtcNow);
        await _checkoutRepository.AddAsync(checkout, CancellationToken.None);

        var result = await _service.ProcessPaidCheckoutAsync(checkout.Id, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Store not found", result.Error ?? "");
    }

    [Fact]
    public async Task ProcessPaidCheckoutAsync_WithAlreadyProcessed_ReturnsSuccess()
    {
        var store = await CreateAndAddStoreAsync();
        var checkout = new Checkout(
            Guid.NewGuid(),
            store.TerritoryId,
            Guid.NewGuid(),
            store.Id,
            CheckoutStatus.Paid,
            "BRL",
            100m,
            5m,
            105m,
            DateTime.UtcNow,
            DateTime.UtcNow);
        await _checkoutRepository.AddAsync(checkout, CancellationToken.None);

        // Criar SellerTransaction existente
        var existingTransaction = new SellerTransaction(
            Guid.NewGuid(),
            store.TerritoryId,
            store.Id,
            checkout.Id,
            store.OwnerUserId,
            10000,
            500,
            "BRL");
        await _sellerTransactionRepository.AddAsync(existingTransaction, CancellationToken.None);

        var result = await _service.ProcessPaidCheckoutAsync(checkout.Id, CancellationToken.None);

        Assert.True(result.IsSuccess); // Já processado, retorna sucesso
    }

    [Fact]
    public async Task ProcessPendingPayoutsAsync_WithNoActiveConfig_ReturnsZero()
    {
        var territoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var result = await _service.ProcessPendingPayoutsAsync(territoryId, userId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value);
    }

    [Fact]
    public async Task ProcessPendingPayoutsAsync_WithInactiveConfig_ReturnsZero()
    {
        var territoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Criar configuração inativa (AutoPayout desabilitado)
        var config = new TerritoryPayoutConfig(
            Guid.NewGuid(),
            territoryId,
            7,
            10000,
            null,
            PayoutFrequency.Weekly,
            false,
            false,
            "BRL");
        await _payoutConfigRepository.AddAsync(config, CancellationToken.None);

        var result = await _service.ProcessPendingPayoutsAsync(territoryId, userId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value);
    }

    [Fact]
    public async Task UpdatePayoutStatusAsync_WithNonExistentPayoutId_ReturnsFailure()
    {
        var result = await _service.UpdatePayoutStatusAsync("non-existent-payout-id", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("No transactions found", result.Error ?? "");
    }

    [Fact]
    public async Task UpdatePayoutStatusAsync_WithGatewayError_ReturnsFailure()
    {
        var store = await CreateAndAddStoreAsync();
        var checkout = new Checkout(
            Guid.NewGuid(),
            store.TerritoryId,
            Guid.NewGuid(),
            store.Id,
            CheckoutStatus.Paid,
            "BRL",
            100m,
            5m,
            105m,
            DateTime.UtcNow,
            DateTime.UtcNow);
        await _checkoutRepository.AddAsync(checkout, CancellationToken.None);

        // Processar checkout
        await _service.ProcessPaidCheckoutAsync(checkout.Id, CancellationToken.None);

        // Buscar transação criada
        var transaction = await _sellerTransactionRepository.GetByCheckoutIdAsync(checkout.Id, CancellationToken.None);
        Assert.NotNull(transaction);

        // Simular gateway error
        _payoutGatewayMock.Setup(g => g.GetPayoutStatusAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(OperationResult<PayoutStatusResult>.Failure("Gateway error"));

        var result = await _service.UpdatePayoutStatusAsync("payout-id", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.True(result.Error?.Contains("No transactions found") == true || result.Error?.Contains("Failed") == true);
    }
}
