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

    private static async Task<(RegisterCoreInstanceResponse Registration, string Token)> RegisterInstanceAsync(
        HttpClient client,
        string mode,
        string baseUrl,
        string version)
    {
        var token = await LoginForTokenAsync(client, "google", "admin-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var register = await client.PostAsJsonAsync(
            "api/v1/core/instances",
            new RegisterCoreInstanceRequest(mode, baseUrl, version));
        register.EnsureSuccessStatusCode();
        var registration = await register.Content.ReadFromJsonAsync<RegisterCoreInstanceResponse>();
        return (registration!, token);
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
    public async Task RegisterInstance_WithSystemAdmin_ReturnsKeyPair()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var (registration, _) = await RegisterInstanceAsync(client, "standalone", "https://keys.example", "1.0.0");

        Assert.NotEqual(Guid.Empty, registration.Instance.Id);
        Assert.Contains("BEGIN PUBLIC KEY", registration.Instance.PublicKeyPem);
        Assert.Contains("BEGIN PRIVATE KEY", registration.PrivateKeyPem);
        Assert.False(string.IsNullOrWhiteSpace(registration.InstanceAuthToken));
    }

    [Fact]
    public async Task Heartbeat_WithInstanceToken_WithoutAdmin_Succeeds()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var (registration, _) = await RegisterInstanceAsync(client, "standalone", "https://hb.example", "1.0.0");

        using var heartbeatClient = factory.CreateClient();
        heartbeatClient.DefaultRequestHeaders.Add("X-Arah-Instance-Token", registration.InstanceAuthToken);

        var heartbeat = await heartbeatClient.PostAsJsonAsync(
            $"api/v1/core/instances/{registration.Instance.Id}/heartbeat",
            new CoreHeartbeatRequest(new Dictionary<string, string> { ["api"] = "ok" }, 60));

        Assert.Equal(HttpStatusCode.NoContent, heartbeat.StatusCode);
    }

    [Fact]
    public async Task PublishRelease_WithSystemAdmin_AppearsInStableChannel()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "admin-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var publish = await client.PostAsJsonAsync(
            "api/v1/core/releases",
            new PublishCoreReleaseRequest("1.2.0", "stable", 1));
        publish.EnsureSuccessStatusCode();

        var list = await client.GetAsync("api/v1/core/releases?channel=stable");
        list.EnsureSuccessStatusCode();
        var releases = await list.Content.ReadFromJsonAsync<List<CoreReleaseResponse>>();
        Assert.Contains(releases!, r => r.Version == "1.2.0");
    }

    [Fact]
    public async Task ListInstances_WithSystemAdmin_ReturnsInstancesWithTelemetry()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var (registrationA, token) = await RegisterInstanceAsync(client, "standalone", "https://a.example", "1.0.0");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var registerB = await client.PostAsJsonAsync(
            "api/v1/core/instances",
            new RegisterCoreInstanceRequest("federated", "https://b.example", "2.0.0"));
        registerB.EnsureSuccessStatusCode();
        var registrationB = await registerB.Content.ReadFromJsonAsync<RegisterCoreInstanceResponse>();

        var heartbeat = await client.PostAsJsonAsync(
            $"api/v1/core/instances/{registrationA.Instance.Id}/heartbeat",
            new CoreHeartbeatRequest(
                new Dictionary<string, string> { ["api"] = "healthy" },
                3600));
        Assert.Equal(HttpStatusCode.NoContent, heartbeat.StatusCode);

        var listResponse = await client.GetAsync("api/v1/core/instances");
        listResponse.EnsureSuccessStatusCode();
        var list = await listResponse.Content.ReadFromJsonAsync<List<CoreInstanceResponse>>();

        Assert.NotNull(list);
        Assert.Equal(2, list!.Count);

        var withTelemetry = list.Single(i => i.Id == registrationA.Instance.Id);
        Assert.Equal("Online", withTelemetry.Status);
        Assert.NotNull(withTelemetry.LastHeartbeatUtc);
        Assert.Equal("healthy", withTelemetry.LastServices!["api"]);
        Assert.Equal(3600, withTelemetry.LastUptimeSeconds);

        var pending = list.Single(i => i.Id == registrationB!.Instance.Id);
        Assert.Equal("Pending", pending.Status);
        Assert.Null(pending.LastServices);
    }

    [Fact]
    public async Task ListReleases_WithSystemAdmin_ReturnsStableSeed()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "admin-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("api/v1/core/releases?channel=stable");
        response.EnsureSuccessStatusCode();
        var releases = await response.Content.ReadFromJsonAsync<List<CoreReleaseResponse>>();

        Assert.NotNull(releases);
        Assert.NotEmpty(releases!);
        Assert.Equal("0.1.0", releases![0].Version);
        Assert.Equal("stable", releases[0].Channel);
    }

    [Fact]
    public async Task PublishTerritory_WhenInstanceRegistered_AppearsInDirectory()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var (registration, token) = await RegisterInstanceAsync(client, "standalone", "https://dir.example", "1.0.0");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var territoryId = Guid.NewGuid();
        var publish = await client.PostAsJsonAsync(
            "api/v1/core/directory/territories",
            new PublishDirectoryTerritoryRequest(territoryId, registration.Instance.Id, "Vale do Sol"));
        publish.EnsureSuccessStatusCode();
        var published = await publish.Content.ReadFromJsonAsync<DirectoryTerritoryResponse>();

        Assert.NotNull(published);
        Assert.Equal(territoryId, published!.TerritoryId);
        Assert.Equal("Vale do Sol", published.Name);

        var list = await client.GetAsync("api/v1/core/directory/territories");
        list.EnsureSuccessStatusCode();
        var entries = await list.Content.ReadFromJsonAsync<List<DirectoryTerritoryResponse>>();
        Assert.Contains(entries!, e => e.TerritoryId == territoryId);
    }
}

public sealed class FederationControllerIntegrationTests
{
    private static async Task<string> LoginForTokenAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "api/v1/auth/social",
            new SocialLoginRequest(
                "google",
                "admin-external",
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
    public async Task ResolveIdentity_AfterProvision_ReturnsUser()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var register = await client.PostAsJsonAsync(
            "api/v1/core/instances",
            new RegisterCoreInstanceRequest("federated", "https://home.example", "1.0.0"));
        register.EnsureSuccessStatusCode();
        var instance = await register.Content.ReadFromJsonAsync<RegisterCoreInstanceResponse>();

        var provision = await client.PostAsJsonAsync(
            "api/v1/federation/identity",
            new ProvisionFederatedIdentityRequest(instance!.Instance.Id, "Ana Territorial"));
        provision.EnsureSuccessStatusCode();
        var identity = await provision.Content.ReadFromJsonAsync<FederatedIdentityResponse>();

        var resolve = await client.GetAsync($"api/v1/federation/identity/{identity!.GlobalUserId}");
        resolve.EnsureSuccessStatusCode();
        var resolved = await resolve.Content.ReadFromJsonAsync<FederatedIdentityResponse>();

        Assert.NotNull(resolved);
        Assert.Equal(identity.GlobalUserId, resolved!.GlobalUserId);
        Assert.Equal("Ana Territorial", resolved.DisplayName);
        Assert.Equal(instance.Instance.Id, resolved.HomeInstanceId);
    }
}
