using Araponga.Modules.Marketplace.Application.Interfaces;
using Araponga.Modules.Marketplace.Domain;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresInquiryRepository : IInquiryRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresInquiryRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(ItemInquiry inquiry, CancellationToken cancellationToken)
    {
        _dbContext.ItemInquiries.Add(inquiry.ToRecord());
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<ItemInquiry>> ListByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.ItemInquiries
            .AsNoTracking()
            .Where(i => i.FromUserId == userId)
            .OrderByDescending(i => i.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<ItemInquiry>> ListByStoreIdsAsync(IReadOnlyCollection<Guid> storeIds, CancellationToken cancellationToken)
    {
        if (storeIds.Count == 0)
        {
            return Array.Empty<ItemInquiry>();
        }

        var records = await _dbContext.ItemInquiries
            .AsNoTracking()
            .Where(i => storeIds.Contains(i.StoreId))
            .OrderByDescending(i => i.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<ItemInquiry>> ListByUserPagedAsync(
        Guid userId,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var records = await _dbContext.ItemInquiries
            .AsNoTracking()
            .Where(i => i.FromUserId == userId)
            .OrderByDescending(i => i.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<ItemInquiry>> ListByStoreIdsPagedAsync(
        IReadOnlyCollection<Guid> storeIds,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        if (storeIds.Count == 0)
        {
            return Array.Empty<ItemInquiry>();
        }

        var records = await _dbContext.ItemInquiries
            .AsNoTracking()
            .Where(i => storeIds.Contains(i.StoreId))
            .OrderByDescending(i => i.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<int> CountByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        const int maxInt32 = int.MaxValue;
        var count = await _dbContext.ItemInquiries
            .Where(i => i.FromUserId == userId)
            .CountAsync(cancellationToken);
        return count > maxInt32 ? maxInt32 : (int)count;
    }

    public async Task<int> CountByStoreIdsAsync(IReadOnlyCollection<Guid> storeIds, CancellationToken cancellationToken)
    {
        if (storeIds.Count == 0)
        {
            return 0;
        }

        const int maxInt32 = int.MaxValue;
        var count = await _dbContext.ItemInquiries
            .Where(i => storeIds.Contains(i.StoreId))
            .CountAsync(cancellationToken);
        return count > maxInt32 ? maxInt32 : (int)count;
    }
}
