namespace Araponga.Infrastructure.Shared.Postgres.Entities;

/// <summary>
/// Record para armazenar Termos de Uso no banco de dados.
/// </summary>
public sealed class TermsOfServiceRecord
{
    public Guid Id { get; set; }
    public string Version { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsActive { get; set; }
    public string? RequiredRoles { get; set; }
    public string? RequiredCapabilities { get; set; }
    public string? RequiredSystemPermissions { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
