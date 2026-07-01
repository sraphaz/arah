namespace Arah.Core.Domain;

public sealed class CoreRelease
{
    public Guid Id { get; }
    public string Version { get; }
    public string Channel { get; }
    public int SchemaVersion { get; }
    public DateTimeOffset PublishedAtUtc { get; }

    public CoreRelease(Guid id, string version, string channel, int schemaVersion, DateTimeOffset publishedAtUtc)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id required", nameof(id));
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);
        if (schemaVersion < 1) throw new ArgumentOutOfRangeException(nameof(schemaVersion));

        Id = id;
        Version = version.Trim();
        Channel = channel.Trim().ToLowerInvariant();
        SchemaVersion = schemaVersion;
        PublishedAtUtc = publishedAtUtc;
    }
}
