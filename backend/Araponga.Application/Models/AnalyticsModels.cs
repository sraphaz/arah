namespace Araponga.Application.Models;

/// <summary>
/// Modelos para analytics e métricas de negócio.
/// </summary>
public sealed record TerritoryStats
{
    public Guid TerritoryId { get; init; }
    public string TerritoryName { get; init; } = string.Empty;
    public int TotalPosts { get; init; }
    public int TotalEvents { get; init; }
    public int TotalMembers { get; init; }
    public int NewMembersLast30Days { get; init; }
    public decimal TotalSalesAmount { get; init; }
    public int TotalSalesCount { get; init; }
    public decimal TotalPayoutsAmount { get; init; }
    public int TotalPayoutsCount { get; init; }
    public DateTime? PeriodStart { get; init; }
    public DateTime? PeriodEnd { get; init; }
}

public sealed record PlatformStats
{
    public int TotalTerritories { get; init; }
    public int TotalUsers { get; init; }
    public int TotalPosts { get; init; }
    public int TotalEvents { get; init; }
    public decimal TotalSalesAmount { get; init; }
    public int TotalSalesCount { get; init; }
    public decimal TotalPayoutsAmount { get; init; }
    public int TotalPayoutsCount { get; init; }
    public DateTime? PeriodStart { get; init; }
    public DateTime? PeriodEnd { get; init; }
}

public sealed record MarketplaceStats
{
    public int TotalStores { get; init; }
    public int TotalItems { get; init; }
    public decimal TotalSalesAmount { get; init; }
    public int TotalSalesCount { get; init; }
    public decimal TotalPayoutsAmount { get; init; }
    public int TotalPayoutsCount { get; init; }
    public Guid? TerritoryId { get; init; }
    public DateTime? PeriodStart { get; init; }
    public DateTime? PeriodEnd { get; init; }
}
