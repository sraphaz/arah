using Araponga.Modules.Map.Application.Interfaces;
using Araponga.Modules.Map.Domain;
using Araponga.Modules.Map.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Modules.Map.Infrastructure.Postgres;

public sealed class PostgresMapEntityRelationRepository : IMapEntityRelationRepository
{
    private readonly MapDbContext _dbContext;

    public PostgresMapEntityRelationRepository(MapDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsAsync(Guid userId, Guid entityId, CancellationToken cancellationToken)
    {
        return await _dbContext.MapEntityRelations
            .AsNoTracking()
            .AnyAsync(relation => relation.UserId == userId && relation.EntityId == entityId, cancellationToken);
    }

    public Task AddAsync(MapEntityRelation relation, CancellationToken cancellationToken)
    {
        _dbContext.MapEntityRelations.Add(new MapEntityRelationRecord
        {
            UserId = relation.UserId,
            EntityId = relation.EntityId,
            CreatedAtUtc = relation.CreatedAtUtc
        });

        return Task.CompletedTask;
    }
}
