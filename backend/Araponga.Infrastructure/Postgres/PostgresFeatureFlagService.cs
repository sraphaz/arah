using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresFeatureFlagService : IFeatureFlagService
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresFeatureFlagService(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool IsEnabled(Guid territoryId, FeatureFlag flag)
    {
        return _dbContext.FeatureFlags
            .AsNoTracking()
            .Any(entry => entry.TerritoryId == territoryId && entry.Flag == flag);
    }

    public IReadOnlyList<FeatureFlag> GetEnabledFlags(Guid territoryId)
    {
        return _dbContext.FeatureFlags
            .AsNoTracking()
            .Where(entry => entry.TerritoryId == territoryId)
            .Select(entry => entry.Flag)
            .ToList();
    }

    public void SetEnabledFlags(Guid territoryId, IReadOnlyList<FeatureFlag> flags)
    {
        var existing = _dbContext.FeatureFlags
            .Where(entry => entry.TerritoryId == territoryId)
            .ToList();

        if (existing.Count > 0)
        {
            _dbContext.FeatureFlags.RemoveRange(existing);
        }

        foreach (var flag in flags.Distinct())
        {
            _dbContext.FeatureFlags.Add(new FeatureFlagRecord
            {
                TerritoryId = territoryId,
                Flag = flag
            });
        }
    }
}
