using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Geo;
using Araponga.Domain.Map;

namespace Araponga.Application.Services;

public sealed class MapService
{
    private readonly MapEntityCacheService? _mapEntityCache;
    private readonly IMapRepository _mapRepository;
    private readonly AccessEvaluator _accessEvaluator;
    private readonly IAuditLogger _auditLogger;
    private readonly UserBlockCacheService? _userBlockCache;
    private readonly IUserBlockRepository _userBlockRepository;
    private readonly IMapEntityRelationRepository _relationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MapService(
        IMapRepository mapRepository,
        AccessEvaluator accessEvaluator,
        IAuditLogger auditLogger,
        IUserBlockRepository userBlockRepository,
        IMapEntityRelationRepository relationRepository,
        IUnitOfWork unitOfWork,
        MapEntityCacheService? mapEntityCache = null,
        UserBlockCacheService? userBlockCache = null)
    {
        _mapRepository = mapRepository;
        _accessEvaluator = accessEvaluator;
        _auditLogger = auditLogger;
        _userBlockRepository = userBlockRepository;
        _relationRepository = relationRepository;
        _unitOfWork = unitOfWork;
        _mapEntityCache = mapEntityCache;
        _userBlockCache = userBlockCache;
    }

    public async Task<IReadOnlyList<MapEntity>> ListEntitiesAsync(
        Guid territoryId,
        Guid? userId,
        CancellationToken cancellationToken)
    {
        var entities = _mapEntityCache is not null
            ? await _mapEntityCache.GetEntitiesByTerritoryAsync(territoryId, cancellationToken)
            : await _mapRepository.ListByTerritoryAsync(territoryId, cancellationToken);
        var blockedUserIds = userId is null
            ? Array.Empty<Guid>()
            : _userBlockCache is not null
                ? await _userBlockCache.GetBlockedUserIdsAsync(userId.Value, cancellationToken)
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

    public async Task<PagedResult<MapEntity>> ListEntitiesPagedAsync(
        Guid territoryId,
        Guid? userId,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        // Cache não é usado em métodos paginados pois a paginação no repositório é mais eficiente
        // Primeiro buscamos a página do repositório, depois aplicamos filtros
        var entitiesPaged = await _mapRepository.ListByTerritoryPagedAsync(territoryId, pagination.Skip, pagination.Take, cancellationToken);
        
        var blockedUserIds = userId is null
            ? Array.Empty<Guid>()
            : _userBlockCache is not null
                ? await _userBlockCache.GetBlockedUserIdsAsync(userId.Value, cancellationToken)
                : await _userBlockRepository.GetBlockedUserIdsAsync(userId.Value, cancellationToken);

        var visibleEntities = blockedUserIds.Count == 0
            ? entitiesPaged
            : entitiesPaged.Where(entity => !blockedUserIds.Contains(entity.CreatedByUserId)).ToList();

        IReadOnlyList<MapEntity> filtered;
        if (userId is null)
        {
            filtered = visibleEntities
                .Where(entity => entity.Visibility == MapEntityVisibility.Public)
                .ToList();
        }
        else
        {
            var isResident = await _accessEvaluator.IsResidentAsync(userId.Value, territoryId, cancellationToken);
            filtered = isResident
                ? visibleEntities.ToList()
                : visibleEntities.Where(entity => entity.Visibility == MapEntityVisibility.Public).ToList();
        }

        // Para contagem total, precisamos buscar todos (mas apenas a contagem)
        // Nota: Isso pode ser otimizado no futuro com um método de contagem que aplica os mesmos filtros
        var allEntities = await _mapRepository.ListByTerritoryAsync(territoryId, cancellationToken);
        var allVisible = blockedUserIds.Count == 0
            ? allEntities
            : allEntities.Where(entity => !blockedUserIds.Contains(entity.CreatedByUserId)).ToList();
        
        var allFiltered = userId is null
            ? allVisible.Where(entity => entity.Visibility == MapEntityVisibility.Public).ToList()
            : await _accessEvaluator.IsResidentAsync(userId.Value, territoryId, cancellationToken)
                ? allVisible.ToList()
                : allVisible.Where(entity => entity.Visibility == MapEntityVisibility.Public).ToList();

        var totalCount = allFiltered.Count;

        return new PagedResult<MapEntity>(filtered, pagination.PageNumber, pagination.PageSize, totalCount);
    }

    public async Task<Result<MapEntity>> SuggestAsync(
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
            return Result<MapEntity>.Failure("Name and category are required.");
        }

        if (!GeoCoordinate.IsValid(latitude, longitude))
        {
            return Result<MapEntity>.Failure("Invalid latitude/longitude.");
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

        // Invalidate cache when map entity is created
        _mapEntityCache?.InvalidateTerritoryEntities(territoryId);

        return Result<MapEntity>.Success(entity);
    }

    public async Task<Result<MapEntityRelation>> RelateAsync(
        Guid territoryId,
        Guid entityId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var entity = await _mapRepository.GetByIdAsync(entityId, cancellationToken);
        if (entity is null || entity.TerritoryId != territoryId)
        {
            return Result<MapEntityRelation>.Failure("Entity not found.");
        }

        var isResident = await _accessEvaluator.IsResidentAsync(userId, territoryId, cancellationToken);
        if (!isResident)
        {
            return Result<MapEntityRelation>.Failure("Only residents can relate to entities.");
        }

        var exists = await _relationRepository.ExistsAsync(userId, entityId, cancellationToken);
        if (exists)
        {
            return Result<MapEntityRelation>.Failure("Relation already exists.");
        }

        var relation = new MapEntityRelation(userId, entityId, DateTime.UtcNow);
        await _relationRepository.AddAsync(relation, cancellationToken);

        await _auditLogger.LogAsync(
            new Models.AuditEntry("map.related", userId, territoryId, entityId, DateTime.UtcNow),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<MapEntityRelation>.Success(relation);
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

        // Invalidate cache when map entity status changes
        _mapEntityCache?.InvalidateTerritoryEntities(territoryId);

        return true;
    }

    public async Task<OperationResult> ConfirmAsync(
        Guid territoryId,
        Guid entityId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var entity = await _mapRepository.GetByIdAsync(entityId, cancellationToken);
        if (entity is null || entity.TerritoryId != territoryId)
        {
            return OperationResult.Failure("Entity not found.");
        }

        var isResident = await _accessEvaluator.IsResidentAsync(userId, territoryId, cancellationToken);
        if (!isResident)
        {
            return OperationResult.Failure("Only residents can confirm entities.");
        }

        await _mapRepository.IncrementConfirmationAsync(entityId, cancellationToken);

        await _auditLogger.LogAsync(
            new Models.AuditEntry("map.confirmed", userId, territoryId, entityId, DateTime.UtcNow),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        return OperationResult.Success();
    }
}
