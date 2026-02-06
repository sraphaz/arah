using Araponga.Application.Services;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Security;
using Microsoft.Extensions.Options;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Testes para SignUpWithPasswordAsync e LoginWithPasswordAsync (cadastro e login com e-mail/senha).
/// </summary>
public sealed class AuthServicePasswordTests
{
    private static (AuthService service, InMemoryUserRepository userRepository) CreateService()
    {
        var sharedStore = new InMemorySharedStore();
        var userRepository = new InMemoryUserRepository(sharedStore);
        var jwtOptions = Options.Create(new JwtOptions
        {
            Issuer = "Araponga",
            Audience = "Araponga",
            SigningKey = "test-secret-key-32-chars-long!",
            ExpirationMinutes = 60
        });
        var tokenService = new JwtTokenService(jwtOptions);
        var refreshTokenStore = new InMemoryRefreshTokenStore();
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new AuthService(userRepository, tokenService, refreshTokenStore, unitOfWork);
        return (service, userRepository);
    }

    [Fact]
    public async Task SignUpWithPasswordAsync_Success_ReturnsTokensAndStoresUserWithPasswordHash()
    {
        var (service, userRepository) = CreateService();

        var result = await service.SignUpWithPasswordAsync(
            "new@example.com",
            "New User",
            "password123",
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        var (user, accessToken, refreshToken) = result.Value!;
        Assert.NotNull(accessToken);
        Assert.NotNull(refreshToken);
        Assert.Equal("New User", user.DisplayName);
        Assert.Equal("new@example.com", user.Email);
        Assert.NotNull(user.PasswordHash);
        Assert.NotEmpty(user.PasswordHash);

        var stored = await userRepository.GetByEmailAsync("new@example.com", CancellationToken.None);
        Assert.NotNull(stored);
        Assert.NotNull(stored.PasswordHash);
    }

    [Fact]
    public async Task SignUpWithPasswordAsync_Fails_WhenEmailAlreadyExists()
    {
        var (service, userRepository) = CreateService();
        var existing = new User(
            Guid.NewGuid(),
            "Existing",
            "existing@example.com",
            null,
            "dev",
            null,
            null,
            "dev",
            "existing@example.com",
            DateTime.UtcNow);
        await userRepository.AddAsync(existing, CancellationToken.None);

        var result = await service.SignUpWithPasswordAsync(
            "existing@example.com",
            "Other",
            "password123",
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("already registered", result.Error!);
    }

    [Fact]
    public async Task SignUpWithPasswordAsync_Fails_WhenPasswordTooShort()
    {
        var (service, _) = CreateService();

        var result = await service.SignUpWithPasswordAsync(
            "new@example.com",
            "New User",
            "12345",
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("6 characters", result.Error!);
    }

    [Fact]
    public async Task SignUpWithPasswordAsync_Fails_WhenEmailEmpty()
    {
        var (service, _) = CreateService();

        var result = await service.SignUpWithPasswordAsync(
            "  ",
            "New User",
            "password123",
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Email", result.Error!);
    }

    [Fact]
    public async Task SignUpWithPasswordAsync_Fails_WhenDisplayNameEmpty()
    {
        var (service, _) = CreateService();

        var result = await service.SignUpWithPasswordAsync(
            "new@example.com",
            "  ",
            "password123",
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Display name", result.Error!);
    }

    [Fact]
    public async Task LoginWithPasswordAsync_Success_ReturnsTokens()
    {
        var (service, userRepository) = CreateService();
        await service.SignUpWithPasswordAsync("login@example.com", "Login User", "mypassword", CancellationToken.None);

        var result = await service.LoginWithPasswordAsync(
            "login@example.com",
            "mypassword",
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        var (user, accessToken, refreshToken) = result.Value!;
        Assert.NotNull(accessToken);
        Assert.NotNull(refreshToken);
        Assert.Equal("Login User", user.DisplayName);
        Assert.Equal("login@example.com", user.Email);
    }

    [Fact]
    public async Task LoginWithPasswordAsync_Fails_WhenWrongPassword()
    {
        var (service, _) = CreateService();
        await service.SignUpWithPasswordAsync("wrong@example.com", "User", "correct", CancellationToken.None);

        var result = await service.LoginWithPasswordAsync(
            "wrong@example.com",
            "wrongpassword",
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Invalid", result.Error!);
    }

    [Fact]
    public async Task LoginWithPasswordAsync_Fails_WhenUserNotFound()
    {
        var (service, _) = CreateService();

        var result = await service.LoginWithPasswordAsync(
            "nonexistent@example.com",
            "anypassword",
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Invalid", result.Error!);
    }

    [Fact]
    public async Task LoginWithPasswordAsync_Fails_WhenUserHasNoPasswordHash()
    {
        var (service, userRepository) = CreateService();
        var socialUser = new User(
            Guid.NewGuid(),
            "Social",
            "social@example.com",
            null,
            "dev",
            null,
            null,
            "google",
            "google-id",
            DateTime.UtcNow);
        await userRepository.AddAsync(socialUser, CancellationToken.None);

        var result = await service.LoginWithPasswordAsync(
            "social@example.com",
            "anypassword",
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Invalid", result.Error!);
    }

    [Fact]
    public async Task LoginWithPasswordAsync_Returns2FARequired_When2FAEnabled()
    {
        var (service, userRepository) = CreateService();
        await service.SignUpWithPasswordAsync("2fa@example.com", "2FA User", "mypass", CancellationToken.None);
        var user = await userRepository.GetByEmailAsync("2fa@example.com", CancellationToken.None);
        Assert.NotNull(user);

        var userWith2FA = new User(
            user.Id,
            user.DisplayName,
            user.Email,
            user.Cpf,
            user.ForeignDocument,
            user.PhoneNumber,
            user.Address,
            user.AuthProvider,
            user.ExternalId,
            true,
            "SECRET",
            "HASH",
            DateTime.UtcNow,
            UserIdentityVerificationStatus.Unverified,
            null,
            null,
            null,
            user.PasswordHash,
            user.CreatedAtUtc);
        await userRepository.UpdateAsync(userWith2FA, CancellationToken.None);

        var result = await service.LoginWithPasswordAsync(
            "2fa@example.com",
            "mypass",
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.StartsWith("2FA_REQUIRED:", result.Error!);
    }

    [Fact]
    public async Task LoginWithPasswordAsync_Fails_WhenPasswordEmpty()
    {
        var (service, _) = CreateService();
        await service.SignUpWithPasswordAsync("empty@example.com", "User", "password", CancellationToken.None);

        var result = await service.LoginWithPasswordAsync(
            "empty@example.com",
            "",
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Invalid", result.Error!);
    }
}
