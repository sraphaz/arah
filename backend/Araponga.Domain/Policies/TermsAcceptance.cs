namespace Araponga.Domain.Policies;

/// <summary>
/// Representa o aceite de Termos de Uso por um usuário.
/// </summary>
public sealed class TermsAcceptance
{
    public const int MaxIpAddressLength = 45; // IPv6 max length
    public const int MaxUserAgentLength = 500;

    public Guid Id { get; private set; }
    
    /// <summary>
    /// ID do usuário que aceitou os termos.
    /// </summary>
    public Guid UserId { get; private set; }
    
    /// <summary>
    /// ID dos termos aceitos.
    /// </summary>
    public Guid TermsOfServiceId { get; private set; }
    
    /// <summary>
    /// Data/hora em que os termos foram aceitos.
    /// </summary>
    public DateTime AcceptedAtUtc { get; private set; }
    
    /// <summary>
    /// Endereço IP do usuário no momento do aceite (para auditoria).
    /// </summary>
    public string? IpAddress { get; private set; }
    
    /// <summary>
    /// User Agent do navegador/aplicativo (para auditoria).
    /// </summary>
    public string? UserAgent { get; private set; }
    
    /// <summary>
    /// Versão dos termos que foi aceita.
    /// </summary>
    public string AcceptedVersion { get; private set; }
    
    /// <summary>
    /// Indica se o aceite foi revogado pelo usuário.
    /// </summary>
    public bool IsRevoked { get; private set; }
    
    /// <summary>
    /// Data/hora em que o aceite foi revogado (nullable).
    /// </summary>
    public DateTime? RevokedAtUtc { get; private set; }

    private TermsAcceptance()
    {
        AcceptedVersion = string.Empty;
    }

    public TermsAcceptance(
        Guid id,
        Guid userId,
        Guid termsOfServiceId,
        DateTime acceptedAtUtc,
        string acceptedVersion,
        string? ipAddress,
        string? userAgent)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("ID is required.", nameof(id));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID is required.", nameof(userId));
        }

        if (termsOfServiceId == Guid.Empty)
        {
            throw new ArgumentException("Terms of Service ID is required.", nameof(termsOfServiceId));
        }

        if (string.IsNullOrWhiteSpace(acceptedVersion))
        {
            throw new ArgumentException("Accepted version is required.", nameof(acceptedVersion));
        }

        if (ipAddress is not null && ipAddress.Length > MaxIpAddressLength)
        {
            throw new ArgumentException($"IP address must not exceed {MaxIpAddressLength} characters.", nameof(ipAddress));
        }

        if (userAgent is not null && userAgent.Length > MaxUserAgentLength)
        {
            throw new ArgumentException($"User agent must not exceed {MaxUserAgentLength} characters.", nameof(userAgent));
        }

        Id = id;
        UserId = userId;
        TermsOfServiceId = termsOfServiceId;
        AcceptedAtUtc = acceptedAtUtc;
        AcceptedVersion = acceptedVersion.Trim();
        IpAddress = ipAddress?.Trim();
        UserAgent = userAgent?.Trim();
        IsRevoked = false;
        RevokedAtUtc = null;
    }

    public void Revoke(DateTime revokedAtUtc)
    {
        if (IsRevoked)
        {
            throw new InvalidOperationException("Acceptance is already revoked.");
        }

        IsRevoked = true;
        RevokedAtUtc = revokedAtUtc;
    }
}
