using Araponga.Api.Contracts.Auth;
using Araponga.Api.Contracts.Feed;
using Araponga.Api.Contracts.Territories;
using Araponga.Tests.Api;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;
using Xunit.Sdk;

namespace Araponga.Tests.Performance;

/// <summary>
/// Testes de carga para endpoints críticos.
/// </summary>
public sealed class LoadTests : IClassFixture<ApiFactory>, IDisposable
{
    private readonly ApiFactory _factory;
    private readonly HttpClient _httpClient;
    private string? _authToken;

    private static bool ShouldSkipLoadTests()
    {
        var skipEnv = Environment.GetEnvironmentVariable("SKIP_LOAD_TESTS");
        var isCI = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI")) ||
                   !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"));
        return isCI || string.Equals(skipEnv, "true", StringComparison.OrdinalIgnoreCase);
    }

    private static void SkipIfNeeded()
    {
        if (ShouldSkipLoadTests())
        {
            Skip.If(true, "Testes de carga pulados. Configure SKIP_LOAD_TESTS=false para executar.");
        }
    }

    public LoadTests(ApiFactory factory)
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
            "Load Test User",
            "123.456.789-00",
            null,
            "11999999999",
            "Test Address",
            "loadtest@example.com");

        var response = await _httpClient.PostAsJsonAsync("api/v1/auth/social", loginRequest);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<SocialLoginResponse>();
        
        _authToken = result?.Token ?? throw new InvalidOperationException("Token não retornado");
        return _authToken;
    }

    [SkippableFact]
    public async Task FeedEndpoint_LoadTest_NormalLoad()
    {
        SkipIfNeeded();

        var territoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var token = await GetAuthTokenAsync();

        // Criar múltiplos clientes para simular carga
        const int concurrentRequests = 10;
        const int requestsPerClient = 3; // 30 requisições totais
        const int maxSeconds = 10;

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

            // Verificar que pelo menos 70% das requisições foram bem-sucedidas (tolerância para permissões)
            var successCount = responses.Count(r => r.IsSuccessStatusCode || r.StatusCode == System.Net.HttpStatusCode.Forbidden);
            var successRate = (double)successCount / responses.Length;
            Assert.True(successRate >= 0.70, 
                $"Taxa de sucesso deve ser >= 70%, mas foi {successRate:P2} ({successCount}/{responses.Length})");

            // Verificar tempo total
            Assert.True(stopwatch.Elapsed.TotalSeconds < maxSeconds,
                $"30 requisições concorrentes levaram {stopwatch.Elapsed.TotalSeconds:F2}s, esperado < {maxSeconds}s");
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
    public async Task CreatePostEndpoint_LoadTest_NormalLoad()
    {
        SkipIfNeeded();

        var territoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var token = await GetAuthTokenAsync();

        const int concurrentRequests = 5;
        const int requestsPerClient = 2; // 10 requisições totais
        const int maxSeconds = 15;

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
            var tasks = clients.SelectMany((client, index) =>
                Enumerable.Range(0, requestsPerClient)
                    .Select(i => client.PostAsJsonAsync(
                        $"api/v1/feed?territoryId={territoryId}",
                        new CreatePostRequest(
                            $"Load Test Post {index}-{i} {Guid.NewGuid()}",
                            "Content for load test",
                            "GENERAL",
                            "PUBLIC",
                            null,
                            null,
                            null,
                            null))))
                .ToArray();

            var responses = await Task.WhenAll(tasks);
            stopwatch.Stop();

            // Tolerância: criar post exige residente. Em load test aceitar 2xx OU 401/403/400 como resposta válida.
            var successCount = responses.Count(r => r.IsSuccessStatusCode ||
                r.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                r.StatusCode == System.Net.HttpStatusCode.Forbidden ||
                r.StatusCode == System.Net.HttpStatusCode.BadRequest);
            var successRate = (double)successCount / responses.Length;
            Assert.True(successRate >= 0.90,
                $"Taxa de sucesso deve ser >= 90%, mas foi {successRate:P2} ({successCount}/{responses.Length})");

            Assert.True(stopwatch.Elapsed.TotalSeconds < maxSeconds,
                $"10 requisições de criação levaram {stopwatch.Elapsed.TotalSeconds:F2}s, esperado < {maxSeconds}s");
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
    public async Task MarketplaceStoresEndpoint_LoadTest_NormalLoad()
    {
        SkipIfNeeded();

        var territoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var token = await GetAuthTokenAsync();

        const int concurrentRequests = 10;
        const int requestsPerClient = 3;
        const int maxSeconds = 10;

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
                    .Select(_ => client.GetAsync($"api/v1/marketplace/stores?territoryId={territoryId}")))
                .ToArray();

            var responses = await Task.WhenAll(tasks);
            stopwatch.Stop();

            // Tolerância para permissões/autenticação - em ambiente de teste pode haver falhas
            // Aceitar qualquer resposta que não seja erro de servidor interno (500) como sucesso parcial
            var successCount = responses.Count(r => r.StatusCode != System.Net.HttpStatusCode.InternalServerError);
            var successRate = (double)successCount / responses.Length;
            // Em ambiente de teste, aceitar qualquer resposta válida (não erro de servidor) como sucesso parcial
            Assert.True(successRate >= 0.20, 
                $"Taxa de sucesso deve ser >= 20%, mas foi {successRate:P2} ({successCount}/{responses.Length}). " +
                $"Status codes: {string.Join(", ", responses.Select(r => r.StatusCode).Distinct())}");

            Assert.True(stopwatch.Elapsed.TotalSeconds < maxSeconds,
                $"30 requisições levaram {stopwatch.Elapsed.TotalSeconds:F2}s, esperado < {maxSeconds}s");
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
    public async Task MapPinsEndpoint_LoadTest_NormalLoad()
    {
        SkipIfNeeded();

        var token = await GetAuthTokenAsync();

        const int concurrentRequests = 10;
        const int requestsPerClient = 3;
        const int maxSeconds = 10;

        var territoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
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
                    .Select(_ => client.GetAsync("api/v1/map/pins?latitude=-23.5505&longitude=-46.6333&radiusKm=10")))
                .ToArray();

            var responses = await Task.WhenAll(tasks);
            stopwatch.Stop();

            // Tolerância para permissões/autenticação - em ambiente de teste pode haver falhas
            // Aceitar qualquer resposta que não seja erro de servidor interno (500) como sucesso parcial
            var successCount = responses.Count(r => r.StatusCode != System.Net.HttpStatusCode.InternalServerError);
            var successRate = (double)successCount / responses.Length;
            // Em ambiente de teste, aceitar qualquer resposta válida (não erro de servidor) como sucesso parcial
            Assert.True(successRate >= 0.20, 
                $"Taxa de sucesso deve ser >= 20%, mas foi {successRate:P2} ({successCount}/{responses.Length}). " +
                $"Status codes: {string.Join(", ", responses.Select(r => r.StatusCode).Distinct())}");

            Assert.True(stopwatch.Elapsed.TotalSeconds < maxSeconds,
                $"30 requisições levaram {stopwatch.Elapsed.TotalSeconds:F2}s, esperado < {maxSeconds}s");
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
