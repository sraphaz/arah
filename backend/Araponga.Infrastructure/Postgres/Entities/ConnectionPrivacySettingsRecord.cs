namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class ConnectionPrivacySettingsRecord
{
    public Guid UserId { get; set; }
    public int WhoCanAddMe { get; set; } // ConnectionRequestPolicy
    public int WhoCanSeeMyConnections { get; set; } // ConnectionVisibility
    public bool ShowConnectionsInProfile { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
