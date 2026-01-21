using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryStoreItemRepository : IStoreItemRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryStoreItemRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<StoreItem?> GetByIdAsync(Guid itemId, CancellationToken cancellationToken)
    {
        var item = _dataStore.StoreItems.FirstOrDefault(l => l.Id == itemId);
        return Task.FromResult(item);
    }

    public Task<IReadOnlyList<StoreItem>> ListByIdsAsync(IReadOnlyCollection<Guid> itemIds, CancellationToken cancellationToken)
    {
        if (itemIds.Count == 0)
        {
            return Task.FromResult<IReadOnlyList<StoreItem>>(Array.Empty<StoreItem>());
        }

        var items = _dataStore.StoreItems.Where(l => itemIds.Contains(l.Id)).ToList();
        return Task.FromResult<IReadOnlyList<StoreItem>>(items);
    }

    public Task<IReadOnlyList<StoreItem>> ListByStoreAsync(Guid storeId, CancellationToken cancellationToken)
    {
        var items = _dataStore.StoreItems.Where(l => l.StoreId == storeId).ToList();
        return Task.FromResult<IReadOnlyList<StoreItem>>(items);
    }

    public Task<IReadOnlyList<StoreItem>> SearchAsync(
        Guid territoryId,
        ItemType? type,
        string? query,
        string? category,
        string? tags,
        ItemStatus? status,
        CancellationToken cancellationToken)
    {
        IEnumerable<StoreItem> items = _dataStore.StoreItems.Where(l => l.TerritoryId == territoryId);

        if (type is not null)
        {
            items = items.Where(l => l.Type == type);
        }

        if (status is not null)
        {
            items = items.Where(l => l.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            items = items.Where(l => l.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(tags))
        {
            items = items.Where(l => l.Tags != null && l.Tags.Contains(tags, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            items = items.Where(l =>
                l.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrWhiteSpace(l.Description) && l.Description.Contains(query, StringComparison.OrdinalIgnoreCase)));
        }

        return Task.FromResult<IReadOnlyList<StoreItem>>(items.OrderByDescending(l => l.CreatedAtUtc).ToList());
    }

    public Task AddAsync(StoreItem item, CancellationToken cancellationToken)
    {
        _dataStore.StoreItems.Add(item);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(StoreItem item, CancellationToken cancellationToken)
    {
        var index = _dataStore.StoreItems.FindIndex(l => l.Id == item.Id);
        if (index >= 0)
        {
            _dataStore.StoreItems[index] = item;
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<StoreItem>> SearchPagedAsync(
        Guid territoryId,
        ItemType? type,
        string? query,
        string? category,
        string? tags,
        ItemStatus? status,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        IEnumerable<StoreItem> items = _dataStore.StoreItems.Where(l => l.TerritoryId == territoryId);

        if (type is not null)
        {
            items = items.Where(l => l.Type == type);
        }

        if (status is not null)
        {
            items = items.Where(l => l.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            items = items.Where(l => l.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(tags))
        {
            items = items.Where(l => l.Tags != null && l.Tags.Contains(tags, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            items = items.Where(l =>
                l.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrWhiteSpace(l.Description) && l.Description.Contains(query, StringComparison.OrdinalIgnoreCase)));
        }

        var result = items
            .OrderByDescending(l => l.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToList();

        return Task.FromResult<IReadOnlyList<StoreItem>>(result);
    }

    public Task<int> CountSearchAsync(
        Guid territoryId,
        ItemType? type,
        string? query,
        string? category,
        string? tags,
        ItemStatus? status,
        CancellationToken cancellationToken)
    {
        IEnumerable<StoreItem> items = _dataStore.StoreItems.Where(l => l.TerritoryId == territoryId);

        if (type is not null)
        {
            items = items.Where(l => l.Type == type);
        }

        if (status is not null)
        {
            items = items.Where(l => l.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            items = items.Where(l => l.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(tags))
        {
            items = items.Where(l => l.Tags != null && l.Tags.Contains(tags, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            items = items.Where(l =>
                l.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                (!string.IsNullOrWhiteSpace(l.Description) && l.Description.Contains(query, StringComparison.OrdinalIgnoreCase)));
        }

        const int maxInt32 = int.MaxValue;
        var count = items.Count();
        return Task.FromResult(count > maxInt32 ? maxInt32 : count);
    }
}
