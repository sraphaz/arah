using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Araponga.Api;
using Araponga.Api.Contracts.Auth;
using Araponga.Api.Contracts.Governance;
using Araponga.Api.Contracts.Users;
using Araponga.Application.Interfaces;
using Araponga.Domain.Governance;
using Araponga.Domain.Membership;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.Api;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Sdk;

namespace Araponga.Tests.Performance;

/// <summary>
/// Testes de performance para sistema de votações com muitos votos.
/// Valida que operações críticas (GetResults, ListVotings) respondem dentro de SLAs mesmo com grandes volumes.
/// </summary>
public sealed class VotingPerformanceTests
{
    private static readonly Guid ActiveTerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");

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
                "test@araponga.com"));
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<SocialLoginResponse>();
        return payload!.Token;
    }

    private static void SetAuthHeader(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private static async Task<Guid> GetUserIdAsync(HttpClient client)
    {
        var response = await client.GetAsync("api/v1/users/me/profile");
        response.EnsureSuccessStatusCode();
        var profile = await response.Content.ReadFromJsonAsync<UserProfileResponse>();
        return profile!.Id;
    }

    private static Task CreateMembershipAsync(ApiFactory factory, Guid userId, Guid territoryId, MembershipRole role = MembershipRole.Resident)
    {
        var dataStore = factory.GetDataStore();
        
        var existing = dataStore.Memberships.FirstOrDefault(m => m.UserId == userId && m.TerritoryId == territoryId);
        if (existing != null)
        {
            return Task.CompletedTask;
        }

        var membership = new TerritoryMembership(
            Guid.NewGuid(),
            userId,
            territoryId,
            role,
            ResidencyVerification.GeoVerified,
            DateTime.UtcNow,
            null,
            DateTime.UtcNow);
        
        dataStore.Memberships.Add(membership);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Testa performance de GetResultsAsync com votação contendo muitos votos.
    /// SLA: Resultados devem ser retornados em < 500ms para até 1000 votos.
    /// </summary>
    [SkippableFact]
    public async Task GetResults_WithManyVotes_RespondsWithinSLA()
    {
        SkipIfNeeded();

        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Criar usuário criador da votação
        var creatorToken = await LoginForTokenAsync(client, "google", "perf-voting-creator");
        SetAuthHeader(client, creatorToken);
        var creatorUserId = await GetUserIdAsync(client);
        await CreateMembershipAsync(factory, creatorUserId, ActiveTerritoryId, MembershipRole.Resident);

        // Criar votação
        var createResponse = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/votings",
            new CreateVotingRequest(
                "ThemePrioritization",
                "Performance Test Voting",
                "Test voting with many votes",
                new[] { "Option1", "Option2", "Option3" },
                "AllMembers",
                null,
                null));

        createResponse.EnsureSuccessStatusCode();
        var voting = await createResponse.Content.ReadFromJsonAsync<VotingResponse>();
        Assert.NotNull(voting);

        // Abrir votação (se estiver em Draft)
        var dataStore = factory.GetDataStore();
        var domainVoting = dataStore.Votings.FirstOrDefault(v => v.Id == voting.Id);
        if (domainVoting == null)
        {
            Assert.Fail("Voting not found in data store");
            return;
        }
        if (domainVoting.Status == VotingStatus.Draft)
        {
            domainVoting.Open();
        }

        // Criar muitos votos diretamente no repositório (mais eficiente que via API)
        var voteRepository = factory.Services.GetRequiredService<IVoteRepository>();
        const int voteCount = 1000;
        var random = new Random(42); // Seed fixo para reprodutibilidade

        for (int i = 0; i < voteCount; i++)
        {
            var userId = Guid.NewGuid();
            var option = $"Option{(i % 3) + 1}"; // Distribuir votos entre as 3 opções

            var vote = new Vote(
                Guid.NewGuid(),
                voting.Id,
                userId,
                option,
                DateTime.UtcNow);

            await voteRepository.AddAsync(vote, CancellationToken.None);
        }

        // Medir tempo de GetResults
        var stopwatch = Stopwatch.StartNew();
        var resultsResponse = await client.GetAsync($"api/v1/territories/{ActiveTerritoryId}/votings/{voting.Id}/results");
        stopwatch.Stop();

        resultsResponse.EnsureSuccessStatusCode();
        var results = await resultsResponse.Content.ReadFromJsonAsync<VotingResultsResponse>();
        Assert.NotNull(results);

        // SLA: < 500ms para 1000 votos
        const int maxMilliseconds = 500;
        Assert.True(
            stopwatch.ElapsedMilliseconds < maxMilliseconds,
            $"GetResults levou {stopwatch.ElapsedMilliseconds}ms para {voteCount} votos, esperado < {maxMilliseconds}ms");

        // Validar que resultados estão corretos
        var totalVotes = results.Results.Values.Sum();
        Assert.True(totalVotes >= voteCount * 0.95, // Tolerância de 5% para processamento assíncrono
            $"Total de votos retornado ({totalVotes}) deve ser próximo de {voteCount}");
    }

    /// <summary>
    /// Testa performance de ListVotings com múltiplas votações.
    /// SLA: Listagem deve responder em < 200ms para até 100 votações.
    /// </summary>
    [SkippableFact]
    public async Task ListVotings_WithManyVotings_RespondsWithinSLA()
    {
        SkipIfNeeded();

        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var creatorToken = await LoginForTokenAsync(client, "google", "perf-voting-list-creator");
        SetAuthHeader(client, creatorToken);
        var creatorUserId = await GetUserIdAsync(client);
        await CreateMembershipAsync(factory, creatorUserId, ActiveTerritoryId, MembershipRole.Resident);

        // Criar múltiplas votações diretamente no repositório
        var votingRepository = factory.Services.GetRequiredService<IVotingRepository>();
        const int votingCount = 100;

        for (int i = 0; i < votingCount; i++)
        {
            var voting = new Voting(
                Guid.NewGuid(),
                ActiveTerritoryId,
                creatorUserId,
                VotingType.ThemePrioritization,
                $"Voting {i}",
                $"Description {i}",
                new[] { "Option1", "Option2" },
                VotingVisibility.AllMembers,
                VotingStatus.Open, // Criar já aberta
                null,
                null,
                DateTime.UtcNow,
                DateTime.UtcNow);

            await votingRepository.AddAsync(voting, CancellationToken.None);
        }

        // Medir tempo de listagem
        var stopwatch = Stopwatch.StartNew();
        var response = await client.GetAsync($"api/v1/territories/{ActiveTerritoryId}/votings");
        stopwatch.Stop();

        response.EnsureSuccessStatusCode();
        var votings = await response.Content.ReadFromJsonAsync<IReadOnlyList<VotingResponse>>();
        Assert.NotNull(votings);

        // SLA: < 200ms para 100 votações
        const int maxMilliseconds = 200;
        Assert.True(
            stopwatch.ElapsedMilliseconds < maxMilliseconds,
            $"ListVotings levou {stopwatch.ElapsedMilliseconds}ms para {votingCount} votações, esperado < {maxMilliseconds}ms");

        // Validar que todas as votações foram retornadas
        Assert.True(votings.Count >= votingCount * 0.95,
            $"Listagem retornou {votings.Count} de {votingCount} votações esperadas");
    }

    /// <summary>
    /// Testa performance de GetResultsAsync com votação contendo muitos votos distribuídos.
    /// Valida que a agregação de votos é eficiente mesmo com distribuição complexa.
    /// </summary>
    [SkippableFact]
    public async Task GetResults_WithDistributedVotes_RespondsWithinSLA()
    {
        SkipIfNeeded();

        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var creatorToken = await LoginForTokenAsync(client, "google", "perf-voting-distributed");
        SetAuthHeader(client, creatorToken);
        var creatorUserId = await GetUserIdAsync(client);
        await CreateMembershipAsync(factory, creatorUserId, ActiveTerritoryId, MembershipRole.Resident);

        // Criar votação com muitas opções
        var createResponse = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/votings",
            new CreateVotingRequest(
                "ThemePrioritization",
                "Distributed Votes Test",
                "Test with many options",
                Enumerable.Range(1, 10).Select(i => $"Option{i}").ToArray(), // 10 opções (máximo permitido)
                "AllMembers",
                null,
                null));

        createResponse.EnsureSuccessStatusCode();
        var voting = await createResponse.Content.ReadFromJsonAsync<VotingResponse>();
        Assert.NotNull(voting);

        // Abrir votação (se estiver em Draft)
        var dataStore = factory.GetDataStore();
        var domainVoting = dataStore.Votings.FirstOrDefault(v => v.Id == voting.Id);
        if (domainVoting == null)
        {
            Assert.Fail("Voting not found in data store");
            return;
        }
        if (domainVoting.Status == VotingStatus.Draft)
        {
            domainVoting.Open();
        }

        // Criar votos distribuídos entre todas as opções
        var voteRepository = factory.Services.GetRequiredService<IVoteRepository>();
        const int voteCount = 500;
        var random = new Random(42);

        for (int i = 0; i < voteCount; i++)
        {
            var userId = Guid.NewGuid();
            var optionIndex = random.Next(1, 11); // 10 opções
            var option = $"Option{optionIndex}";

            var vote = new Vote(
                Guid.NewGuid(),
                voting.Id,
                userId,
                option,
                DateTime.UtcNow);

            await voteRepository.AddAsync(vote, CancellationToken.None);
        }

        // Medir tempo de GetResults
        var stopwatch = Stopwatch.StartNew();
        var resultsResponse = await client.GetAsync($"api/v1/territories/{ActiveTerritoryId}/votings/{voting.Id}/results");
        stopwatch.Stop();

        resultsResponse.EnsureSuccessStatusCode();
        var results = await resultsResponse.Content.ReadFromJsonAsync<VotingResultsResponse>();
        Assert.NotNull(results);

        // SLA: < 500ms para 500 votos distribuídos em 20 opções
        const int maxMilliseconds = 500;
        Assert.True(
            stopwatch.ElapsedMilliseconds < maxMilliseconds,
            $"GetResults levou {stopwatch.ElapsedMilliseconds}ms para {voteCount} votos distribuídos, esperado < {maxMilliseconds}ms");

        // Validar que todas as opções têm resultados
        Assert.True(results.Results.Count >= 8, // Pelo menos 8 das 10 opções devem ter votos
            $"Resultados devem incluir múltiplas opções, mas apenas {results.Results.Count} opções têm votos");
    }
}
