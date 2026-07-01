namespace Arah.Core.Application;

using Arah.Core.Domain;

public sealed class InMemoryCoreInstanceRegistry : ICoreInstanceRegistry
{
    private readonly Dictionary<Guid, CoreInstance> _instances = new();

    public CoreInstance Register(string mode, Uri baseUrl, string version)
    {
        var instance = new CoreInstance(Guid.NewGuid(), mode, baseUrl, version);
        _instances[instance.Id] = instance;
        return instance;
    }

    public CoreInstance? GetById(Guid id) =>
        _instances.TryGetValue(id, out var instance) ? instance : null;

    public void RecordHeartbeat(Guid instanceId, HealthCheckReport report)
    {
        if (!_instances.TryGetValue(instanceId, out var instance))
        {
            throw new InvalidOperationException($"Instance {instanceId} not registered");
        }

        instance.MarkOnline(report.ReportedAtUtc);
    }
}
