using Araponga.Application.Interfaces;
using Araponga.Domain.Map;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresMapRepository : IMapRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresMapRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<MapEntity>> ListByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        var records = await _dbContext.MapEntities
            .AsNoTracking()
            .Where(entity => entity.TerritoryId == territoryId)
            .ToListAsync(cancellationToken);
        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<MapEntity?> GetByIdAsync(Guid entityId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.MapEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.Id == entityId, cancellationToken);
        return record?.ToDomain();
    }

    public Task AddAsync(MapEntity entity, CancellationToken cancellationToken)
    {
        _dbContext.MapEntities.Add(entity.ToRecord());
        return Task.CompletedTask;
    }

    public async Task UpdateStatusAsync(Guid entityId, MapEntityStatus status, CancellationToken cancellationToken)
    {
        var record = await _dbContext.MapEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == entityId, cancellationToken);

        if (record is null)
        {
            return;
        }

        record.Status = status;
        _dbContext.MapEntities.Update(record);
    }

    public async Task IncrementConfirmationAsync(Guid entityId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.MapEntities
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == entityId, cancellationToken);

        if (record is null)
        {
            return;
        }

        record.ConfirmationCount += 1;
        _dbContext.MapEntities.Update(record);
    }
}
