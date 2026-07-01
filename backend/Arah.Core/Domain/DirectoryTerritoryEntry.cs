namespace Arah.Core.Domain;

/// <summary>
/// Território publicado no diretório global do Core (metadados — dados sociais ficam na instância).
/// </summary>
public sealed class DirectoryTerritoryEntry
{
    public Guid TerritoryId { get; }
    public Guid InstanceId { get; }
    public string Name { get; }
    public DateTimeOffset PublishedAtUtc { get; }

    public DirectoryTerritoryEntry(Guid territoryId, Guid instanceId, string name, DateTimeOffset publishedAtUtc)
    {
        if (territoryId == Guid.Empty) throw new ArgumentException("TerritoryId required", nameof(territoryId));
        if (instanceId == Guid.Empty) throw new ArgumentException("InstanceId required", nameof(instanceId));
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        TerritoryId = territoryId;
        InstanceId = instanceId;
        Name = name.Trim();
        PublishedAtUtc = publishedAtUtc;
    }
}
