using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryListingRepository : IListingRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryListingRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<StoreListing?> GetByIdAsync(Guid listingId, CancellationToken cancellationToken)
    {
        var listing = _dataStore.StoreListings.FirstOrDefault(l => l.Id == listingId);
        return Task.FromResult(listing);
    }

    public Task<IReadOnlyList<StoreListing>> ListByIdsAsync(IReadOnlyCollection<Guid> listingIds, CancellationToken cancellationToken)
    {
        if (listingIds.Count == 0)
        {
            return Task.FromResult<IReadOnlyList<StoreListing>>(Array.Empty<StoreListing>());
        }

        var listings = _dataStore.StoreListings.Where(l => listingIds.Contains(l.Id)).ToList();
        return Task.FromResult<IReadOnlyList<StoreListing>>(listings);
    }

    public Task<IReadOnlyList<StoreListing>> ListByStoreAsync(Guid storeId, CancellationToken cancellationToken)
    {
        var listings = _dataStore.StoreListings.Where(l => l.StoreId == storeId).ToList();
        return Task.FromResult<IReadOnlyList<StoreListing>>(listings);
    }

    public Task<IReadOnlyList<StoreListing>> SearchAsync(
        Guid territoryId,
        ListingType? type,
        string? query,
        string? category,
        string? tags,
        ListingStatus? status,
        CancellationToken cancellationToken)
    {
        IEnumerable<StoreListing> listings = _dataStore.StoreListings.Where(l => l.TerritoryId == territoryId);

        if (type is not null)
        {
            listings = listings.Where(l => l.Type == type);
        }

        if (status is not null)
        {
            listings = listings.Where(l => l.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            listings = listings.Where(l => l.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(tags))
        {
            listings = listings.Where(l => l.Tags != null && l.Tags.Contains(tags, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            listings = listings.Where(l =>
                l.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrWhiteSpace(l.Description) && l.Description.Contains(query, StringComparison.OrdinalIgnoreCase)));
        }

        return Task.FromResult<IReadOnlyList<StoreListing>>(listings.OrderByDescending(l => l.CreatedAtUtc).ToList());
    }

    public Task AddAsync(StoreListing listing, CancellationToken cancellationToken)
    {
        _dataStore.StoreListings.Add(listing);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(StoreListing listing, CancellationToken cancellationToken)
    {
        var index = _dataStore.StoreListings.FindIndex(l => l.Id == listing.Id);
        if (index >= 0)
        {
            _dataStore.StoreListings[index] = listing;
        }

        return Task.CompletedTask;
    }
}
