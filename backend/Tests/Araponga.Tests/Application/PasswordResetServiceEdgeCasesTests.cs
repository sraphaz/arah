using Araponga.Application.Common;
using Araponga.Application.Configuration;
using Araponga.Application.Interfaces;
using Araponga.Application.Services;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Araponga.Infrastructure.Shared.InMemory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Araponga.Tests.Application;

/// <summary>
/// Edge case tests for PasswordResetService,
/// focusing on null/empty inputs, expired tokens, invalid tokens, and error handling.
/// </summary>
public sealed class PasswordResetServiceEdgeCasesTests
{
    private readonly InMemorySharedStore _sharedStore;
    private readonly InMemoryDataStore _dataStore;
    private readonly InMemoryUserRepository _userRepository;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly IDistributedCache _cache;
    private readonly Mock<ILogger<PasswordResetService>> _loggerMock;
    private readonly PasswordResetOptions _options;
    private readonly PasswordResetService _service;

    public PasswordResetServiceEdgeCasesTests()
    {
        _sharedStore = new InMemorySharedStore();
        _dataStore = new InMemoryDataStore();
        _userRepository = new InMemoryUserRepository(_sharedStore);
        _tokenServiceMock = new Mock<ITokenService>();
        _emailSenderMock = new Mock<IEmailSender>();
        _cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions { }));
        _loggerMock = new Mock<ILogger<PasswordResetService>>();
        _options = new PasswordResetOptions
        {
            TokenTtlMinutes = 30,
            ResetUrlBase = "https://example.com/reset"
        };
        _service = new PasswordResetService(
            _userRepository,
            _tokenServiceMock.Object,
            _emailSenderMock.Object,
            _cache,
            _loggerMock.Object,
            Options.Create(_options));
    }

    [Fact]
    public async Task RequestAsync_WithNullEmail_ReturnsFailure()
    {
        var result = await _service.RequestAsync(null!, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("obrigatório", result.Error ?? "");
    }

    [Fact]
    public async Task RequestAsync_WithEmptyEmail_ReturnsFailure()
    {
        var result = await _service.RequestAsync("", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("obrigatório", result.Error ?? "");
    }

    [Fact]
    public async Task RequestAsync_WithWhitespaceEmail_ReturnsFailure()
    {
        var result = await _service.RequestAsync("   ", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("obrigatório", result.Error ?? "");
    }

    [Fact]
    public async Task RequestAsync_WithNonExistentEmail_ReturnsSuccess()
    {
        var result = await _service.RequestAsync("nonexistent@example.com", CancellationToken.None);

        Assert.True(result.IsSuccess); // Não revela se email existe
        _emailSenderMock.Verify(s => s.SendEmailAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RequestAsync_WithExistingEmail_SendsEmail()
    {
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "test",
            "test-external-id",
            DateTime.UtcNow);
        await _userRepository.AddAsync(user, CancellationToken.None);

        _emailSenderMock.Setup(s => s.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(OperationResult.Success());

        var result = await _service.RequestAsync("test@example.com", CancellationToken.None);

        Assert.True(result.IsSuccess);
        _emailSenderMock.Verify(s => s.SendEmailAsync(
            "test@example.com",
            "Recuperação de acesso",
            It.Is<string>(body => body.Contains("token") || body.Contains("reset")),
            false,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RequestAsync_WithUnicodeEmail_HandlesCorrectly()
    {
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test+café@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "test",
            "test-external-id",
            DateTime.UtcNow);
        await _userRepository.AddAsync(user, CancellationToken.None);

        _emailSenderMock.Setup(s => s.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(OperationResult.Success());

        var result = await _service.RequestAsync("test+café@example.com", CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task RequestAsync_WithResetUrlBase_IncludesLink()
    {
        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "test",
            "test-external-id",
            DateTime.UtcNow);
        await _userRepository.AddAsync(user, CancellationToken.None);

        _emailSenderMock.Setup(s => s.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(OperationResult.Success());

        var result = await _service.RequestAsync("test@example.com", CancellationToken.None);

        Assert.True(result.IsSuccess);
        _emailSenderMock.Verify(s => s.SendEmailAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.Is<string>(body => body.Contains("https://example.com/reset")),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RequestAsync_WithoutResetUrlBase_UsesTokenOnly()
    {
        var optionsWithoutUrl = new PasswordResetOptions
        {
            TokenTtlMinutes = 30,
            ResetUrlBase = null
        };
        var serviceWithoutUrl = new PasswordResetService(
            _userRepository,
            _tokenServiceMock.Object,
            _emailSenderMock.Object,
            _cache,
            _loggerMock.Object,
            Options.Create(optionsWithoutUrl));

        var user = new User(
            Guid.NewGuid(),
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "test",
            "test-external-id",
            DateTime.UtcNow);
        await _userRepository.AddAsync(user, CancellationToken.None);

        _emailSenderMock.Setup(s => s.SendEmailAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(OperationResult.Success());

        var result = await serviceWithoutUrl.RequestAsync("test@example.com", CancellationToken.None);

        Assert.True(result.IsSuccess);
        _emailSenderMock.Verify(s => s.SendEmailAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.Is<string>(body => body.Contains("token")),
            It.IsAny<bool>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ConfirmAsync_WithNullToken_ReturnsFailure()
    {
        var result = await _service.ConfirmAsync(null!, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("inválido", result.Error ?? "");
    }

    [Fact]
    public async Task ConfirmAsync_WithEmptyToken_ReturnsFailure()
    {
        var result = await _service.ConfirmAsync("", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("inválido", result.Error ?? "");
    }

    [Fact]
    public async Task ConfirmAsync_WithWhitespaceToken_ReturnsFailure()
    {
        var result = await _service.ConfirmAsync("   ", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("inválido", result.Error ?? "");
    }

    [Fact]
    public async Task ConfirmAsync_WithNonExistentToken_ReturnsFailure()
    {
        var result = await _service.ConfirmAsync("invalid-token", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("inválido", result.Error ?? "");
    }

    [Fact]
    public async Task ConfirmAsync_WithValidToken_ReturnsJwt()
    {
        var userId = Guid.NewGuid();
        var user = new User(
            userId,
            "Test User",
            "test@example.com",
            "123.456.789-00",
            null,
            null,
            null,
            "test",
            "test-external-id",
            DateTime.UtcNow);
        await _userRepository.AddAsync(user, CancellationToken.None);

        // Criar token válido manualmente (simulando o que RequestAsync faria)
        var token = "valid-token-123456789";
        var tokenHash = HashToken(token);
        var cacheKey = $"password_reset:{tokenHash}";
        var payload = new Araponga.Application.Models.PasswordResetTokenPayload(
            userId,
            DateTime.UtcNow.AddMinutes(30));
        await _cache.SetStringAsync(
            cacheKey,
            System.Text.Json.JsonSerializer.Serialize(payload),
            new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            },
            CancellationToken.None);

        _tokenServiceMock.Setup(t => t.IssueToken(userId))
            .Returns("jwt-token");

        var confirmResult = await _service.ConfirmAsync(token, CancellationToken.None);

        Assert.True(confirmResult.IsSuccess);
        Assert.Equal("jwt-token", confirmResult.Value);
        _tokenServiceMock.Verify(t => t.IssueToken(userId), Times.Once);
    }

    [Fact]
    public async Task ConfirmAsync_WithExpiredToken_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var token = "expired-token";
        var tokenHash = HashToken(token);
        var cacheKey = $"password_reset:{tokenHash}";
        var payload = new Araponga.Application.Models.PasswordResetTokenPayload(
            userId,
            DateTime.UtcNow.AddMinutes(-1)); // Expirado
        
        // Não podemos usar expiração negativa, então vamos simular um token expirado
        // colocando-o no cache com expiração imediata e esperando um pouco
        await _cache.SetStringAsync(
            cacheKey,
            System.Text.Json.JsonSerializer.Serialize(payload),
            new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(1)
            },
            CancellationToken.None);

        // Aguardar para o token expirar
        await Task.Delay(100, CancellationToken.None);

        var result = await _service.ConfirmAsync(token, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("expirado", result.Error ?? "");
    }

    private static string HashToken(string token)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashBytes);
    }
}
