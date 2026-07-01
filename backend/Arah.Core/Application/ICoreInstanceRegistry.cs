namespace Arah.Core.Application;

using Arah.Core.Domain;

public interface ICoreInstanceRegistry
{
    CoreInstance Register(string mode, Uri baseUrl, string version);
    CoreInstance? GetById(Guid id);
    void RecordHeartbeat(Guid instanceId, HealthCheckReport report);
}
