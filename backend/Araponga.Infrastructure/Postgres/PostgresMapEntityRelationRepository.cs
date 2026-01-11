using Araponga.Application.Interfaces;
using Araponga.Domain.Map;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresMapEntityRelationRepository : IMapEntityRelationRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresMapEntityRelationRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsAsync(Guid userId, Guid entityId, CancellationToken cancellationToken)
    {
        return await _dbContext.MapEntityRelations
            .AsNoTracking()
            .AnyAsync(relation => relation.UserId == userId && relation.EntityId == entityId, cancellationToken);
    }

    public async Task AddAsync(MapEntityRelation relation, CancellationToken cancellationToken)
    {
        _dbContext.MapEntityRelations.Add(new MapEntityRelationRecord
        {
            UserId = relation.UserId,
            EntityId = relation.EntityId,
            CreatedAtUtc = relation.CreatedAtUtc
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
