namespace Arah.Core.Application;

using Arah.Core.Domain;

/// <summary>
/// Cache local para operação autônoma quando o Core está indisponível (FASE54+).
/// </summary>
public interface ICoreAvailabilityCache
{
    void RememberRegistration(CoreInstance instance, string instanceAuthToken);
    void RememberReleases(IReadOnlyList<CoreRelease> releases, string channel);
    CoreInstance? GetCachedInstance(Guid instanceId);
    IReadOnlyList<CoreRelease> GetCachedReleases(string channel);
}

public sealed class InMemoryCoreAvailabilityCache : ICoreAvailabilityCache
{
    private readonly System.Collections.Concurrent.ConcurrentDictionary<Guid, CoreInstance> _instances = new();
    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, IReadOnlyList<CoreRelease>> _releases = new();

    public void RememberRegistration(CoreInstance instance, string instanceAuthToken)
    {
        ArgumentNullException.ThrowIfNull(instance);
        _instances[instance.Id] = instance;
    }

    public void RememberReleases(IReadOnlyList<CoreRelease> releases, string channel)
    {
        _releases[channel.Trim().ToLowerInvariant()] = releases;
    }

    public CoreInstance? GetCachedInstance(Guid instanceId) =>
        _instances.TryGetValue(instanceId, out var instance) ? instance : null;

    public IReadOnlyList<CoreRelease> GetCachedReleases(string channel)
    {
        var key = channel.Trim().ToLowerInvariant();
        return _releases.TryGetValue(key, out var list) ? list : Array.Empty<CoreRelease>();
    }
}
