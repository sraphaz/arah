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
    public string PublicKeyPem { get; }
    public string InstanceAuthToken { get; private set; } = string.Empty;
    public CoreInstanceStatus Status { get; private set; }
    public DateTimeOffset RegisteredAtUtc { get; }
    public DateTimeOffset? LastHeartbeatUtc { get; private set; }
    public IReadOnlyDictionary<string, string>? LastServices { get; private set; }
    public TimeSpan? LastUptime { get; private set; }

    public CoreInstance(Guid id, string mode, Uri baseUrl, string version, string publicKeyPem)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id required", nameof(id));
        ArgumentException.ThrowIfNullOrWhiteSpace(mode);
        ArgumentNullException.ThrowIfNull(baseUrl);
        ArgumentException.ThrowIfNullOrWhiteSpace(version);
        ArgumentException.ThrowIfNullOrWhiteSpace(publicKeyPem);

        Id = id;
        Mode = mode.Trim();
        BaseUrl = baseUrl;
        Version = version.Trim();
        PublicKeyPem = publicKeyPem;
        Status = CoreInstanceStatus.Pending;
        RegisteredAtUtc = DateTimeOffset.UtcNow;
    }

    public void SetInstanceAuthToken(string token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);
        InstanceAuthToken = token;
    }

    public bool ValidateAuthToken(string? token) =>
        !string.IsNullOrWhiteSpace(token) &&
        !string.IsNullOrWhiteSpace(InstanceAuthToken) &&
        string.Equals(InstanceAuthToken, token, StringComparison.Ordinal);

    public void ApplyHeartbeat(HealthCheckReport report)
    {
        ArgumentNullException.ThrowIfNull(report);
        LastHeartbeatUtc = report.ReportedAtUtc;
        LastServices = report.Services;
        LastUptime = report.Uptime;
        Status = CoreInstanceStatus.Online;
    }

    public void MarkOnline(DateTimeOffset heartbeatUtc) =>
        ApplyHeartbeat(new HealthCheckReport(Id, new Dictionary<string, string>(), TimeSpan.Zero, heartbeatUtc));

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
