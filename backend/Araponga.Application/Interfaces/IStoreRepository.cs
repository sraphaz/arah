using Araponga.Domain.Marketplace;

namespace Araponga.Application.Interfaces;

public interface IStoreRepository
{
    Task<TerritoryStore?> GetByIdAsync(Guid storeId, CancellationToken cancellationToken);
    Task<TerritoryStore?> GetByOwnerAsync(Guid territoryId, Guid ownerUserId, CancellationToken cancellationToken);
    Task<IReadOnlyList<TerritoryStore>> ListByOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken);
    Task<IReadOnlyList<TerritoryStore>> ListByIdsAsync(IReadOnlyCollection<Guid> storeIds, CancellationToken cancellationToken);
    Task AddAsync(TerritoryStore store, CancellationToken cancellationToken);
    Task UpdateAsync(TerritoryStore store, CancellationToken cancellationToken);
}
