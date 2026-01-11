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

    public async Task AddAsync(ModerationReport report, CancellationToken cancellationToken)
    {
        _dbContext.ModerationReports.Add(new ModerationReportRecord
        {
            Id = report.Id,
            ReporterUserId = report.ReporterUserId,
            TargetType = report.TargetType,
            TargetId = report.TargetId,
            Reason = report.Reason,
            Details = report.Details,
            CreatedAtUtc = report.CreatedAtUtc
        });

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
