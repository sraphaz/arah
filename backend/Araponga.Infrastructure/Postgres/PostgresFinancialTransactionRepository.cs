using Araponga.Application.Interfaces;
using Araponga.Domain.Financial;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresFinancialTransactionRepository : IFinancialTransactionRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresFinancialTransactionRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FinancialTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.FinancialTransactions
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<List<FinancialTransaction>> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.FinancialTransactions
            .Where(r => r.TerritoryId == territoryId)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<List<FinancialTransaction>> GetByRelatedEntityIdAsync(Guid relatedEntityId, string? relatedEntityType, CancellationToken cancellationToken)
    {
        var query = _dbContext.FinancialTransactions
            .Where(r => r.RelatedEntityId == relatedEntityId);

        if (!string.IsNullOrWhiteSpace(relatedEntityType))
        {
            query = query.Where(r => r.RelatedEntityType == relatedEntityType);
        }

        var records = await query
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<List<FinancialTransaction>> GetByTypeAsync(Guid territoryId, TransactionType type, CancellationToken cancellationToken)
    {
        var records = await _dbContext.FinancialTransactions
            .Where(r => r.TerritoryId == territoryId && r.Type == (int)type)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<List<FinancialTransaction>> GetByStatusAsync(Guid territoryId, TransactionStatus status, CancellationToken cancellationToken)
    {
        var records = await _dbContext.FinancialTransactions
            .Where(r => r.TerritoryId == territoryId && r.Status == (int)status)
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public Task AddAsync(FinancialTransaction transaction, CancellationToken cancellationToken)
    {
        _dbContext.FinancialTransactions.Add(transaction.ToRecord());
        return Task.CompletedTask;
    }

    public Task UpdateAsync(FinancialTransaction transaction, CancellationToken cancellationToken)
    {
        _dbContext.FinancialTransactions.Update(transaction.ToRecord());
        return Task.CompletedTask;
    }
}
