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
/// Integração HTTP FASE55 — AC-55-2 (quote) e AC-55-3 (receipt).
/// Territory1 é Active => FeeSplitRule 40/30/30 já semeada pelo InMemoryDataStore.
/// </summary>
public sealed class TransactionsControllerTests
{
    private static void SeedCheckout(InMemoryDataStore store, Guid checkoutId, CheckoutStatus status)
    {
        store.Checkouts.Add(new Checkout(
            checkoutId,
            territoryId: TestIds.Territory1,
            buyerUserId: Guid.NewGuid(),
            storeId: Guid.NewGuid(),
            status: status,
            currency: "BRL",
            itemsSubtotalAmount: 100m,
            platformFeeAmount: 10m,
            totalAmount: 110m,
            createdAtUtc: DateTime.UtcNow,
            updatedAtUtc: DateTime.UtcNow));
    }

    private static (Guid StoreId, Guid CheckoutId) SeedCheckoutWithStore(InMemoryDataStore store, CheckoutStatus status)
    {
        var storeId = Guid.NewGuid();
        var checkoutId = Guid.NewGuid();
        store.TerritoryStores.Add(new Store(
            storeId,
            TestIds.Territory1,
            TestIds.ResidentUser,
            "HTTP Payment Store",
            null,
            StoreStatus.Active,
            true,
            StoreContactVisibility.Public,
            null,
            null,
            null,
            null,
            null,
            null,
            DateTime.UtcNow,
            DateTime.UtcNow));
        SeedCheckout(store, checkoutId, status, storeId);
        return (storeId, checkoutId);
    }

    private static void SeedCheckout(
        InMemoryDataStore store,
        Guid checkoutId,
        CheckoutStatus status,
        Guid storeId)
    {
        store.Checkouts.Add(new Checkout(
            checkoutId,
            territoryId: TestIds.Territory1,
            buyerUserId: Guid.NewGuid(),
            storeId: storeId,
            status: status,
            currency: "BRL",
            itemsSubtotalAmount: 100m,
            platformFeeAmount: 10m,
            totalAmount: 110m,
            createdAtUtc: DateTime.UtcNow,
            updatedAtUtc: DateTime.UtcNow));
    }

    [Fact] // AC-55-2
    public async Task Quote_WhenCheckoutFinalized_Returns200WithSplitByActiveRule()
    {
        using var factory = new ApiFactory();
        var checkoutId = Guid.NewGuid();
        SeedCheckout(factory.GetDataStore(), checkoutId, CheckoutStatus.Created);

        using var client = factory.CreateClient();
        var response = await client.PostAsync($"api/v1/transactions/{checkoutId}/quote", content: null);

        response.EnsureSuccessStatusCode();
        var quote = await response.Content.ReadFromJsonAsync<TransactionQuoteResponse>();

        Assert.NotNull(quote);
        Assert.Equal(TestIds.Territory1, quote!.TerritoryId);
        Assert.Equal(100m, quote.GrossAmount);
        Assert.Equal(10m, quote.FeeAmount);
        Assert.Equal(90m, quote.NetToSeller);
        Assert.Equal(3, quote.Split.Count);
        Assert.Equal(4m, quote.Split.First(s => s.Recipient == "implementer").Amount);
        Assert.Equal(3m, quote.Split.First(s => s.Recipient == "territory_fund").Amount);
        Assert.Equal(3m, quote.Split.First(s => s.Recipient == "platform").Amount);
        // Reconciliação: soma dos splits == taxa.
        Assert.Equal(quote.FeeAmount, quote.Split.Sum(s => s.Amount));
        Assert.NotEqual(Guid.Empty, quote.FeeSplitRuleId);
    }

    [Fact] // AC-55-2
    public async Task Quote_WhenTransactionMissing_Returns404()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsync($"api/v1/transactions/{Guid.NewGuid()}/quote", content: null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact] // AC-55-3
    public async Task Receipt_WhenCheckoutPaid_Returns200WithPaidAt()
    {
        using var factory = new ApiFactory();
        var checkoutId = Guid.NewGuid();
        SeedCheckout(factory.GetDataStore(), checkoutId, CheckoutStatus.Paid);

        using var client = factory.CreateClient();
        var response = await client.GetAsync($"api/v1/transactions/{checkoutId}/receipt");

        response.EnsureSuccessStatusCode();
        var receipt = await response.Content.ReadFromJsonAsync<TransactionReceiptResponse>();

        Assert.NotNull(receipt);
        Assert.Equal("Paid", receipt!.Status);
        Assert.NotNull(receipt.PaidAtUtc);
        Assert.Equal(3, receipt.Split.Count);
        Assert.Equal(receipt.FeeAmount, receipt.Split.Sum(s => s.Amount));
    }

    [Fact] // AC-55-3
    public async Task Receipt_WhenCheckoutNotPaid_Returns400()
    {
        using var factory = new ApiFactory();
        var checkoutId = Guid.NewGuid();
        SeedCheckout(factory.GetDataStore(), checkoutId, CheckoutStatus.Created);

        using var client = factory.CreateClient();
        var response = await client.GetAsync($"api/v1/transactions/{checkoutId}/receipt");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact] // AC-55-3
    public async Task Receipt_WhenTransactionMissing_Returns404()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"api/v1/transactions/{Guid.NewGuid()}/receipt");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact] // AC-55-6
    public async Task Refund_WhenPaid_Returns200AndReversesFeeAndSplitProportionally()
    {
        using var factory = new ApiFactory();
        var checkoutId = Guid.NewGuid();
        SeedCheckout(factory.GetDataStore(), checkoutId, CheckoutStatus.Paid);

        using var client = factory.CreateClient();
        var response = await client.PostAsync($"api/v1/transactions/{checkoutId}/refund", content: null);

        response.EnsureSuccessStatusCode();
        var refund = await response.Content.ReadFromJsonAsync<TransactionRefundResponse>();

        Assert.NotNull(refund);
        Assert.Equal("Refunded", refund!.Status);
        Assert.Equal(-10m, refund.ReversedFeeAmount);
        Assert.Equal(-90m, refund.ReversedNetToSeller);
        Assert.Equal(3, refund.ReversedSplit.Count);
        Assert.Equal(-4m, refund.ReversedSplit.First(s => s.Recipient == "implementer").Amount);
        Assert.Equal(-3m, refund.ReversedSplit.First(s => s.Recipient == "territory_fund").Amount);
        Assert.Equal(-3m, refund.ReversedSplit.First(s => s.Recipient == "platform").Amount);
        // Reconciliação: soma das reversões == taxa revertida.
        Assert.Equal(refund.ReversedFeeAmount, refund.ReversedSplit.Sum(s => s.Amount));
    }

    [Fact] // AC-55-6
    public async Task Refund_WhenAlreadyRefunded_Returns409()
    {
        using var factory = new ApiFactory();
        var checkoutId = Guid.NewGuid();
        SeedCheckout(factory.GetDataStore(), checkoutId, CheckoutStatus.Paid);

        using var client = factory.CreateClient();
        var first = await client.PostAsync($"api/v1/transactions/{checkoutId}/refund", content: null);
        first.EnsureSuccessStatusCode();

        var second = await client.PostAsync($"api/v1/transactions/{checkoutId}/refund", content: null);

        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact] // AC-55-6
    public async Task Refund_WhenNotPaid_Returns400()
    {
        using var factory = new ApiFactory();
        var checkoutId = Guid.NewGuid();
        SeedCheckout(factory.GetDataStore(), checkoutId, CheckoutStatus.Created);

        using var client = factory.CreateClient();
        var response = await client.PostAsync($"api/v1/transactions/{checkoutId}/refund", content: null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact] // AC-55-6
    public async Task Refund_WhenTransactionMissing_Returns404()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsync($"api/v1/transactions/{Guid.NewGuid()}/refund", content: null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact] // DOD-05
    public async Task Pay_WhenCheckoutCreated_Returns200WithGatewayPaymentId()
    {
        using var factory = new ApiFactory();
        var (_, checkoutId) = SeedCheckoutWithStore(factory.GetDataStore(), CheckoutStatus.Created);

        using var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync(
            $"api/v1/transactions/{checkoutId}/pay",
            new TransactionPayRequest("pix"));

        response.EnsureSuccessStatusCode();
        var payment = await response.Content.ReadFromJsonAsync<TransactionPayResponse>();

        Assert.NotNull(payment);
        Assert.Equal("AwaitingPayment", payment!.Status);
        Assert.False(string.IsNullOrWhiteSpace(payment.GatewayPaymentId));
        Assert.NotNull(payment.PixCopyPasteCode);
    }

    [Fact] // DOD-05
    public async Task PayThenMockWebhook_ThenReceipt_ReturnsPaid()
    {
        using var factory = new ApiFactory();
        var (_, checkoutId) = SeedCheckoutWithStore(factory.GetDataStore(), CheckoutStatus.Created);

        using var client = factory.CreateClient();
        var payResponse = await client.PostAsJsonAsync(
            $"api/v1/transactions/{checkoutId}/pay",
            new TransactionPayRequest("pix"));
        payResponse.EnsureSuccessStatusCode();
        var payment = await payResponse.Content.ReadFromJsonAsync<TransactionPayResponse>();

        var webhookResponse = await client.PostAsJsonAsync(
            "api/v1/webhooks/checkout/mock-approved",
            new { checkoutId, gatewayPaymentId = payment!.GatewayPaymentId });
        webhookResponse.EnsureSuccessStatusCode();

        var receiptResponse = await client.GetAsync($"api/v1/transactions/{checkoutId}/receipt");
        receiptResponse.EnsureSuccessStatusCode();
        var receipt = await receiptResponse.Content.ReadFromJsonAsync<TransactionReceiptResponse>();

        Assert.NotNull(receipt);
        Assert.Equal("Paid", receipt!.Status);
        Assert.NotNull(receipt.PaidAtUtc);
    }

    [Fact] // DOD-05
    public async Task ConfirmPayment_WhenAlreadyPaid_Returns200WithAlreadyPaidFlag()
    {
        using var factory = new ApiFactory();
        var (_, checkoutId) = SeedCheckoutWithStore(factory.GetDataStore(), CheckoutStatus.Created);

        using var client = factory.CreateClient();
        var payResponse = await client.PostAsJsonAsync(
            $"api/v1/transactions/{checkoutId}/pay",
            new TransactionPayRequest("pix"));
        var payment = await payResponse.Content.ReadFromJsonAsync<TransactionPayResponse>();

        await client.PostAsJsonAsync(
            "api/v1/webhooks/checkout/mock-approved",
            new { checkoutId, gatewayPaymentId = payment!.GatewayPaymentId });

        var confirmResponse = await client.PostAsJsonAsync(
            $"api/v1/transactions/{checkoutId}/confirm-payment",
            new TransactionConfirmPaymentRequest(payment.GatewayPaymentId));
        confirmResponse.EnsureSuccessStatusCode();
        var confirm = await confirmResponse.Content.ReadFromJsonAsync<TransactionConfirmPaymentResponse>();

        Assert.NotNull(confirm);
        Assert.True(confirm!.AlreadyPaid);
        Assert.Equal("Paid", confirm.Status);
    }

    [Fact] // DOD-05
    public async Task Pay_WhenTransactionMissing_Returns404()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            $"api/v1/transactions/{Guid.NewGuid()}/pay",
            new TransactionPayRequest("pix"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
