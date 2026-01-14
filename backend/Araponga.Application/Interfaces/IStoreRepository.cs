using Araponga.Domain.Marketplace;

namespace Araponga.Application.Interfaces;

public interface IStoreRepository
{
    Task<Store?> GetByIdAsync(Guid storeId, CancellationToken cancellationToken);
    Task<Store?> GetByOwnerAsync(Guid territoryId, Guid ownerUserId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Store>> ListByOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Store>> ListByIdsAsync(IReadOnlyCollection<Guid> storeIds, CancellationToken cancellationToken);
    Task AddAsync(Store store, CancellationToken cancellationToken);
    Task UpdateAsync(Store store, CancellationToken cancellationToken);
}
