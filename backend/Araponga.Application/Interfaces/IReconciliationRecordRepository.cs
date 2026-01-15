using Araponga.Domain.Financial;

namespace Araponga.Application.Interfaces;

public interface IReconciliationRecordRepository
{
    Task<ReconciliationRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<ReconciliationRecord>> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken);
    Task<List<ReconciliationRecord>> GetByStatusAsync(Guid territoryId, ReconciliationStatus status, CancellationToken cancellationToken);
    Task AddAsync(ReconciliationRecord record, CancellationToken cancellationToken);
    Task UpdateAsync(ReconciliationRecord record, CancellationToken cancellationToken);
}
