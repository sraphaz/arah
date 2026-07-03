using Arah.Application.Interfaces;
using Arah.Domain.Financial;
using Microsoft.EntityFrameworkCore;

namespace Arah.Infrastructure.Postgres;

public sealed class PostgresFeeSplitRuleRepository : IFeeSplitRuleRepository
{
    private readonly ArahDbContext _dbContext;

    public PostgresFeeSplitRuleRepository(ArahDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FeeSplitRule?> GetActiveAsync(
        Guid territoryId,
        string revenueType,
        DateTimeOffset atUtc,
        CancellationToken cancellationToken)
    {
        var records = await _dbContext.FeeSplitRules
            .Where(r =>
                r.TerritoryId == territoryId &&
                r.RevenueType == revenueType &&
                r.EffectiveFromUtc <= atUtc &&
                r.SupersededAtUtc == null)
            .OrderByDescending(r => r.EffectiveFromUtc)
            .ToListAsync(cancellationToken);

        return records.Select(r => r.ToDomain()).FirstOrDefault(r => r.IsActiveAt(atUtc));
    }

    public Task AddAsync(FeeSplitRule rule, CancellationToken cancellationToken)
    {
        _dbContext.FeeSplitRules.Add(rule.ToRecord());
        return Task.CompletedTask;
    }

    public async Task SupersedeAsync(Guid ruleId, DateTimeOffset supersededAtUtc, CancellationToken cancellationToken)
    {
        var record = await _dbContext.FeeSplitRules
            .FirstOrDefaultAsync(r => r.Id == ruleId, cancellationToken)
            ?? throw new InvalidOperationException($"FeeSplitRule {ruleId} not found.");

        var rule = record.ToDomain();
        rule.Supersede(supersededAtUtc);
        record.SupersededAtUtc = rule.SupersededAtUtc;
    }
}
