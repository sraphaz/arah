namespace Arah.Core.Domain;

/// <summary>
/// Identidade federada resolvível entre instâncias (control plane — FASE53 base).
/// </summary>
public sealed class FederatedGlobalUser
{
    public Guid GlobalUserId { get; }
    public Guid HomeInstanceId { get; }
    public string DisplayName { get; }
    public DateTimeOffset CreatedAtUtc { get; }

    public FederatedGlobalUser(Guid globalUserId, Guid homeInstanceId, string displayName, DateTimeOffset createdAtUtc)
    {
        if (globalUserId == Guid.Empty) throw new ArgumentException("GlobalUserId required", nameof(globalUserId));
        if (homeInstanceId == Guid.Empty) throw new ArgumentException("HomeInstanceId required", nameof(homeInstanceId));
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        GlobalUserId = globalUserId;
        HomeInstanceId = homeInstanceId;
        DisplayName = displayName.Trim();
        CreatedAtUtc = createdAtUtc;
    }
}
