namespace Arah.Core.Application;

using Arah.Core.Domain;

public sealed class CoreInstanceService
{
    private readonly ICoreInstanceRegistry _registry;
    private readonly ICoreAvailabilityCache _cache;

    public CoreInstanceService(ICoreInstanceRegistry registry, ICoreAvailabilityCache cache)
    {
        _registry = registry;
        _cache = cache;
    }

    public InstanceRegistrationResult Register(string mode, Uri baseUrl, string version)
    {
        var registration = _registry.Register(mode, baseUrl, version);
        _cache.RememberRegistration(registration.Instance, registration.InstanceAuthToken);
        return registration;
    }

    public CoreInstance? GetById(Guid id) => _registry.GetById(id);

    public IReadOnlyList<CoreInstance> ListAll() => _registry.ListAll();

    public bool ValidateAuthToken(Guid instanceId, string? token) =>
        _registry.ValidateAuthToken(instanceId, token);

    public void RecordHeartbeat(Guid instanceId, IReadOnlyDictionary<string, string> services, TimeSpan uptime)
    {
        var report = new HealthCheckReport(
            instanceId,
            services,
            uptime,
            DateTimeOffset.UtcNow);
        _registry.RecordHeartbeat(instanceId, report);
        var instance = _registry.GetById(instanceId);
        if (instance is not null)
        {
            _cache.RememberRegistration(instance, instance.InstanceAuthToken);
        }
    }
}
