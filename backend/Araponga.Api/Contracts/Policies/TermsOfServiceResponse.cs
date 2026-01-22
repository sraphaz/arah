namespace Araponga.Api.Contracts.Policies;

/// <summary>
/// Response para Termos de Uso.
/// </summary>
public sealed record TermsOfServiceResponse
{
    public Guid Id { get; init; }
    public string Version { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime EffectiveDate { get; init; }
    public DateTime? ExpirationDate { get; init; }
    public bool IsActive { get; init; }
    public List<int>? RequiredRoles { get; init; }
    public List<int>? RequiredCapabilities { get; init; }
    public List<int>? RequiredSystemPermissions { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime UpdatedAtUtc { get; init; }
}
