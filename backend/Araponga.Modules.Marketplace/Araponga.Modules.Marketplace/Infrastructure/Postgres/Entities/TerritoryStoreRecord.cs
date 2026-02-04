using Araponga.Modules.Marketplace.Domain;

namespace Araponga.Modules.Marketplace.Infrastructure.Postgres.Entities;

public sealed class TerritoryStoreRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public Guid OwnerUserId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public StoreStatus Status { get; set; }
    public bool PaymentsEnabled { get; set; }
    public StoreContactVisibility ContactVisibility { get; set; }
    public string? Phone { get; set; }
    public string? Whatsapp { get; set; }
    public string? Email { get; set; }
    public string? Instagram { get; set; }
    public string? Website { get; set; }
    public string? PreferredContactMethod { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
}
