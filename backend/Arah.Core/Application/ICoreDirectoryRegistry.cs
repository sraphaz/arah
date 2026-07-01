namespace Arah.Core.Application;

using Arah.Core.Domain;

public interface ICoreDirectoryRegistry
{
    DirectoryTerritoryEntry Publish(Guid territoryId, Guid instanceId, string name);
    DirectoryTerritoryEntry? GetByTerritoryId(Guid territoryId);
    IReadOnlyList<DirectoryTerritoryEntry> ListAll();
}

public sealed class InMemoryCoreDirectoryRegistry : ICoreDirectoryRegistry
{
    private readonly System.Collections.Concurrent.ConcurrentDictionary<Guid, DirectoryTerritoryEntry> _entries = new();

    public DirectoryTerritoryEntry Publish(Guid territoryId, Guid instanceId, string name)
    {
        var entry = new DirectoryTerritoryEntry(territoryId, instanceId, name, DateTimeOffset.UtcNow);
        _entries[territoryId] = entry;
        return entry;
    }

    public DirectoryTerritoryEntry? GetByTerritoryId(Guid territoryId) =>
        _entries.TryGetValue(territoryId, out var entry) ? entry : null;

    public IReadOnlyList<DirectoryTerritoryEntry> ListAll() =>
        _entries.Values.OrderBy(e => e.PublishedAtUtc).ToList();
}
