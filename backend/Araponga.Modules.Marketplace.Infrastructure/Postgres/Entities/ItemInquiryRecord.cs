using Araponga.Domain.Marketplace;

namespace Araponga.Modules.Marketplace.Infrastructure.Postgres.Entities;

public sealed class ItemInquiryRecord
{
    public Guid Id { get; set; }
    public Guid TerritoryId { get; set; }
    public Guid ItemId { get; set; }
    public Guid StoreId { get; set; }
    public Guid FromUserId { get; set; }
    public string? Message { get; set; }
    public InquiryStatus Status { get; set; }
    public Guid? BatchId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
