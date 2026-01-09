using Araponga.Domain.Feed;
using Araponga.Domain.Map;
using Araponga.Domain.Social;
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
            new Territory(Guid.NewGuid(), "", null, TerritoryStatus.Active, "Cidade", "ST", 0, 0, DateTime.UtcNow));

        Assert.Contains("name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Territory_RequiresCity()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new Territory(Guid.NewGuid(), "Território", null, TerritoryStatus.Active, "", "ST", 0, 0, DateTime.UtcNow));

        Assert.Contains("city", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Territory_RequiresState()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new Territory(Guid.NewGuid(), "Território", null, TerritoryStatus.Active, "Cidade", "", 0, 0, DateTime.UtcNow));

        Assert.Contains("state", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void User_RequiresDisplayName()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new User(Guid.NewGuid(), "", "user@araponga.com", "123.456.789-00", null, null, null, "google", "ext", UserRole.Visitor, DateTime.UtcNow));

        Assert.Contains("display", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void User_RequiresCpfOrForeignDocument()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new User(Guid.NewGuid(), "User", null, null, null, null, null, "google", "ext", UserRole.Visitor, DateTime.UtcNow));

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
                UserRole.Visitor,
                DateTime.UtcNow));

        Assert.Contains("either", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void User_RequiresProvider()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new User(Guid.NewGuid(), "User", "user@araponga.com", "123.456.789-00", null, null, null, "", "ext", UserRole.Visitor, DateTime.UtcNow));

        Assert.Contains("provider", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void User_RequiresExternalId()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new User(Guid.NewGuid(), "User", "user@araponga.com", "123.456.789-00", null, null, null, "google", "", UserRole.Visitor, DateTime.UtcNow));

        Assert.Contains("external", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TerritoryMembership_RequiresUserId()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new TerritoryMembership(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), MembershipRole.Visitor, VerificationStatus.Pending, DateTime.UtcNow));

        Assert.Contains("user", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TerritoryMembership_RequiresTerritoryId()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new TerritoryMembership(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, MembershipRole.Visitor, VerificationStatus.Pending, DateTime.UtcNow));

        Assert.Contains("territory", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CommunityPost_RequiresTerritoryId()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new CommunityPost(Guid.NewGuid(), Guid.Empty, "Title", "Content", PostType.General, PostVisibility.Public, DateTime.UtcNow));

        Assert.Contains("territory", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CommunityPost_RequiresTitle()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new CommunityPost(Guid.NewGuid(), Guid.NewGuid(), "", "Content", PostType.General, PostVisibility.Public, DateTime.UtcNow));

        Assert.Contains("title", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CommunityPost_RequiresContent()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new CommunityPost(Guid.NewGuid(), Guid.NewGuid(), "Title", "", PostType.General, PostVisibility.Public, DateTime.UtcNow));

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
            new MapEntity(Guid.NewGuid(), Guid.Empty, "Ponto", "Categoria", MapEntityStatus.Suggested, MapEntityVisibility.Public, 0, DateTime.UtcNow));

        Assert.Contains("territory", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MapEntity_RequiresName()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new MapEntity(Guid.NewGuid(), Guid.NewGuid(), "", "Categoria", MapEntityStatus.Suggested, MapEntityVisibility.Public, 0, DateTime.UtcNow));

        Assert.Contains("name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MapEntity_RequiresCategory()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new MapEntity(Guid.NewGuid(), Guid.NewGuid(), "Ponto", "", MapEntityStatus.Suggested, MapEntityVisibility.Public, 0, DateTime.UtcNow));

        Assert.Contains("category", exception.Message, StringComparison.OrdinalIgnoreCase);
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
