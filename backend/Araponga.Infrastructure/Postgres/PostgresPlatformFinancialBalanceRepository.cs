using Araponga.Application.Interfaces;
using Araponga.Domain.Financial;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresPlatformFinancialBalanceRepository : IPlatformFinancialBalanceRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresPlatformFinancialBalanceRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PlatformFinancialBalance?> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.PlatformFinancialBalances
            .FirstOrDefaultAsync(r => r.TerritoryId == territoryId, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<List<PlatformFinancialBalance>> GetAllAsync(CancellationToken cancellationToken)
    {
        var records = await _dbContext.PlatformFinancialBalances
            .OrderBy(r => r.TerritoryId)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public Task AddAsync(PlatformFinancialBalance balance, CancellationToken cancellationToken)
    {
        _dbContext.PlatformFinancialBalances.Add(balance.ToRecord());
        return Task.CompletedTask;
    }

    public Task UpdateAsync(PlatformFinancialBalance balance, CancellationToken cancellationToken)
    {
        _dbContext.PlatformFinancialBalances.Update(balance.ToRecord());
        return Task.CompletedTask;
    }
}
