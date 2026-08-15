using System.Collections.Concurrent;
using Arah.Application.Interfaces;
using Arah.Domain.Financial;

namespace Arah.Infrastructure.InMemory;

public sealed class InMemoryWalletRepository : IWalletRepository
{
    private readonly ConcurrentDictionary<Guid, Wallet> _wallets = new();

    public Task<Wallet?> GetByIdAsync(Guid walletId, CancellationToken cancellationToken)
    {
        _wallets.TryGetValue(walletId, out var wallet);
        return Task.FromResult(wallet);
    }

    public Task AddAsync(Wallet wallet, CancellationToken cancellationToken)
    {
        _wallets.TryAdd(wallet.Id, wallet);
        return Task.CompletedTask;
    }

    public Task UpsertAsync(Wallet wallet, CancellationToken cancellationToken)
    {
        _wallets[wallet.Id] = wallet;
        return Task.CompletedTask;
    }
}
