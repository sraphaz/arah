using Araponga.Application.Interfaces;
using Araponga.Domain.Financial;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresPlatformRevenueTransactionRepository : IPlatformRevenueTransactionRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresPlatformRevenueTransactionRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PlatformRevenueTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.PlatformRevenueTransactions
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<List<PlatformRevenueTransaction>> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.PlatformRevenueTransactions
            .Where(r => r.TerritoryId == territoryId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<List<PlatformRevenueTransaction>> GetByCheckoutIdAsync(Guid checkoutId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.PlatformRevenueTransactions
            .Where(r => r.CheckoutId == checkoutId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public Task AddAsync(PlatformRevenueTransaction transaction, CancellationToken cancellationToken)
    {
        _dbContext.PlatformRevenueTransactions.Add(transaction.ToRecord());
        return Task.CompletedTask;
    }
}
