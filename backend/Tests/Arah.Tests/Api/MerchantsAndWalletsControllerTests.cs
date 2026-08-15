using System.Net;
using System.Net.Http.Json;
using Arah.Api.Contracts.Subscriptions;
using Arah.Api.Contracts.Transactions;
using Arah.Infrastructure.InMemory;
using Arah.Modules.Marketplace.Domain;
using Arah.Tests.ApiSupport;
using Arah.Tests.Shared;
using Xunit;

namespace Arah.Tests.Api;

/// <summary>
/// FASE55 — AC-55-9 (alias merchant subscription), AC-55-10 (wallet), AC-55-11 (consumption).
/// </summary>
public sealed class MerchantsAndWalletsControllerTests
{
    [Fact] // AC-55-9
    public async Task CreateMerchantSubscription_AsOwner_CreatesOrRejectsLikeSubscriptions()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var login = await AuthTestHelper.LoginAndGetResponseAsync(client, "google", "merchant-sub-owner", "merchant@Arah.com");
        AuthTestHelper.SetAuthHeader(client, login.Token!);

        var storeId = Guid.NewGuid();
        factory.GetDataStore().TerritoryStores.Add(new Store(
            storeId,
            TestIds.Territory1,
            login.User!.Id,
            "Loja AC-55-9",
            null,
            StoreStatus.Active,
            paymentsEnabled: false,
            StoreContactVisibility.Public,
            null, null, null, null, null, null,
            DateTime.UtcNow,
            DateTime.UtcNow));

        var plans = await client.GetFromJsonAsync<List<SubscriptionPlanResponse>>("api/v1/subscription-plans");
        Assert.NotNull(plans);
        var freePlan = plans!.First(p => p.Tier == "FREE");

        var response = await client.PostAsJsonAsync(
            $"api/v1/merchants/{storeId}/subscription",
            new CreateMerchantSubscriptionRequest { PlanId = freePlan.Id });

        Assert.True(
            response.StatusCode == HttpStatusCode.Created ||
            response.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact] // AC-55-9
    public async Task CreateMerchantSubscription_WhenNotOwner_ReturnsForbidden()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var login = await AuthTestHelper.LoginAndGetResponseAsync(client, "google", "merchant-sub-other", "other@Arah.com");
        AuthTestHelper.SetAuthHeader(client, login.Token!);

        var storeId = Guid.NewGuid();
        factory.GetDataStore().TerritoryStores.Add(new Store(
            storeId,
            TestIds.Territory1,
            Guid.NewGuid(),
            "Loja alheia",
            null,
            StoreStatus.Active,
            false,
            StoreContactVisibility.Public,
            null, null, null, null, null, null,
            DateTime.UtcNow,
            DateTime.UtcNow));

        var response = await client.PostAsJsonAsync(
            $"api/v1/merchants/{storeId}/subscription",
            new CreateMerchantSubscriptionRequest { PlanId = Guid.NewGuid() });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact] // AC-55-11
    public async Task GetMerchantConsumption_AsOwner_ReturnsZeroedMeters()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var login = await AuthTestHelper.LoginAndGetResponseAsync(client, "google", "merchant-cons-owner", "cons@Arah.com");
        AuthTestHelper.SetAuthHeader(client, login.Token!);

        var storeId = Guid.NewGuid();
        factory.GetDataStore().TerritoryStores.Add(new Store(
            storeId,
            TestIds.Territory1,
            login.User!.Id,
            "Loja consumo",
            null,
            StoreStatus.Active,
            false,
            StoreContactVisibility.Public,
            null, null, null, null, null, null,
            DateTime.UtcNow,
            DateTime.UtcNow));

        var response = await client.GetAsync($"api/v1/merchants/{storeId}/consumption");
        response.EnsureSuccessStatusCode();
        var meters = await response.Content.ReadFromJsonAsync<List<ConsumptionMeterResponse>>();
        Assert.NotNull(meters);
        Assert.Equal(3, meters!.Count);
        Assert.Contains(meters, m => m.Metric == "ai" && m.Usage == 0);
        Assert.Contains(meters, m => m.Metric == "media");
        Assert.Contains(meters, m => m.Metric == "notifications");
    }

    [Fact] // AC-55-10
    public async Task GetWallet_ProjectsSellerBalance_ForOwner()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var login = await AuthTestHelper.LoginAndGetResponseAsync(client, "google", "wallet-owner", "wallet@Arah.com");
        AuthTestHelper.SetAuthHeader(client, login.Token!);

        var balanceId = Guid.NewGuid();
        var balance = new SellerBalance(balanceId, TestIds.Territory1, login.User!.Id, "BRL");
        balance.AddPendingAmount(1500);
        factory.GetDataStore().SellerBalances.Add(balance);

        var response = await client.GetAsync($"api/v1/wallets/{balanceId}");
        response.EnsureSuccessStatusCode();
        var wallet = await response.Content.ReadFromJsonAsync<WalletResponse>();
        Assert.NotNull(wallet);
        Assert.Equal(balanceId, wallet!.Id);
        Assert.Equal("seller", wallet.OwnerType);
        Assert.Equal(login.User.Id, wallet.OwnerId);
        Assert.Equal(15.00m, wallet.Balance);
        Assert.Equal("BRL", wallet.Currency);
    }

    [Fact] // AC-55-10
    public async Task GetWallet_WhenNotOwner_ReturnsForbidden()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var login = await AuthTestHelper.LoginAndGetResponseAsync(client, "google", "wallet-other", "wother@Arah.com");
        AuthTestHelper.SetAuthHeader(client, login.Token!);

        var balanceId = Guid.NewGuid();
        factory.GetDataStore().SellerBalances.Add(
            new SellerBalance(balanceId, TestIds.Territory1, Guid.NewGuid(), "BRL"));

        var response = await client.GetAsync($"api/v1/wallets/{balanceId}");
        Assert.True(
            response.StatusCode == HttpStatusCode.Forbidden ||
            response.StatusCode == HttpStatusCode.NotFound);
    
    [Fact] // AC-55-9 Location
    public async Task CreateMerchantSubscription_AsOwner_ReturnsAbsoluteLocation()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var login = await AuthTestHelper.LoginAndGetResponseAsync(client, "google", "merchant-loc-owner", "mloc@Arah.com");
        AuthTestHelper.SetAuthHeader(client, login.Token!);

        var storeId = Guid.NewGuid();
        factory.GetDataStore().TerritoryStores.Add(new Store(
            storeId,
            TestIds.Territory1,
            login.User!.Id,
            "Loja Location",
            null,
            StoreStatus.Active,
            paymentsEnabled: false,
            StoreContactVisibility.Public,
            null, null, null, null, null, null,
            DateTime.UtcNow,
            DateTime.UtcNow));

        var plans = await client.GetFromJsonAsync<List<SubscriptionPlanResponse>>("api/v1/subscription-plans");
        Assert.NotNull(plans);
        var freePlan = plans!.First(p => p.Tier == "FREE");

        var response = await client.PostAsJsonAsync(
            $"api/v1/merchants/{storeId}/subscription",
            new CreateMerchantSubscriptionRequest { PlanId = freePlan.Id });

        if (response.StatusCode == HttpStatusCode.Created)
        {
            Assert.NotNull(response.Headers.Location);
            Assert.StartsWith("/api/v1/subscriptions/", response.Headers.Location!.ToString());
        }
        else
        {
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }

    [Fact] // AC-55-10
    public async Task GetWallet_ExcludesPaidAmount_FromBalance()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var login = await AuthTestHelper.LoginAndGetResponseAsync(client, "google", "wallet-paid", "wpaid@Arah.com");
        AuthTestHelper.SetAuthHeader(client, login.Token!);

        var balanceId = Guid.NewGuid();
        var balance = new SellerBalance(balanceId, TestIds.Territory1, login.User!.Id, "BRL");
        balance.AddPendingAmount(5000);
        balance.MoveToReadyForPayout(2000);
        balance.MarkAsPaid(2000);
        balance.AddPendingAmount(1500);
        factory.GetDataStore().SellerBalances.Add(balance);

        var response = await client.GetAsync($"api/v1/wallets/{balanceId}");
        response.EnsureSuccessStatusCode();
        var wallet = await response.Content.ReadFromJsonAsync<WalletResponse>();
        Assert.NotNull(wallet);
        // Available = pending(1500) + ready(0) = 15.00; paid 20.00 must not inflate balance
        Assert.Equal(15.00m, wallet!.Balance);
    }

    [Fact] // AC-55-10
    public async Task GetWallet_RefreshesBalance_AfterSellerBalanceChanges()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var login = await AuthTestHelper.LoginAndGetResponseAsync(client, "google", "wallet-refresh", "wref@Arah.com");
        AuthTestHelper.SetAuthHeader(client, login.Token!);

        var balanceId = Guid.NewGuid();
        var balance = new SellerBalance(balanceId, TestIds.Territory1, login.User!.Id, "BRL");
        balance.AddPendingAmount(1000);
        factory.GetDataStore().SellerBalances.Add(balance);

        var first = await client.GetAsync($"api/v1/wallets/{balanceId}");
        first.EnsureSuccessStatusCode();
        var wallet1 = await first.Content.ReadFromJsonAsync<WalletResponse>();
        Assert.Equal(10.00m, wallet1!.Balance);

        balance.AddPendingAmount(2500);

        var second = await client.GetAsync($"api/v1/wallets/{balanceId}");
        second.EnsureSuccessStatusCode();
        var wallet2 = await second.Content.ReadFromJsonAsync<WalletResponse>();
        Assert.Equal(35.00m, wallet2!.Balance);
    }

    [Fact] // AC-55-11
    public async Task GetMerchantConsumption_ConcurrentFirstReads_DoNotDuplicateMeters()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var login = await AuthTestHelper.LoginAndGetResponseAsync(client, "google", "merchant-cons-race", "consrace@Arah.com");
        AuthTestHelper.SetAuthHeader(client, login.Token!);

        var storeId = Guid.NewGuid();
        factory.GetDataStore().TerritoryStores.Add(new Store(
            storeId,
            TestIds.Territory1,
            login.User!.Id,
            "Loja race",
            null,
            StoreStatus.Active,
            false,
            StoreContactVisibility.Public,
            null, null, null, null, null, null,
            DateTime.UtcNow,
            DateTime.UtcNow));

        var tasks = Enumerable.Range(0, 8)
            .Select(_ => client.GetAsync($"api/v1/merchants/{storeId}/consumption"))
            .ToArray();
        var responses = await Task.WhenAll(tasks);
        foreach (var response in responses)
        {
            response.EnsureSuccessStatusCode();
            var meters = await response.Content.ReadFromJsonAsync<List<ConsumptionMeterResponse>>();
            Assert.NotNull(meters);
            Assert.Equal(3, meters!.Count);
            Assert.Equal(3, meters.Select(m => m.Metric).Distinct().Count());
        }
    }
}
