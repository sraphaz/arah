using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Marketplace.Infrastructure.Postgres;

public sealed class PostgresTerritoryPayoutConfigRepository : ITerritoryPayoutConfigRepository
{
    private readonly MarketplaceDbContext _dbContext;

    public PostgresTerritoryPayoutConfigRepository(MarketplaceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TerritoryPayoutConfig?> GetActiveAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryPayoutConfigs
            .FirstOrDefaultAsync(c => c.TerritoryId == territoryId && c.IsActive, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<TerritoryPayoutConfig?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryPayoutConfigs
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<List<TerritoryPayoutConfig>> ListByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.TerritoryPayoutConfigs
            .Where(c => c.TerritoryId == territoryId)
            .OrderByDescending(c => c.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public Task AddAsync(TerritoryPayoutConfig config, CancellationToken cancellationToken)
    {
        _dbContext.TerritoryPayoutConfigs.Add(config.ToRecord());
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TerritoryPayoutConfig config, CancellationToken cancellationToken)
    {
        _dbContext.TerritoryPayoutConfigs.Update(config.ToRecord());
        return Task.CompletedTask;
    }
}
