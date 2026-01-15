using Araponga.Domain.Financial;

namespace Araponga.Application.Interfaces;

public interface IFinancialTransactionRepository
{
    Task<FinancialTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<FinancialTransaction>> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken);
    Task<List<FinancialTransaction>> GetByRelatedEntityIdAsync(Guid relatedEntityId, string? relatedEntityType, CancellationToken cancellationToken);
    Task<List<FinancialTransaction>> GetByTypeAsync(Guid territoryId, TransactionType type, CancellationToken cancellationToken);
    Task<List<FinancialTransaction>> GetByStatusAsync(Guid territoryId, TransactionStatus status, CancellationToken cancellationToken);
    Task AddAsync(FinancialTransaction transaction, CancellationToken cancellationToken);
    Task UpdateAsync(FinancialTransaction transaction, CancellationToken cancellationToken);
}
