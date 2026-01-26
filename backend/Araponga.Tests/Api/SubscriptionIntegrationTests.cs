using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Araponga.Api;
using Araponga.Api.Contracts.Auth;
using Araponga.Api.Contracts.Subscriptions;
using Araponga.Domain.Subscriptions;
using Xunit;

namespace Araponga.Tests.Api;

/// <summary>
/// Testes de integração end-to-end para sistema de assinaturas.
/// </summary>
public sealed class SubscriptionIntegrationTests
{
    private static async Task<string> LoginForTokenAsync(HttpClient client, string provider, string externalId, string email)
    {
        var response = await client.PostAsJsonAsync(
            "api/v1/auth/social",
            new SocialLoginRequest(
                provider,
                externalId,
                "Test User",
                "123.456.789-00",
                null,
                null,
                null,
                email));
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<SocialLoginResponse>();
        return payload!.Token;
    }


    [Fact]
    public async Task GetMySubscription_ReturnsFreePlan_WhenNoActiveSubscription()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-sub-free", "test@araponga.com");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("api/v1/subscriptions/me");

        response.EnsureSuccessStatusCode();
        var subscription = await response.Content.ReadFromJsonAsync<SubscriptionResponse>();
        Assert.NotNull(subscription);
        Assert.Equal("FREE", subscription.Tier);
        Assert.Equal("ACTIVE", subscription.Status);
    }

    [Fact]
    public async Task GetMySubscription_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("api/v1/subscriptions/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ListSubscriptionPlans_ReturnsAvailablePlans()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("api/v1/subscription-plans");

        response.EnsureSuccessStatusCode();
        var plans = await response.Content.ReadFromJsonAsync<List<SubscriptionPlanResponse>>();
        Assert.NotNull(plans);
        Assert.NotEmpty(plans);
        // Deve ter pelo menos o plano FREE
        Assert.Contains(plans, p => p.Tier == "FREE" && p.IsDefault);
    }

    [Fact]
    public async Task ListSubscriptionPlans_WithTerritoryId_ReturnsTerritoryPlans()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var territoryId = Guid.NewGuid();
        var response = await client.GetAsync($"api/v1/subscription-plans?territoryId={territoryId}");

        response.EnsureSuccessStatusCode();
        var plans = await response.Content.ReadFromJsonAsync<List<SubscriptionPlanResponse>>();
        Assert.NotNull(plans);
        // Deve retornar planos globais + territoriais (se existirem)
        Assert.NotEmpty(plans);
    }

    [Fact]
    public async Task GetSubscriptionPlan_ReturnsPlan_WhenExists()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Primeiro, listar planos para obter um ID válido
        var listResponse = await client.GetAsync("api/v1/subscription-plans");
        listResponse.EnsureSuccessStatusCode();
        var plans = await listResponse.Content.ReadFromJsonAsync<List<SubscriptionPlanResponse>>();
        Assert.NotNull(plans);
        Assert.NotEmpty(plans);

        var planId = plans[0].Id;

        // Buscar plano específico
        var getResponse = await client.GetAsync($"api/v1/subscription-plans/{planId}");

        getResponse.EnsureSuccessStatusCode();
        var plan = await getResponse.Content.ReadFromJsonAsync<SubscriptionPlanResponse>();
        Assert.NotNull(plan);
        Assert.Equal(planId, plan.Id);
    }

    [Fact]
    public async Task GetSubscriptionPlan_ReturnsNotFound_WhenNotExists()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var nonExistentPlanId = Guid.NewGuid();
        var response = await client.GetAsync($"api/v1/subscription-plans/{nonExistentPlanId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateSubscription_WithValidPlan_CreatesSubscription()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-sub-create", "test@araponga.com");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Primeiro, listar planos para obter um ID válido
        var listResponse = await client.GetAsync("api/v1/subscription-plans");
        listResponse.EnsureSuccessStatusCode();
        var plans = await listResponse.Content.ReadFromJsonAsync<List<SubscriptionPlanResponse>>();
        Assert.NotNull(plans);
        Assert.NotEmpty(plans);

        // Tentar criar assinatura com um plano válido
        // Nota: Para planos pagos, seria necessário configurar gateway de pagamento
        // Por enquanto, testamos apenas a estrutura da API
        var freePlan = plans.FirstOrDefault(p => p.Tier == "FREE");
        if (freePlan != null)
        {
            var createRequest = new CreateSubscriptionRequest
            {
                PlanId = freePlan.Id,
                TerritoryId = null,
                CouponCode = null
            };

            var createResponse = await client.PostAsJsonAsync("api/v1/subscriptions", createRequest);

            // Pode retornar 201 (criado) ou 400 (já existe assinatura ativa)
            Assert.True(
                createResponse.StatusCode == HttpStatusCode.Created ||
                createResponse.StatusCode == HttpStatusCode.BadRequest);
        }
    }

    [Fact]
    public async Task CreateSubscription_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var createRequest = new CreateSubscriptionRequest
        {
            PlanId = Guid.NewGuid(),
            TerritoryId = null,
            CouponCode = null
        };

        var response = await client.PostAsJsonAsync("api/v1/subscriptions", createRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CancelSubscription_WithValidSubscription_CancelsSubscription()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-sub-cancel", "test@araponga.com");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Obter assinatura atual
        var getResponse = await client.GetAsync("api/v1/subscriptions/me");
        getResponse.EnsureSuccessStatusCode();
        var subscription = await getResponse.Content.ReadFromJsonAsync<SubscriptionResponse>();
        Assert.NotNull(subscription);

        // Tentar cancelar
        var cancelResponse = await client.PostAsync(
            $"api/v1/subscriptions/{subscription.Id}/cancel?cancelAtPeriodEnd=false",
            null);

        // Pode retornar 200 (sucesso) ou 400 (não pode cancelar FREE)
        Assert.True(
            cancelResponse.StatusCode == HttpStatusCode.OK ||
            cancelResponse.StatusCode == HttpStatusCode.BadRequest);
    }
}
