using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Feed;
using Araponga.Domain.Health;

namespace Araponga.Application.Services;

public sealed class HealthService
{
    private readonly IHealthAlertRepository _alertRepository;
    private readonly IFeedRepository _feedRepository;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;

    public HealthService(
        IHealthAlertRepository alertRepository,
        IFeedRepository feedRepository,
        IAuditLogger auditLogger,
        IUnitOfWork unitOfWork)
    {
        _alertRepository = alertRepository;
        _feedRepository = feedRepository;
        _auditLogger = auditLogger;
        _unitOfWork = unitOfWork;
    }

    public Task<IReadOnlyList<HealthAlert>> ListAlertsAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        return _alertRepository.ListByTerritoryAsync(territoryId, cancellationToken);
    }

    public async Task<PagedResult<HealthAlert>> ListAlertsPagedAsync(
        Guid territoryId,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var alerts = await _alertRepository.ListByTerritoryAsync(territoryId, cancellationToken);
        var totalCount = alerts.Count;
        var pagedItems = alerts
            .OrderByDescending(a => a.CreatedAtUtc)
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToList();

        return new PagedResult<HealthAlert>(pagedItems, pagination.PageNumber, pagination.PageSize, totalCount);
    }

    public async Task<Result<HealthAlert>> ReportAlertAsync(
        Guid territoryId,
        Guid userId,
        string title,
        string description,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
        {
            return Result<HealthAlert>.Failure("Title and description are required.");
        }

        var alert = new HealthAlert(
            Guid.NewGuid(),
            territoryId,
            userId,
            title,
            description,
            HealthAlertStatus.Pending,
            DateTime.UtcNow);

        await _alertRepository.AddAsync(alert, cancellationToken);

        await _auditLogger.LogAsync(
            new Models.AuditEntry("alert.reported", userId, territoryId, alert.Id, DateTime.UtcNow),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<HealthAlert>.Success(alert);
    }

    public async Task<bool> ValidateAlertAsync(
        Guid territoryId,
        Guid alertId,
        Guid curatorId,
        HealthAlertStatus status,
        CancellationToken cancellationToken)
    {
        var alert = await _alertRepository.GetByIdAsync(alertId, cancellationToken);
        if (alert is null || alert.TerritoryId != territoryId)
        {
            return false;
        }

        await _alertRepository.UpdateStatusAsync(alertId, status, cancellationToken);

        if (status == HealthAlertStatus.Validated)
        {
            var post = new CommunityPost(
                Guid.NewGuid(),
                territoryId,
                alert.ReporterUserId,
                alert.Title,
                alert.Description,
                PostType.Alert,
                PostVisibility.Public,
                PostStatus.Published,
                null,
                DateTime.UtcNow);

            await _feedRepository.AddPostAsync(post, cancellationToken);
        }

        await _auditLogger.LogAsync(
            new Models.AuditEntry($"alert.{status.ToString().ToLowerInvariant()}", curatorId, territoryId, alertId, DateTime.UtcNow),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return true;
    }
}
