using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Araponga.Api;
using Araponga.Api.Contracts.Auth;
using Araponga.Api.Contracts.Feed;
using Araponga.Api.Contracts.Territories;
using Araponga.Tests.Api;
using Xunit;
using Xunit.Sdk;

namespace Araponga.Tests.Performance;

/// <summary>
/// Testes de performance básicos para validar que endpoints críticos respondem dentro de SLAs.
/// Para testes de carga completos, usar k6 ou NBomber.
/// </summary>
public sealed class PerformanceTests
{
    private static readonly Guid ActiveTerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    // Verificar se os testes de performance devem ser pulados
    private static bool ShouldSkipPerformanceTests()
    {
        var skipEnv = Environment.GetEnvironmentVariable("SKIP_PERFORMANCE_TESTS");
        var isCI = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI")) ||
                   !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTIONS")) ||
                   !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TF_BUILD")) ||
                   !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JENKINS_URL"));
        
        return isCI || string.Equals(skipEnv, "true", StringComparison.OrdinalIgnoreCase);
    }

    private static void SkipIfNeeded()
    {
        if (ShouldSkipPerformanceTests())
        {
            Skip.If(true, "Testes de performance pulados em CI/CD. Configure SKIP_PERFORMANCE_TESTS=false para executar.");
        }
    }

    [SkippableFact]
    public async Task TerritoriesList_RespondsWithinSLA()
    {
        SkipIfNeeded();

        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var startTime = DateTime.UtcNow;
        var response = await client.GetAsync("api/v1/territories");
        var duration = DateTime.UtcNow - startTime;

        response.EnsureSuccessStatusCode();

        // SLA: Listagem de territórios deve responder em menos de 500ms
        Assert.True(duration.TotalMilliseconds < 500,
            $"Territories list took {duration.TotalMilliseconds}ms, expected < 500ms");
    }

    [SkippableFact]
    public async Task TerritoriesPaged_RespondsWithinSLA()
    {
        SkipIfNeeded();

        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var startTime = DateTime.UtcNow;
        var response = await client.GetAsync("api/v1/territories/paged?pageNumber=1&pageSize=20");
        var duration = DateTime.UtcNow - startTime;

        response.EnsureSuccessStatusCode();

        // SLA: Listagem paginada deve responder em menos de 300ms
        Assert.True(duration.TotalMilliseconds < 300,
            $"Territories paged took {duration.TotalMilliseconds}ms, expected < 300ms");
    }

    [SkippableFact]
    public async Task FeedList_RespondsWithinSLA()
    {
        SkipIfNeeded();

        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "perf-feed-test");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "perf-session");

        await client.PostAsJsonAsync(
            "api/v1/territories/selection",
            new Araponga.Api.Contracts.Territories.TerritorySelectionRequest(ActiveTerritoryId));

        var startTime = DateTime.UtcNow;
        var response = await client.GetAsync($"api/v1/feed?territoryId={ActiveTerritoryId}");
        var duration = DateTime.UtcNow - startTime;

        // Pode retornar OK, Unauthorized ou Forbidden dependendo de permissões
        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.Forbidden);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            // SLA: Feed deve responder em menos de 800ms
            Assert.True(duration.TotalMilliseconds < 800,
                $"Feed list took {duration.TotalMilliseconds}ms, expected < 800ms");
        }
    }

    [SkippableFact]
    public async Task FeedPaged_RespondsWithinSLA()
    {
        SkipIfNeeded();

        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "perf-feed-paged-test");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "perf-paged-session");

        await client.PostAsJsonAsync(
            "api/v1/territories/selection",
            new Araponga.Api.Contracts.Territories.TerritorySelectionRequest(ActiveTerritoryId));

        var startTime = DateTime.UtcNow;
        var response = await client.GetAsync($"api/v1/feed/paged?territoryId={ActiveTerritoryId}&pageNumber=1&pageSize=20");
        var duration = DateTime.UtcNow - startTime;

        // Pode retornar OK, Unauthorized ou Forbidden dependendo de permissões
        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.Forbidden);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            // SLA: Feed paginado deve responder em menos de 500ms
            Assert.True(duration.TotalMilliseconds < 500,
                $"Feed paged took {duration.TotalMilliseconds}ms, expected < 500ms");
        }
    }

    [SkippableFact]
    public async Task AssetsList_RespondsWithinSLA()
    {
        SkipIfNeeded();

        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "perf-assets-test");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var startTime = DateTime.UtcNow;
        var response = await client.GetAsync($"api/v1/assets?territoryId={ActiveTerritoryId}");
        var duration = DateTime.UtcNow - startTime;

        // Pode retornar OK, Unauthorized ou Forbidden
        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.Unauthorized ||
            response.StatusCode == HttpStatusCode.Forbidden);

        if (response.StatusCode == HttpStatusCode.OK)
        {
            // SLA: Assets list deve responder em menos de 600ms
            Assert.True(duration.TotalMilliseconds < 600,
                $"Assets list took {duration.TotalMilliseconds}ms, expected < 600ms");
        }
    }

    [SkippableFact]
    public async Task Authentication_RespondsWithinSLA()
    {
        SkipIfNeeded();

        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var startTime = DateTime.UtcNow;
        var response = await client.PostAsJsonAsync(
            "api/v1/auth/social",
            new SocialLoginRequest(
                "google",
                $"perf-auth-{Guid.NewGuid()}",
                "Performance Test",
                "123.456.789-00",
                null,
                "(11) 90000-0000",
                "Rua Test, 100",
                "perf@araponga.com"));
        var duration = DateTime.UtcNow - startTime;

        response.EnsureSuccessStatusCode();

        // SLA: Autenticação deve responder em menos de 1000ms
        Assert.True(duration.TotalMilliseconds < 1000,
            $"Authentication took {duration.TotalMilliseconds}ms, expected < 1000ms");
    }

    [SkippableFact]
    public async Task MultipleConcurrentRequests_HandlesGracefully()
    {
        SkipIfNeeded();

        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Fazer 10 requisições concorrentes
        var tasks = Enumerable.Range(0, 10)
            .Select(_ => client.GetAsync("api/v1/territories"))
            .ToArray();

        var startTime = DateTime.UtcNow;
        var responses = await Task.WhenAll(tasks);
        var duration = DateTime.UtcNow - startTime;

        // Todas devem ter sucesso
        foreach (var response in responses)
        {
            response.EnsureSuccessStatusCode();
        }

        // 10 requisições concorrentes devem completar em menos de 2 segundos
        Assert.True(duration.TotalMilliseconds < 2000,
            $"10 concurrent requests took {duration.TotalMilliseconds}ms, expected < 2000ms");
    }

    private static async Task<string> LoginForTokenAsync(HttpClient client, string provider, string externalId)
    {
        var response = await client.PostAsJsonAsync(
            "api/v1/auth/social",
            new SocialLoginRequest(
                provider,
                externalId,
                "Tester",
                "123.456.789-00",
                null,
                "(11) 90000-0000",
                "Rua das Flores, 100",
                "tester@araponga.com"));

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<SocialLoginResponse>();
        return payload!.Token;
    }
}
