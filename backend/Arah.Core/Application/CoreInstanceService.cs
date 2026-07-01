namespace Arah.Core.Application;

using Arah.Core.Domain;

public sealed class CoreInstanceService
{
    private readonly ICoreInstanceRegistry _registry;

    public CoreInstanceService(ICoreInstanceRegistry registry)
    {
        _registry = registry;
    }

    public InstanceRegistrationResult Register(string mode, Uri baseUrl, string version) =>
        _registry.Register(mode, baseUrl, version);

    public CoreInstance? GetById(Guid id) => _registry.GetById(id);

    public IReadOnlyList<CoreInstance> ListAll() => _registry.ListAll();

    public void RecordHeartbeat(Guid instanceId, IReadOnlyDictionary<string, string> services, TimeSpan uptime)
    {
        var report = new HealthCheckReport(
            instanceId,
            services,
            uptime,
            DateTimeOffset.UtcNow);
        _registry.RecordHeartbeat(instanceId, report);
    }
}
