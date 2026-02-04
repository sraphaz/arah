using Araponga.Modules.Moderation.Application.Interfaces;
using Araponga.Modules.Moderation.Domain.Moderation;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryReportRepository : IReportRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryReportRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<ModerationReport?> GetByIdAsync(Guid reportId, CancellationToken cancellationToken)
    {
        var report = _dataStore.ModerationReports.FirstOrDefault(r => r.Id == reportId);
        return Task.FromResult<ModerationReport?>(report);
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

        const int maxInt32 = int.MaxValue;
        return Task.FromResult(count > maxInt32 ? maxInt32 : count);
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

    public Task<IReadOnlyList<ModerationReport>> ListPagedAsync(
        Guid territoryId,
        ReportTargetType? targetType,
        ReportStatus? status,
        DateTime? fromUtc,
        DateTime? toUtc,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var query = _dataStore.ModerationReports
            .Where(report => report.TerritoryId == territoryId)
            .AsEnumerable();

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

        var result = query
            .OrderByDescending(report => report.CreatedAtUtc)
            .Skip(skip)
            .Take(take)
            .ToList();

        return Task.FromResult<IReadOnlyList<ModerationReport>>(result);
    }

    public Task<int> CountAsync(
        Guid territoryId,
        ReportTargetType? targetType,
        ReportStatus? status,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken)
    {
        var query = _dataStore.ModerationReports
            .Where(report => report.TerritoryId == territoryId)
            .AsEnumerable();

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

        const int maxInt32 = int.MaxValue;
        var count = query.Count();
        return Task.FromResult(count > maxInt32 ? maxInt32 : count);
    }

    public Task UpdateStatusAsync(Guid reportId, ReportStatus status, CancellationToken cancellationToken)
    {
        var idx = _dataStore.ModerationReports.FindIndex(r => r.Id == reportId);
        if (idx < 0)
        {
            return Task.CompletedTask;
        }

        var existing = _dataStore.ModerationReports[idx];
        var updated = new ModerationReport(
            existing.Id,
            existing.ReporterUserId,
            existing.TerritoryId,
            existing.TargetType,
            existing.TargetId,
            existing.Reason,
            existing.Details,
            status,
            existing.CreatedAtUtc);

        _dataStore.ModerationReports[idx] = updated;
        return Task.CompletedTask;
    }
}
