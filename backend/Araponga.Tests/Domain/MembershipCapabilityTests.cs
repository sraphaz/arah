using Araponga.Domain.Membership;
using Xunit;

namespace Araponga.Tests.Domain;

public sealed class MembershipCapabilityTests
{
    [Fact]
    public void MembershipCapability_RequiresId()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new MembershipCapability(
                Guid.Empty,
                Guid.NewGuid(),
                MembershipCapabilityType.Curator,
                DateTime.UtcNow,
                null,
                null,
                null));

        Assert.Contains("ID", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MembershipCapability_RequiresMembershipId()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new MembershipCapability(
                Guid.NewGuid(),
                Guid.Empty,
                MembershipCapabilityType.Curator,
                DateTime.UtcNow,
                null,
                null,
                null));

        Assert.Contains("Membership ID", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MembershipCapability_IsActive_WhenNotRevoked()
    {
        var capability = new MembershipCapability(
            Guid.NewGuid(),
            Guid.NewGuid(),
            MembershipCapabilityType.Curator,
            DateTime.UtcNow,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test reason");

        Assert.True(capability.IsActive());
    }

    [Fact]
    public void MembershipCapability_IsNotActive_WhenRevoked()
    {
        var capability = new MembershipCapability(
            Guid.NewGuid(),
            Guid.NewGuid(),
            MembershipCapabilityType.Curator,
            DateTime.UtcNow,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test reason");

        capability.Revoke(DateTime.UtcNow);

        Assert.False(capability.IsActive());
        Assert.NotNull(capability.RevokedAtUtc);
    }

    [Fact]
    public void MembershipCapability_Revoke_SetsRevokedAt()
    {
        var capability = new MembershipCapability(
            Guid.NewGuid(),
            Guid.NewGuid(),
            MembershipCapabilityType.Moderator,
            DateTime.UtcNow,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test reason");

        var revokedAt = DateTime.UtcNow;
        capability.Revoke(revokedAt);

        Assert.False(capability.IsActive());
        Assert.Equal(revokedAt, capability.RevokedAtUtc);
    }

    [Fact]
    public void MembershipCapability_Revoke_Throws_WhenAlreadyRevoked()
    {
        var capability = new MembershipCapability(
            Guid.NewGuid(),
            Guid.NewGuid(),
            MembershipCapabilityType.Curator,
            DateTime.UtcNow,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test reason");

        capability.Revoke(DateTime.UtcNow);

        var exception = Assert.Throws<InvalidOperationException>(() =>
            capability.Revoke(DateTime.UtcNow));

        Assert.Contains("already revoked", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MembershipCapability_NormalizesReason()
    {
        var capability = new MembershipCapability(
            Guid.NewGuid(),
            Guid.NewGuid(),
            MembershipCapabilityType.Curator,
            DateTime.UtcNow,
            null,
            null,
            "  Test reason with spaces  ");

        Assert.Equal("Test reason with spaces", capability.Reason);
    }

    [Fact]
    public void MembershipCapability_NullReason_WhenEmpty()
    {
        var capability = new MembershipCapability(
            Guid.NewGuid(),
            Guid.NewGuid(),
            MembershipCapabilityType.Curator,
            DateTime.UtcNow,
            null,
            null,
            "   ");

        Assert.Null(capability.Reason);
    }

    [Fact]
    public void MembershipCapability_Throws_WhenReasonExceedsMaxLength()
    {
        var longReason = new string('a', 501); // 501 caracteres, excede o limite de 500

        var exception = Assert.Throws<ArgumentException>(() =>
            new MembershipCapability(
                Guid.NewGuid(),
                Guid.NewGuid(),
                MembershipCapabilityType.Curator,
                DateTime.UtcNow,
                null,
                null,
                longReason));

        Assert.Contains("500", exception.Message);
    }

    [Fact]
    public void MembershipCapability_AcceptsReason_AtMaxLength()
    {
        var maxReason = new string('a', 500); // Exatamente 500 caracteres

        var capability = new MembershipCapability(
            Guid.NewGuid(),
            Guid.NewGuid(),
            MembershipCapabilityType.Curator,
            DateTime.UtcNow,
            null,
            null,
            maxReason);

        Assert.NotNull(capability.Reason);
        Assert.Equal(500, capability.Reason!.Length);
    }
}
