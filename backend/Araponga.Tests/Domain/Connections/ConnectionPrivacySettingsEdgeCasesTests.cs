using Araponga.Domain.Connections;
using Xunit;

namespace Araponga.Tests.Domain.Connections;

public sealed class ConnectionPrivacySettingsEdgeCasesTests
{
    private static readonly Guid UserId = Guid.NewGuid();

    [Fact]
    public void CreateDefault_SetsExpectedValues()
    {
        var now = DateTime.UtcNow;
        var s = ConnectionPrivacySettings.CreateDefault(UserId, now);

        Assert.Equal(UserId, s.UserId);
        Assert.Equal(ConnectionRequestPolicy.Anyone, s.WhoCanAddMe);
        Assert.Equal(ConnectionVisibility.MyConnections, s.WhoCanSeeMyConnections);
        Assert.True(s.ShowConnectionsInProfile);
        Assert.Equal(now, s.CreatedAtUtc);
        Assert.Equal(now, s.UpdatedAtUtc);
    }

    [Fact]
    public void Update_WithWhoCanAddMe_Updates()
    {
        var now = DateTime.UtcNow;
        var s = ConnectionPrivacySettings.CreateDefault(UserId, now);

        s.Update(whoCanAddMe: ConnectionRequestPolicy.ResidentsOnly);

        Assert.Equal(ConnectionRequestPolicy.ResidentsOnly, s.WhoCanAddMe);
        Assert.Equal(ConnectionVisibility.MyConnections, s.WhoCanSeeMyConnections);
    }

    [Fact]
    public void Update_WithWhoCanSeeMyConnections_Updates()
    {
        var now = DateTime.UtcNow;
        var s = ConnectionPrivacySettings.CreateDefault(UserId, now);

        s.Update(whoCanSeeMyConnections: ConnectionVisibility.OnlyMe);

        Assert.Equal(ConnectionVisibility.OnlyMe, s.WhoCanSeeMyConnections);
    }

    [Fact]
    public void Update_WithShowConnectionsInProfile_Updates()
    {
        var now = DateTime.UtcNow;
        var s = ConnectionPrivacySettings.CreateDefault(UserId, now);

        s.Update(showConnectionsInProfile: false);

        Assert.False(s.ShowConnectionsInProfile);
    }
}
