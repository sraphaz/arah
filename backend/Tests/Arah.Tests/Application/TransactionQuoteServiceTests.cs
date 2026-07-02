using Arah.Application.Interfaces;
using Arah.Application.Services;
using Arah.Domain.Financial;
using Arah.Infrastructure.InMemory;
using Arah.Modules.Marketplace.Application.Interfaces;
using Arah.Modules.Marketplace.Domain;
using Xunit;

namespace Arah.Tests.Application;

public sealed class TransactionQuoteServiceTests
{
    [Fact]
    public async Task QuoteAsync_ReturnsSplit_WhenCheckoutHasAmounts()
    {
        var store = new InMemoryDataStore();
        var checkoutId = Guid.NewGuid();
        var territoryId = store.Territories[0].Id;

        store.FeeSplitRules.Add(new FeeSplitRule(
            Guid.NewGuid(),
            territoryId,
            TransactionQuoteService.DefaultRevenueType,
            40m,
            30m,
            30m,
            DateTimeOffset.UtcNow.AddDays(-1)));

        store.Checkouts.Add(new Checkout(
            checkoutId,
            territoryId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            CheckoutStatus.Created,
            "BRL",
            itemsSubtotalAmount: 100m,
            platformFeeAmount: 10m,
            totalAmount: 110m,
            DateTime.UtcNow,
            DateTime.UtcNow));

        var checkoutRepo = new InMemoryCheckoutRepository(store);
        var feeRepo = new InMemoryFeeSplitRuleRepository(store);
        var service = new TransactionQuoteService(checkoutRepo, feeRepo);

        var result = await service.QuoteAsync(checkoutId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(100m, result.Value!.GrossAmount);
        Assert.Equal(10m, result.Value.FeeAmount);
        Assert.Equal(90m, result.Value.NetToSeller);
        Assert.Equal(3, result.Value.Split.Count);
        Assert.Equal(4m, result.Value.Split.First(s => s.Recipient == "implementer").Amount);
    }

    [Fact] // AC-55-4 — banker's rounding + reconciliação: soma dos splits == taxa
    public async Task QuoteAsync_UsesBankersRounding_AndSplitsReconcileToFee()
    {
        var store = new InMemoryDataStore();
        var checkoutId = Guid.NewGuid();
        var territoryId = store.Territories[0].Id;

        // Regra 25/25/50 sobre taxa 0.10 força meio-centavo (0.025 -> 0.02 banker's).
        store.FeeSplitRules.Clear();
        store.FeeSplitRules.Add(new FeeSplitRule(
            Guid.NewGuid(),
            territoryId,
            TransactionQuoteService.DefaultRevenueType,
            25m,
            25m,
            50m,
            DateTimeOffset.UtcNow.AddDays(-1)));

        store.Checkouts.Add(new Checkout(
            checkoutId,
            territoryId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            CheckoutStatus.Created,
            "BRL",
            itemsSubtotalAmount: 100m,
            platformFeeAmount: 0.10m,
            totalAmount: 100.10m,
            DateTime.UtcNow,
            DateTime.UtcNow));

        var service = new TransactionQuoteService(
            new InMemoryCheckoutRepository(store),
            new InMemoryFeeSplitRuleRepository(store));

        var result = await service.QuoteAsync(checkoutId, CancellationToken.None);

        Assert.True(result.IsSuccess);
        var split = result.Value!.Split;
        Assert.Equal(0.02m, split.First(s => s.Recipient == "implementer").Amount);
        Assert.Equal(0.02m, split.First(s => s.Recipient == "territory_fund").Amount);
        Assert.Equal(0.06m, split.First(s => s.Recipient == "platform").Amount);
        // Nenhum centavo perdido: soma dos splits == taxa.
        Assert.Equal(0.10m, split.Sum(s => s.Amount));
    }
}
