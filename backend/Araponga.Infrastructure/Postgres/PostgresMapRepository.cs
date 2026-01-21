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
            .FirstOrDefaultAsync(e => e.Id == entityId, cancellationToken);

        if (record is null)
        {
            return;
        }

        record.Status = status;
    }

    public async Task IncrementConfirmationAsync(Guid entityId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.MapEntities
            .FirstOrDefaultAsync(e => e.Id == entityId, cancellationToken);

        if (record is null)
        {
            return;
        }

        const int maxInt32 = int.MaxValue;
        if (record.ConfirmationCount < maxInt32)
        {
            record.ConfirmationCount += 1;
        }
    }

    public async Task<IReadOnlyList<MapEntity>> ListByTerritoryPagedAsync(
        Guid territoryId,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var records = await _dbContext.MapEntities
            .AsNoTracking()
            .Where(entity => entity.TerritoryId == territoryId)
            .OrderByDescending(entity => entity.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
        return records.Select(record => record.ToDomain()).ToList();
    }

    public async Task<int> CountByTerritoryAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        const int maxInt32 = int.MaxValue;
        var count = await _dbContext.MapEntities
            .Where(entity => entity.TerritoryId == territoryId)
            .CountAsync(cancellationToken);
        return count > maxInt32 ? maxInt32 : (int)count;
    }
}
