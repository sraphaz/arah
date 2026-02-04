using Araponga.Modules.Assets.Application.Interfaces;
using Araponga.Modules.Assets.Domain;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemoryAssetValidationRepository : IAssetValidationRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemoryAssetValidationRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<bool> ExistsAsync(Guid assetId, Guid userId, CancellationToken cancellationToken)
    {
        var exists = _dataStore.AssetValidations.Any(validation =>
            validation.AssetId == assetId && validation.UserId == userId);
        return Task.FromResult(exists);
    }

    public Task AddAsync(AssetValidation validation, CancellationToken cancellationToken)
    {
        _dataStore.AssetValidations.Add(validation);
        return Task.CompletedTask;
    }

    public Task<int> CountByAssetIdAsync(Guid assetId, CancellationToken cancellationToken)
    {
        const int maxInt32 = int.MaxValue;
        var count = _dataStore.AssetValidations.Count(validation => validation.AssetId == assetId);
        return Task.FromResult(count > maxInt32 ? maxInt32 : count);
    }

    public Task<IReadOnlyDictionary<Guid, int>> CountByAssetIdsAsync(
        IReadOnlyCollection<Guid> assetIds,
        CancellationToken cancellationToken)
    {
        const int maxInt32 = int.MaxValue;
        var counts = _dataStore.AssetValidations
            .Where(validation => assetIds.Contains(validation.AssetId))
            .GroupBy(validation => validation.AssetId)
            .ToDictionary(
                group => group.Key,
                group =>
                {
                    var count = group.Count();
                    return count > maxInt32 ? maxInt32 : count;
                });

        return Task.FromResult<IReadOnlyDictionary<Guid, int>>(counts);
    }
}
