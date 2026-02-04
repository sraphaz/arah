using Araponga.Application.Interfaces;
using Araponga.Modules.Marketplace.Application.Interfaces;
using Araponga.Modules.Marketplace.Domain;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresSellerTransactionRepository : ISellerTransactionRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresSellerTransactionRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SellerTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.SellerTransactions
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<SellerTransaction?> GetByCheckoutIdAsync(Guid checkoutId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.SellerTransactions
            .FirstOrDefaultAsync(r => r.CheckoutId == checkoutId, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<List<SellerTransaction>> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.SellerTransactions
            .Where(r => r.TerritoryId == territoryId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<List<SellerTransaction>> GetBySellerUserIdAsync(Guid sellerUserId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.SellerTransactions
            .Where(r => r.SellerUserId == sellerUserId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<List<SellerTransaction>> GetByStatusAsync(Guid territoryId, SellerTransactionStatus status, CancellationToken cancellationToken)
    {
        var records = await _dbContext.SellerTransactions
            .Where(r => r.TerritoryId == territoryId && r.Status == (int)status)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<List<SellerTransaction>> GetReadyForPayoutAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.SellerTransactions
            .Where(r => r.TerritoryId == territoryId && r.Status == (int)SellerTransactionStatus.ReadyForPayout)
            .OrderBy(r => r.ReadyForPayoutAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<List<SellerTransaction>> GetByPayoutIdAsync(string payoutId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.SellerTransactions
            .Where(r => r.PayoutId == payoutId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public Task AddAsync(SellerTransaction transaction, CancellationToken cancellationToken)
    {
        _dbContext.SellerTransactions.Add(transaction.ToRecord());
        return Task.CompletedTask;
    }

    public Task UpdateAsync(SellerTransaction transaction, CancellationToken cancellationToken)
    {
        _dbContext.SellerTransactions.Update(transaction.ToRecord());
        return Task.CompletedTask;
    }
}
