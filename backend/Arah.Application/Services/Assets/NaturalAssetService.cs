using Arah.Application.Common;
using Arah.Application.Interfaces;
using Arah.Application.Models;
using Arah.Modules.Assets.Application.Interfaces;
using Arah.Modules.Assets.Domain;

namespace Arah.Application.Services;

public sealed class NaturalAssetService
{
    private readonly INaturalAssetRepository _repository;
    private readonly IAuditLogger _auditLogger;
    private readonly IUnitOfWork _unitOfWork;

    public NaturalAssetService(
        INaturalAssetRepository repository,
        IAuditLogger auditLogger,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _auditLogger = auditLogger;
        _unitOfWork = unitOfWork;
    }

    public Task<NaturalAsset?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => _repository.GetByIdAsync(id, cancellationToken);

    public async Task<PagedResult<NaturalAsset>> ListPagedAsync(
        Guid territoryId,
        NaturalAssetStatus? status,
        IReadOnlyCollection<string>? types,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var total = await _repository.CountAsync(territoryId, status, types, cancellationToken);
        var items = await _repository.ListPagedAsync(
            territoryId,
            status,
            types,
            pagination.Skip,
            pagination.Take,
            cancellationToken);
        return new PagedResult<NaturalAsset>(items, pagination.PageNumber, pagination.PageSize, total);
    }

    public async Task<Result<NaturalAsset>> CreateAsync(
        Guid territoryId,
        Guid userId,
        string type,
        string name,
        string? description,
        double latitude,
        double longitude,
        string? waterType,
        string? potabilityNotes,
        DateTime? lastTestedAtUtc,
        CancellationToken cancellationToken)
    {
        if (territoryId == Guid.Empty)
        {
            return Result<NaturalAsset>.Failure("Territory ID is required.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<NaturalAsset>.Failure("Name is required.");
        }

        if (!NaturalAssetType.TryValidatePointType(type, out _, out var typeError))
        {
            return Result<NaturalAsset>.Failure(typeError ?? "Invalid type.");
        }

        WaterPointDetails waterPoint;
        try
        {
            waterPoint = new WaterPointDetails(latitude, longitude, waterType, potabilityNotes, lastTestedAtUtc);
        }
        catch (Exception ex) when (ex is ArgumentException or ArgumentOutOfRangeException)
        {
            return Result<NaturalAsset>.Failure(ex.Message);
        }

        NaturalAsset asset;
        try
        {
            asset = NaturalAsset.CreatePending(
                territoryId,
                type,
                name,
                description,
                waterPoint,
                userId,
                DateTime.UtcNow);
        }
        catch (ArgumentException ex)
        {
            return Result<NaturalAsset>.Failure(ex.Message);
        }

        await _repository.AddAsync(asset, cancellationToken);
        await _auditLogger.LogAsync(
            new AuditEntry("natural_asset.created", userId, territoryId, asset.Id, asset.CreatedAtUtc),
            cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<NaturalAsset>.Success(asset);
    }

    public async Task<Result<NaturalAsset>> PublishAsync(
        Guid assetId,
        Guid territoryId,
        Guid curatorUserId,
        CancellationToken cancellationToken)
    {
        var asset = await _repository.GetByIdAsync(assetId, cancellationToken);
        if (asset is null || asset.TerritoryId != territoryId)
        {
            return Result<NaturalAsset>.Failure("Natural asset not found.");
        }

        try
        {
            asset.Publish(curatorUserId, DateTime.UtcNow);
        }
        catch (InvalidOperationException ex)
        {
            return Result<NaturalAsset>.Failure(ex.Message);
        }

        var updated = await _repository.UpdateAsync(asset, cancellationToken);
        if (!updated)
        {
            return Result<NaturalAsset>.Failure("Natural asset not found.");
        }

        await _auditLogger.LogAsync(
            new AuditEntry("natural_asset.published", curatorUserId, territoryId, asset.Id, asset.UpdatedAtUtc),
            cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return Result<NaturalAsset>.Success(asset);
    }
}
