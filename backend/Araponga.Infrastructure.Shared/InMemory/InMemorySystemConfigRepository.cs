using Araponga.Application.Interfaces;
using Araponga.Domain.Configuration;

namespace Araponga.Infrastructure.Shared.InMemory;

/// <summary>Implementação InMemory de ISystemConfigRepository usando InMemorySharedStore.</summary>
public sealed class InMemorySystemConfigRepository : ISystemConfigRepository
{
    private readonly InMemorySharedStore _store;

    public InMemorySystemConfigRepository(InMemorySharedStore store) => _store = store;

    public Task<SystemConfig?> GetByKeyAsync(string key, CancellationToken cancellationToken)
    {
        var k = NormalizeKey(key);
        return Task.FromResult(_store.SystemConfigs.FirstOrDefault(c => c.Key == k));
    }

    public Task<IReadOnlyList<SystemConfig>> ListAsync(SystemConfigCategory? category, CancellationToken cancellationToken)
    {
        var q = _store.SystemConfigs.AsEnumerable();
        if (category.HasValue) q = q.Where(c => c.Category == category.Value);
        return Task.FromResult<IReadOnlyList<SystemConfig>>(q.OrderBy(c => c.Key).ToList());
    }

    public Task UpsertAsync(SystemConfig config, CancellationToken cancellationToken)
    {
        var i = _store.SystemConfigs.FindIndex(c => c.Key == config.Key);
        if (i >= 0) _store.SystemConfigs[i] = config;
        else _store.SystemConfigs.Add(config);
        return Task.CompletedTask;
    }

    private static string NormalizeKey(string key) => string.IsNullOrWhiteSpace(key) ? string.Empty : key.Trim().ToLowerInvariant();
}
