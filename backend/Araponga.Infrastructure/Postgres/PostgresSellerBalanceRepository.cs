using Araponga.Application.Interfaces;
using Araponga.Modules.Marketplace.Application.Interfaces;
using Araponga.Modules.Marketplace.Domain;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresSellerBalanceRepository : ISellerBalanceRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresSellerBalanceRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SellerBalance?> GetByTerritoryAndSellerAsync(Guid territoryId, Guid sellerUserId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.SellerBalances
            .FirstOrDefaultAsync(r => r.TerritoryId == territoryId && r.SellerUserId == sellerUserId, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<List<SellerBalance>> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.SellerBalances
            .Where(r => r.TerritoryId == territoryId)
            .OrderBy(r => r.SellerUserId)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<List<SellerBalance>> GetBySellerUserIdAsync(Guid sellerUserId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.SellerBalances
            .Where(r => r.SellerUserId == sellerUserId)
            .OrderBy(r => r.TerritoryId)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public Task AddAsync(SellerBalance balance, CancellationToken cancellationToken)
    {
        _dbContext.SellerBalances.Add(balance.ToRecord());
        return Task.CompletedTask;
    }

    public Task UpdateAsync(SellerBalance balance, CancellationToken cancellationToken)
    {
        _dbContext.SellerBalances.Update(balance.ToRecord());
        return Task.CompletedTask;
    }
}
