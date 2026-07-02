namespace Arah.Core.Application;

using Arah.Core.Domain;

public sealed class CoreReleaseService
{
    private readonly ICoreReleaseCatalog _catalog;
    private readonly ICoreAvailabilityCache _cache;

    public CoreReleaseService(ICoreReleaseCatalog catalog, ICoreAvailabilityCache cache)
    {
        _catalog = catalog;
        _cache = cache;
    }

    public CoreRelease Publish(string version, string channel, int schemaVersion)
    {
        var release = new CoreRelease(Guid.NewGuid(), version, channel, schemaVersion, DateTimeOffset.UtcNow);
        _catalog.Publish(release);
        RememberChannel(channel);
        return release;
    }

    public IReadOnlyList<CoreRelease> ListByChannel(string channel)
    {
        var list = _catalog.ListByChannel(channel);
        _cache.RememberReleases(list, channel);
        return list;
    }

    private void RememberChannel(string channel) =>
        _cache.RememberReleases(_catalog.ListByChannel(channel), channel);
}
