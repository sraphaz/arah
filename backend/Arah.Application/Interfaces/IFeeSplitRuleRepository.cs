using Arah.Domain.Financial;

namespace Arah.Application.Interfaces;

public interface IFeeSplitRuleRepository
{
    Task<FeeSplitRule?> GetActiveAsync(
        Guid territoryId,
        string revenueType,
        DateTimeOffset atUtc,
        CancellationToken cancellationToken);

    Task AddAsync(FeeSplitRule rule, CancellationToken cancellationToken);

    Task SupersedeAsync(Guid ruleId, DateTimeOffset supersededAtUtc, CancellationToken cancellationToken);
}
