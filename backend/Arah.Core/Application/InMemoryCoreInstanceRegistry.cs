namespace Arah.Core.Application;

using System.Collections.Concurrent;
using Arah.Core.Domain;

public sealed class InMemoryCoreInstanceRegistry : ICoreInstanceRegistry
{
    private readonly ConcurrentDictionary<Guid, CoreInstance> _instances = new();

    public InstanceRegistrationResult Register(string mode, Uri baseUrl, string version)
    {
        var (publicKeyPem, privateKeyPem) = InstanceKeyPairGenerator.CreateRsa2048();
        var authToken = InstanceAuthTokenGenerator.CreateToken();
        var instance = new CoreInstance(Guid.NewGuid(), mode, baseUrl, version, publicKeyPem);
        instance.SetInstanceAuthToken(authToken);
        _instances[instance.Id] = instance;
        return new InstanceRegistrationResult(instance, privateKeyPem, authToken);
    }

    public bool ValidateAuthToken(Guid instanceId, string? token)
    {
        var instance = GetById(instanceId);
        return instance is not null && instance.ValidateAuthToken(token);
    }

    public CoreInstance? GetById(Guid id) =>
        _instances.TryGetValue(id, out var instance) ? instance : null;

    public IReadOnlyList<CoreInstance> ListAll() =>
        _instances.Values.OrderBy(i => i.RegisteredAtUtc).ToList();

    public void RecordHeartbeat(Guid instanceId, HealthCheckReport report)
    {
        if (!_instances.TryGetValue(instanceId, out var instance))
        {
            throw new InvalidOperationException($"Instance {instanceId} not registered");
        }

        instance.ApplyHeartbeat(report);
    }
}
