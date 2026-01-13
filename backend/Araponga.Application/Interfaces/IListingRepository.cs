using Araponga.Domain.Marketplace;

namespace Araponga.Application.Interfaces;

public interface IListingRepository
{
    Task<StoreListing?> GetByIdAsync(Guid listingId, CancellationToken cancellationToken);
    Task<IReadOnlyList<StoreListing>> ListByIdsAsync(IReadOnlyCollection<Guid> listingIds, CancellationToken cancellationToken);
    Task<IReadOnlyList<StoreListing>> ListByStoreAsync(Guid storeId, CancellationToken cancellationToken);
    Task<IReadOnlyList<StoreListing>> SearchAsync(
        Guid territoryId,
        ListingType? type,
        string? query,
        string? category,
        string? tags,
        ListingStatus? status,
        CancellationToken cancellationToken);
    Task AddAsync(StoreListing listing, CancellationToken cancellationToken);
    Task UpdateAsync(StoreListing listing, CancellationToken cancellationToken);
}
