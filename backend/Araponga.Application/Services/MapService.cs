using Araponga.Application.Interfaces;
using Araponga.Domain.Geo;
using Araponga.Domain.Map;

namespace Araponga.Application.Services;

public sealed class MapService
{
    private readonly IMapRepository _mapRepository;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly IAuditLogger _auditLogger;
    private readonly IUserBlockRepository _userBlockRepository;
    private readonly IMapEntityRelationRepository _relationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MapService(
        IMapRepository mapRepository,
        AccessEvaluator accessEvaluator,
        IAuditLogger auditLogger,
        IUserBlockRepository userBlockRepository,
        IMapEntityRelationRepository relationRepository,
        IUnitOfWork unitOfWork)
    {
        _mapRepository = mapRepository;
        _accessEvaluator = accessEvaluator;
        _auditLogger = auditLogger;
        _userBlockRepository = userBlockRepository;
        _relationRepository = relationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<MapEntity>> ListEntitiesAsync(
        Guid territoryId,
        Guid? userId,
        CancellationToken cancellationToken)
    {
        var entities = await _mapRepository.ListByTerritoryAsync(territoryId, cancellationToken);
        var blockedUserIds = userId is null
            ? Array.Empty<Guid>()
            : await _userBlockRepository.GetBlockedUserIdsAsync(userId.Value, cancellationToken);

        var visibleEntities = blockedUserIds.Count == 0
            ? entities
            : entities.Where(entity => !blockedUserIds.Contains(entity.CreatedByUserId)).ToList();

        if (userId is null)
        {
            return visibleEntities
                .Where(entity => entity.Visibility == MapEntityVisibility.Public)
                .ToList();
        }

        var isResident = await _accessEvaluator.IsResidentAsync(userId.Value, territoryId, cancellationToken);

        return isResident
            ? visibleEntities.ToList()
            : visibleEntities.Where(entity => entity.Visibility == MapEntityVisibility.Public).ToList();
    }

    public async Task<(bool success, string? error, MapEntity? entity)> SuggestAsync(
        Guid territoryId,
        Guid userId,
        string name,
        string category,
        double latitude,
        double longitude,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(category))
        {
            return (false, "Name and category are required.", null);
        }

        if (!GeoCoordinate.IsValid(latitude, longitude))
        {
            return (false, "Invalid latitude/longitude.", null);
        }

        var entity = new MapEntity(
            Guid.NewGuid(),
            territoryId,
            userId,
            name,
            category,
            latitude,
            longitude,
            MapEntityStatus.Suggested,
            MapEntityVisibility.ResidentsOnly,
            0,
            DateTime.UtcNow);

        await _mapRepository.AddAsync(entity, cancellationToken);

        await _auditLogger.LogAsync(
            new Models.AuditEntry("map.suggested", userId, territoryId, entity.Id, DateTime.UtcNow),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return (true, null, entity);
    }

    public async Task<(bool success, string? error, MapEntityRelation? relation)> RelateAsync(
        Guid territoryId,
        Guid entityId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var entity = await _mapRepository.GetByIdAsync(entityId, cancellationToken);
        if (entity is null || entity.TerritoryId != territoryId)
        {
            return (false, "Entity not found.", null);
        }

        var isResident = await _accessEvaluator.IsResidentAsync(userId, territoryId, cancellationToken);
        if (!isResident)
        {
            return (false, "Only residents can relate to entities.", null);
        }

        var exists = await _relationRepository.ExistsAsync(userId, entityId, cancellationToken);
        if (exists)
        {
            return (false, null, null);
        }

        var relation = new MapEntityRelation(userId, entityId, DateTime.UtcNow);
        await _relationRepository.AddAsync(relation, cancellationToken);

        await _auditLogger.LogAsync(
            new Models.AuditEntry("map.related", userId, territoryId, entityId, DateTime.UtcNow),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return (true, null, relation);
    }

    public async Task<bool> ValidateAsync(
        Guid territoryId,
        Guid entityId,
        Guid curatorId,
        MapEntityStatus status,
        CancellationToken cancellationToken)
    {
        var entity = await _mapRepository.GetByIdAsync(entityId, cancellationToken);
        if (entity is null || entity.TerritoryId != territoryId)
        {
            return false;
        }

        await _mapRepository.UpdateStatusAsync(entityId, status, cancellationToken);

        await _auditLogger.LogAsync(
            new Models.AuditEntry($"map.{status.ToString().ToLowerInvariant()}", curatorId, territoryId, entityId, DateTime.UtcNow),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return true;
    }

    public async Task<(bool success, string? error)> ConfirmAsync(
        Guid territoryId,
        Guid entityId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var entity = await _mapRepository.GetByIdAsync(entityId, cancellationToken);
        if (entity is null || entity.TerritoryId != territoryId)
        {
            return (false, "Entity not found.");
        }

        var isResident = await _accessEvaluator.IsResidentAsync(userId, territoryId, cancellationToken);
        if (!isResident)
        {
            return (false, "Only residents can confirm entities.");
        }

        await _mapRepository.IncrementConfirmationAsync(entityId, cancellationToken);

        await _auditLogger.LogAsync(
            new Models.AuditEntry("map.confirmed", userId, territoryId, entityId, DateTime.UtcNow),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return (true, null);
    }
}
