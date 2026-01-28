using Araponga.Api.Security;
using Araponga.Application.Interfaces;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Araponga.Tests.Api;

/// <summary>
/// Edge case tests for Authentication and Authorization,
/// focusing on JWT token validation, expired tokens, invalid tokens, and permissions.
/// </summary>
public class AuthEdgeCasesTests
{
    [Fact]
    public async Task CurrentUserAccessor_GetAsync_WithMissingAuthorizationHeader_ReturnsMissingStatus()
    {
        var tokenService = new Mock<ITokenService>();
        var userRepository = new Mock<IUserRepository>();
        var accessor = new CurrentUserAccessor(tokenService.Object, userRepository.Object);

        var httpContext = new DefaultHttpContext();
        var request = httpContext.Request;

        var result = await accessor.GetAsync(request, CancellationToken.None);

        Assert.Equal(TokenStatus.Missing, result.Status);
        Assert.Null(result.User);
    }

    [Fact]
    public async Task CurrentUserAccessor_GetAsync_WithEmptyAuthorizationHeader_ReturnsMissingStatus()
    {
        var tokenService = new Mock<ITokenService>();
        var userRepository = new Mock<IUserRepository>();
        var accessor = new CurrentUserAccessor(tokenService.Object, userRepository.Object);

        var httpContext = new DefaultHttpContext();
        var request = httpContext.Request;
        request.Headers["Authorization"] = "";

        var result = await accessor.GetAsync(request, CancellationToken.None);

        Assert.Equal(TokenStatus.Missing, result.Status);
        Assert.Null(result.User);
    }

    [Fact]
    public async Task CurrentUserAccessor_GetAsync_WithNonBearerToken_ReturnsInvalidStatus()
    {
        var tokenService = new Mock<ITokenService>();
        var userRepository = new Mock<IUserRepository>();
        var accessor = new CurrentUserAccessor(tokenService.Object, userRepository.Object);

        var httpContext = new DefaultHttpContext();
        var request = httpContext.Request;
        request.Headers.Add("Authorization", "Basic dGVzdDp0ZXN0"); // Basic auth ao invés de Bearer

        var result = await accessor.GetAsync(request, CancellationToken.None);

        Assert.Equal(TokenStatus.Invalid, result.Status);
        Assert.Null(result.User);
    }

    [Fact]
    public async Task CurrentUserAccessor_GetAsync_WithInvalidToken_ReturnsInvalidStatus()
    {
        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.ParseToken(It.IsAny<string>())).Returns((Guid?)null); // Token inválido

        var userRepository = new Mock<IUserRepository>();
        var accessor = new CurrentUserAccessor(tokenService.Object, userRepository.Object);

        var httpContext = new DefaultHttpContext();
        var request = httpContext.Request;
        request.Headers.Add("Authorization", "Bearer invalid-token");

        var result = await accessor.GetAsync(request, CancellationToken.None);

        Assert.Equal(TokenStatus.Invalid, result.Status);
        Assert.Null(result.User);
    }

    [Fact]
    public async Task CurrentUserAccessor_GetAsync_WithValidTokenButNonExistentUser_ReturnsInvalidStatus()
    {
        var userId = Guid.NewGuid();
        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.ParseToken(It.IsAny<string>())).Returns(userId);

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null); // Usuário não existe

        var accessor = new CurrentUserAccessor(tokenService.Object, userRepository.Object);

        var httpContext = new DefaultHttpContext();
        var request = httpContext.Request;
        request.Headers.Add("Authorization", "Bearer valid-token");

        var result = await accessor.GetAsync(request, CancellationToken.None);

        Assert.Equal(TokenStatus.Invalid, result.Status);
        Assert.Null(result.User);
    }

    [Fact]
    public async Task CurrentUserAccessor_GetAsync_WithValidTokenAndUser_ReturnsValidStatus()
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
            $"test-{userId}",
            DateTime.UtcNow);

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.ParseToken(It.IsAny<string>())).Returns(userId);

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var accessor = new CurrentUserAccessor(tokenService.Object, userRepository.Object);

        var httpContext = new DefaultHttpContext();
        var request = httpContext.Request;
        request.Headers.Add("Authorization", "Bearer valid-token");

        var result = await accessor.GetAsync(request, CancellationToken.None);

        Assert.Equal(TokenStatus.Valid, result.Status);
        Assert.NotNull(result.User);
        Assert.Equal(userId, result.User.Id);
    }

    [Fact]
    public async Task CurrentUserAccessor_GetAsync_WithBearerPrefixCaseInsensitive_HandlesCorrectly()
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
            $"test-{userId}",
            DateTime.UtcNow);

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.ParseToken(It.IsAny<string>())).Returns(userId);

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var accessor = new CurrentUserAccessor(tokenService.Object, userRepository.Object);

        var httpContext = new DefaultHttpContext();
        var request = httpContext.Request;
        request.Headers.Add("Authorization", "bearer valid-token"); // lowercase

        var result = await accessor.GetAsync(request, CancellationToken.None);

        Assert.Equal(TokenStatus.Valid, result.Status);
        Assert.NotNull(result.User);
    }

    [Fact]
    public async Task CurrentUserAccessor_GetAsync_WithTokenWithWhitespace_TrimsCorrectly()
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
            $"test-{userId}",
            DateTime.UtcNow);

        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.ParseToken("valid-token")).Returns(userId); // Espera token sem espaços

        var userRepository = new Mock<IUserRepository>();
        userRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var accessor = new CurrentUserAccessor(tokenService.Object, userRepository.Object);

        var httpContext = new DefaultHttpContext();
        var request = httpContext.Request;
        request.Headers.Add("Authorization", "Bearer   valid-token   "); // Com espaços extras

        var result = await accessor.GetAsync(request, CancellationToken.None);

        // Deve trimar os espaços e passar o token limpo para o tokenService
        tokenService.Verify(t => t.ParseToken("valid-token"), Times.Once);
    }

    [Fact]
    public async Task CurrentUserAccessor_GetAsync_WithExpiredToken_ReturnsInvalidStatus()
    {
        var tokenService = new Mock<ITokenService>();
        // Token expirado retorna null quando parseado
        tokenService.Setup(t => t.ParseToken(It.IsAny<string>())).Returns((Guid?)null);

        var userRepository = new Mock<IUserRepository>();
        var accessor = new CurrentUserAccessor(tokenService.Object, userRepository.Object);

        var httpContext = new DefaultHttpContext();
        var request = httpContext.Request;
        request.Headers.Add("Authorization", "Bearer expired-token");

        var result = await accessor.GetAsync(request, CancellationToken.None);

        Assert.Equal(TokenStatus.Invalid, result.Status);
        Assert.Null(result.User);
    }

    [Fact]
    public async Task CurrentUserAccessor_GetAsync_WithMalformedToken_ReturnsInvalidStatus()
    {
        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.ParseToken(It.IsAny<string>())).Returns((Guid?)null);

        var userRepository = new Mock<IUserRepository>();
        var accessor = new CurrentUserAccessor(tokenService.Object, userRepository.Object);

        var httpContext = new DefaultHttpContext();
        var request = httpContext.Request;
        request.Headers.Add("Authorization", "Bearer not.a.valid.jwt.token");

        var result = await accessor.GetAsync(request, CancellationToken.None);

        Assert.Equal(TokenStatus.Invalid, result.Status);
        Assert.Null(result.User);
    }

    [Fact]
    public async Task CurrentUserAccessor_GetAsync_WithEmptyToken_ReturnsInvalidStatus()
    {
        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(t => t.ParseToken("")).Returns((Guid?)null);

        var userRepository = new Mock<IUserRepository>();
        var accessor = new CurrentUserAccessor(tokenService.Object, userRepository.Object);

        var httpContext = new DefaultHttpContext();
        var request = httpContext.Request;
        request.Headers.Add("Authorization", "Bearer "); // Token vazio

        var result = await accessor.GetAsync(request, CancellationToken.None);

        Assert.Equal(TokenStatus.Invalid, result.Status);
        Assert.Null(result.User);
    }
}
