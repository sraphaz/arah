namespace Araponga.Infrastructure.Security;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = "Araponga";
    public string Audience { get; set; } = "Araponga";
    public string SigningKey { get; set; } = "dev-only-change-me";
    public int ExpirationMinutes { get; set; } = 60;
}
