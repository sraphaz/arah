using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Arah.Api.Contracts.Auth;
using Arah.Api.Contracts.Core;
using Xunit;

namespace Arah.Tests.Api;

public sealed class CoreControllerIntegrationTests
{
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
                null,
                null,
                "test@Arah.com"));
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<SocialLoginResponse>();
        return payload!.Token;
    }

    [Fact]
    public async Task ListInstances_WithoutAuth_ReturnsUnauthorized()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("api/v1/core/instances");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ListInstances_WithSystemAdmin_ReturnsInstancesWithTelemetry()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "admin-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var registerA = await client.PostAsJsonAsync(
            "api/v1/core/instances",
            new RegisterCoreInstanceRequest("standalone", "https://a.example", "1.0.0"));
        registerA.EnsureSuccessStatusCode();
        var instanceA = await registerA.Content.ReadFromJsonAsync<CoreInstanceResponse>();

        var registerB = await client.PostAsJsonAsync(
            "api/v1/core/instances",
            new RegisterCoreInstanceRequest("federated", "https://b.example", "2.0.0"));
        registerB.EnsureSuccessStatusCode();
        var instanceB = await registerB.Content.ReadFromJsonAsync<CoreInstanceResponse>();

        var heartbeat = await client.PostAsJsonAsync(
            $"api/v1/core/instances/{instanceA!.Id}/heartbeat",
            new CoreHeartbeatRequest(
                new Dictionary<string, string> { ["api"] = "healthy" },
                3600));
        Assert.Equal(HttpStatusCode.NoContent, heartbeat.StatusCode);

        var listResponse = await client.GetAsync("api/v1/core/instances");
        listResponse.EnsureSuccessStatusCode();
        var list = await listResponse.Content.ReadFromJsonAsync<List<CoreInstanceResponse>>();

        Assert.NotNull(list);
        Assert.Equal(2, list!.Count);

        var withTelemetry = list.Single(i => i.Id == instanceA.Id);
        Assert.Equal("Online", withTelemetry.Status);
        Assert.NotNull(withTelemetry.LastHeartbeatUtc);
        Assert.Equal("healthy", withTelemetry.LastServices!["api"]);
        Assert.Equal(3600, withTelemetry.LastUptimeSeconds);

        var pending = list.Single(i => i.Id == instanceB!.Id);
        Assert.Equal("Pending", pending.Status);
        Assert.Null(pending.LastServices);
    }
}
