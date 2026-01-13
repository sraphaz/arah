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
