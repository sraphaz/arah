using Araponga.Application.Interfaces;
using Araponga.Domain.Moderation;
using Araponga.Infrastructure.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace Araponga.Infrastructure.Postgres;

public sealed class PostgresReportRepository : IReportRepository
{
    private readonly ArapongaDbContext _dbContext;

    public PostgresReportRepository(ArapongaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> HasRecentReportAsync(
        Guid reporterUserId,
        ReportTargetType targetType,
        Guid targetId,
        DateTime sinceUtc,
        CancellationToken cancellationToken)
    {
        return await _dbContext.ModerationReports
            .AsNoTracking()
            .AnyAsync(report =>
                report.ReporterUserId == reporterUserId &&
                report.TargetType == targetType &&
                report.TargetId == targetId &&
                report.CreatedAtUtc >= sinceUtc,
                cancellationToken);
    }

    public Task AddAsync(ModerationReport report, CancellationToken cancellationToken)
    {
        _dbContext.ModerationReports.Add(new ModerationReportRecord
        {
            Id = report.Id,
            ReporterUserId = report.ReporterUserId,
            TerritoryId = report.TerritoryId,
            TargetType = report.TargetType,
            TargetId = report.TargetId,
            Reason = report.Reason,
            Details = report.Details,
            Status = report.Status,
            CreatedAtUtc = report.CreatedAtUtc
        });
        return Task.CompletedTask;
    }

    public async Task<int> CountDistinctReportersAsync(
        ReportTargetType targetType,
        Guid targetId,
        DateTime sinceUtc,
        CancellationToken cancellationToken)
    {
        return await _dbContext.ModerationReports
            .AsNoTracking()
            .Where(report =>
                report.TargetType == targetType &&
                report.TargetId == targetId &&
                report.CreatedAtUtc >= sinceUtc)
            .Select(report => report.ReporterUserId)
            .Distinct()
            .CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ModerationReport>> ListAsync(
        Guid territoryId,
        ReportTargetType? targetType,
        ReportStatus? status,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.ModerationReports
            .AsNoTracking()
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

        var records = await query.ToListAsync(cancellationToken);

        return records.Select(record => new ModerationReport(
            record.Id,
            record.ReporterUserId,
            record.TerritoryId,
            record.TargetType,
            record.TargetId,
            record.Reason,
            record.Details,
            record.Status,
            record.CreatedAtUtc)).ToList();
    }
}
