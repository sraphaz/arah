namespace Araponga.Modules.Connections.Infrastructure.Postgres.Entities;

public sealed class ConnectionPrivacySettingsRecord
{
    public Guid UserId { get; set; }
    public int WhoCanAddMe { get; set; }
    public int WhoCanSeeMyConnections { get; set; }
    public bool ShowConnectionsInProfile { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
