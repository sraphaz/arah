using Araponga.Domain.Feed;
using Araponga.Domain.Map;
using Araponga.Domain.Moderation;
using Araponga.Domain.Membership;
using Araponga.Domain.Territories;
using Araponga.Domain.Users;
using Xunit;

namespace Araponga.Tests.Domain;

public sealed class DomainValidationTests
{
    [Fact]
    public void Territory_RequiresName()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new Territory(Guid.NewGuid(), null, "", null, TerritoryStatus.Active, "Cidade", "ST", 0, 0, DateTime.UtcNow));

        Assert.Contains("name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Territory_RequiresCity()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new Territory(Guid.NewGuid(), null, "Território", null, TerritoryStatus.Active, "", "ST", 0, 0, DateTime.UtcNow));

        Assert.Contains("city", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Territory_RequiresState()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new Territory(Guid.NewGuid(), null, "Território", null, TerritoryStatus.Active, "Cidade", "", 0, 0, DateTime.UtcNow));

        Assert.Contains("state", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void User_RequiresDisplayName()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new User(Guid.NewGuid(), "", "user@araponga.com", "123.456.789-00", null, null, null, "google", "ext", DateTime.UtcNow));

        Assert.Contains("display", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void User_RequiresCpfOrForeignDocument()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new User(Guid.NewGuid(), "User", null, null, null, null, null, "google", "ext", DateTime.UtcNow));

        Assert.Contains("cpf", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void User_DoesNotAllowCpfAndForeignDocumentTogether()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new User(
                Guid.NewGuid(),
                "User",
                "user@araponga.com",
                "123.456.789-00",
                "PASS-123",
                null,
                null,
                "google",
                "ext",
                DateTime.UtcNow));

        Assert.Contains("either", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void User_RequiresAuthProvider()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new User(Guid.NewGuid(), "User", "user@araponga.com", "123.456.789-00", null, null, null, "", "ext", DateTime.UtcNow));

        Assert.Contains("auth provider", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void User_RequiresExternalId()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new User(Guid.NewGuid(), "User", "user@araponga.com", "123.456.789-00", null, null, null, "google", "", DateTime.UtcNow));

        Assert.Contains("external", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TerritoryMembership_RequiresUserId()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new TerritoryMembership(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), MembershipRole.Visitor, ResidencyVerification.None, null, null, DateTime.UtcNow));

        Assert.Contains("user", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TerritoryMembership_RequiresTerritoryId()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new TerritoryMembership(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, MembershipRole.Visitor, ResidencyVerification.None, null, null, DateTime.UtcNow));

        Assert.Contains("territory", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TerritoryMembership_ResidencyVerification_Initialized_AsUnverified()
    {
        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            MembershipRole.Visitor,
            ResidencyVerification.None,
            null,
            null,
            DateTime.UtcNow);

        Assert.Equal(ResidencyVerification.None, membership.ResidencyVerification);
    }

    [Fact]
    public void TerritoryMembership_UpdateResidencyVerification_ChangesState()
    {
        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            DateTime.UtcNow);

        membership.UpdateResidencyVerification(ResidencyVerification.GeoVerified);

        Assert.Equal(ResidencyVerification.GeoVerified, membership.ResidencyVerification);
    }

    [Fact]
    public void TerritoryMembership_UpdateGeoVerification_SetsTimestamp()
    {
        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            DateTime.UtcNow);

        var verifiedAt = DateTime.UtcNow;
        membership.AddGeoVerification(verifiedAt);

        Assert.True(membership.IsGeoVerified());
        Assert.NotNull(membership.LastGeoVerifiedAtUtc);
        Assert.Equal(verifiedAt, membership.LastGeoVerifiedAtUtc!.Value, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void TerritoryMembership_UpdateDocumentVerification_SetsTimestamp()
    {
        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            MembershipRole.Resident,
            ResidencyVerification.None,
            null,
            null,
            DateTime.UtcNow);

        var verifiedAt = DateTime.UtcNow;
        membership.AddDocumentVerification(verifiedAt);

        Assert.True(membership.IsDocumentVerified());
        Assert.NotNull(membership.LastDocumentVerifiedAtUtc);
        Assert.Equal(verifiedAt, membership.LastDocumentVerifiedAtUtc!.Value, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void TerritoryMembership_DocumentVerified_HasHighestPriority()
    {
        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            MembershipRole.Resident,
            ResidencyVerification.GeoVerified,
            DateTime.UtcNow.AddDays(-1),
            null,
            DateTime.UtcNow);

        var verifiedAt = DateTime.UtcNow;
        membership.AddDocumentVerification(verifiedAt);

        Assert.True(membership.IsDocumentVerified());
        Assert.NotNull(membership.LastDocumentVerifiedAtUtc);
    }

    [Fact]
    public void User_EnableTwoFactor_SetsProperties()
    {
        var user = new User(
            Guid.NewGuid(),
            "User",
            "user@araponga.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);

        var secret = "SECRET123";
        var recoveryCodesHash = "HASH123";
        var verifiedAt = DateTime.UtcNow;

        user.EnableTwoFactor(secret, recoveryCodesHash, verifiedAt);

        Assert.True(user.TwoFactorEnabled);
        Assert.Equal(secret, user.TwoFactorSecret);
        Assert.Equal(recoveryCodesHash, user.TwoFactorRecoveryCodesHash);
        Assert.Equal(verifiedAt, user.TwoFactorVerifiedAtUtc);
    }

    [Fact]
    public void User_DisableTwoFactor_ClearsSecrets()
    {
        var user = new User(
            Guid.NewGuid(),
            "User",
            "user@araponga.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);

        user.EnableTwoFactor("SECRET", "HASH", DateTime.UtcNow);
        user.DisableTwoFactor();

        Assert.False(user.TwoFactorEnabled);
        Assert.Null(user.TwoFactorSecret);
        Assert.Null(user.TwoFactorRecoveryCodesHash);
        Assert.Null(user.TwoFactorVerifiedAtUtc);
    }

    [Fact]
    public void User_UpdateIdentityVerification_UpdatesStatus()
    {
        var user = new User(
            Guid.NewGuid(),
            "User",
            "user@araponga.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);

        Assert.Equal(UserIdentityVerificationStatus.Unverified, user.IdentityVerificationStatus);
        Assert.Null(user.IdentityVerifiedAtUtc);

        var verifiedAt = DateTime.UtcNow;
        user.UpdateIdentityVerification(UserIdentityVerificationStatus.Verified, verifiedAt);

        Assert.Equal(UserIdentityVerificationStatus.Verified, user.IdentityVerificationStatus);
        Assert.Equal(verifiedAt, user.IdentityVerifiedAtUtc);
    }

    [Fact]
    public void User_UpdateIdentityVerification_WithoutTimestamp()
    {
        var user = new User(
            Guid.NewGuid(),
            "User",
            "user@araponga.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);

        user.UpdateIdentityVerification(UserIdentityVerificationStatus.Pending);

        Assert.Equal(UserIdentityVerificationStatus.Pending, user.IdentityVerificationStatus);
        Assert.Null(user.IdentityVerifiedAtUtc);
    }

    [Fact]
    public void CommunityPost_RequiresTerritoryId()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new CommunityPost(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), "Title", "Content", PostType.General, PostVisibility.Public, PostStatus.Published, null, DateTime.UtcNow));

        Assert.Contains("territory", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CommunityPost_RequiresTitle()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new CommunityPost(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "", "Content", PostType.General, PostVisibility.Public, PostStatus.Published, null, DateTime.UtcNow));

        Assert.Contains("title", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CommunityPost_RequiresContent()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new CommunityPost(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Title", "", PostType.General, PostVisibility.Public, PostStatus.Published, null, DateTime.UtcNow));

        Assert.Contains("content", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void PostComment_RequiresPostId()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new PostComment(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), "Comentário", DateTime.UtcNow));

        Assert.Contains("post", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void PostComment_RequiresUserId()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new PostComment(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, "Comentário", DateTime.UtcNow));

        Assert.Contains("user", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void PostComment_RequiresContent()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new PostComment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "", DateTime.UtcNow));

        Assert.Contains("content", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MapEntity_RequiresTerritoryId()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new MapEntity(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), "Ponto", "espaço natural", -23.0, -45.0, MapEntityStatus.Suggested, MapEntityVisibility.Public, 0, DateTime.UtcNow));

        Assert.Contains("territory", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MapEntity_RequiresName()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new MapEntity(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "", "espaço natural", -23.0, -45.0, MapEntityStatus.Suggested, MapEntityVisibility.Public, 0, DateTime.UtcNow));

        Assert.Contains("name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MapEntity_RequiresCategory()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new MapEntity(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Ponto", "", -23.0, -45.0, MapEntityStatus.Suggested, MapEntityVisibility.Public, 0, DateTime.UtcNow));

        Assert.Contains("category", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ModerationReport_RequiresReporterAndReason()
    {
        var missingReporter = Assert.Throws<ArgumentException>(() =>
            new ModerationReport(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), ReportTargetType.Post, Guid.NewGuid(), "SPAM", null, ReportStatus.Open, DateTime.UtcNow));

        Assert.Contains("reporter", missingReporter.Message, StringComparison.OrdinalIgnoreCase);

        var missingReason = Assert.Throws<ArgumentException>(() =>
            new ModerationReport(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), ReportTargetType.Post, Guid.NewGuid(), "", null, ReportStatus.Open, DateTime.UtcNow));

        Assert.Contains("reason", missingReason.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UserBlock_RejectsSelfBlock()
    {
        var userId = Guid.NewGuid();
        var exception = Assert.Throws<ArgumentException>(() =>
            new UserBlock(userId, userId, DateTime.UtcNow));

        Assert.Contains("block", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MapEntityRelation_RequiresUserAndEntity()
    {
        var missingUser = Assert.Throws<ArgumentException>(() =>
            new MapEntityRelation(Guid.Empty, Guid.NewGuid(), DateTime.UtcNow));

        Assert.Contains("user", missingUser.Message, StringComparison.OrdinalIgnoreCase);

        var missingEntity = Assert.Throws<ArgumentException>(() =>
            new MapEntityRelation(Guid.NewGuid(), Guid.Empty, DateTime.UtcNow));

        Assert.Contains("entity", missingEntity.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void HealthAlert_RequiresTerritory()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new Araponga.Domain.Health.HealthAlert(
                Guid.NewGuid(),
                Guid.Empty,
                Guid.NewGuid(),
                "Título",
                "Descrição",
                Araponga.Domain.Health.HealthAlertStatus.Pending,
                DateTime.UtcNow));

        Assert.Contains("territory", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void HealthAlert_RequiresReporter()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new Araponga.Domain.Health.HealthAlert(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.Empty,
                "Título",
                "Descrição",
                Araponga.Domain.Health.HealthAlertStatus.Pending,
                DateTime.UtcNow));

        Assert.Contains("reporter", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void HealthAlert_UpdatesStatus()
    {
        var alert = new Araponga.Domain.Health.HealthAlert(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Título",
            "Descrição",
            Araponga.Domain.Health.HealthAlertStatus.Pending,
            DateTime.UtcNow);

        alert.UpdateStatus(Araponga.Domain.Health.HealthAlertStatus.Validated);

        Assert.Equal(Araponga.Domain.Health.HealthAlertStatus.Validated, alert.Status);
    }
}
