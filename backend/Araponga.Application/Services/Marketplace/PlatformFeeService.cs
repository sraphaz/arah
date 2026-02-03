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
        ItemType itemType,
        CancellationToken cancellationToken)
    {
        return _configRepository.GetActiveAsync(territoryId, itemType, cancellationToken);
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
        var totalCount = await _configRepository.CountActiveAsync(territoryId, cancellationToken);
        var configs = await _configRepository.ListActivePagedAsync(territoryId, pagination.Skip, pagination.Take, cancellationToken);

        return new PagedResult<PlatformFeeConfig>(configs, pagination.PageNumber, pagination.PageSize, totalCount);
    }

    public async Task<PlatformFeeConfig> UpsertFeeConfigAsync(
        Guid territoryId,
        ItemType itemType,
        PlatformFeeMode feeMode,
        decimal feeValue,
        string? currency,
        bool isActive,
        CancellationToken cancellationToken)
    {
        var existing = await _configRepository.GetActiveAsync(territoryId, itemType, cancellationToken);
        var now = DateTime.UtcNow;

        if (existing is null)
        {
            var config = new PlatformFeeConfig(
                Guid.NewGuid(),
                territoryId,
                itemType,
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
