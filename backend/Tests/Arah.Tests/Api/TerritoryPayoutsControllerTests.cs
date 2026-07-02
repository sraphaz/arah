using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using Arah.Api.Contracts.Transactions;
using Arah.Infrastructure.InMemory;
using Arah.Modules.Marketplace.Domain;
using Arah.Tests.Shared;
using Xunit;

namespace Arah.Tests.Api;

/// <summary>
/// Integração HTTP FASE55 — AC-55-5 (payout consolidado por período).
/// Territory1 é Active => FeeSplitRule 40/30/30 vigente há 1 ano.
/// </summary>
public sealed class TerritoryPayoutsControllerTests
{
    private static void SeedCheckout(
        InMemoryDataStore store,
        CheckoutStatus status,
        decimal subtotal,
        decimal fee,
        DateTime updatedAtUtc)
    {
        store.Checkouts.Add(new Checkout(
            Guid.NewGuid(),
            territoryId: TestIds.Territory1,
            buyerUserId: Guid.NewGuid(),
            storeId: Guid.NewGuid(),
            status: status,
            currency: "BRL",
            itemsSubtotalAmount: subtotal,
            platformFeeAmount: fee,
            totalAmount: subtotal + fee,
            createdAtUtc: updatedAtUtc,
            updatedAtUtc: updatedAtUtc));
    }

    private static string Iso(DateTimeOffset value) =>
        value.ToString("o", CultureInfo.InvariantCulture);

    [Fact] // AC-55-5
    public async Task Consolidated_SumsPaidSplitByRecipient_ExcludesRefundedAndOutOfPeriod()
    {
        using var factory = new ApiFactory();
        var store = factory.GetDataStore();
        var now = DateTime.UtcNow;

        SeedCheckout(store, CheckoutStatus.Paid, 100m, 10m, now.AddDays(-1));
        SeedCheckout(store, CheckoutStatus.Paid, 100m, 10m, now.AddDays(-2));
        SeedCheckout(store, CheckoutStatus.Refunded, 100m, 10m, now.AddDays(-1)); // excluído
        SeedCheckout(store, CheckoutStatus.Paid, 100m, 10m, now.AddDays(-30)); // fora do período

        var from = new DateTimeOffset(now.AddDays(-7), TimeSpan.Zero);
        var to = new DateTimeOffset(now.AddDays(1), TimeSpan.Zero);

        using var client = factory.CreateClient();
        var response = await client.GetAsync(
            $"api/v1/territories/{TestIds.Territory1}/payouts/consolidated?from={Uri.EscapeDataString(Iso(from))}&to={Uri.EscapeDataString(Iso(to))}");

        response.EnsureSuccessStatusCode();
        var payout = await response.Content.ReadFromJsonAsync<ConsolidatedPayoutResponse>();

        Assert.NotNull(payout);
        Assert.Equal(2, payout!.TransactionCount);
        Assert.Equal(200m, payout.TotalGrossAmount);
        Assert.Equal(20m, payout.TotalFeeAmount);
        Assert.Equal(8m, payout.Lines.First(l => l.Recipient == "implementer").Amount);
        Assert.Equal(6m, payout.Lines.First(l => l.Recipient == "territory_fund").Amount);
        Assert.Equal(6m, payout.Lines.First(l => l.Recipient == "platform").Amount);
        // Reconciliação: soma consolidada == taxa total.
        Assert.Equal(payout.TotalFeeAmount, payout.Lines.Sum(l => l.Amount));
    }

    [Fact] // AC-55-5
    public async Task Consolidated_WhenNoPaidInPeriod_ReturnsEmpty()
    {
        using var factory = new ApiFactory();
        var now = DateTimeOffset.UtcNow;

        using var client = factory.CreateClient();
        var response = await client.GetAsync(
            $"api/v1/territories/{TestIds.Territory1}/payouts/consolidated?from={Uri.EscapeDataString(Iso(now.AddDays(-1)))}&to={Uri.EscapeDataString(Iso(now))}");

        response.EnsureSuccessStatusCode();
        var payout = await response.Content.ReadFromJsonAsync<ConsolidatedPayoutResponse>();

        Assert.NotNull(payout);
        Assert.Equal(0, payout!.TransactionCount);
        Assert.Empty(payout.Lines);
    }

    [Fact] // AC-55-5
    public async Task Consolidated_WhenFromAfterTo_Returns400()
    {
        using var factory = new ApiFactory();
        var now = DateTimeOffset.UtcNow;

        using var client = factory.CreateClient();
        var response = await client.GetAsync(
            $"api/v1/territories/{TestIds.Territory1}/payouts/consolidated?from={Uri.EscapeDataString(Iso(now))}&to={Uri.EscapeDataString(Iso(now.AddDays(-1)))}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
