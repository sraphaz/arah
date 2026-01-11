using Araponga.Application.Interfaces;
using Araponga.Domain.Moderation;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryReportRepository : IReportRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryReportRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<bool> HasRecentReportAsync(
        Guid reporterUserId,
        ReportTargetType targetType,
        Guid targetId,
        DateTime sinceUtc,
        CancellationToken cancellationToken)
    {
        var exists = _dataStore.ModerationReports.Any(report =>
            report.ReporterUserId == reporterUserId &&
            report.TargetType == targetType &&
            report.TargetId == targetId &&
            report.CreatedAtUtc >= sinceUtc);

        return Task.FromResult(exists);
    }

    public Task AddAsync(ModerationReport report, CancellationToken cancellationToken)
    {
        _dataStore.ModerationReports.Add(report);
        return Task.CompletedTask;
    }

    public Task<int> CountDistinctReportersAsync(
        ReportTargetType targetType,
        Guid targetId,
        DateTime sinceUtc,
        CancellationToken cancellationToken)
    {
        var count = _dataStore.ModerationReports
            .Where(report => report.TargetType == targetType &&
                             report.TargetId == targetId &&
                             report.CreatedAtUtc >= sinceUtc)
            .Select(report => report.ReporterUserId)
            .Distinct()
            .Count();

        return Task.FromResult(count);
    }

    public Task<IReadOnlyList<ModerationReport>> ListAsync(
        Guid territoryId,
        ReportTargetType? targetType,
        ReportStatus? status,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken)
    {
        var query = _dataStore.ModerationReports
            .Where(report => report.TerritoryId == territoryId);

        if (targetType is not null)
        {
            query = query.Where(report => report.TargetType == targetType);
        }

        if (status is not null)
        {
            query = query.Where(report => report.Status == status);
        }

        if (fromUtc is not null)
        {
            query = query.Where(report => report.CreatedAtUtc >= fromUtc.Value);
        }

        if (toUtc is not null)
        {
            query = query.Where(report => report.CreatedAtUtc <= toUtc.Value);
        }

        return Task.FromResult<IReadOnlyList<ModerationReport>>(query.ToList());
    }
}
