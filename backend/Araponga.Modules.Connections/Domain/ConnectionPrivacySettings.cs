namespace Araponga.Domain.Connections;

/// <summary>
/// Configurações de privacidade do usuário para o módulo de conexões.
/// </summary>
public sealed class ConnectionPrivacySettings
{
    public Guid UserId { get; }
    public ConnectionRequestPolicy WhoCanAddMe { get; private set; }
    public ConnectionVisibility WhoCanSeeMyConnections { get; private set; }
    public bool ShowConnectionsInProfile { get; private set; }
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }

    private ConnectionPrivacySettings(
        Guid userId,
        ConnectionRequestPolicy whoCanAddMe,
        ConnectionVisibility whoCanSeeMyConnections,
        bool showConnectionsInProfile,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        UserId = userId;
        WhoCanAddMe = whoCanAddMe;
        WhoCanSeeMyConnections = whoCanSeeMyConnections;
        ShowConnectionsInProfile = showConnectionsInProfile;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    public static ConnectionPrivacySettings CreateDefault(Guid userId, DateTime createdAtUtc)
    {
        return new ConnectionPrivacySettings(
            userId,
            ConnectionRequestPolicy.Anyone,
            ConnectionVisibility.MyConnections,
            showConnectionsInProfile: true,
            createdAtUtc,
            updatedAtUtc: createdAtUtc);
    }

    public void Update(
        ConnectionRequestPolicy? whoCanAddMe = null,
        ConnectionVisibility? whoCanSeeMyConnections = null,
        bool? showConnectionsInProfile = null,
        DateTime? updatedAtUtc = null)
    {
        if (whoCanAddMe.HasValue)
            WhoCanAddMe = whoCanAddMe.Value;

        if (whoCanSeeMyConnections.HasValue)
            WhoCanSeeMyConnections = whoCanSeeMyConnections.Value;

        if (showConnectionsInProfile.HasValue)
            ShowConnectionsInProfile = showConnectionsInProfile.Value;

        UpdatedAtUtc = updatedAtUtc ?? DateTime.UtcNow;
    }
}
