namespace Arah.Core.Application;

using Arah.Core.Domain;

public sealed class CoreDirectoryService
{
    private readonly ICoreDirectoryRegistry _directory;
    private readonly ICoreInstanceRegistry _instances;

    public CoreDirectoryService(ICoreDirectoryRegistry directory, ICoreInstanceRegistry instances)
    {
        _directory = directory;
        _instances = instances;
    }

    public DirectoryTerritoryEntry PublishTerritory(Guid territoryId, Guid instanceId, string name)
    {
        if (_instances.GetById(instanceId) is null)
        {
            throw new InvalidOperationException($"Instance {instanceId} is not registered");
        }

        return _directory.Publish(territoryId, instanceId, name);
    }

    public IReadOnlyList<DirectoryTerritoryEntry> ListTerritories() => _directory.ListAll();
}
