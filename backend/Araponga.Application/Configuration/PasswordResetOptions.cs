namespace Araponga.Application.Configuration;

public sealed class PasswordResetOptions
{
    public string? ResetUrlBase { get; set; }
    public int TokenTtlMinutes { get; set; } = 30;
}
