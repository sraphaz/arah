using Araponga.Domain.Membership;
using Xunit;

namespace Araponga.Tests.Domain.Membership;

/// <summary>
/// Edge case tests for TerritoryMembership domain entity,
/// focusing on verification flags, role updates, and branch coverage.
/// </summary>
public sealed class TerritoryMembershipEdgeCasesTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid TerritoryId = Guid.NewGuid();
    private static readonly DateTime CreatedAt = DateTime.UtcNow;

    [Fact]
    public void UpdateRole_FromVisitorToResident_UpdatesSuccessfully()
    {
        var m = new TerritoryMembership(
            Guid.NewGuid(),
            UserId,
            TerritoryId,
            MembershipRole.Visitor,
            ResidencyVerification.None,
            null,
            null,
            CreatedAt);

        m.UpdateRole(MembershipRole.Resident);

        Assert.Equal(MembershipRole.Resident, m.Role);
    }

    [Fact]
    public void UpdateRole_FromResidentToVisitor_UpdatesSuccessfully()
    {
        var m = new TerritoryMembership(
            Guid.NewGuid(),
            UserId,
            TerritoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            CreatedAt);

        m.UpdateRole(MembershipRole.Visitor);

        Assert.Equal(MembershipRole.Visitor, m.Role);
    }

    [Fact]
    public void AddGeoVerification_SetsFlagAndTimestamp()
    {
        var m = new TerritoryMembership(
            Guid.NewGuid(),
            UserId,
            TerritoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            CreatedAt);
        var at = DateTime.UtcNow.AddMinutes(1);

        m.AddGeoVerification(at);

        Assert.True(m.IsGeoVerified());
        Assert.True(m.HasAnyVerification());
        Assert.Equal(at, m.LastGeoVerifiedAtUtc);
    }

    [Fact]
    public void AddDocumentVerification_SetsFlagAndTimestamp()
    {
        var m = new TerritoryMembership(
            Guid.NewGuid(),
            UserId,
            TerritoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            CreatedAt);
        var at = DateTime.UtcNow.AddMinutes(1);

        m.AddDocumentVerification(at);

        Assert.True(m.IsDocumentVerified());
        Assert.True(m.HasAnyVerification());
        Assert.Equal(at, m.LastDocumentVerifiedAtUtc);
    }

    [Fact]
    public void AddGeoVerification_ThenRemoveGeoVerification_ClearsFlag()
    {
        var m = new TerritoryMembership(
            Guid.NewGuid(),
            UserId,
            TerritoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            CreatedAt);
        m.AddGeoVerification(DateTime.UtcNow);

        m.RemoveGeoVerification();

        Assert.False(m.IsGeoVerified());
        Assert.Null(m.LastGeoVerifiedAtUtc);
    }

    [Fact]
    public void AddDocumentVerification_ThenRemoveDocumentVerification_ClearsFlag()
    {
        var m = new TerritoryMembership(
            Guid.NewGuid(),
            UserId,
            TerritoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            CreatedAt);
        m.AddDocumentVerification(DateTime.UtcNow);

        m.RemoveDocumentVerification();

        Assert.False(m.IsDocumentVerified());
        Assert.Null(m.LastDocumentVerifiedAtUtc);
    }

    [Fact]
    public void HasAnyVerification_WhenNone_ReturnsFalse()
    {
        var m = new TerritoryMembership(
            Guid.NewGuid(),
            UserId,
            TerritoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            CreatedAt);

        Assert.False(m.HasAnyVerification());
    }

    [Fact]
    public void HasAnyVerification_WhenGeoVerified_ReturnsTrue()
    {
        var m = new TerritoryMembership(
            Guid.NewGuid(),
            UserId,
            TerritoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            CreatedAt);
        m.AddGeoVerification(DateTime.UtcNow);

        Assert.True(m.HasAnyVerification());
    }

    [Fact]
    public void IsGeoVerified_WhenNone_ReturnsFalse()
    {
        var m = new TerritoryMembership(
            Guid.NewGuid(),
            UserId,
            TerritoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            CreatedAt);

        Assert.False(m.IsGeoVerified());
    }

    [Fact]
    public void IsDocumentVerified_WhenNone_ReturnsFalse()
    {
        var m = new TerritoryMembership(
            Guid.NewGuid(),
            UserId,
            TerritoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            CreatedAt);

        Assert.False(m.IsDocumentVerified());
    }

    [Fact]
    public void UpdateResidencyVerification_ToGeoVerified_UpdatesState()
    {
        var m = new TerritoryMembership(
            Guid.NewGuid(),
            UserId,
            TerritoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            CreatedAt);

        m.UpdateResidencyVerification(ResidencyVerification.GeoVerified);

        Assert.Equal(ResidencyVerification.GeoVerified, m.ResidencyVerification);
    }

    [Fact]
    public void AddBothVerifications_ThenRemoveOne_OtherRemains()
    {
        var m = new TerritoryMembership(
            Guid.NewGuid(),
            UserId,
            TerritoryId,
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            CreatedAt);
        m.AddGeoVerification(DateTime.UtcNow);
        m.AddDocumentVerification(DateTime.UtcNow);

        m.RemoveGeoVerification();

        Assert.False(m.IsGeoVerified());
        Assert.True(m.IsDocumentVerified());
        Assert.True(m.HasAnyVerification());
    }
}
