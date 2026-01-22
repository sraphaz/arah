namespace Araponga.Api.Contracts.Policies;

/// <summary>
/// Response para aceite de Termos de Uso.
/// </summary>
public sealed record TermsAcceptanceResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public Guid TermsOfServiceId { get; init; }
    public DateTime AcceptedAtUtc { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
    public string AcceptedVersion { get; init; } = string.Empty;
    public bool IsRevoked { get; init; }
    public DateTime? RevokedAtUtc { get; init; }
}
