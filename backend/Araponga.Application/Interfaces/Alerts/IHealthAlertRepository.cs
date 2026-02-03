using Araponga.Domain.Health;

namespace Araponga.Application.Interfaces;

public interface IHealthAlertRepository
{
    Task<IReadOnlyList<HealthAlert>> ListByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken);
    Task<HealthAlert?> GetByIdAsync(Guid alertId, CancellationToken cancellationToken);
    Task AddAsync(HealthAlert alert, CancellationToken cancellationToken);
    Task UpdateStatusAsync(Guid alertId, HealthAlertStatus status, CancellationToken cancellationToken);
    
    /// <summary>
    /// Lists health alerts by territory with pagination.
    /// </summary>
    Task<IReadOnlyList<HealthAlert>> ListByTerritoryPagedAsync(
        Guid territoryId,
        int skip,
        int take,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Counts health alerts by territory.
    /// </summary>
    Task<int> CountByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken);
}
