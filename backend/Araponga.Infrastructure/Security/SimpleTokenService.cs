using Araponga.Application.Interfaces;

namespace Araponga.Infrastructure.Security;

public sealed class SimpleTokenService : ITokenService
{
    private const string Prefix = "user:";

    public string IssueToken(Guid userId)
    {
        return $"{Prefix}{userId}";
    }

    public Guid? ParseToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        if (!token.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var raw = token[Prefix.Length..];
        return Guid.TryParse(raw, out var userId) ? userId : null;
    }
}
