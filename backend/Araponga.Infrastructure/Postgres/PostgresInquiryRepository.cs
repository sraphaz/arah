using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresInquiryRepository : IInquiryRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresInquiryRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(ListingInquiry inquiry, CancellationToken cancellationToken)
    {
        _dbContext.ListingInquiries.Add(inquiry.ToRecord());
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<ListingInquiry>> ListByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.ListingInquiries
            .AsNoTracking()
            .Where(i => i.FromUserId == userId)
            .OrderByDescending(i => i.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<ListingInquiry>> ListByStoreIdsAsync(IReadOnlyCollection<Guid> storeIds, CancellationToken cancellationToken)
    {
        if (storeIds.Count == 0)
        {
            return Array.Empty<ListingInquiry>();
        }

        var records = await _dbContext.ListingInquiries
            .AsNoTracking()
            .Where(i => storeIds.Contains(i.StoreId))
            .OrderByDescending(i => i.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }
}
