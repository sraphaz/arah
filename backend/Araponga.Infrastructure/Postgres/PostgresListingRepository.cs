using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresListingRepository : IListingRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresListingRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StoreListing?> GetByIdAsync(Guid listingId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.StoreListings
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == listingId, cancellationToken);

        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<StoreListing>> ListByIdsAsync(IReadOnlyCollection<Guid> listingIds, CancellationToken cancellationToken)
    {
        if (listingIds.Count == 0)
        {
            return Array.Empty<StoreListing>();
        }

        var records = await _dbContext.StoreListings
            .AsNoTracking()
            .Where(l => listingIds.Contains(l.Id))
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<StoreListing>> ListByStoreAsync(Guid storeId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.StoreListings
            .AsNoTracking()
            .Where(l => l.StoreId == storeId)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<StoreListing>> SearchAsync(
        Guid territoryId,
        ListingType? type,
        string? query,
        string? category,
        string? tags,
        ListingStatus? status,
        CancellationToken cancellationToken)
    {
        var listings = _dbContext.StoreListings.AsNoTracking()
            .Where(l => l.TerritoryId == territoryId);

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
            listings = listings.Where(l => l.Category != null && l.Category == category);
        }

        if (!string.IsNullOrWhiteSpace(tags))
        {
            listings = listings.Where(l => l.Tags != null && EF.Functions.ILike(l.Tags, $"%{tags}%"));
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            listings = listings.Where(l =>
                EF.Functions.ILike(l.Title, $"%{query}%") ||
                (l.Description != null && EF.Functions.ILike(l.Description, $"%{query}%")));
        }

        var records = await listings
            .OrderByDescending(l => l.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public Task AddAsync(StoreListing listing, CancellationToken cancellationToken)
    {
        _dbContext.StoreListings.Add(listing.ToRecord());
        return Task.CompletedTask;
    }

    public Task UpdateAsync(StoreListing listing, CancellationToken cancellationToken)
    {
        _dbContext.StoreListings.Update(listing.ToRecord());
        return Task.CompletedTask;
    }
}
