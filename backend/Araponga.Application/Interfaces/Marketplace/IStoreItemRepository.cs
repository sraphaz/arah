using Araponga.Domain.Marketplace;

namespace Araponga.Application.Interfaces;

public interface IStoreItemRepository
{
    Task<StoreItem?> GetByIdAsync(Guid itemId, CancellationToken cancellationToken);
    Task<IReadOnlyList<StoreItem>> ListByIdsAsync(IReadOnlyCollection<Guid> itemIds, CancellationToken cancellationToken);
    Task<IReadOnlyList<StoreItem>> ListByStoreAsync(Guid storeId, CancellationToken cancellationToken);
    Task<IReadOnlyList<StoreItem>> SearchAsync(
        Guid territoryId,
        ItemType? type,
        string? query,
        string? category,
        string? tags,
        ItemStatus? status,
        CancellationToken cancellationToken);
    Task AddAsync(StoreItem item, CancellationToken cancellationToken);
    Task UpdateAsync(StoreItem item, CancellationToken cancellationToken);
    
    /// <summary>
    /// Searches items with pagination.
    /// </summary>
    Task<IReadOnlyList<StoreItem>> SearchPagedAsync(
        Guid territoryId,
        ItemType? type,
        string? query,
        string? category,
        string? tags,
        ItemStatus? status,
        int skip,
        int take,
        CancellationToken cancellationToken);
    
    /// <summary>
    /// Counts items matching search criteria.
    /// </summary>
    Task<int> CountSearchAsync(
        Guid territoryId,
        ItemType? type,
        string? query,
        string? category,
        string? tags,
        ItemStatus? status,
        CancellationToken cancellationToken);
}
