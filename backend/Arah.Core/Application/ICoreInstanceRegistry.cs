namespace Arah.Core.Application;

using Arah.Core.Domain;

public interface ICoreInstanceRegistry
{
    InstanceRegistrationResult Register(string mode, Uri baseUrl, string version);
    CoreInstance? GetById(Guid id);
    IReadOnlyList<CoreInstance> ListAll();
    void RecordHeartbeat(Guid instanceId, HealthCheckReport report);
}
