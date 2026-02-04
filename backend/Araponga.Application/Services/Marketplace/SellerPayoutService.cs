using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Financial;
using Araponga.Modules.Marketplace.Application.Interfaces;
using Araponga.Modules.Marketplace.Domain;

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
    private readonly IPlatformExpenseTransactionRepository _platformExpenseTransactionRepository;
    private readonly ITerritoryPayoutConfigRepository _payoutConfigRepository;
    private readonly IPayoutGateway _payoutGateway;
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
        IPlatformExpenseTransactionRepository platformExpenseTransactionRepository,
        ITerritoryPayoutConfigRepository payoutConfigRepository,
        IPayoutGateway payoutGateway,
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
        _platformExpenseTransactionRepository = platformExpenseTransactionRepository;
        _payoutConfigRepository = payoutConfigRepository;
        _payoutGateway = payoutGateway;
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

        // Verificar configuração de payout para aplicar retenção e valor mínimo
        await TryMarkTransactionAsReadyForPayoutAsync(
            sellerTransaction,
            sellerBalance,
            checkout.TerritoryId,
            cancellationToken);

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

    /// <summary>
    /// Tenta marcar transação como ReadyForPayout baseado na configuração (retenção e valor mínimo).
    /// </summary>
    private async Task TryMarkTransactionAsReadyForPayoutAsync(
        SellerTransaction transaction,
        SellerBalance balance,
        Guid territoryId,
        CancellationToken cancellationToken)
    {
        var config = await _payoutConfigRepository.GetActiveAsync(territoryId, cancellationToken);
        if (config is null || !config.IsActive)
        {
            // Sem configuração, não marca como ready
            return;
        }

        // Verificar período de retenção
        var retentionPeriodEnd = transaction.CreatedAtUtc.AddDays(config.RetentionPeriodDays);
        if (DateTime.UtcNow < retentionPeriodEnd)
        {
            // Ainda está no período de retenção
            return;
        }

        // Verificar valor mínimo (verificar saldo total ready + pending)
        var totalReadyForPayout = balance.ReadyForPayoutAmountInCents + transaction.NetAmountInCents;
        if (totalReadyForPayout < config.MinimumPayoutAmountInCents)
        {
            // Não atingiu valor mínimo ainda
            return;
        }

        // Marcar transação como ReadyForPayout
        transaction.MarkAsReadyForPayout();
        balance.MoveToReadyForPayout(transaction.NetAmountInCents);

        await _sellerTransactionRepository.UpdateAsync(transaction, cancellationToken);
        await _sellerBalanceRepository.UpdateAsync(balance, cancellationToken);
    }

    /// <summary>
    /// Processa payouts pendentes para um território (chamado por background job ou manualmente).
    /// </summary>
    public async Task<OperationResult<int>> ProcessPendingPayoutsAsync(
        Guid territoryId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var config = await _payoutConfigRepository.GetActiveAsync(territoryId, cancellationToken);
        if (config is null || !config.IsActive || !config.AutoPayoutEnabled)
        {
            return OperationResult<int>.Success(0); // Nenhum payout processado
        }

        // Buscar transações prontas para payout
        var readyTransactions = await _sellerTransactionRepository.GetReadyForPayoutAsync(
            territoryId,
            cancellationToken);

        if (readyTransactions.Count == 0)
        {
            return OperationResult<int>.Success(0);
        }

        // Agrupar por sellerUserId e moeda
        var groupedTransactions = readyTransactions
            .GroupBy(t => new { t.SellerUserId, t.Currency })
            .ToList();

        int processedCount = 0;

        foreach (var group in groupedTransactions)
        {
            var sellerUserId = group.Key.SellerUserId;
            var currency = group.Key.Currency;
            var transactions = group.ToList();

            // Calcular valor total do payout
            var totalAmountInCents = transactions.Sum(t => t.NetAmountInCents);

            // Verificar valor mínimo (não deve ser necessário aqui, mas verificamos de novo)
            if (totalAmountInCents < config.MinimumPayoutAmountInCents)
            {
                continue; // Não atingiu valor mínimo
            }

            // Verificar valor máximo (dividir em múltiplos payouts se necessário)
            if (config.MaximumPayoutAmountInCents.HasValue &&
                totalAmountInCents > config.MaximumPayoutAmountInCents.Value)
            {
                // Dividir em múltiplos payouts
                var remainingAmount = totalAmountInCents;
                var transactionIndex = 0;

                while (remainingAmount > 0 && transactionIndex < transactions.Count)
                {
                    var payoutAmount = Math.Min(remainingAmount, config.MaximumPayoutAmountInCents.Value);
                    var payoutTransactions = new List<SellerTransaction>();

                    // Selecionar transações para este payout
                    long accumulated = 0;
                    for (int i = transactionIndex; i < transactions.Count && accumulated < payoutAmount; i++)
                    {
                        payoutTransactions.Add(transactions[i]);
                        accumulated += transactions[i].NetAmountInCents;
                        transactionIndex++;
                    }

                    var payoutResult = await CreatePayoutAsync(
                        territoryId,
                        sellerUserId,
                        currency,
                        payoutAmount,
                        payoutTransactions,
                        userId,
                        cancellationToken);

                    if (payoutResult.IsSuccess)
                    {
                        processedCount++;
                    }

                    remainingAmount -= payoutAmount;
                }
            }
            else
            {
                // Payout único
                var payoutResult = await CreatePayoutAsync(
                    territoryId,
                    sellerUserId,
                    currency,
                    totalAmountInCents,
                    transactions,
                    userId,
                    cancellationToken);

                if (payoutResult.IsSuccess)
                {
                    processedCount++;
                }
            }
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult<int>.Success(processedCount);
    }

    /// <summary>
    /// Cria um payout através do gateway e atualiza transações e saldos.
    /// </summary>
    private async Task<OperationResult> CreatePayoutAsync(
        Guid territoryId,
        Guid sellerUserId,
        string currency,
        long amountInCents,
        List<SellerTransaction> transactions,
        Guid userId,
        CancellationToken cancellationToken)
    {
        // Buscar sellerAccountId (por enquanto, usar sellerUserId como accountId)
        // TODO: Adicionar campo SellerAccountId na Store ou SellerProfile
        var sellerAccountId = sellerUserId.ToString();

        // Criar payout no gateway
        var payoutResult = await _payoutGateway.CreatePayoutAsync(
            amountInCents,
            currency,
            sellerAccountId,
            $"Payout for {transactions.Count} transaction(s)",
            new Dictionary<string, string>
            {
                { "territoryId", territoryId.ToString() },
                { "sellerUserId", sellerUserId.ToString() },
                { "transactionCount", transactions.Count.ToString() }
            },
            cancellationToken);

        if (!payoutResult.IsSuccess)
        {
            return OperationResult.Failure($"Failed to create payout: {payoutResult.Error}");
        }

        var payout = payoutResult.Value!;
        var payoutId = payout.PayoutId;

        // Atualizar todas as transações
        foreach (var transaction in transactions)
        {
            transaction.StartPayout(payoutId);

            // Atualizar status da FinancialTransaction relacionada
            if (transaction.FinancialTransactionId.HasValue)
            {
                var financialTransaction = await _financialTransactionRepository.GetByIdAsync(
                    transaction.FinancialTransactionId.Value,
                    cancellationToken);

                if (financialTransaction is not null)
                {
                    financialTransaction.UpdateStatus(TransactionStatus.Processing);
                    await _financialTransactionRepository.UpdateAsync(financialTransaction, cancellationToken);

                    // Registrar mudança de status
                    var statusHistory = new TransactionStatusHistory(
                        Guid.NewGuid(),
                        financialTransaction.Id,
                        TransactionStatus.Pending,
                        TransactionStatus.Processing,
                        userId,
                        $"Payout started: {payoutId}");
                    await _transactionStatusHistoryRepository.AddAsync(statusHistory, cancellationToken);
                }
            }

            await _sellerTransactionRepository.UpdateAsync(transaction, cancellationToken);
        }

        // Atualizar SellerBalance
        var sellerBalance = await _sellerBalanceRepository.GetByTerritoryAndSellerAsync(
            territoryId,
            sellerUserId,
            cancellationToken);

        if (sellerBalance is not null)
        {
            sellerBalance.MarkAsPaid(amountInCents);
            await _sellerBalanceRepository.UpdateAsync(sellerBalance, cancellationToken);
        }

        // Criar FinancialTransaction para despesa (uma única transação financeira para todo o payout)
        var expenseFinancialTransaction = new FinancialTransaction(
            Guid.NewGuid(),
            territoryId,
            TransactionType.Payout,
            amountInCents,
            currency,
            $"Payout {payoutId} for {transactions.Count} transaction(s)",
            transactions.First().Id,
            "SellerTransaction",
            new Dictionary<string, string>
            {
                { "payoutId", payoutId },
                { "sellerUserId", sellerUserId.ToString() },
                { "transactionCount", transactions.Count.ToString() }
            });

        await _financialTransactionRepository.AddAsync(expenseFinancialTransaction, cancellationToken);

        // Criar PlatformExpenseTransaction para cada transação
        foreach (var transaction in transactions)
        {
            var expenseTransaction = new PlatformExpenseTransaction(
                Guid.NewGuid(),
                territoryId,
                transaction.Id,
                transaction.NetAmountInCents,
                currency,
                payoutId);

            // Associar FinancialTransaction imediatamente
            expenseTransaction.SetFinancialTransactionId(expenseFinancialTransaction.Id);
            await _platformExpenseTransactionRepository.AddAsync(expenseTransaction, cancellationToken);
        }

        // Atualizar PlatformFinancialBalance
        var platformBalance = await _platformFinancialBalanceRepository.GetByTerritoryIdAsync(
            territoryId,
            cancellationToken);

        if (platformBalance is null)
        {
            platformBalance = new PlatformFinancialBalance(
                Guid.NewGuid(),
                territoryId,
                currency);
            await _platformFinancialBalanceRepository.AddAsync(platformBalance, cancellationToken);
        }

        platformBalance.AddExpense(amountInCents);
        await _platformFinancialBalanceRepository.UpdateAsync(platformBalance, cancellationToken);

        // Auditoria (usar primeira transação como relatedEntityId já que payoutId é string)
        await _auditLogger.LogAsync(
            new Models.AuditEntry(
                "seller.payout.created",
                userId,
                territoryId,
                transactions.First().Id,
                DateTime.UtcNow),
            cancellationToken);

        return OperationResult.Success();
    }

    /// <summary>
    /// Atualiza o status de um payout baseado no status do gateway.
    /// </summary>
    public async Task<OperationResult> UpdatePayoutStatusAsync(
        string payoutId,
        CancellationToken cancellationToken)
    {
        // Buscar transações com este payoutId
        var transactionsWithPayout = await _sellerTransactionRepository.GetByPayoutIdAsync(
            payoutId,
            cancellationToken);

        if (transactionsWithPayout.Count == 0)
        {
            return OperationResult.Failure("No transactions found for this payout.");
        }

        // Buscar status no gateway
        var statusResult = await _payoutGateway.GetPayoutStatusAsync(payoutId, cancellationToken);
        if (!statusResult.IsSuccess)
        {
            return OperationResult.Failure($"Failed to get payout status: {statusResult.Error}");
        }

        var payoutStatus = statusResult.Value!;

        // Atualizar transações baseado no status
        foreach (var transaction in transactionsWithPayout)
        {
            switch (payoutStatus.Status)
            {
                case PayoutStatus.Completed:
                    transaction.CompletePayout();
                    break;
                case PayoutStatus.Failed:
                    transaction.FailPayout();
                    break;
                case PayoutStatus.Canceled:
                    transaction.Cancel();
                    break;
                // Pending e Processing não mudam nada
            }

            await _sellerTransactionRepository.UpdateAsync(transaction, cancellationToken);
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult.Success();
    }
}
