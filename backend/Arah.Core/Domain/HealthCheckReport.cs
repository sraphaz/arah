namespace Arah.Core.Domain;

public sealed class HealthCheckReport
{
    public Guid InstanceId { get; }
    public IReadOnlyDictionary<string, string> Services { get; }
    public TimeSpan Uptime { get; }
    public DateTimeOffset ReportedAtUtc { get; }

    public HealthCheckReport(
        Guid instanceId,
        IReadOnlyDictionary<string, string> services,
        TimeSpan uptime,
        DateTimeOffset reportedAtUtc)
    {
        if (instanceId == Guid.Empty) throw new ArgumentException("InstanceId required", nameof(instanceId));
        ArgumentNullException.ThrowIfNull(services);

        InstanceId = instanceId;
        Services = services;
        Uptime = uptime;
        ReportedAtUtc = reportedAtUtc;
    }
}
