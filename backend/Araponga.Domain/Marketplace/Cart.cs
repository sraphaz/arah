namespace Araponga.Domain.Marketplace;

public sealed class Cart
{
    public Cart(
        Guid id,
        Guid territoryId,
        Guid userId,
        DateTime createdAtUtc,
        DateTime updatedAtUtc)
    {
        if (territoryId == Guid.Empty)
        {
            throw new ArgumentException("Territory ID is required.", nameof(territoryId));
        }

        if (userId == Guid.Empty)
        {
            throw new ArgumentException("User ID is required.", nameof(userId));
        }

        Id = id;
        TerritoryId = territoryId;
        UserId = userId;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    public Guid Id { get; }
    public Guid TerritoryId { get; }
    public Guid UserId { get; }
    public DateTime CreatedAtUtc { get; }
    public DateTime UpdatedAtUtc { get; private set; }

    public void Touch(DateTime updatedAtUtc)
    {
        UpdatedAtUtc = updatedAtUtc;
    }
}
