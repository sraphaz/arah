using Araponga.Domain.Financial;

namespace Araponga.Application.Interfaces;

public interface IPlatformExpenseTransactionRepository
{
    Task<PlatformExpenseTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<PlatformExpenseTransaction>> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken);
    Task<List<PlatformExpenseTransaction>> GetBySellerTransactionIdAsync(Guid sellerTransactionId, CancellationToken cancellationToken);
    Task AddAsync(PlatformExpenseTransaction transaction, CancellationToken cancellationToken);
}
