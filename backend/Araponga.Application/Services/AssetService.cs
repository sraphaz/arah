using Araponga.Application.Interfaces;
using Araponga.Application.Models;
using Araponga.Domain.Assets;
using Araponga.Domain.Geo;

namespace Araponga.Application.Services;

public sealed class AssetService
{
    private readonly IAssetRepository _assetRepository;
    private readonly IAssetGeoAnchorRepository _anchorRepository;
    private readonly IAssetValidationRepository _validationRepository;
    private readonly ITerritoryMembershipRepository _membershipRepository;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;

    public AssetService(
        IAssetRepository assetRepository,
        IAssetGeoAnchorRepository anchorRepository,
        IAssetValidationRepository validationRepository,
        ITerritoryMembershipRepository membershipRepository,
        IAuditLogger auditLogger,
        IUnitOfWork unitOfWork)
    {
        _assetRepository = assetRepository;
        _anchorRepository = anchorRepository;
        _validationRepository = validationRepository;
        _membershipRepository = membershipRepository;
        _auditLogger = auditLogger;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<AssetDetails>> ListAsync(
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

    public async Task<AssetDetails?> GetByIdAsync(Guid assetId, CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByIdAsync(assetId, cancellationToken);
        if (asset is null)
        {
            return null;
        }

        var details = await BuildAssetDetailsAsync(asset.TerritoryId, new[] { asset }, cancellationToken);
        return details.Count > 0 ? details[0] : null;
    }

    public async Task<(bool success, string? error, AssetDetails? asset)> CreateAsync(
        Guid territoryId,
        Guid userId,
        string type,
        string name,
        string? description,
        IReadOnlyCollection<AssetGeoAnchorInput>? geoAnchors,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(name))
        {
            return (false, "Type and name are required.", null);
        }

        if (geoAnchors is null || geoAnchors.Count == 0)
        {
            return (false, "At least one geoAnchor is required.", null);
        }

        var anchors = BuildAnchors(geoAnchors);
        if (anchors.Count == 0)
        {
            return (false, "Invalid geoAnchors.", null);
        }

        var now = DateTime.UtcNow;
        var asset = new TerritoryAsset(
            Guid.NewGuid(),
            territoryId,
            NormalizeType(type),
            name.Trim(),
            string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            AssetStatus.Active,
            userId,
            now,
            userId,
            now,
            null,
            null,
            null);

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

        await _unitOfWork.CommitAsync(cancellationToken);

        var details = await BuildAssetDetailsAsync(territoryId, new[] { asset }, cancellationToken);
        return (true, null, details[0]);
    }

    public async Task<(bool success, string? error, AssetDetails? asset)> UpdateAsync(
        Guid assetId,
        Guid territoryId,
        Guid userId,
        string type,
        string name,
        string? description,
        IReadOnlyCollection<AssetGeoAnchorInput>? geoAnchors,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(name))
        {
            return (false, "Type and name are required.", null);
        }

        if (geoAnchors is null || geoAnchors.Count == 0)
        {
            return (false, "At least one geoAnchor is required.", null);
        }

        var asset = await _assetRepository.GetByIdAsync(assetId, cancellationToken);
        if (asset is null || asset.TerritoryId != territoryId)
        {
            return (false, "Asset not found.", null);
        }

        var anchors = BuildAnchors(geoAnchors);
        if (anchors.Count == 0)
        {
            return (false, "Invalid geoAnchors.", null);
        }

        var now = DateTime.UtcNow;
        asset.UpdateDetails(
            NormalizeType(type),
            name.Trim(),
            string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            userId,
            now);

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

        var details = await BuildAssetDetailsAsync(territoryId, new[] { asset }, cancellationToken);
        return (true, null, details[0]);
    }

    public async Task<(bool success, string? error, AssetDetails? asset)> ArchiveAsync(
        Guid assetId,
        Guid territoryId,
        Guid userId,
        string? reason,
        CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByIdAsync(assetId, cancellationToken);
        if (asset is null || asset.TerritoryId != territoryId)
        {
            return (false, "Asset not found.", null);
        }

        var now = DateTime.UtcNow;
        asset.Archive(userId, now, string.IsNullOrWhiteSpace(reason) ? null : reason.Trim(), userId, now);
        await _assetRepository.UpdateAsync(asset, cancellationToken);

        await _auditLogger.LogAsync(
            new AuditEntry("asset.archived", userId, territoryId, asset.Id, now),
            cancellationToken);

        await _unitOfWork.CommitAsync(cancellationToken);

        var details = await BuildAssetDetailsAsync(territoryId, new[] { asset }, cancellationToken);
        return (true, null, details[0]);
    }

    public async Task<(bool success, string? error, AssetDetails? asset, bool created)> ValidateAsync(
        Guid assetId,
        Guid territoryId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByIdAsync(assetId, cancellationToken);
        if (asset is null || asset.TerritoryId != territoryId)
        {
            return (false, "Asset not found.", null, false);
        }

        var exists = await _validationRepository.ExistsAsync(assetId, userId, cancellationToken);
        if (!exists)
        {
            var now = DateTime.UtcNow;
            await _validationRepository.AddAsync(new AssetValidation(assetId, userId, now), cancellationToken);
            await _auditLogger.LogAsync(
                new AuditEntry("asset.validated", userId, territoryId, assetId, now),
                cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
        }

        var details = await BuildAssetDetailsAsync(territoryId, new[] { asset }, cancellationToken);
        return (true, null, details[0], !exists);
    }

    private async Task<IReadOnlyList<AssetDetails>> BuildAssetDetailsAsync(
        Guid territoryId,
        IReadOnlyCollection<TerritoryAsset> assets,
        CancellationToken cancellationToken)
    {
        if (assets.Count == 0)
        {
            return Array.Empty<AssetDetails>();
        }

        var assetIds = assets.Select(asset => asset.Id).ToList();
        var anchors = await _anchorRepository.ListByAssetIdsAsync(assetIds, cancellationToken);
        var anchorLookup = anchors.GroupBy(anchor => anchor.AssetId)
            .ToDictionary(group => group.Key, group => (IReadOnlyList<AssetGeoAnchor>)group.ToList());

        var validationCounts = await _validationRepository.CountByAssetIdsAsync(assetIds, cancellationToken);
        var eligibleResidentsCount = (await _membershipRepository.ListResidentUserIdsAsync(territoryId, cancellationToken)).Count;

        return assets.Select(asset => new AssetDetails(
            asset,
            anchorLookup.TryGetValue(asset.Id, out var assetAnchors)
                ? assetAnchors
                : Array.Empty<AssetGeoAnchor>(),
            validationCounts.TryGetValue(asset.Id, out var count) ? count : 0,
            eligibleResidentsCount)).ToList();
    }

    private static IReadOnlyCollection<(double Latitude, double Longitude)> BuildAnchors(
        IReadOnlyCollection<AssetGeoAnchorInput> geoAnchors)
    {
        const int MaxAnchors = 50;
        const int Precision = 5;

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
}
