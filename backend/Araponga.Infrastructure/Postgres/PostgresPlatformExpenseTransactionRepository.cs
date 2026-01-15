using Araponga.Application.Interfaces;
using Araponga.Domain.Financial;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresPlatformExpenseTransactionRepository : IPlatformExpenseTransactionRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresPlatformExpenseTransactionRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PlatformExpenseTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.PlatformExpenseTransactions
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<List<PlatformExpenseTransaction>> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.PlatformExpenseTransactions
            .Where(r => r.TerritoryId == territoryId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<List<PlatformExpenseTransaction>> GetBySellerTransactionIdAsync(Guid sellerTransactionId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.PlatformExpenseTransactions
            .Where(r => r.SellerTransactionId == sellerTransactionId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public Task AddAsync(PlatformExpenseTransaction transaction, CancellationToken cancellationToken)
    {
        _dbContext.PlatformExpenseTransactions.Add(transaction.ToRecord());
        return Task.CompletedTask;
    }
}
