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
public sealed class AuthServiceSignUpLoginTests
{
    private static (AuthService service, InMemoryUserRepository userRepo) CreateService()
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
    public async Task SignUpWithPasswordAsync_CreatesUser_WithHashedPassword()
    {
        var (service, userRepo) = CreateService();
        var email = "newuser@example.com";
        var displayName = "New User";
        var password = "SecurePass123";

        var result = await service.SignUpWithPasswordAsync(email, displayName, password, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var (user, accessToken, refreshToken) = result.Value!;
        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
        Assert.Equal(displayName, user.DisplayName);
        Assert.NotNull(user.PasswordHash);
        Assert.NotEmpty(user.PasswordHash);
        Assert.NotNull(accessToken);
        Assert.NotNull(refreshToken);

        var fromDb = await userRepo.GetByEmailAsync(email, CancellationToken.None);
        Assert.NotNull(fromDb);
        Assert.NotNull(fromDb.PasswordHash);
    }

    [Fact]
    public async Task SignUpWithPasswordAsync_Fails_WhenEmailAlreadyExists()
    {
        var (service, userRepo) = CreateService();
        var email = "existing@example.com";
        var existingUser = new User(
            Guid.NewGuid(),
            "Existing",
            email,
            null,
            "dev",
            null,
            null,
            "dev",
            email,
            DateTime.UtcNow);
        await userRepo.AddAsync(existingUser, CancellationToken.None);

        var result = await service.SignUpWithPasswordAsync(email, "Other", "Password123", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("already registered", result.Error!);
    }

    [Fact]
    public async Task SignUpWithPasswordAsync_Fails_WhenPasswordTooShort()
    {
        var (service, _) = CreateService();

        var result = await service.SignUpWithPasswordAsync("a@b.com", "Name", "12345", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("6 characters", result.Error!);
    }

    [Fact]
    public async Task SignUpWithPasswordAsync_Fails_WhenEmailEmpty()
    {
        var (service, _) = CreateService();

        var result = await service.SignUpWithPasswordAsync("", "Name", "Password123", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Email", result.Error!);
    }

    [Fact]
    public async Task LoginWithPasswordAsync_ReturnsTokens_WhenPasswordCorrect()
    {
        var (service, userRepo) = CreateService();
        var email = "login@example.com";
        var password = "MyPassword1";
        await service.SignUpWithPasswordAsync(email, "Login User", password, CancellationToken.None);

        var result = await service.LoginWithPasswordAsync(email, password, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var (user, accessToken, refreshToken) = result.Value!;
        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
        Assert.NotNull(accessToken);
        Assert.NotNull(refreshToken);
    }

    [Fact]
    public async Task LoginWithPasswordAsync_Fails_WhenPasswordWrong()
    {
        var (service, _) = CreateService();
        var email = "wrong@example.com";
        await service.SignUpWithPasswordAsync(email, "User", "CorrectPassword", CancellationToken.None);

        var result = await service.LoginWithPasswordAsync(email, "WrongPassword", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Invalid", result.Error!);
    }

    [Fact]
    public async Task LoginWithPasswordAsync_Fails_WhenEmailNotRegistered()
    {
        var (service, _) = CreateService();

        var result = await service.LoginWithPasswordAsync("nobody@example.com", "AnyPass123", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Invalid", result.Error!);
    }

    [Fact]
    public async Task LoginWithPasswordAsync_Fails_WhenUserHasNoPasswordHash()
    {
        var (service, userRepo) = CreateService();
        var email = "social@example.com";
        var user = new User(
            Guid.NewGuid(),
            "Social User",
            email,
            null,
            "dev",
            null,
            null,
            "google",
            "google-ext",
            DateTime.UtcNow);
        await userRepo.AddAsync(user, CancellationToken.None);

        var result = await service.LoginWithPasswordAsync(email, "AnyPass123", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("Invalid", result.Error!);
    }
}
