using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryInquiryRepository : IInquiryRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryInquiryRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task AddAsync(ItemInquiry inquiry, CancellationToken cancellationToken)
    {
        _dataStore.ItemInquiries.Add(inquiry);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<ItemInquiry>> ListByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var inquiries = _dataStore.ItemInquiries
            .Where(i => i.FromUserId == userId)
            .OrderByDescending(i => i.CreatedAtUtc)
            .ToList();

        return Task.FromResult<IReadOnlyList<ItemInquiry>>(inquiries);
    }

    public Task<IReadOnlyList<ItemInquiry>> ListByStoreIdsAsync(IReadOnlyCollection<Guid> storeIds, CancellationToken cancellationToken)
    {
        if (storeIds.Count == 0)
        {
            return Task.FromResult<IReadOnlyList<ItemInquiry>>(Array.Empty<ItemInquiry>());
        }

        var inquiries = _dataStore.ItemInquiries
            .Where(i => storeIds.Contains(i.StoreId))
            .OrderByDescending(i => i.CreatedAtUtc)
            .ToList();

        return Task.FromResult<IReadOnlyList<ItemInquiry>>(inquiries);
    }

    public Task<IReadOnlyList<ItemInquiry>> ListByUserPagedAsync(
        Guid userId,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var inquiries = _dataStore.ItemInquiries
            .Where(i => i.FromUserId == userId)
            .OrderByDescending(i => i.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToList();

        return Task.FromResult<IReadOnlyList<ItemInquiry>>(inquiries);
    }

    public Task<IReadOnlyList<ItemInquiry>> ListByStoreIdsPagedAsync(
        IReadOnlyCollection<Guid> storeIds,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        if (storeIds.Count == 0)
        {
            return Task.FromResult<IReadOnlyList<ItemInquiry>>(Array.Empty<ItemInquiry>());
        }

        var inquiries = _dataStore.ItemInquiries
            .Where(i => storeIds.Contains(i.StoreId))
            .OrderByDescending(i => i.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToList();

        return Task.FromResult<IReadOnlyList<ItemInquiry>>(inquiries);
    }

    public Task<int> CountByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var count = _dataStore.ItemInquiries.Count(i => i.FromUserId == userId);
        return Task.FromResult(count);
    }

    public Task<int> CountByStoreIdsAsync(IReadOnlyCollection<Guid> storeIds, CancellationToken cancellationToken)
    {
        if (storeIds.Count == 0)
        {
            return Task.FromResult(0);
        }

        var count = _dataStore.ItemInquiries.Count(i => storeIds.Contains(i.StoreId));
        return Task.FromResult(count);
    }
}
