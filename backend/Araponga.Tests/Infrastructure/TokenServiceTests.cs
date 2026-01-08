using Araponga.Infrastructure.Security;
using Xunit;

namespace Araponga.Tests.Infrastructure;

public sealed class TokenServiceTests
{
    [Fact]
    public void TokenService_IssuesAndParsesTokens()
    {
        var service = new SimpleTokenService();
        var userId = Guid.NewGuid();

        var token = service.IssueToken(userId);

        Assert.StartsWith("user:", token);
        Assert.Equal(userId, service.ParseToken(token));
    }

    [Fact]
    public void TokenService_RejectsInvalidTokens()
    {
        var service = new SimpleTokenService();

        Assert.Null(service.ParseToken(""));
        Assert.Null(service.ParseToken("abc"));
        Assert.Null(service.ParseToken("user:not-a-guid"));
    }
}
