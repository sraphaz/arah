using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresPlatformFeeConfigRepository : IPlatformFeeConfigRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresPlatformFeeConfigRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PlatformFeeConfig?> GetActiveAsync(Guid territoryId, ListingType listingType, CancellationToken cancellationToken)
    {
        var record = await _dbContext.PlatformFeeConfigs
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.TerritoryId == territoryId && c.ListingType == listingType && c.IsActive, cancellationToken);

        return record?.ToDomain();
    }

    public async Task<IReadOnlyList<PlatformFeeConfig>> ListActiveAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.PlatformFeeConfigs
            .AsNoTracking()
            .Where(c => c.TerritoryId == territoryId && c.IsActive)
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<PlatformFeeConfig?> GetByIdAsync(Guid configId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.PlatformFeeConfigs
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == configId, cancellationToken);

        return record?.ToDomain();
    }

    public Task AddAsync(PlatformFeeConfig config, CancellationToken cancellationToken)
    {
        _dbContext.PlatformFeeConfigs.Add(config.ToRecord());
        return Task.CompletedTask;
    }

    public Task UpdateAsync(PlatformFeeConfig config, CancellationToken cancellationToken)
    {
        _dbContext.PlatformFeeConfigs.Update(config.ToRecord());
        return Task.CompletedTask;
    }
}
