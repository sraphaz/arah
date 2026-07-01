namespace Arah.Core.Application;

using Arah.Core.Domain;

public interface ICoreReleaseCatalog
{
    IReadOnlyList<CoreRelease> ListByChannel(string channel);
    void Publish(CoreRelease release);
}

public sealed class InMemoryCoreReleaseCatalog : ICoreReleaseCatalog
{
    private readonly List<CoreRelease> _releases = new();

    public IReadOnlyList<CoreRelease> ListByChannel(string channel)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);
        var normalized = channel.Trim().ToLowerInvariant();
        return _releases
            .Where(r => r.Channel == normalized)
            .OrderByDescending(r => r.PublishedAtUtc)
            .ToList();
    }

    public void Publish(CoreRelease release)
    {
        ArgumentNullException.ThrowIfNull(release);
        _releases.Add(release);
    }
}
