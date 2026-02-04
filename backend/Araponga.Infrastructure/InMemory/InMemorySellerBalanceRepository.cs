using Araponga.Application.Interfaces;
using Araponga.Modules.Marketplace.Application.Interfaces;
using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemorySellerBalanceRepository : ISellerBalanceRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemorySellerBalanceRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<SellerBalance?> GetByTerritoryAndSellerAsync(Guid territoryId, Guid sellerUserId, CancellationToken cancellationToken)
    {
        var balance = _dataStore.SellerBalances
            .FirstOrDefault(b => b.TerritoryId == territoryId && b.SellerUserId == sellerUserId);
        return Task.FromResult(balance);
    }

    public Task<List<SellerBalance>> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var balances = _dataStore.SellerBalances
            .Where(b => b.TerritoryId == territoryId)
            .OrderBy(b => b.SellerUserId)
            .ToList();
        return Task.FromResult(balances);
    }

    public Task<List<SellerBalance>> GetBySellerUserIdAsync(Guid sellerUserId, CancellationToken cancellationToken)
    {
        var balances = _dataStore.SellerBalances
            .Where(b => b.SellerUserId == sellerUserId)
            .OrderBy(b => b.TerritoryId)
            .ToList();
        return Task.FromResult(balances);
    }

    public Task AddAsync(SellerBalance balance, CancellationToken cancellationToken)
    {
        _dataStore.SellerBalances.Add(balance);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(SellerBalance balance, CancellationToken cancellationToken)
    {
        var index = _dataStore.SellerBalances.FindIndex(b => b.Id == balance.Id);
        if (index >= 0)
        {
            _dataStore.SellerBalances[index] = balance;
        }
        return Task.CompletedTask;
    }
}
