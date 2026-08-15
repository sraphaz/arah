namespace Arah.Domain.Financial;

/// <summary>
/// Carteira Aratá (FASE55) — saldo e metadados de payout por titular.
/// </summary>
public sealed class Wallet
{
    public Wallet(
        Guid id,
        string ownerType,
        Guid ownerId,
        Guid territoryId,
        decimal balance,
        string currency,
        string? payoutMethod,
        DateTimeOffset createdAtUtc,
        DateTimeOffset updatedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(ownerType))
        {
            throw new ArgumentException("Owner type is required.", nameof(ownerType));
        }

        if (ownerId == Guid.Empty)
        {
            throw new ArgumentException("Owner ID is required.", nameof(ownerId));
        }

        if (territoryId == Guid.Empty)
        {
            throw new ArgumentException("Territory ID is required.", nameof(territoryId));
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency is required.", nameof(currency));
        }

        if (balance < 0)
        {
            throw new ArgumentException("Balance cannot be negative.", nameof(balance));
        }

        Id = id;
        OwnerType = ownerType.Trim().ToLowerInvariant();
        OwnerId = ownerId;
        TerritoryId = territoryId;
        Balance = balance;
        Currency = currency.Trim().ToUpperInvariant();
        PayoutMethod = string.IsNullOrWhiteSpace(payoutMethod) ? null : payoutMethod.Trim();
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    public Guid Id { get; }
    public string OwnerType { get; }
    public Guid OwnerId { get; }
    public Guid TerritoryId { get; }
    public decimal Balance { get; private set; }
    public string Currency { get; }
    public string? PayoutMethod { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; }
    public DateTimeOffset UpdatedAtUtc { get; private set; }

    public void SetBalance(decimal balance, DateTimeOffset atUtc)
    {
        if (balance < 0)
        {
            throw new ArgumentException("Balance cannot be negative.", nameof(balance));
        }

        Balance = balance;
        UpdatedAtUtc = atUtc;
    }

    public void SetPayoutMethod(string? payoutMethod, DateTimeOffset atUtc)
    {
        PayoutMethod = string.IsNullOrWhiteSpace(payoutMethod) ? null : payoutMethod.Trim();
        UpdatedAtUtc = atUtc;
    }

    public static Wallet ForSeller(
        Guid id,
        Guid sellerUserId,
        Guid territoryId,
        decimal balance,
        string currency,
        DateTimeOffset atUtc) =>
        new(id, "seller", sellerUserId, territoryId, balance, currency, payoutMethod: null, atUtc, atUtc);
}
