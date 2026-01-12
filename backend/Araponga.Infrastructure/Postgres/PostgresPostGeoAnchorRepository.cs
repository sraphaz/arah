using Araponga.Application.Interfaces;
using Araponga.Domain.Map;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresPostGeoAnchorRepository : IPostGeoAnchorRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresPostGeoAnchorRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(IReadOnlyCollection<PostGeoAnchor> anchors, CancellationToken cancellationToken)
    {
        if (anchors.Count == 0)
        {
            return Task.CompletedTask;
        }

        var records = anchors.Select(anchor => anchor.ToRecord());
        _dbContext.PostGeoAnchors.AddRange(records);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<PostGeoAnchor>> ListByPostIdsAsync(
        IReadOnlyCollection<Guid> postIds,
        CancellationToken cancellationToken)
    {
        if (postIds.Count == 0)
        {
            return Array.Empty<PostGeoAnchor>();
        }

        var records = await _dbContext.PostGeoAnchors
            .AsNoTracking()
            .Where(anchor => postIds.Contains(anchor.PostId))
            .ToListAsync(cancellationToken);

        return records.Select(record => record.ToDomain()).ToList();
    }
}
