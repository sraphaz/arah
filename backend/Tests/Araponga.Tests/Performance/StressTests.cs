using Araponga.Api;
using Araponga.Api.Contracts.Auth;
using Araponga.Api.Contracts.Territories;
using Araponga.Tests.Api;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;
using Xunit.Sdk;

namespace Araponga.Tests.Performance;

/// <summary>
/// Testes de stress para validar comportamento sob carga extrema.
/// </summary>
public sealed class StressTests : IClassFixture<ApiFactory>, IDisposable
{
    private readonly ApiFactory _factory;
    private readonly HttpClient _httpClient;
    private string? _authToken;

    private static bool ShouldSkipStressTests()
    {
        var skipEnv = Environment.GetEnvironmentVariable("SKIP_STRESS_TESTS");
        var isCI = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI")) ||
                   !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"));
        return isCI || string.Equals(skipEnv, "true", StringComparison.OrdinalIgnoreCase);
    }

    private static void SkipIfNeeded()
    {
        if (ShouldSkipStressTests())
        {
            Skip.If(true, "Testes de stress pulados. Configure SKIP_STRESS_TESTS=false para executar.");
        }
    }

    public StressTests(ApiFactory factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }

    private async Task<string> GetAuthTokenAsync()
    {
        if (_authToken is not null)
        {
            return _authToken;
        }

        var loginRequest = new SocialLoginRequest(
            "google",
            Guid.NewGuid().ToString(),
            "Stress Test User",
            "123.456.789-00",
            null,
            "11999999999",
            "Test Address",
            "stresstest@example.com");

        var response = await _httpClient.PostAsJsonAsync("api/v1/auth/social", loginRequest);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<SocialLoginResponse>();
        
        _authToken = result?.Token ?? throw new InvalidOperationException("Token não retornado");
        return _authToken;
    }

    [SkippableFact]
    public async Task FeedEndpoint_StressTest_PeakLoad()
    {
        SkipIfNeeded();

        var territoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var token = await GetAuthTokenAsync();

        // Carga pico: 2x carga normal (20 clientes, 3 req cada = 60 requisições)
        const int concurrentRequests = 20;
        const int requestsPerClient = 3;
        const int maxSeconds = 20;

        var clients = Enumerable.Range(0, concurrentRequests)
            .Select(_ =>
            {
                var client = _factory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                if (!client.DefaultRequestHeaders.Contains("X-Session-Id"))
                {
                    client.DefaultRequestHeaders.Add("X-Session-Id", Guid.NewGuid().ToString());
                }
                return client;
            })
            .ToArray();

        try
        {
            // Selecionar território para todos os clientes
            var selectTasks = clients.Select(client =>
                client.PostAsJsonAsync("api/v1/territories/selection", new TerritorySelectionRequest(territoryId)));
            await Task.WhenAll(selectTasks);

            var stopwatch = Stopwatch.StartNew();
            var tasks = clients.SelectMany(client =>
                Enumerable.Range(0, requestsPerClient)
                    .Select(_ => client.GetAsync($"api/v1/feed?territoryId={territoryId}")))
                .ToArray();

            var responses = await Task.WhenAll(tasks);
            stopwatch.Stop();

            // Em carga pico, aceitamos até 30% de falhas (rate limiting, permissões, etc)
            var successCount = responses.Count(r => r.IsSuccessStatusCode || r.StatusCode == System.Net.HttpStatusCode.Forbidden);
            var failureRate = 1.0 - ((double)successCount / responses.Length);
            Assert.True(failureRate < 0.30, 
                $"Taxa de falha deve ser < 30% em carga pico, mas foi {failureRate:P2}");

            Assert.True(stopwatch.Elapsed.TotalSeconds < maxSeconds,
                $"60 requisições em carga pico levaram {stopwatch.Elapsed.TotalSeconds:F2}s, esperado < {maxSeconds}s");
        }
        finally
        {
            foreach (var client in clients)
            {
                client.Dispose();
            }
        }
    }

    [SkippableFact]
    public async Task FeedEndpoint_StressTest_ExtremeLoad()
    {
        SkipIfNeeded();

        var territoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var token = await GetAuthTokenAsync();

        // Carga extrema: 5x carga normal (50 clientes, 2 req cada = 100 requisições)
        const int concurrentRequests = 50;
        const int requestsPerClient = 2;
        const int maxSeconds = 60; // Aumentado para ambiente de teste

        var clients = Enumerable.Range(0, concurrentRequests)
            .Select(_ =>
            {
                var client = _factory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                if (!client.DefaultRequestHeaders.Contains("X-Session-Id"))
                {
                    client.DefaultRequestHeaders.Add("X-Session-Id", Guid.NewGuid().ToString());
                }
                return client;
            })
            .ToArray();

        try
        {
            // Selecionar território para todos os clientes
            var selectTasks = clients.Select(client =>
                client.PostAsJsonAsync("api/v1/territories/selection", new TerritorySelectionRequest(territoryId)));
            await Task.WhenAll(selectTasks);

            var stopwatch = Stopwatch.StartNew();
            var tasks = clients.SelectMany(client =>
                Enumerable.Range(0, requestsPerClient)
                    .Select(_ => client.GetAsync($"api/v1/feed?territoryId={territoryId}")))
                .ToArray();

            var responses = await Task.WhenAll(tasks);
            stopwatch.Stop();

            // Em carga extrema, aceitamos até 30% de falhas (rate limiting, permissões, etc)
            var successCount = responses.Count(r => r.IsSuccessStatusCode || r.StatusCode == System.Net.HttpStatusCode.Forbidden);
            var failureRate = 1.0 - ((double)successCount / responses.Length);
            Assert.True(failureRate < 0.30, 
                $"Taxa de falha deve ser < 30% em carga extrema, mas foi {failureRate:P2}");

            // Sistema não deve travar completamente
            Assert.True(successCount > 0, 
                "Sistema deve processar pelo menos algumas requisições mesmo em carga extrema");

            Assert.True(stopwatch.Elapsed.TotalSeconds < maxSeconds,
                $"100 requisições em carga extrema levaram {stopwatch.Elapsed.TotalSeconds:F2}s, esperado < {maxSeconds}s");
        }
        finally
        {
            foreach (var client in clients)
            {
                client.Dispose();
            }
        }
    }

    [SkippableFact]
    public async Task MultipleEndpoints_StressTest_ConcurrentLoad()
    {
        SkipIfNeeded();

        var territoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var token = await GetAuthTokenAsync();

        const int concurrentRequests = 10;
        const int requestsPerClient = 3;

        var clients = Enumerable.Range(0, concurrentRequests)
            .Select(_ =>
            {
                var client = _factory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                if (!client.DefaultRequestHeaders.Contains("X-Session-Id"))
                {
                    client.DefaultRequestHeaders.Add("X-Session-Id", Guid.NewGuid().ToString());
                }
                return client;
            })
            .ToArray();

        try
        {
            // Selecionar território para todos os clientes
            var selectTasks = clients.Select(client =>
                client.PostAsJsonAsync("api/v1/territories/selection", new TerritorySelectionRequest(territoryId)));
            await Task.WhenAll(selectTasks);

            // Feed
            var feedTasks = clients.SelectMany(client =>
                Enumerable.Range(0, requestsPerClient)
                    .Select(_ => client.GetAsync($"api/v1/feed?territoryId={territoryId}")));

            // Marketplace
            var storesTasks = clients.SelectMany(client =>
                Enumerable.Range(0, requestsPerClient)
                    .Select(_ => client.GetAsync($"api/v1/marketplace/stores?territoryId={territoryId}")));

            // Map Pins
            var pinsTasks = clients.SelectMany(client =>
                Enumerable.Range(0, requestsPerClient)
                    .Select(_ => client.GetAsync("api/v1/map/pins?latitude=-23.5505&longitude=-46.6333&radiusKm=10")));

            var allTasks = feedTasks.Concat(storesTasks).Concat(pinsTasks).ToArray();

            var stopwatch = Stopwatch.StartNew();
            var responses = await Task.WhenAll(allTasks);
            stopwatch.Stop();

            // Verificar que a maioria teve sucesso (tolerância para permissões, timeouts, etc.)
            var successCount = responses.Count(r => r.IsSuccessStatusCode ||
                r.StatusCode == System.Net.HttpStatusCode.Forbidden ||
                r.StatusCode == System.Net.HttpStatusCode.BadRequest);
            var successRate = (double)successCount / responses.Length;
            Assert.True(successRate >= 0.40,
                $"Taxa de sucesso deve ser >= 40% em carga concorrente, mas foi {successRate:P2} ({successCount}/{responses.Length})");

            Assert.True(stopwatch.Elapsed.TotalSeconds < 35,
                $"{responses.Length} requisições concorrentes levaram {stopwatch.Elapsed.TotalSeconds:F2}s, esperado < 35s");
        }
        finally
        {
            foreach (var client in clients)
            {
                client.Dispose();
            }
        }
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}
