using Arah.Domain.Financial;

namespace Arah.Application.Interfaces;

public interface IWalletRepository
{
    Task<Wallet?> GetByIdAsync(Guid walletId, CancellationToken cancellationToken);
    Task AddAsync(Wallet wallet, CancellationToken cancellationToken);
}
