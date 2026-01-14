using Araponga.Application.Interfaces;
using Araponga.Domain.Configuration;

namespace Araponga.Infrastructure.InMemory;

public sealed class InMemorySystemConfigRepository : ISystemConfigRepository
{
    private readonly InMemoryDataStore _dataStore;

    public InMemorySystemConfigRepository(InMemoryDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public Task<SystemConfig?> GetByKeyAsync(string key, CancellationToken cancellationToken)
    {
        var normalizedKey = NormalizeKey(key);
        var config = _dataStore.SystemConfigs.FirstOrDefault(c => c.Key == normalizedKey);
        return Task.FromResult<SystemConfig?>(config);
    }

    public Task<IReadOnlyList<SystemConfig>> ListAsync(SystemConfigCategory? category, CancellationToken cancellationToken)
    {
        var ensured = _dataStore.SystemConfigs.AsEnumerable();
        if (category.HasValue)
        {
            ensured = ensured.Where(c => c.Category == category.Value);
        }

        return Task.FromResult<IReadOnlyList<SystemConfig>>(ensured
            .OrderBy(c => c.Key)
            .ToList());
    }

    public Task UpsertAsync(SystemConfig config, CancellationToken cancellationToken)
    {
        var existingIndex = _dataStore.SystemConfigs.FindIndex(c => c.Key == config.Key);
        if (existingIndex >= 0)
        {
            _dataStore.SystemConfigs[existingIndex] = config;
        }
        else
        {
            _dataStore.SystemConfigs.Add(config);
        }

        return Task.CompletedTask;
    }

    private static string NormalizeKey(string key)
        => string.IsNullOrWhiteSpace(key) ? string.Empty : key.Trim().ToLowerInvariant();
}

