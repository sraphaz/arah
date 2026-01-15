using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Feed;
using Araponga.Domain.Health;

namespace Araponga.Application.Services;

public sealed class HealthService
{
    private readonly IHealthAlertRepository _alertRepository;
    private readonly AlertCacheService? _alertCache;
    private readonly IFeedRepository _feedRepository;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TerritoryFeatureFlagGuard _featureGuard;

    public HealthService(
        IHealthAlertRepository alertRepository,
        IFeedRepository feedRepository,
        IAuditLogger auditLogger,
        IUnitOfWork unitOfWork,
        TerritoryFeatureFlagGuard featureGuard,
        AlertCacheService? alertCache = null)
    {
        _alertRepository = alertRepository;
        _feedRepository = feedRepository;
        _auditLogger = auditLogger;
        _unitOfWork = unitOfWork;
        _alertCache = alertCache;
        _featureGuard = featureGuard;
    }

    public Task<IReadOnlyList<HealthAlert>> ListAlertsAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        return _alertCache is not null
            ? _alertCache.GetAlertsByTerritoryAsync(territoryId, cancellationToken)
            : _alertRepository.ListByTerritoryAsync(territoryId, cancellationToken);
    }

    public async Task<PagedResult<HealthAlert>> ListAlertsPagedAsync(
        Guid territoryId,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        // Cache não é usado em métodos paginados pois a paginação no repositório é mais eficiente
        var totalCount = await _alertRepository.CountByTerritoryAsync(territoryId, cancellationToken);
        var alertsPaged = await _alertRepository.ListByTerritoryPagedAsync(territoryId, pagination.Skip, pagination.Take, cancellationToken);

        return new PagedResult<HealthAlert>(alertsPaged, pagination.PageNumber, pagination.PageSize, totalCount);
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

        // Invalidate cache when alert is created
        _alertCache?.InvalidateTerritoryAlerts(territoryId);

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
            // Alert posts são controlados por feature flag por território.
            // Se estiver desabilitada, validamos o alerta mas não publicamos post no feed.
            if (_featureGuard.IsEnabled(territoryId, FeatureFlag.AlertPosts))
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
        }

        await _auditLogger.LogAsync(
            new Models.AuditEntry($"alert.{status.ToString().ToLowerInvariant()}", curatorId, territoryId, alertId, DateTime.UtcNow),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        // Invalidate cache when alert status changes
        _alertCache?.InvalidateTerritoryAlerts(territoryId);

        return true;
    }
}
