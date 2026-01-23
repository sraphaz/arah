namespace Araponga.Domain.Policies;

/// <summary>
/// Representa o aceite da Política de Privacidade por um usuário.
/// </summary>
public sealed class PrivacyPolicyAcceptance
{
    public const int MaxIpAddressLength = 45; // IPv6 max length
    public const int MaxUserAgentLength = 500;

    public Guid Id { get; private set; }
    
    /// <summary>
    /// ID do usuário que aceitou a política.
    /// </summary>
    public Guid UserId { get; private set; }
    
    /// <summary>
    /// ID da política aceita.
    /// </summary>
    public Guid PrivacyPolicyId { get; private set; }
    
    /// <summary>
    /// Data/hora em que a política foi aceita.
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
    /// Versão da política que foi aceita.
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

    private PrivacyPolicyAcceptance()
    {
        AcceptedVersion = string.Empty;
    }

    public PrivacyPolicyAcceptance(
        Guid id,
        Guid userId,
        Guid privacyPolicyId,
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

        if (privacyPolicyId == Guid.Empty)
        {
            throw new ArgumentException("Privacy Policy ID is required.", nameof(privacyPolicyId));
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
        PrivacyPolicyId = privacyPolicyId;
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
