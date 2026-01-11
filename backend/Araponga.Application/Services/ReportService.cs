using Araponga.Application.Interfaces;
using Araponga.Domain.Moderation;

namespace Araponga.Application.Services;

public sealed class ReportService
{
    private static readonly TimeSpan DuplicateWindow = TimeSpan.FromHours(24);
    private readonly IReportRepository _reportRepository;
    private readonly IFeedRepository _feedRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuditLogger _auditLogger;

    public ReportService(
        IReportRepository reportRepository,
        IFeedRepository feedRepository,
        IUserRepository userRepository,
        IAuditLogger auditLogger)
    {
        _reportRepository = reportRepository;
        _feedRepository = feedRepository;
        _userRepository = userRepository;
        _auditLogger = auditLogger;
    }

    public async Task<(bool created, string? error, ModerationReport? report)> ReportPostAsync(
        Guid reporterUserId,
        Guid postId,
        string reason,
        string? details,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            return (false, "Reason is required.", null);
        }

        var post = await _feedRepository.GetPostAsync(postId, cancellationToken);
        if (post is null)
        {
            return (false, "Post not found.", null);
        }

        var sinceUtc = DateTime.UtcNow.Subtract(DuplicateWindow);
        var alreadyReported = await _reportRepository.HasRecentReportAsync(
            reporterUserId,
            ReportTargetType.Post,
            postId,
            sinceUtc,
            cancellationToken);

        if (alreadyReported)
        {
            return (false, null, null);
        }

        var report = new ModerationReport(
            Guid.NewGuid(),
            reporterUserId,
            ReportTargetType.Post,
            postId,
            reason,
            details,
            DateTime.UtcNow);

        await _reportRepository.AddAsync(report, cancellationToken);

        await _auditLogger.LogAsync(
            new Models.AuditEntry("report.post", reporterUserId, post.TerritoryId, postId, DateTime.UtcNow),
            cancellationToken);

        return (true, null, report);
    }

    public async Task<(bool created, string? error, ModerationReport? report)> ReportUserAsync(
        Guid reporterUserId,
        Guid reportedUserId,
        string reason,
        string? details,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            return (false, "Reason is required.", null);
        }

        var user = await _userRepository.GetByIdAsync(reportedUserId, cancellationToken);
        if (user is null)
        {
            return (false, "User not found.", null);
        }

        var sinceUtc = DateTime.UtcNow.Subtract(DuplicateWindow);
        var alreadyReported = await _reportRepository.HasRecentReportAsync(
            reporterUserId,
            ReportTargetType.User,
            reportedUserId,
            sinceUtc,
            cancellationToken);

        if (alreadyReported)
        {
            return (false, null, null);
        }

        var report = new ModerationReport(
            Guid.NewGuid(),
            reporterUserId,
            ReportTargetType.User,
            reportedUserId,
            reason,
            details,
            DateTime.UtcNow);

        await _reportRepository.AddAsync(report, cancellationToken);

        await _auditLogger.LogAsync(
            new Models.AuditEntry("report.user", reporterUserId, Guid.Empty, reportedUserId, DateTime.UtcNow),
            cancellationToken);

        return (true, null, report);
    }
}
