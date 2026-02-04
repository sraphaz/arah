using Araponga.Domain.Membership;
using Xunit;

namespace Araponga.Tests.Domain;

public sealed class MembershipSettingsTests
{
    [Fact]
    public void MembershipSettings_RequiresMembershipId()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new MembershipSettings(
                Guid.Empty,
                false,
                DateTime.UtcNow,
                DateTime.UtcNow));

        Assert.Contains("Membership ID", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MembershipSettings_UpdateMarketplaceOptIn_UpdatesValue()
    {
        var settings = new MembershipSettings(
            Guid.NewGuid(),
            false,
            DateTime.UtcNow,
            DateTime.UtcNow);

        var updatedAt = DateTime.UtcNow.AddMinutes(1);
        settings.UpdateMarketplaceOptIn(true, updatedAt);

        Assert.True(settings.MarketplaceOptIn);
        Assert.Equal(updatedAt, settings.UpdatedAtUtc);
    }

    [Fact]
    public void MembershipSettings_UpdateMarketplaceOptIn_UpdatesTimestamp()
    {
        var createdAt = DateTime.UtcNow;
        var settings = new MembershipSettings(
            Guid.NewGuid(),
            false,
            createdAt,
            createdAt);

        var updatedAt = createdAt.AddHours(1);
        settings.UpdateMarketplaceOptIn(true, updatedAt);

        Assert.Equal(updatedAt, settings.UpdatedAtUtc);
        Assert.NotEqual(createdAt, settings.UpdatedAtUtc);
    }
}
