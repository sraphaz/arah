using Araponga.Application.Interfaces;
using Araponga.Domain.Health;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Alerts.Infrastructure.Postgres;

public sealed class PostgresHealthAlertRepository : IHealthAlertRepository
{
    private readonly AlertsDbContext _dbContext;

    public PostgresHealthAlertRepository(AlertsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<HealthAlert>> ListByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.HealthAlerts
            .AsNoTracking()
            .Where(alert => alert.TerritoryId == territoryId)
            .ToListAsync(cancellationToken);
        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<HealthAlert?> GetByIdAsync(Guid alertId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.HealthAlerts
            .AsNoTracking()
            .FirstOrDefaultAsync(alert => alert.Id == alertId, cancellationToken);
        return record?.ToDomain();
    }

    public Task AddAsync(HealthAlert alert, CancellationToken cancellationToken)
    {
        _dbContext.HealthAlerts.Add(alert.ToRecord());
        return Task.CompletedTask;
    }

    public async Task UpdateStatusAsync(Guid alertId, HealthAlertStatus status, CancellationToken cancellationToken)
    {
        var record = await _dbContext.HealthAlerts
            .FirstOrDefaultAsync(existing => existing.Id == alertId, cancellationToken);

        if (record is null)
        {
            return;
        }

        record.Status = status;
    }

    public async Task<IReadOnlyList<HealthAlert>> ListByTerritoryPagedAsync(
        Guid territoryId,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var records = await _dbContext.HealthAlerts
            .AsNoTracking()
            .Where(alert => alert.TerritoryId == territoryId)
            .OrderByDescending(alert => alert.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<int> CountByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        const int maxInt32 = int.MaxValue;
        var count = await _dbContext.HealthAlerts
            .Where(alert => alert.TerritoryId == territoryId)
            .CountAsync(cancellationToken);
        return count > maxInt32 ? maxInt32 : (int)count;
    }
}
