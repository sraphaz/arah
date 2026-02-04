using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Modules.Marketplace.Application.Interfaces;

public interface ITerritoryPayoutConfigRepository
{
    Task<TerritoryPayoutConfig?> GetActiveAsync(Guid territoryId, CancellationToken cancellationToken);
    Task<TerritoryPayoutConfig?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<TerritoryPayoutConfig>> ListByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken);
    Task AddAsync(TerritoryPayoutConfig config, CancellationToken cancellationToken);
    Task UpdateAsync(TerritoryPayoutConfig config, CancellationToken cancellationToken);
}
