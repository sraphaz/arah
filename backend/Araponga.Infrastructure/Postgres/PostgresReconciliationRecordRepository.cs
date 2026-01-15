using Araponga.Application.Interfaces;
using Araponga.Domain.Financial;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresReconciliationRecordRepository : IReconciliationRecordRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresReconciliationRecordRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ReconciliationRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var record = await _dbContext.ReconciliationRecords
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        return record?.ToDomain();
    }

    public async Task<List<ReconciliationRecord>> GetByTerritoryIdAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.ReconciliationRecords
            .Where(r => r.TerritoryId == territoryId)
            .OrderByDescending(r => r.ReconciliationDate)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<List<ReconciliationRecord>> GetByStatusAsync(Guid territoryId, ReconciliationStatus status, CancellationToken cancellationToken)
    {
        var records = await _dbContext.ReconciliationRecords
            .Where(r => r.TerritoryId == territoryId && r.Status == (int)status)
            .OrderByDescending(r => r.ReconciliationDate)
            .ToListAsync(cancellationToken);
        return records.Select(r => r.ToDomain()).ToList();
    }

    public Task AddAsync(ReconciliationRecord record, CancellationToken cancellationToken)
    {
        _dbContext.ReconciliationRecords.Add(record.ToRecord());
        return Task.CompletedTask;
    }

    public Task UpdateAsync(ReconciliationRecord record, CancellationToken cancellationToken)
    {
        _dbContext.ReconciliationRecords.Update(record.ToRecord());
        return Task.CompletedTask;
    }
}
