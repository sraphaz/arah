using Araponga.Application.Interfaces;
using Araponga.Domain.Health;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresHealthAlertRepository : IHealthAlertRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresHealthAlertRepository(ArapongaDbContext dbContext)
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
}
