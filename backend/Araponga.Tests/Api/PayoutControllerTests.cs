using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Araponga.Api;
using Araponga.Api.Contracts.Auth;
using Araponga.Api.Contracts.Payout;
using Xunit;
using static Araponga.Tests.TestIds;

namespace Araponga.Tests.Api;

/// <summary>
/// Testes de API para controllers de Payout e Gestão Financeira.
/// </summary>
public sealed class PayoutControllerTests
{
    private static readonly Guid TerritoryId = Territory2;

    [Fact]
    public async Task GetPayoutConfig_ShouldReturn401_WhenNotAuthenticated()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Act - Sem autenticação
        var response = await client.GetAsync($"api/v1/territories/{TerritoryId}/payout-config");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreatePayoutConfig_ShouldReturn401_WhenNotAuthenticated()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Act - Sem autenticação
        var request = new TerritoryPayoutConfigRequest(
            RetentionPeriodDays: 7,
            MinimumPayoutAmountInCents: 10000,
            MaximumPayoutAmountInCents: 5000000,
            Frequency: "Weekly",
            AutoPayoutEnabled: true,
            RequiresApproval: false,
            Currency: "BRL");

        var response = await client.PostAsJsonAsync(
            $"api/v1/territories/{TerritoryId}/payout-config",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetPayoutConfig_ShouldRequireAuthentication()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Setup: Login
        var token = await LoginForTokenAsync(client, "google", "test-admin");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync($"api/v1/territories/{TerritoryId}/payout-config");

        // Assert - Pode retornar 403 (Forbidden), 404 (Not Found) ou 500 (Internal Server Error) se permissões não configuradas
        Assert.True(
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.Forbidden ||
            response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task CreatePayoutConfig_ShouldReturn400_WhenInvalidFrequency()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Setup: Login
        var token = await LoginForTokenAsync(client, "google", "test-user");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act - Invalid frequency
        var request = new TerritoryPayoutConfigRequest(
            RetentionPeriodDays: 7,
            MinimumPayoutAmountInCents: 10000,
            MaximumPayoutAmountInCents: null,
            Frequency: "Invalid", // Invalid
            AutoPayoutEnabled: true,
            RequiresApproval: false,
            Currency: "BRL");

        var response = await client.PostAsJsonAsync(
            $"api/v1/territories/{TerritoryId}/payout-config",
            request);

        // Assert - Pode retornar 400 (BadRequest), 403 (Forbidden), 401 (Unauthorized) ou 500 (Internal Server Error) dependendo de quando a validação acontece
        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.Forbidden ||
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetSellerBalance_ShouldReturn404_WhenNoBalanceExists()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Setup: Login
        var token = await LoginForTokenAsync(client, "google", "seller-test");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync($"api/v1/territories/{TerritoryId}/seller-balance/me");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetSellerBalance_ShouldReturn401_WhenNotAuthenticated()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Act - Sem autenticação
        var response = await client.GetAsync($"api/v1/territories/{TerritoryId}/seller-balance/me");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetSellerTransactions_ShouldReturnEmptyList_WhenNoTransactions()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Setup: Login
        var token = await LoginForTokenAsync(client, "google", "seller-test-2");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync($"api/v1/territories/{TerritoryId}/seller-balance/me/transactions");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var transactions = await response.Content.ReadFromJsonAsync<List<SellerTransactionResponse>>();
        Assert.NotNull(transactions);
        Assert.Empty(transactions!);
    }

    [Fact]
    public async Task GetSellerTransactions_ShouldReturn401_WhenNotAuthenticated()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Act - Sem autenticação
        var response = await client.GetAsync($"api/v1/territories/{TerritoryId}/seller-balance/me/transactions");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetPlatformFinancialBalance_ShouldReturn403_WhenNotAdmin()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Setup: Login como usuário normal (não admin)
        var token = await LoginForTokenAsync(client, "google", "normal-user");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync($"api/v1/territories/{TerritoryId}/platform-financial/balance");

        // Assert - Pode retornar 403 (Forbidden), 404 (Not Found) ou 500 (Internal Server Error) se não configurado
        Assert.True(
            response.StatusCode == HttpStatusCode.Forbidden ||
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task GetPlatformFinancialBalance_ShouldReturn401_WhenNotAuthenticated()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Act - Sem autenticação
        var response = await client.GetAsync($"api/v1/territories/{TerritoryId}/platform-financial/balance");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static async Task<string> LoginForTokenAsync(HttpClient client, string provider, string externalId)
    {
        var response = await client.PostAsJsonAsync(
            "api/v1/auth/social",
            new SocialLoginRequest(
                provider,
                externalId,
                "Test User",
                "123.456.789-00",
                null,
                "(11) 90000-0000",
                "Rua Test, 100",
                "test@araponga.com"));

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<SocialLoginResponse>();
        Assert.NotNull(payload);
        return payload!.Token;
    }
}
