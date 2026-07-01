namespace Arah.Core.Domain;

/// <summary>
/// Instância Arah registrada no control plane (FASE53).
/// Dados de território permanecem na instância — Core só metadados operacionais.
/// </summary>
public sealed class CoreInstance
{
    public Guid Id { get; }
    public string Mode { get; }
    public Uri BaseUrl { get; }
    public string Version { get; }
    public CoreInstanceStatus Status { get; private set; }
    public DateTimeOffset RegisteredAtUtc { get; }
    public DateTimeOffset? LastHeartbeatUtc { get; private set; }

    public CoreInstance(Guid id, string mode, Uri baseUrl, string version)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id required", nameof(id));
        ArgumentException.ThrowIfNullOrWhiteSpace(mode);
        ArgumentNullException.ThrowIfNull(baseUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(version);

        Id = id;
        Mode = mode.Trim();
        BaseUrl = baseUrl;
        Version = version.Trim();
        Status = CoreInstanceStatus.Pending;
        RegisteredAtUtc = DateTimeOffset.UtcNow;
    }

    public void MarkOnline(DateTimeOffset heartbeatUtc)
    {
        LastHeartbeatUtc = heartbeatUtc;
        Status = CoreInstanceStatus.Online;
    }

    public void MarkOffline()
    {
        Status = CoreInstanceStatus.Offline;
    }
}

public enum CoreInstanceStatus
{
    Pending,
    Online,
    Offline
}
