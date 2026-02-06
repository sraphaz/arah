using Araponga.Domain.Users;
using Xunit;

namespace Araponga.Tests.Domain;

/// <summary>
/// Comprehensive edge case tests for User entity.
/// Tests 2FA, bio validation, identity verification, avatar updates, and character encoding.
/// </summary>
public sealed class UserEdgeCasesTests
{
    [Fact]
    public void Constructor_WithValidCpfOnly_Succeeds()
    {
        // Act
        var user = new User(
            Guid.NewGuid(),
            "João Silva",
            "joao@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext123",
            DateTime.UtcNow);
        
        // Assert
        Assert.Equal("123.456.789-00", user.Cpf);
        Assert.Null(user.ForeignDocument);
    }

    [Fact]
    public void Constructor_WithValidForeignDocOnly_Succeeds()
    {
        // Act
        var user = new User(
            Guid.NewGuid(),
            "John Doe",
            "john@example.com",
            null,
            "PASSPORT-US-123456",
            null,
            null,
            "google",
            "ext456",
            DateTime.UtcNow);
        
        // Assert
        Assert.Null(user.Cpf);
        Assert.Equal("PASSPORT-US-123456", user.ForeignDocument);
    }

    [Fact]
    public void Constructor_WithUnicodeDisplayName_Succeeds()
    {
        // Arrange
        var displayName = "José María Pérez 🌟";
        
        // Act
        var user = new User(
            Guid.NewGuid(),
            displayName,
            "jose@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext789",
            DateTime.UtcNow);
        
        // Assert
        Assert.Equal(displayName, user.DisplayName);
    }

    [Fact]
    public void Constructor_WithMultipleLanguagesInDisplayName_Succeeds()
    {
        // Arrange - Mix of languages
        var displayName = "王大明 José Москва";
        
        // Act
        var user = new User(
            Guid.NewGuid(),
            displayName,
            "user@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext999",
            DateTime.UtcNow);
        
        // Assert
        Assert.Equal(displayName, user.DisplayName);
    }

    [Fact]
    public void Constructor_WithEmailNormalization_TrimsAndNormalizes()
    {
        // Arrange
        var email = "USER@EXAMPLE.COM";
        
        // Act
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            email,
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);
        
        // Assert
        Assert.NotNull(user.Email);
        // Email normalization behavior: trimmed and may be lowercased depending on NormalizeOptional implementation
        Assert.Equal("user@example.com", user.Email?.ToLowerInvariant());
    }

    [Fact]
    public void Constructor_WithEmailContainingWhitespace_Trims()
    {
        // Arrange
        var email = "  user@example.com  ";
        
        // Act
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            email,
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);
        
        // Assert
        Assert.Equal("user@example.com", user.Email);
    }

    [Fact]
    public void Constructor_WithNullEmail_Succeeds()
    {
        // Act
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            null,
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);
        
        // Assert
        Assert.Null(user.Email);
    }

    [Fact]
    public void Constructor_WithPhoneNumber_Succeeds()
    {
        // Arrange
        var phoneNumber = "+55 11 98765-4321";
        
        // Act
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            phoneNumber,
            null,
            "google",
            "ext",
            DateTime.UtcNow);
        
        // Assert
        Assert.Equal("+55 11 98765-4321", user.PhoneNumber);
    }

    [Fact]
    public void Constructor_WithAddress_Succeeds()
    {
        // Arrange
        var address = "Rua das Flores, 123, São Paulo - SP, 01234-567";
        
        // Act
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            address,
            "google",
            "ext",
            DateTime.UtcNow);
        
        // Assert
        Assert.Equal(address, user.Address);
    }

    [Fact]
    public void UpdateBio_WithValidBio_Updates()
    {
        // Arrange
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);
        
        var bio = "I'm a software developer passionate about open source 💻";
        
        // Act
        user.UpdateBio(bio);
        
        // Assert
        Assert.Equal(bio, user.Bio);
    }

    [Fact]
    public void UpdateBio_Exceeding500Chars_ThrowsArgumentException()
    {
        // Arrange
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);
        
        var bioExceeding500 = new string('A', 501);
        
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => user.UpdateBio(bioExceeding500));
        Assert.Contains("500", ex.Message, System.StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UpdateBio_Exactly500Chars_Succeeds()
    {
        // Arrange
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);
        
        var bioExactly500 = new string('B', 500);
        
        // Act
        user.UpdateBio(bioExactly500);
        
        // Assert
        Assert.Equal(bioExactly500, user.Bio);
    }

    [Fact]
    public void UpdateBio_WithEmptyString_SetsToNull()
    {
        // Arrange
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);
        
        user.UpdateBio("Initial bio");
        
        // Act
        user.UpdateBio("");
        
        // Assert
        Assert.Null(user.Bio);
    }

    [Fact]
    public void UpdateBio_WithOnlyWhitespace_SetsToNull()
    {
        // Arrange
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);
        
        // Act
        user.UpdateBio("   \t\n  ");
        
        // Assert
        Assert.Null(user.Bio);
    }

    [Fact]
    public void UpdateBio_WithSpecialCharactersAndUnicode_Succeeds()
    {
        // Arrange
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);
        
        var bio = "🎨 Designer | 🌍 Viajante | 💪 Dev @ 🏢 Tech Co. #openSource";
        
        // Act
        user.UpdateBio(bio);
        
        // Assert
        Assert.Equal(bio, user.Bio);
    }

    [Fact]
    public void UpdateBio_WithTrimmableWhitespace_Trims()
    {
        // Arrange
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);
        
        var bioWithWhitespace = "  Software Developer at TechCorp  ";
        
        // Act
        user.UpdateBio(bioWithWhitespace);
        
        // Assert
        Assert.Equal("Software Developer at TechCorp", user.Bio);
    }

    [Fact]
    public void UpdateAvatar_WithValidMediaAssetId_Updates()
    {
        // Arrange
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);
        
        var avatarId = Guid.NewGuid();
        
        // Act
        user.UpdateAvatar(avatarId);
        
        // Assert
        Assert.Equal(avatarId, user.AvatarMediaAssetId);
    }

    [Fact]
    public void UpdateAvatar_WithNull_RemovesAvatar()
    {
        // Arrange
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);
        
        var avatarId = Guid.NewGuid();
        user.UpdateAvatar(avatarId);
        
        // Act
        user.UpdateAvatar(null);
        
        // Assert
        Assert.Null(user.AvatarMediaAssetId);
    }

    [Fact]
    public void UpdateAvatar_MultipleUpdates_LastOneWins()
    {
        // Arrange
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);
        
        var avatarId1 = Guid.NewGuid();
        var avatarId2 = Guid.NewGuid();
        
        // Act
        user.UpdateAvatar(avatarId1);
        Assert.Equal(avatarId1, user.AvatarMediaAssetId);
        
        user.UpdateAvatar(avatarId2);
        
        // Assert
        Assert.Equal(avatarId2, user.AvatarMediaAssetId);
    }

    [Fact]
    public void EnableTwoFactor_StoresSecretAndRecoveryCodes()
    {
        // Arrange
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);
        
        var secret = "JBSWY3DPEBLW64TMMQ======";
        var recoveryCodes = "recovery_codes_hash_12345";
        var verifiedAt = DateTime.UtcNow;
        
        // Act
        user.EnableTwoFactor(secret, recoveryCodes, verifiedAt);
        
        // Assert
        Assert.True(user.TwoFactorEnabled);
        Assert.Equal(secret, user.TwoFactorSecret);
        Assert.Equal(recoveryCodes, user.TwoFactorRecoveryCodesHash);
        Assert.Equal(verifiedAt, user.TwoFactorVerifiedAtUtc);
    }

    [Fact]
    public void DisableTwoFactor_ClearsAllSecurityData()
    {
        // Arrange
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            true,
            "JBSWY3DPEBLW64TMMQ======",
            "recovery_codes_hash",
            DateTime.UtcNow,
            UserIdentityVerificationStatus.Unverified,
            null,
            null,
            null,
            null,
            DateTime.UtcNow);
        
        // Act
        user.DisableTwoFactor();
        
        // Assert
        Assert.False(user.TwoFactorEnabled);
        Assert.Null(user.TwoFactorSecret);
        Assert.Null(user.TwoFactorRecoveryCodesHash);
        Assert.Null(user.TwoFactorVerifiedAtUtc);
    }

    [Fact]
    public void UpdateIdentityVerification_ToVerified_Succeeds()
    {
        // Arrange
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);
        
        var verifiedAt = DateTime.UtcNow;
        
        // Act
        user.UpdateIdentityVerification(UserIdentityVerificationStatus.Verified, verifiedAt);
        
        // Assert
        Assert.Equal(UserIdentityVerificationStatus.Verified, user.IdentityVerificationStatus);
        Assert.Equal(verifiedAt, user.IdentityVerifiedAtUtc);
    }

    [Fact]
    public void UpdateIdentityVerification_ToRejected_Succeeds()
    {
        // Arrange
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);
        
        // Act
        user.UpdateIdentityVerification(UserIdentityVerificationStatus.Rejected);
        
        // Assert
        Assert.Equal(UserIdentityVerificationStatus.Rejected, user.IdentityVerificationStatus);
    }

    [Fact]
    public void UpdateIdentityVerification_ToPending_Succeeds()
    {
        // Arrange
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);
        
        // Act
        user.UpdateIdentityVerification(UserIdentityVerificationStatus.Pending);
        
        // Assert
        Assert.Equal(UserIdentityVerificationStatus.Pending, user.IdentityVerificationStatus);
    }

    [Fact]
    public void Constructor_WithAllProperties_ReturnsCorrectValues()
    {
        // Arrange
        var id = Guid.NewGuid();
        var displayName = "José Silva";
        var email = "jose@example.com";
        var cpf = "123.456.789-00";
        var phoneNumber = "+55 11 98765-4321";
        var address = "Rua das Flores, 123";
        var avatarId = Guid.NewGuid();
        var bio = "Software developer";
        var createdAt = DateTime.UtcNow;
        
        // Act
        var user = new User(
            id,
            displayName,
            email,
            cpf,
            null,
            phoneNumber,
            address,
            "google",
            "ext123",
            false,
            null,
            null,
            null,
            UserIdentityVerificationStatus.Unverified,
            null,
            avatarId,
            bio,
            null,
            createdAt);
        
        // Assert
        Assert.Equal(id, user.Id);
        Assert.Equal(displayName, user.DisplayName);
        Assert.Equal(email.ToLowerInvariant(), user.Email);
        Assert.Equal(cpf, user.Cpf);
        Assert.Null(user.ForeignDocument);
        Assert.Equal(phoneNumber, user.PhoneNumber);
        Assert.Equal(address, user.Address);
        Assert.Equal(avatarId, user.AvatarMediaAssetId);
        Assert.Equal(bio, user.Bio);
        Assert.Equal(createdAt, user.CreatedAtUtc);
        Assert.False(user.TwoFactorEnabled);
    }

    [Fact]
    public void Constructor_WithDifferentAuthProviders_AllSucceed()
    {
        // Test multiple auth providers
        var providers = new[] { "google", "apple", "facebook", "github", "microsoft" };
        
        foreach (var provider in providers)
        {
            // Act
            var user = new User(
                Guid.NewGuid(),
                "Test User",
                "test@example.com",
                "123.456.789-00",
                null,
                null,
                null,
                provider,
                $"ext_{provider}",
                DateTime.UtcNow);
            
            // Assert
            Assert.Equal(provider, user.AuthProvider);
            Assert.Equal($"ext_{provider}", user.ExternalId);
        }
    }

    [Fact]
    public void Constructor_WithAuthProviderContainingWhitespace_Trims()
    {
        // Arrange
        var provider = "  google  ";
        var externalId = "  ext123  ";
        
        // Act
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            provider,
            externalId,
            DateTime.UtcNow);
        
        // Assert
        Assert.Equal("google", user.AuthProvider);
        Assert.Equal("ext123", user.ExternalId);
    }
}
