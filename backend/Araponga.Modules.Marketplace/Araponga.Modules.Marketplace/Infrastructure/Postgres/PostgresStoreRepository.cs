using Araponga.Modules.Marketplace.Application.Interfaces;
using Araponga.Modules.Marketplace.Domain;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Marketplace.Infrastructure.Postgres;

public sealed class PostgresStoreRepository : IStoreRepository
{
    private readonly MarketplaceDbContext _dbContext;

    public PostgresStoreRepository(MarketplaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Store?> GetByIdAsync(Guid storeId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryStores
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == storeId, cancellationToken);

        return record?.ToDomain();
    }

    public async Task<Store?> GetByOwnerAsync(Guid territoryId, Guid ownerUserId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryStores
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.TerritoryId == territoryId && s.OwnerUserId == ownerUserId, cancellationToken);

        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<Store>> ListByOwnerAsync(Guid ownerUserId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.TerritoryStores
            .AsNoTracking()
            .Where(s => s.OwnerUserId == ownerUserId)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<IReadOnlyList<Store>> ListByIdsAsync(IReadOnlyCollection<Guid> storeIds, CancellationToken cancellationToken)
    {
        if (storeIds.Count == 0)
        {
            return Array.Empty<Store>();
        }

        var records = await _dbContext.TerritoryStores
            .AsNoTracking()
            .Where(store => storeIds.Contains(store.Id))
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public Task AddAsync(Store store, CancellationToken cancellationToken)
    {
        _dbContext.TerritoryStores.Add(store.ToRecord());
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Store store, CancellationToken cancellationToken)
    {
        _dbContext.TerritoryStores.Update(store.ToRecord());
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<Store>> ListByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.TerritoryStores
            .AsNoTracking()
            .Where(s => s.TerritoryId == territoryId)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }
}
