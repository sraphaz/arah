using Araponga.Infrastructure.Security;
using Microsoft.Extensions.Options;
using Xunit;

namespace Araponga.Tests.Infrastructure;

public sealed class TokenServiceTests
{
    [Fact]
    public void TokenService_IssuesAndParsesTokens()
    {
        var service = CreateService();
        var userId = Guid.NewGuid();

        var token = service.IssueToken(userId);

        Assert.Contains('.', token);
        Assert.Equal(userId, service.ParseToken(token));
    }

    [Fact]
    public void TokenService_RejectsInvalidTokens()
    {
        var service = CreateService();

        Assert.Null(service.ParseToken(""));
        Assert.Null(service.ParseToken("abc"));
        Assert.Null(service.ParseToken("not.a.jwt"));
    }

    private static JwtTokenService CreateService()
    {
        var options = Options.Create(new JwtOptions
        {
            Issuer = "Araponga",
            Audience = "Araponga",
            SigningKey = "test-key-for-jwt-service",
            ExpirationMinutes = 60
        });

        return new JwtTokenService(options);
    }
}
