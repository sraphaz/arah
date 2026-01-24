using Araponga.Application.Services;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Security;
using Microsoft.Extensions.Options;
using Xunit;
using static Araponga.Tests.TestIds;

namespace Araponga.Tests.Application;

public sealed class AuthService2FATests
{
    // Usar TestIds.TestUserId2FA para manter coerência com a massa de testes
    private static readonly Guid UserId = TestUserId2FA;

    [Fact]
    public async Task Setup2FAAsync_GeneratesSecretAndQR()
    {
        var dataStore = new InMemoryDataStore();
        var userRepository = new InMemoryUserRepository(dataStore);
        var jwtOptions = Options.Create(new JwtOptions 
        { 
            Issuer = "Araponga",
            Audience = "Araponga",
            SigningKey = "test-secret-key-32-chars-long!",
            ExpirationMinutes = 60
        });
        var tokenService = new JwtTokenService(jwtOptions);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new AuthService(userRepository, tokenService, unitOfWork);

        var user = new User(
            UserId,
            "User",
            "user@araponga.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);
        await userRepository.AddAsync(user, CancellationToken.None);

        var result = await service.Setup2FAAsync(UserId, "user@araponga.com", CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.NotNull(result.Value!.Secret);
        Assert.NotNull(result.Value.QrCodeUri);
        Assert.Contains("otpauth://totp", result.Value.QrCodeUri);
        Assert.Contains("Araponga", result.Value.QrCodeUri);
    }

    [Fact]
    public async Task Setup2FAAsync_Fails_IfAlreadyEnabled()
    {
        var dataStore = new InMemoryDataStore();
        var userRepository = new InMemoryUserRepository(dataStore);
        var jwtOptions = Options.Create(new JwtOptions 
        { 
            Issuer = "Araponga",
            Audience = "Araponga",
            SigningKey = "test-secret-key-32-chars-long!",
            ExpirationMinutes = 60
        });
        var tokenService = new JwtTokenService(jwtOptions);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new AuthService(userRepository, tokenService, unitOfWork);

        var user = new User(
            UserId,
            "User",
            "user@araponga.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            true,
            "SECRET",
            "HASH",
            DateTime.UtcNow,
            UserIdentityVerificationStatus.Unverified,
            null,
            null,
            null,
            DateTime.UtcNow);
        await userRepository.AddAsync(user, CancellationToken.None);

        var result = await service.Setup2FAAsync(UserId, "user@araponga.com", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("already enabled", result.Error!);
    }

    [Fact]
    public async Task Confirm2FAAsync_ValidatesCode()
    {
        var dataStore = new InMemoryDataStore();
        var userRepository = new InMemoryUserRepository(dataStore);
        var jwtOptions = Options.Create(new JwtOptions 
        { 
            Issuer = "Araponga",
            Audience = "Araponga",
            SigningKey = "test-secret-key-32-chars-long!",
            ExpirationMinutes = 60
        });
        var tokenService = new JwtTokenService(jwtOptions);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new AuthService(userRepository, tokenService, unitOfWork);

        var user = new User(
            UserId,
            "User",
            "user@araponga.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);
        await userRepository.AddAsync(user, CancellationToken.None);

        // Setup
        var setupResult = await service.Setup2FAAsync(UserId, "user@araponga.com", CancellationToken.None);
        Assert.True(setupResult.IsSuccess);

        // Gerar código TOTP válido (usando Otp.NET)
        var secretBytes = OtpNet.Base32Encoding.ToBytes(setupResult.Value!.Secret);
        var totp = new OtpNet.Totp(secretBytes);
        var code = totp.ComputeTotp();

        // Confirmar
        var confirmResult = await service.Confirm2FAAsync(UserId, setupResult.Value.Secret, code, CancellationToken.None);

        Assert.True(confirmResult.IsSuccess);
        Assert.NotNull(confirmResult.Value);
        Assert.NotEmpty(confirmResult.Value!.RecoveryCodes);

        // Verificar que 2FA foi habilitado
        var updatedUser = await userRepository.GetByIdAsync(UserId, CancellationToken.None);
        Assert.NotNull(updatedUser);
        Assert.True(updatedUser!.TwoFactorEnabled);
    }

    [Fact]
    public async Task Confirm2FAAsync_Fails_OnInvalidCode()
    {
        var dataStore = new InMemoryDataStore();
        var userRepository = new InMemoryUserRepository(dataStore);
        var jwtOptions = Options.Create(new JwtOptions 
        { 
            Issuer = "Araponga",
            Audience = "Araponga",
            SigningKey = "test-secret-key-32-chars-long!",
            ExpirationMinutes = 60
        });
        var tokenService = new JwtTokenService(jwtOptions);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new AuthService(userRepository, tokenService, unitOfWork);

        var user = new User(
            UserId,
            "User",
            "user@araponga.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            DateTime.UtcNow);
        await userRepository.AddAsync(user, CancellationToken.None);

        var setupResult = await service.Setup2FAAsync(UserId, "user@araponga.com", CancellationToken.None);
        Assert.True(setupResult.IsSuccess);

        var confirmResult = await service.Confirm2FAAsync(UserId, setupResult.Value!.Secret, "000000", CancellationToken.None);

        Assert.True(confirmResult.IsFailure);
        Assert.Contains("Invalid", confirmResult.Error!);
    }

    [Fact]
    public async Task LoginSocialAsync_Returns2FARequired_WhenEnabled()
    {
        var dataStore = new InMemoryDataStore();
        var userRepository = new InMemoryUserRepository(dataStore);
        var jwtOptions = Options.Create(new JwtOptions 
        { 
            Issuer = "Araponga",
            Audience = "Araponga",
            SigningKey = "test-secret-key-32-chars-long!",
            ExpirationMinutes = 60
        });
        var tokenService = new JwtTokenService(jwtOptions);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new AuthService(userRepository, tokenService, unitOfWork);

        var user = new User(
            UserId,
            "User",
            "user@araponga.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            true,
            "SECRET",
            "HASH",
            DateTime.UtcNow,
            UserIdentityVerificationStatus.Unverified,
            null,
            null,
            null,
            DateTime.UtcNow);
        await userRepository.AddAsync(user, CancellationToken.None);

        var result = await service.LoginSocialAsync(
            "google",
            "ext",
            "User",
            "user@araponga.com",
            "123.456.789-00",
            null,
            null,
            null,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.StartsWith("2FA_REQUIRED:", result.Error!);
    }

    [Fact]
    public async Task LoginSocialAsync_ReturnsJWT_When2FADisabled()
    {
        var dataStore = new InMemoryDataStore();
        var userRepository = new InMemoryUserRepository(dataStore);
        var jwtOptions = Options.Create(new JwtOptions 
        { 
            Issuer = "Araponga",
            Audience = "Araponga",
            SigningKey = "test-secret-key-32-chars-long!",
            ExpirationMinutes = 60
        });
        var tokenService = new JwtTokenService(jwtOptions);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new AuthService(userRepository, tokenService, unitOfWork);

        var result = await service.LoginSocialAsync(
            "google",
            "new-ext",
            "User",
            "user@araponga.com",
            "123.456.789-00",
            null,
            null,
            null,
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value!.token);
    }

    [Fact]
    public async Task Verify2FAAsync_ReturnsJWT_OnValidCode()
    {
        var dataStore = new InMemoryDataStore();
        var userRepository = new InMemoryUserRepository(dataStore);
        var jwtOptions = Options.Create(new JwtOptions 
        { 
            Issuer = "Araponga",
            Audience = "Araponga",
            SigningKey = "test-secret-key-32-chars-long!",
            ExpirationMinutes = 60
        });
        var tokenService = new JwtTokenService(jwtOptions);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new AuthService(userRepository, tokenService, unitOfWork);

        var user = new User(
            UserId,
            "User",
            "user@araponga.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            true,
            "SECRET",
            "HASH",
            DateTime.UtcNow,
            UserIdentityVerificationStatus.Unverified,
            null,
            null,
            null,
            DateTime.UtcNow);
        await userRepository.AddAsync(user, CancellationToken.None);

        // Login para gerar challenge
        var loginResult = await service.LoginSocialAsync(
            "google",
            "ext",
            "User",
            "user@araponga.com",
            "123.456.789-00",
            null,
            null,
            null,
            CancellationToken.None);
        Assert.True(loginResult.IsFailure);
        var challengeId = loginResult.Error!.Substring("2FA_REQUIRED:".Length);

        // Gerar código TOTP válido
        var secretBytes = OtpNet.Base32Encoding.ToBytes("SECRET");
        var totp = new OtpNet.Totp(secretBytes);
        var code = totp.ComputeTotp();

        // Verificar 2FA
        var verifyResult = await service.Verify2FAAsync(challengeId, code, CancellationToken.None);

        Assert.True(verifyResult.IsSuccess);
        Assert.NotNull(verifyResult.Value);
        Assert.NotNull(verifyResult.Value!);
    }

    [Fact]
    public async Task Verify2FAAsync_Fails_OnInvalidCode()
    {
        var dataStore = new InMemoryDataStore();
        var userRepository = new InMemoryUserRepository(dataStore);
        var jwtOptions = Options.Create(new JwtOptions 
        { 
            Issuer = "Araponga",
            Audience = "Araponga",
            SigningKey = "test-secret-key-32-chars-long!",
            ExpirationMinutes = 60
        });
        var tokenService = new JwtTokenService(jwtOptions);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new AuthService(userRepository, tokenService, unitOfWork);

        var user = new User(
            UserId,
            "User",
            "user@araponga.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            true,
            "SECRET",
            "HASH",
            DateTime.UtcNow,
            UserIdentityVerificationStatus.Unverified,
            null,
            null,
            null,
            DateTime.UtcNow);
        await userRepository.AddAsync(user, CancellationToken.None);

        // Login para gerar challenge
        var loginResult = await service.LoginSocialAsync(
            "google",
            "ext",
            "User",
            "user@araponga.com",
            "123.456.789-00",
            null,
            null,
            null,
            CancellationToken.None);
        Assert.True(loginResult.IsFailure);
        var challengeId = loginResult.Error!.Substring("2FA_REQUIRED:".Length);

        // Tentar com código inválido
        var verifyResult = await service.Verify2FAAsync(challengeId, "000000", CancellationToken.None);

        Assert.True(verifyResult.IsFailure);
        Assert.Contains("Invalid", verifyResult.Error!);
    }

    [Fact]
    public async Task Disable2FAAsync_ClearsSecrets()
    {
        var dataStore = new InMemoryDataStore();
        var userRepository = new InMemoryUserRepository(dataStore);
        var jwtOptions = Options.Create(new JwtOptions 
        { 
            Issuer = "Araponga",
            Audience = "Araponga",
            SigningKey = "test-secret-key-32-chars-long!",
            ExpirationMinutes = 60
        });
        var tokenService = new JwtTokenService(jwtOptions);
        var unitOfWork = new InMemoryUnitOfWork();
        var service = new AuthService(userRepository, tokenService, unitOfWork);

        var user = new User(
            UserId,
            "User",
            "user@araponga.com",
            "123.456.789-00",
            null,
            null,
            null,
            "google",
            "ext",
            true,
            "SECRET",
            "HASH",
            DateTime.UtcNow,
            UserIdentityVerificationStatus.Unverified,
            null,
            null,
            null,
            DateTime.UtcNow);
        await userRepository.AddAsync(user, CancellationToken.None);

        var result = await service.Disable2FAAsync(UserId, null, CancellationToken.None);

        Assert.True(result.IsSuccess);

        var updatedUser = await userRepository.GetByIdAsync(UserId, CancellationToken.None);
        Assert.NotNull(updatedUser);
        Assert.False(updatedUser!.TwoFactorEnabled);
        Assert.Null(updatedUser.TwoFactorSecret);
    }
}
