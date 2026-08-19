using System.Net;
using System.Net.Http.Json;
using Arah.Api.Contracts.Fiscal;
using Arah.Tests.ApiSupport;
using Arah.Tests.Shared;
using Xunit;

namespace Arah.Tests.Api;

/// <summary>
/// FASE62.0 — catálogo, binding fiscal e meios de pagamento por território.
/// </summary>
public sealed class TerritoryFiscalPackControllerTests
{
    [Fact]
    public async Task ListFiscalPacks_ReturnsBrazilV1()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("api/v1/fiscal-packs");
        response.EnsureSuccessStatusCode();

        var packs = await response.Content.ReadFromJsonAsync<List<FiscalPackResponse>>();
        Assert.NotNull(packs);
        Assert.Contains(packs!, p => p.Id == "brazil.v1" && p.CountryCode == "BR");
    }

    [Fact]
    public async Task GetFiscalPack_WithoutBinding_ReturnsLegacyOff()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var token = await AuthTestHelper.LoginForTokenAsync(client, "google", "fiscal-reader");
        AuthTestHelper.SetAuthHeader(client, token);

        var response = await client.GetAsync($"api/v1/territories/{TestIds.Territory1}/fiscal-pack");
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<TerritoryFiscalPackBindingResponse>();
        Assert.NotNull(body);
        Assert.Equal("Off", body!.Status);
        Assert.True(body.IsLegacyDefault);
        Assert.Null(body.PackId);
    }

    [Fact]
    public async Task GetPaymentMethods_WithoutConfig_ReturnsEmptyDefault()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"api/v1/territories/{TestIds.Territory1}/payment-methods");
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<TerritoryPaymentMethodsResponse>();
        Assert.NotNull(body);
        Assert.True(body!.IsDefaultEmpty);
        Assert.Empty(body.Methods);
    }

    [Fact]
    public async Task ActivateFiscalPack_WithoutPix_ReturnsBadRequest()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var token = await AuthTestHelper.LoginForTokenAsync(client, "google", "admin-external");
        AuthTestHelper.SetAuthHeader(client, token);

        var response = await client.PutAsJsonAsync(
            $"api/v1/territories/{TestIds.Territory1}/fiscal-pack",
            new UpsertTerritoryFiscalPackBindingRequest("brazil.v1", "Active", "3550704"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpsertPaymentMethods_ThenActivatePack_Succeeds()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var token = await AuthTestHelper.LoginForTokenAsync(client, "google", "admin-external");
        AuthTestHelper.SetAuthHeader(client, token);

        var methodsResponse = await client.PutAsJsonAsync(
            $"api/v1/territories/{TestIds.Territory2}/payment-methods",
            new UpsertTerritoryPaymentMethodsRequest(new[] { "Pix" }, "mock-psp"));
        methodsResponse.EnsureSuccessStatusCode();
        var methods = await methodsResponse.Content.ReadFromJsonAsync<TerritoryPaymentMethodsResponse>();
        Assert.Contains("Pix", methods!.Methods);
        Assert.False(methods.IsDefaultEmpty);

        var packResponse = await client.PutAsJsonAsync(
            $"api/v1/territories/{TestIds.Territory2}/fiscal-pack",
            new UpsertTerritoryFiscalPackBindingRequest("brazil.v1", "Active", "3550704"));
        packResponse.EnsureSuccessStatusCode();
        var pack = await packResponse.Content.ReadFromJsonAsync<TerritoryFiscalPackBindingResponse>();
        Assert.Equal("brazil.v1", pack!.PackId);
        Assert.Equal("Active", pack.Status);
        Assert.False(pack.IsLegacyDefault);
        Assert.Equal("3550704", pack.MunicipalityIbge);
    }

    [Fact]
    public async Task UpsertFiscalPack_WithoutAdmin_ReturnsForbid()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var token = await AuthTestHelper.LoginForTokenAsync(client, "google", "fiscal-non-admin");
        AuthTestHelper.SetAuthHeader(client, token);

        var response = await client.PutAsJsonAsync(
            $"api/v1/territories/{TestIds.Territory1}/fiscal-pack",
            new UpsertTerritoryFiscalPackBindingRequest("brazil.v1", "Off", null));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task RemovePix_WhilePackActive_ReturnsBadRequest()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var token = await AuthTestHelper.LoginForTokenAsync(client, "google", "admin-external");
        AuthTestHelper.SetAuthHeader(client, token);

        (await client.PutAsJsonAsync(
            $"api/v1/territories/{TestIds.Territory2}/payment-methods",
            new UpsertTerritoryPaymentMethodsRequest(new[] { "Pix" }, "mock-psp"))).EnsureSuccessStatusCode();
        (await client.PutAsJsonAsync(
            $"api/v1/territories/{TestIds.Territory2}/fiscal-pack",
            new UpsertTerritoryFiscalPackBindingRequest("brazil.v1", "Active", "3550704"))).EnsureSuccessStatusCode();

        var response = await client.PutAsJsonAsync(
            $"api/v1/territories/{TestIds.Territory2}/payment-methods",
            new UpsertTerritoryPaymentMethodsRequest(new[] { "Card" }, "mock-psp"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpsertFiscalPack_WithInvalidMunicipality_ReturnsBadRequest()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var token = await AuthTestHelper.LoginForTokenAsync(client, "google", "admin-external");
        AuthTestHelper.SetAuthHeader(client, token);

        (await client.PutAsJsonAsync(
            $"api/v1/territories/{TestIds.Territory1}/payment-methods",
            new UpsertTerritoryPaymentMethodsRequest(new[] { "Pix" }, null))).EnsureSuccessStatusCode();

        var response = await client.PutAsJsonAsync(
            $"api/v1/territories/{TestIds.Territory1}/fiscal-pack",
            new UpsertTerritoryFiscalPackBindingRequest("brazil.v1", "Active", "123"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CheckoutCanListPaymentMethods_WithoutAuth()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();
        var admin = await AuthTestHelper.LoginForTokenAsync(client, "google", "admin-external");
        AuthTestHelper.SetAuthHeader(client, admin);
        (await client.PutAsJsonAsync(
            $"api/v1/territories/{TestIds.Territory1}/payment-methods",
            new UpsertTerritoryPaymentMethodsRequest(new[] { "Pix", "Card" }, null))).EnsureSuccessStatusCode();

        client.DefaultRequestHeaders.Authorization = null;
        var response = await client.GetAsync($"api/v1/territories/{TestIds.Territory1}/payment-methods");
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<TerritoryPaymentMethodsResponse>();
        Assert.Equal(2, body!.Methods.Count);
    }
}
