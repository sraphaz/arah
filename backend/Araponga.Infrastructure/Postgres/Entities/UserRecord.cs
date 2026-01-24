using Araponga.Domain.Users;

namespace Araponga.Infrastructure.Postgres.Entities;

public sealed class UserRecord
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Cpf { get; set; }
    public string? ForeignDocument { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string AuthProvider { get; set; } = string.Empty;
    public string ExternalId { get; set; } = string.Empty;
    
    // 2FA fields
    public bool TwoFactorEnabled { get; set; }
    public string? TwoFactorSecret { get; set; }
    public string? TwoFactorRecoveryCodesHash { get; set; }
    public DateTime? TwoFactorVerifiedAtUtc { get; set; }
    
    // Identity verification fields
    public UserIdentityVerificationStatus IdentityVerificationStatus { get; set; }
    public DateTime? IdentityVerifiedAtUtc { get; set; }
    
    // Profile fields
    public Guid? AvatarMediaAssetId { get; set; }
    public string? Bio { get; set; }
    
    public DateTime CreatedAtUtc { get; set; }
}
