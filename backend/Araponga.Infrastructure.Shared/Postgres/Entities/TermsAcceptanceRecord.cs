namespace Araponga.Infrastructure.Shared.Postgres.Entities;

/// <summary>
/// Record para armazenar aceites de Termos de Uso no banco de dados.
/// </summary>
public sealed class TermsAcceptanceRecord
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid TermsOfServiceId { get; set; }
    public DateTime AcceptedAtUtc { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string AcceptedVersion { get; set; } = string.Empty;
    public bool IsRevoked { get; set; }
    public DateTime? RevokedAtUtc { get; set; }
}
