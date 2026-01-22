namespace Araponga.Application.Models;

/// <summary>
/// Modelo para exportação de dados do usuário (LGPD).
/// </summary>
public sealed record UserDataExport
{
    public UserExportData User { get; init; } = null!;
    public IReadOnlyList<MembershipExportData> Memberships { get; init; } = Array.Empty<MembershipExportData>();
    public IReadOnlyList<PostExportData> Posts { get; init; } = Array.Empty<PostExportData>();
    public IReadOnlyList<EventExportData> Events { get; init; } = Array.Empty<EventExportData>();
    public IReadOnlyList<EventParticipationExportData> EventParticipations { get; init; } = Array.Empty<EventParticipationExportData>();
    public IReadOnlyList<NotificationExportData> Notifications { get; init; } = Array.Empty<NotificationExportData>();
    public PreferencesExportData? Preferences { get; init; }
    public IReadOnlyList<TermsAcceptanceExportData> TermsAcceptances { get; init; } = Array.Empty<TermsAcceptanceExportData>();
    public IReadOnlyList<PrivacyPolicyAcceptanceExportData> PrivacyPolicyAcceptances { get; init; } = Array.Empty<PrivacyPolicyAcceptanceExportData>();
    public DateTime ExportedAtUtc { get; init; }
}

public sealed record UserExportData
{
    public Guid Id { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Address { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}

public sealed record MembershipExportData
{
    public Guid Id { get; init; }
    public Guid TerritoryId { get; init; }
    public string Role { get; init; } = string.Empty;
    public string ResidencyVerification { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? LastGeoVerifiedAtUtc { get; init; }
    public DateTime? LastDocumentVerifiedAtUtc { get; init; }
}

public sealed record PostExportData
{
    public Guid Id { get; init; }
    public Guid TerritoryId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Visibility { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? EditedAtUtc { get; init; }
    public int EditCount { get; init; }
}

public sealed record EventExportData
{
    public Guid Id { get; init; }
    public Guid TerritoryId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime StartsAtUtc { get; init; }
    public DateTime? EndsAtUtc { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}

public sealed record EventParticipationExportData
{
    public Guid EventId { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime UpdatedAtUtc { get; init; }
}

public sealed record NotificationExportData
{
    public Guid Id { get; init; }
    public string Kind { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string? Body { get; init; }
    public string? DataJson { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime? ReadAtUtc { get; init; }
}

public sealed record PreferencesExportData
{
    public string ProfileVisibility { get; init; } = string.Empty;
    public string ContactVisibility { get; init; } = string.Empty;
    public bool ShareLocation { get; init; }
    public bool ShowMemberships { get; init; }
    public NotificationPreferencesExportData NotificationPreferences { get; init; } = null!;
    public DateTime CreatedAtUtc { get; init; }
    public DateTime UpdatedAtUtc { get; init; }
}

public sealed record NotificationPreferencesExportData
{
    public bool PostsEnabled { get; init; }
    public bool CommentsEnabled { get; init; }
    public bool EventsEnabled { get; init; }
    public bool AlertsEnabled { get; init; }
    public bool MarketplaceEnabled { get; init; }
    public bool ModerationEnabled { get; init; }
    public bool MembershipRequestsEnabled { get; init; }
}

public sealed record TermsAcceptanceExportData
{
    public Guid Id { get; init; }
    public Guid TermsOfServiceId { get; init; }
    public string AcceptedVersion { get; init; } = string.Empty;
    public DateTime AcceptedAtUtc { get; init; }
    public bool IsRevoked { get; init; }
    public DateTime? RevokedAtUtc { get; init; }
}

public sealed record PrivacyPolicyAcceptanceExportData
{
    public Guid Id { get; init; }
    public Guid PrivacyPolicyId { get; init; }
    public string AcceptedVersion { get; init; } = string.Empty;
    public DateTime AcceptedAtUtc { get; init; }
    public bool IsRevoked { get; init; }
    public DateTime? RevokedAtUtc { get; init; }
}
