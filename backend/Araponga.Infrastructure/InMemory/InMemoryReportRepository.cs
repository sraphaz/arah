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
}
