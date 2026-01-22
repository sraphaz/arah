namespace Araponga.Infrastructure.Postgres.Entities;

/// <summary>
/// Record para armazenar aceites de Políticas de Privacidade no banco de dados.
/// </summary>
public sealed class PrivacyPolicyAcceptanceRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PrivacyPolicyId { get; set; }
    public DateTime AcceptedAtUtc { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string AcceptedVersion { get; set; } = string.Empty;
    public bool IsRevoked { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
}
