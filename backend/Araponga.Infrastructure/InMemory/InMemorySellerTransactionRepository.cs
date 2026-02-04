using Araponga.Application.Interfaces;
using Araponga.Modules.Marketplace.Application.Interfaces;
using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemorySellerTransactionRepository : ISellerTransactionRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemorySellerTransactionRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<SellerTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var transaction = _dataStore.SellerTransactions.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(transaction);
    }

    public Task<SellerTransaction?> GetByCheckoutIdAsync(Guid checkoutId, CancellationToken cancellationToken)
    {
        var transaction = _dataStore.SellerTransactions.FirstOrDefault(t => t.CheckoutId == checkoutId);
        return Task.FromResult(transaction);
    }

    public Task<List<SellerTransaction>> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var transactions = _dataStore.SellerTransactions
            .Where(t => t.TerritoryId == territoryId)
            .OrderByDescending(t => t.CreatedAtUtc)
            .ToList();
        return Task.FromResult(transactions);
    }

    public Task<List<SellerTransaction>> GetBySellerUserIdAsync(Guid sellerUserId, CancellationToken cancellationToken)
    {
        var transactions = _dataStore.SellerTransactions
            .Where(t => t.SellerUserId == sellerUserId)
            .OrderByDescending(t => t.CreatedAtUtc)
            .ToList();
        return Task.FromResult(transactions);
    }

    public Task<List<SellerTransaction>> GetByStatusAsync(Guid territoryId, SellerTransactionStatus status, CancellationToken cancellationToken)
    {
        var transactions = _dataStore.SellerTransactions
            .Where(t => t.TerritoryId == territoryId && t.Status == status)
            .OrderByDescending(t => t.CreatedAtUtc)
            .ToList();
        return Task.FromResult(transactions);
    }

    public Task<List<SellerTransaction>> GetReadyForPayoutAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var transactions = _dataStore.SellerTransactions
            .Where(t => t.TerritoryId == territoryId && t.Status == SellerTransactionStatus.ReadyForPayout)
            .OrderBy(t => t.ReadyForPayoutAtUtc)
            .ToList();
        return Task.FromResult(transactions);
    }

    public Task<List<SellerTransaction>> GetByPayoutIdAsync(string payoutId, CancellationToken cancellationToken)
    {
        var transactions = _dataStore.SellerTransactions
            .Where(t => t.PayoutId == payoutId)
            .OrderByDescending(t => t.CreatedAtUtc)
            .ToList();
        return Task.FromResult(transactions);
    }

    public Task AddAsync(SellerTransaction transaction, CancellationToken cancellationToken)
    {
        _dataStore.SellerTransactions.Add(transaction);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(SellerTransaction transaction, CancellationToken cancellationToken)
    {
        var index = _dataStore.SellerTransactions.FindIndex(t => t.Id == transaction.Id);
        if (index >= 0)
        {
            _dataStore.SellerTransactions[index] = transaction;
        }
        return Task.CompletedTask;
    }
}
