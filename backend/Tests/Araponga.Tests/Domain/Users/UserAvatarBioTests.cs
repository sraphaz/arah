using Araponga.Domain.Users;
using Xunit;

namespace Araponga.Tests.Domain.Users;

/// <summary>
/// Testes unitários para funcionalidades de avatar e bio do modelo User.
/// </summary>
public sealed class UserAvatarBioTests
{
    [Fact]
    public void UpdateAvatar_WithValidMediaAssetId_UpdatesAvatar()
    {
        var userId = Guid.NewGuid();
        var mediaAssetId = Guid.NewGuid();
        var user = CreateUser(userId);

        user.UpdateAvatar(mediaAssetId);

        Assert.Equal(mediaAssetId, user.AvatarMediaAssetId);
    }

    [Fact]
    public void UpdateAvatar_WithNull_RemovesAvatar()
    {
        var userId = Guid.NewGuid();
        var mediaAssetId = Guid.NewGuid();
        var user = CreateUser(userId);
        user.UpdateAvatar(mediaAssetId);

        user.UpdateAvatar(null);

        Assert.Null(user.AvatarMediaAssetId);
    }

    [Fact]
    public void UpdateBio_WithValidBio_UpdatesBio()
    {
        var userId = Guid.NewGuid();
        var user = CreateUser(userId);
        var bio = "Esta é minha biografia.";

        user.UpdateBio(bio);

        Assert.Equal(bio, user.Bio);
    }

    [Fact]
    public void UpdateBio_WithNull_RemovesBio()
    {
        var userId = Guid.NewGuid();
        var user = CreateUser(userId);
        user.UpdateBio("Bio inicial");

        user.UpdateBio(null);

        Assert.Null(user.Bio);
    }

    [Fact]
    public void UpdateBio_WithEmptyString_RemovesBio()
    {
        var userId = Guid.NewGuid();
        var user = CreateUser(userId);
        user.UpdateBio("Bio inicial");

        user.UpdateBio("");

        Assert.Null(user.Bio);
    }

    [Fact]
    public void UpdateBio_WithWhitespace_RemovesBio()
    {
        var userId = Guid.NewGuid();
        var user = CreateUser(userId);
        user.UpdateBio("Bio inicial");

        user.UpdateBio("   ");

        Assert.Null(user.Bio);
    }

    [Fact]
    public void UpdateBio_WithMaxLength_Accepts()
    {
        var userId = Guid.NewGuid();
        var user = CreateUser(userId);
        var bio = new string('a', 500); // Exatamente 500 caracteres

        user.UpdateBio(bio);

        Assert.Equal(bio, user.Bio);
    }

    [Fact]
    public void UpdateBio_WithExceedsMaxLength_ThrowsArgumentException()
    {
        var userId = Guid.NewGuid();
        var user = CreateUser(userId);
        var bio = new string('a', 501); // Mais de 500 caracteres

        var ex = Assert.Throws<ArgumentException>(() => user.UpdateBio(bio));

        Assert.Contains("500", ex.Message);
    }

    [Fact]
    public void UpdateBio_TrimsWhitespace()
    {
        var userId = Guid.NewGuid();
        var user = CreateUser(userId);
        var bio = "  Minha bio com espaços  ";

        user.UpdateBio(bio);

        Assert.Equal("Minha bio com espaços", user.Bio);
    }

    [Fact]
    public void User_Constructor_WithAvatarAndBio_InitializesCorrectly()
    {
        var userId = Guid.NewGuid();
        var mediaAssetId = Guid.NewGuid();
        var bio = "Biografia inicial";
        var user = new User(
            userId,
            "Test User",
            "test@example.com",
            "111.111.111-11",
            null,
            null,
            null,
            "google",
            "ext-123",
            false,
            null,
            null,
            null,
            UserIdentityVerificationStatus.Unverified,
            null,
            mediaAssetId,
            bio,
            null,
            DateTime.UtcNow);

        Assert.Equal(mediaAssetId, user.AvatarMediaAssetId);
        Assert.Equal(bio, user.Bio);
    }

    private static User CreateUser(Guid userId)
    {
        return new User(
            userId,
            "Test User",
            "test@example.com",
            "111.111.111-11",
            null,
            null,
            null,
            "google",
            "ext-" + userId.ToString("N")[..8],
            DateTime.UtcNow);
    }
}
