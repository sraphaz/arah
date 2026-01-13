using Araponga.Application.Common;
using Araponga.Application.Interfaces;
using Araponga.Domain.Marketplace;

namespace Araponga.Application.Services;

public sealed class PlatformFeeService
{
    private readonly IPlatformFeeConfigRepository _configRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PlatformFeeService(IPlatformFeeConfigRepository configRepository, IUnitOfWork unitOfWork)
    {
        _configRepository = configRepository;
        _unitOfWork = unitOfWork;
    }

    public Task<PlatformFeeConfig?> GetActiveFeeConfigAsync(
        Guid territoryId,
        ListingType listingType,
        CancellationToken cancellationToken)
    {
        return _configRepository.GetActiveAsync(territoryId, listingType, cancellationToken);
    }

    public Task<IReadOnlyList<PlatformFeeConfig>> ListActiveAsync(Guid territoryId, CancellationToken cancellationToken)
    {
        return _configRepository.ListActiveAsync(territoryId, cancellationToken);
    }

    public async Task<PagedResult<PlatformFeeConfig>> ListActivePagedAsync(
        Guid territoryId,
        PaginationParameters pagination,
        CancellationToken cancellationToken)
    {
        var configs = await _configRepository.ListActiveAsync(territoryId, cancellationToken);
        var totalCount = configs.Count;
        var pagedItems = configs
            .OrderBy(c => c.ListingType)
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToList();

        return new PagedResult<PlatformFeeConfig>(pagedItems, pagination.PageNumber, pagination.PageSize, totalCount);
    }

    public async Task<PlatformFeeConfig> UpsertFeeConfigAsync(
        Guid territoryId,
        ListingType listingType,
        PlatformFeeMode feeMode,
        decimal feeValue,
        string? currency,
        bool isActive,
        CancellationToken cancellationToken)
    {
        var existing = await _configRepository.GetActiveAsync(territoryId, listingType, cancellationToken);
        var now = DateTime.UtcNow;

        if (existing is null)
        {
            var config = new PlatformFeeConfig(
                Guid.NewGuid(),
                territoryId,
                listingType,
                feeMode,
                feeValue,
                currency,
                isActive,
                now,
                now);

            await _configRepository.AddAsync(config, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);
            return config;
        }

        existing.Update(feeMode, feeValue, currency, isActive, now);
        await _configRepository.UpdateAsync(existing, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);
        return existing;
    }
}
