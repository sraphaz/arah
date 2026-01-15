using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Financial;
using Araponga.Domain.Marketplace;

namespace Araponga.Application.Services;

/// <summary>
/// Serviço para gerenciar payouts de vendedores e rastreabilidade financeira.
/// </summary>
public sealed class SellerPayoutService
{
    private readonly ICheckoutRepository _checkoutRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly ISellerTransactionRepository _sellerTransactionRepository;
    private readonly ISellerBalanceRepository _sellerBalanceRepository;
    private readonly IFinancialTransactionRepository _financialTransactionRepository;
    private readonly ITransactionStatusHistoryRepository _transactionStatusHistoryRepository;
    private readonly IPlatformRevenueTransactionRepository _platformRevenueTransactionRepository;
    private readonly IPlatformFinancialBalanceRepository _platformFinancialBalanceRepository;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;

    public SellerPayoutService(
        ICheckoutRepository checkoutRepository,
        IStoreRepository storeRepository,
        ISellerTransactionRepository sellerTransactionRepository,
        ISellerBalanceRepository sellerBalanceRepository,
        IFinancialTransactionRepository financialTransactionRepository,
        ITransactionStatusHistoryRepository transactionStatusHistoryRepository,
        IPlatformRevenueTransactionRepository platformRevenueTransactionRepository,
        IPlatformFinancialBalanceRepository platformFinancialBalanceRepository,
        IAuditLogger auditLogger,
        IUnitOfWork unitOfWork)
    {
        _checkoutRepository = checkoutRepository;
        _storeRepository = storeRepository;
        _sellerTransactionRepository = sellerTransactionRepository;
        _sellerBalanceRepository = sellerBalanceRepository;
        _financialTransactionRepository = financialTransactionRepository;
        _transactionStatusHistoryRepository = transactionStatusHistoryRepository;
        _platformRevenueTransactionRepository = platformRevenueTransactionRepository;
        _platformFinancialBalanceRepository = platformFinancialBalanceRepository;
        _auditLogger = auditLogger;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Processa um checkout pago, criando SellerTransaction e atualizando saldos.
    /// Deve ser chamado quando um checkout é marcado como Paid.
    /// </summary>
    public async Task<OperationResult> ProcessPaidCheckoutAsync(
        Guid checkoutId,
        CancellationToken cancellationToken)
    {
        // Buscar checkout
        var checkout = await _checkoutRepository.GetByIdAsync(checkoutId, cancellationToken);
        if (checkout is null)
        {
            return OperationResult.Failure("Checkout not found.");
        }

        if (checkout.Status != CheckoutStatus.Paid)
        {
            return OperationResult.Failure($"Checkout is not paid. Current status: {checkout.Status}");
        }

        // Verificar se já existe SellerTransaction para este checkout
        var existingTransaction = await _sellerTransactionRepository.GetByCheckoutIdAsync(checkoutId, cancellationToken);
        if (existingTransaction is not null)
        {
            return OperationResult.Success(); // Já processado
        }

        // Buscar store para obter sellerUserId
        var store = await _storeRepository.GetByIdAsync(checkout.StoreId, cancellationToken);
        if (store is null)
        {
            return OperationResult.Failure("Store not found.");
        }

        if (checkout.TotalAmount is null || checkout.PlatformFeeAmount is null || checkout.ItemsSubtotalAmount is null)
        {
            return OperationResult.Failure("Checkout amounts are not set.");
        }

        // Converter valores para centavos
        var grossAmountInCents = (long)(checkout.ItemsSubtotalAmount.Value * 100);
        var platformFeeInCents = (long)(checkout.PlatformFeeAmount.Value * 100);
        var netAmountInCents = grossAmountInCents - platformFeeInCents;

        // Criar SellerTransaction
        var sellerTransaction = new SellerTransaction(
            Guid.NewGuid(),
            checkout.TerritoryId,
            checkout.StoreId,
            checkout.Id,
            store.OwnerUserId,
            grossAmountInCents,
            platformFeeInCents,
            checkout.Currency);

        await _sellerTransactionRepository.AddAsync(sellerTransaction, cancellationToken);

        // Atualizar ou criar SellerBalance
        var sellerBalance = await _sellerBalanceRepository.GetByTerritoryAndSellerAsync(
            checkout.TerritoryId,
            store.OwnerUserId,
            cancellationToken);

        if (sellerBalance is null)
        {
            sellerBalance = new SellerBalance(
                Guid.NewGuid(),
                checkout.TerritoryId,
                store.OwnerUserId,
                checkout.Currency);
            await _sellerBalanceRepository.AddAsync(sellerBalance, cancellationToken);
        }

        sellerBalance.AddPendingAmount(netAmountInCents);
        await _sellerBalanceRepository.UpdateAsync(sellerBalance, cancellationToken);

        // Criar FinancialTransaction para rastreabilidade
        var financialTransaction = new FinancialTransaction(
            Guid.NewGuid(),
            checkout.TerritoryId,
            TransactionType.Seller,
            netAmountInCents,
            checkout.Currency,
            $"Seller transaction for checkout {checkout.Id}",
            checkout.Id,
            "Checkout",
            new Dictionary<string, string>
            {
                { "checkoutId", checkout.Id.ToString() },
                { "storeId", checkout.StoreId.ToString() },
                { "sellerUserId", store.OwnerUserId.ToString() }
            });

        await _financialTransactionRepository.AddAsync(financialTransaction, cancellationToken);
        sellerTransaction.SetFinancialTransactionId(financialTransaction.Id);

        // Criar TransactionStatusHistory
        var statusHistory = new TransactionStatusHistory(
            Guid.NewGuid(),
            financialTransaction.Id,
            TransactionStatus.Pending,
            TransactionStatus.Pending,
            null,
            "Initial status when seller transaction created");
        await _transactionStatusHistoryRepository.AddAsync(statusHistory, cancellationToken);

        // Criar PlatformRevenueTransaction (fee da plataforma)
        var platformRevenueTransaction = new PlatformRevenueTransaction(
            Guid.NewGuid(),
            checkout.TerritoryId,
            checkout.Id,
            platformFeeInCents,
            checkout.Currency);

        await _platformRevenueTransactionRepository.AddAsync(platformRevenueTransaction, cancellationToken);

        // Criar FinancialTransaction para fee da plataforma
        var platformFeeTransaction = new FinancialTransaction(
            Guid.NewGuid(),
            checkout.TerritoryId,
            TransactionType.PlatformFee,
            platformFeeInCents,
            checkout.Currency,
            $"Platform fee for checkout {checkout.Id}",
            checkout.Id,
            "Checkout",
            new Dictionary<string, string>
            {
                { "checkoutId", checkout.Id.ToString() },
                { "storeId", checkout.StoreId.ToString() }
            });

        await _financialTransactionRepository.AddAsync(platformFeeTransaction, cancellationToken);
        platformRevenueTransaction.SetFinancialTransactionId(platformFeeTransaction.Id);

        // Relacionar transações
        financialTransaction.AddRelatedTransaction(platformFeeTransaction.Id);
        platformFeeTransaction.AddRelatedTransaction(financialTransaction.Id);
        await _financialTransactionRepository.UpdateAsync(financialTransaction, cancellationToken);
        await _financialTransactionRepository.UpdateAsync(platformFeeTransaction, cancellationToken);

        // Atualizar ou criar PlatformFinancialBalance
        var platformBalance = await _platformFinancialBalanceRepository.GetByTerritoryIdAsync(
            checkout.TerritoryId,
            cancellationToken);

        if (platformBalance is null)
        {
            platformBalance = new PlatformFinancialBalance(
                Guid.NewGuid(),
                checkout.TerritoryId,
                checkout.Currency);
            await _platformFinancialBalanceRepository.AddAsync(platformBalance, cancellationToken);
        }

        platformBalance.AddRevenue(platformFeeInCents);
        await _platformFinancialBalanceRepository.UpdateAsync(platformBalance, cancellationToken);

        // Auditoria
        await _auditLogger.LogAsync(
            new Models.AuditEntry(
                "seller.transaction.created",
                store.OwnerUserId,
                checkout.TerritoryId,
                sellerTransaction.Id,
                DateTime.UtcNow),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult.Success();
    }
}
