using Araponga.Application.Interfaces;
using Araponga.Domain.Governance;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresTerritoryModerationRuleRepository : ITerritoryModerationRuleRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresTerritoryModerationRuleRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(TerritoryModerationRule rule, CancellationToken cancellationToken)
    {
        _dbContext.TerritoryModerationRules.Add(rule.ToRecord());
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(TerritoryModerationRule rule, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryModerationRules
            .FirstOrDefaultAsync(r => r.Id == rule.Id, cancellationToken);

        if (record is null)
        {
            _dbContext.TerritoryModerationRules.Add(rule.ToRecord());
        }
        else
        {
            var updatedRecord = rule.ToRecord();
            _dbContext.Entry(record).CurrentValues.SetValues(updatedRecord);
        }
    }

    public async Task<IReadOnlyList<TerritoryModerationRule>> ListByTerritoryAsync(
        Guid territoryId,
        bool? isActive,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.TerritoryModerationRules
            .AsNoTracking()
            .Where(r => r.TerritoryId == territoryId);

        if (isActive.HasValue)
        {
            query = query.Where(r => r.IsActive == isActive.Value);
        }

        var records = await query
            .OrderByDescending(r => r.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return records.Select(r => r.ToDomain()).ToList();
    }

    public async Task<TerritoryModerationRule?> GetByIdAsync(Guid ruleId, CancellationToken cancellationToken)
    {
        var record = await _dbContext.TerritoryModerationRules
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == ruleId, cancellationToken);

        return record?.ToDomain();
    }
}
