using Arah.Application.Common;
using Arah.Application.Interfaces;
using Arah.Application.Models;
using Arah.Modules.Assets.Application.Interfaces;
using Arah.Modules.Assets.Domain;
using Arah.Domain.Geo;
using Arah.Domain.Membership;
using Arah.Modules.Moderation.Application.Interfaces;
using Arah.Modules.Moderation.Domain.Work;

namespace Arah.Application.Services;

public sealed class TerritoryAssetService
{
    private readonly ITerritoryAssetRepository _assetRepository;
    private readonly IAssetGeoAnchorRepository _anchorRepository;
    private readonly IAssetValidationRepository _validationRepository;
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly IWorkItemRepository _workItemRepository;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CacheInvalidationService? _cacheInvalidation;

    public TerritoryAssetService(
        ITerritoryAssetRepository assetRepository,
        IAssetGeoAnchorRepository anchorRepository,
        IAssetValidationRepository validationRepository,
        ITerritoryMembershipRepository membershipRepository,
        IWorkItemRepository workItemRepository,
        IAuditLogger auditLogger,
        IUnitOfWork unitOfWork,
        CacheInvalidationService? cacheInvalidation = null)
    {
        _assetRepository = assetRepository;
        _anchorRepository = anchorRepository;
        _validationRepository = validationRepository;
        _membershipRepository = membershipRepository;
        _workItemRepository = workItemRepository;
        _auditLogger = auditLogger;
        _unitOfWork = unitOfWork;
        _cacheInvalidation = cacheInvalidation;
    }

    public async Task<IReadOnlyList<TerritoryAssetDetails>> ListAsync(
        Guid territoryId,
        IReadOnlyCollection<string>? types,
        AssetStatus? status,
        string? search,
        CancellationToken cancellationToken)
    {
        var normalizedTypes = NormalizeTypes(types);
        var assets = await _assetRepository.ListAsync(
            territoryId,
            null,
            normalizedTypes,
            status,
            search,
            cancellationToken);

        return await BuildAssetDetailsAsync(territoryId, assets, cancellationToken);
    }

    public async Task<PagedResult<TerritoryAssetDetails>> ListPagedAsync(
        Guid territoryId,
        IReadOnlyCollection<string>? types,
        AssetStatus? status,
        string? search,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var normalizedTypes = NormalizeTypes(types);
        var assets = await _assetRepository.ListAsync(
            territoryId,
            null,
            normalizedTypes,
            status,
            search,
            cancellationToken);

        var details = await BuildAssetDetailsAsync(territoryId, assets, cancellationToken);
        var ordered = details.OrderByDescending(d => d.Asset.CreatedAtUtc).ToList();
        return ordered.ToPagedResult(pagination);
    }

    public async Task<TerritoryAssetDetails?> GetByIdAsync(Guid assetId, CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByIdAsync(assetId, cancellationToken);
        if (asset is null)
        {
            return null;
        }

        var details = await BuildAssetDetailsAsync(asset.TerritoryId, new[] { asset }, cancellationToken);
        return details.Count > 0 ? details[0] : null;
    }

    public async Task<Result<TerritoryAssetDetails>> CreateAsync(
        Guid territoryId,
        Guid userId,
        string type,
        string name,
        string? description,
        IReadOnlyCollection<TerritoryAssetGeoAnchorInput>? geoAnchors,
        CancellationToken cancellationToken,
        string? subtype = null)
    {
        if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(name))
        {
            return Result<TerritoryAssetDetails>.Failure("Type and name are required.");
        }

        if (geoAnchors is null || geoAnchors.Count == 0)
        {
            return Result<TerritoryAssetDetails>.Failure("At least one geoAnchor is required.");
        }

        var normalizedType = NormalizeType(type);
        var normalizedSubtype = NaturalWaterSubtype.Normalize(subtype);
        if (!NaturalWaterSubtype.TryValidate(normalizedType, normalizedSubtype, out var subtypeError))
        {
            return Result<TerritoryAssetDetails>.Failure(subtypeError ?? "Invalid subtype.");
        }

        var anchors = BuildAnchors(geoAnchors);
        if (anchors.Count == 0)
        {
            return Result<TerritoryAssetDetails>.Failure("Invalid geoAnchors.");
        }

        var now = DateTime.UtcNow;
        TerritoryAsset asset;
        try
        {
            asset = new TerritoryAsset(
                Guid.NewGuid(),
                territoryId,
                normalizedType,
                name.Trim(),
                string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
                AssetStatus.Suggested,
                userId,
                now,
                userId,
                now,
                null,
                null,
                null,
                normalizedSubtype);
        }
        catch (ArgumentException ex)
        {
            return Result<TerritoryAssetDetails>.Failure(ex.Message);
        }

        await _assetRepository.AddAsync(asset, cancellationToken);
        await _anchorRepository.AddAsync(anchors.Select(anchor => new AssetGeoAnchor(
            Guid.NewGuid(),
            asset.Id,
            anchor.Latitude,
            anchor.Longitude,
            now)).ToList(), cancellationToken);

        await _auditLogger.LogAsync(
            new AuditEntry("asset.created", userId, territoryId, asset.Id, now),
            cancellationToken);

        // Enfileirar curadoria para Curator (fallback humano).
        var workItem = new WorkItem(
            Guid.NewGuid(),
            WorkItemType.AssetCuration,
            WorkItemStatus.RequiresHumanReview,
            territoryId,
            userId,
            now,
            requiredSystemPermission: null,
            requiredCapability: MembershipCapabilityType.Curator,
            subjectType: "ASSET",
            subjectId: asset.Id,
            payloadJson: $$"""{"assetId":"{{asset.Id}}","territoryId":"{{territoryId}}"}""",
            outcome: WorkItemOutcome.None,
            completedAtUtc: null,
            completedByUserId: null,
            completionNotes: null);

        await _workItemRepository.AddAsync(workItem, cancellationToken);
        await _auditLogger.LogAsync(
            new AuditEntry("work_item.created", userId, territoryId, workItem.Id, now),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        // Invalidar cache de assets do território
        _cacheInvalidation?.InvalidateAssetCache(territoryId, asset.Id);

        var details = await BuildAssetDetailsAsync(territoryId, new[] { asset }, cancellationToken);
        if (details.Count == 0)
        {
            return Result<TerritoryAssetDetails>.Failure("Unable to build asset details.");
        }
        return Result<TerritoryAssetDetails>.Success(details[0]);
    }

    public async Task<Result<TerritoryAssetDetails>> UpdateAsync(
        Guid assetId,
        Guid territoryId,
        Guid userId,
        string type,
        string name,
        string? description,
        IReadOnlyCollection<TerritoryAssetGeoAnchorInput>? geoAnchors,
        CancellationToken cancellationToken,
        string? subtype = null,
        bool subtypeSpecified = false)
    {
        if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(name))
        {
            return Result<TerritoryAssetDetails>.Failure("Type and name are required.");
        }

        if (geoAnchors is null || geoAnchors.Count == 0)
        {
            return Result<TerritoryAssetDetails>.Failure("At least one geoAnchor is required.");
        }

        var asset = await _assetRepository.GetByIdAsync(assetId, cancellationToken);
        if (asset is null || asset.TerritoryId != territoryId)
        {
            return Result<TerritoryAssetDetails>.Failure("Asset not found.");
        }

        var normalizedType = NormalizeType(type);
        var normalizedSubtype = ResolveSubtypeForUpdate(normalizedType, asset.Subtype, subtype, subtypeSpecified);
        if (!NaturalWaterSubtype.TryValidate(normalizedType, normalizedSubtype, out var subtypeError))
        {
            return Result<TerritoryAssetDetails>.Failure(subtypeError ?? "Invalid subtype.");
        }

        var anchors = BuildAnchors(geoAnchors);
        if (anchors.Count == 0)
        {
            return Result<TerritoryAssetDetails>.Failure("Invalid geoAnchors.");
        }

        var now = DateTime.UtcNow;
        try
        {
            asset.UpdateDetails(
                normalizedType,
                name.Trim(),
                string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
                userId,
                now,
                normalizedSubtype);
        }
        catch (ArgumentException ex)
        {
            return Result<TerritoryAssetDetails>.Failure(ex.Message);
        }

        await _assetRepository.UpdateAsync(asset, cancellationToken);
        await _anchorRepository.ReplaceForAssetAsync(asset.Id, anchors.Select(anchor => new AssetGeoAnchor(
            Guid.NewGuid(),
            asset.Id,
            anchor.Latitude,
            anchor.Longitude,
            now)).ToList(), cancellationToken);

        await _auditLogger.LogAsync(
            new AuditEntry("asset.updated", userId, territoryId, asset.Id, now),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        // Invalidar cache de assets do território
        _cacheInvalidation?.InvalidateAssetCache(territoryId, asset.Id);

        var details = await BuildAssetDetailsAsync(territoryId, new[] { asset }, cancellationToken);
        if (details.Count == 0)
        {
            return Result<TerritoryAssetDetails>.Failure("Unable to build asset details.");
        }
        return Result<TerritoryAssetDetails>.Success(details[0]);
    }

    public async Task<Result<TerritoryAssetDetails>> ArchiveAsync(
        Guid assetId,
        Guid territoryId,
        Guid userId,
        string? reason,
        CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByIdAsync(assetId, cancellationToken);
        if (asset is null || asset.TerritoryId != territoryId)
        {
            return Result<TerritoryAssetDetails>.Failure("Asset not found.");
        }

        var now = DateTime.UtcNow;
        asset.Archive(userId, now, string.IsNullOrWhiteSpace(reason) ? null : reason.Trim(), userId, now);
        await _assetRepository.UpdateAsync(asset, cancellationToken);

        await _auditLogger.LogAsync(
            new AuditEntry("asset.archived", userId, territoryId, asset.Id, now),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        // Invalidar cache de assets do território
        _cacheInvalidation?.InvalidateAssetCache(territoryId, asset.Id);

        var details = await BuildAssetDetailsAsync(territoryId, new[] { asset }, cancellationToken);
        if (details.Count == 0)
        {
            return Result<TerritoryAssetDetails>.Failure("Unable to build asset details.");
        }
        return Result<TerritoryAssetDetails>.Success(details[0]);
    }

    public async Task<Result<TerritoryAssetDetails>> CurateAsync(
        Guid assetId,
        Guid territoryId,
        Guid curatorUserId,
        WorkItemOutcome outcome,
        string? notes,
        CancellationToken cancellationToken)
    {
        if (outcome != WorkItemOutcome.Approved && outcome != WorkItemOutcome.Rejected)
        {
            return Result<TerritoryAssetDetails>.Failure("Invalid outcome.");
        }

        var asset = await _assetRepository.GetByIdAsync(assetId, cancellationToken);
        if (asset is null || asset.TerritoryId != territoryId)
        {
            return Result<TerritoryAssetDetails>.Failure("Asset not found.");
        }

        var now = DateTime.UtcNow;
        if (outcome == WorkItemOutcome.Approved)
        {
            asset.Approve(curatorUserId, now);
            await _auditLogger.LogAsync(new AuditEntry("asset.approved", curatorUserId, territoryId, asset.Id, now), cancellationToken);
        }
        else
        {
            asset.Reject(curatorUserId, now, notes);
            await _auditLogger.LogAsync(new AuditEntry("asset.rejected", curatorUserId, territoryId, asset.Id, now), cancellationToken);
        }

        await _assetRepository.UpdateAsync(asset, cancellationToken);

        // Completar work item associado (se existir)
        var workItem = await _workItemRepository.GetLatestOpenBySubjectAsync(
            WorkItemType.AssetCuration,
            "ASSET",
            asset.Id,
            cancellationToken);

        if (workItem is not null)
        {
            workItem.Complete(outcome, curatorUserId, notes, now);
            await _workItemRepository.UpdateAsync(workItem, cancellationToken);
            await _auditLogger.LogAsync(new AuditEntry("work_item.completed", curatorUserId, territoryId, workItem.Id, now), cancellationToken);
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        // Invalidar cache de assets do território após curadoria
        _cacheInvalidation?.InvalidateAssetCache(territoryId, asset.Id);

        var details = await BuildAssetDetailsAsync(territoryId, new[] { asset }, cancellationToken);
        if (details.Count == 0)
        {
            return Result<TerritoryAssetDetails>.Failure("Unable to build asset details.");
        }
        return Result<TerritoryAssetDetails>.Success(details[0]);
    }

    public async Task<Result<TerritoryAssetValidationResult>> ValidateAsync(
        Guid assetId,
        Guid territoryId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByIdAsync(assetId, cancellationToken);
        if (asset is null || asset.TerritoryId != territoryId)
        {
            return Result<TerritoryAssetValidationResult>.Failure("Asset not found.");
        }

        var exists = await _validationRepository.ExistsAsync(assetId, userId, cancellationToken);
        var created = false;
        if (!exists)
        {
            var now = DateTime.UtcNow;
            await _validationRepository.AddAsync(new AssetValidation(assetId, userId, now), cancellationToken);
            await _auditLogger.LogAsync(
                new AuditEntry("asset.validated", userId, territoryId, assetId, now),
                cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            created = true;
        }

        var details = await BuildAssetDetailsAsync(territoryId, new[] { asset }, cancellationToken);
        if (details.Count == 0)
        {
            return Result<TerritoryAssetValidationResult>.Failure("Unable to build asset details.");
        }
        return Result<TerritoryAssetValidationResult>.Success(new TerritoryAssetValidationResult(details[0], created));
    }

    private async Task<IReadOnlyList<TerritoryAssetDetails>> BuildAssetDetailsAsync(
        Guid territoryId,
        IReadOnlyCollection<TerritoryAsset> assets,
        CancellationToken cancellationToken)
    {
        if (assets.Count == 0)
        {
            return Array.Empty<TerritoryAssetDetails>();
        }

        var assetIds = assets.Select(asset => asset.Id).ToList();
        var anchors = await _anchorRepository.ListByAssetIdsAsync(assetIds, cancellationToken);
        var anchorLookup = anchors.GroupBy(anchor => anchor.AssetId)
            .ToDictionary(group => group.Key, group => (IReadOnlyList<AssetGeoAnchor>)group.ToList());

        var validationCounts = await _validationRepository.CountByAssetIdsAsync(assetIds, cancellationToken);
        var residentUserIds = await _membershipRepository.ListResidentUserIdsAsync(territoryId, cancellationToken);
        const int maxInt32 = int.MaxValue;
        var residentCount = residentUserIds.Count;
        var eligibleResidentsCount = residentCount > maxInt32 ? maxInt32 : residentCount;

        return assets.Select(asset => new TerritoryAssetDetails(
            asset,
            anchorLookup.TryGetValue(asset.Id, out var assetAnchors)
                ? assetAnchors
                : Array.Empty<AssetGeoAnchor>(),
            validationCounts.TryGetValue(asset.Id, out var count) ? count : 0,
            eligibleResidentsCount)).ToList();
    }

    private static IReadOnlyCollection<(double Latitude, double Longitude)> BuildAnchors(
        IReadOnlyCollection<TerritoryAssetGeoAnchorInput> geoAnchors)
    {
        const int MaxAnchors = Constants.Posts.MaxAnchors;
        const int Precision = Constants.Posts.GeoAnchorPrecision;

        return geoAnchors
            .Where(anchor => GeoCoordinate.IsValid(anchor.Latitude, anchor.Longitude))
            .Select(anchor => new
            {
                Latitude = Math.Round(anchor.Latitude, Precision, MidpointRounding.AwayFromZero),
                Longitude = Math.Round(anchor.Longitude, Precision, MidpointRounding.AwayFromZero)
            })
            .Distinct()
            .Take(MaxAnchors)
            .Select(anchor => (anchor.Latitude, anchor.Longitude))
            .ToList();
    }

    private static IReadOnlyCollection<string>? NormalizeTypes(IReadOnlyCollection<string>? types)
    {
        if (types is null || types.Count == 0)
        {
            return null;
        }

        return types
            .Select(type => NormalizeType(type))
            .Where(type => !string.IsNullOrWhiteSpace(type))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string NormalizeType(string type)
    {
        return type.Trim().ToLowerInvariant();
    }

    /// <summary>
    /// Omitido → preserva subtype se continuar natural; limpa se type deixar de ser natural.
    /// Presente (inclui null) → aplica valor normalizado.
    /// </summary>
    private static string? ResolveSubtypeForUpdate(
        string normalizedType,
        string? existingSubtype,
        string? requestedSubtype,
        bool subtypeSpecified)
    {
        if (subtypeSpecified)
        {
            return NaturalWaterSubtype.Normalize(requestedSubtype);
        }

        if (string.Equals(normalizedType, NaturalWaterSubtype.NaturalType, StringComparison.Ordinal))
        {
            return existingSubtype;
        }

        return null;
    }
}
