using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Araponga.Api;
using Araponga.Api.Contracts.Auth;
using Araponga.Api.Contracts.Governance;
using Araponga.Api.Contracts.Users;
using Araponga.Domain.Membership;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Api;

/// <summary>
/// Testes de integração para sistema de governança comunitária (interesses, votações, moderação).
/// </summary>
public sealed class GovernanceIntegrationTests
{
    private static readonly Guid ActiveTerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid ResidentUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

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
        
        // Verificar se já existe
        var existing = dataStore.Memberships.FirstOrDefault(m => m.UserId == userId && m.TerritoryId == territoryId);
        if (existing != null)
        {
            return Task.CompletedTask;
        }

        // Criar membership com verificação para que HasAnyVerification() retorne true
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

    [Fact]
    public async Task AddInterest_WhenValid_ReturnsSuccess()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-interest-1");
        SetAuthHeader(client, token);

        var response = await client.PostAsJsonAsync(
            "api/v1/users/me/interests",
            new AddInterestRequest("meio ambiente"));

        // Controller retorna OK, não Created
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ListInterests_WhenUserHasInterests_ReturnsList()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-interest-2");
        SetAuthHeader(client, token);

        // Adicionar interesse
        await client.PostAsJsonAsync(
            "api/v1/users/me/interests",
            new AddInterestRequest("eventos"));

        // Listar interesses
        var response = await client.GetAsync("api/v1/users/me/interests");
        response.EnsureSuccessStatusCode();

        var interests = await response.Content.ReadFromJsonAsync<IReadOnlyList<string>>();
        Assert.NotNull(interests);
        Assert.Contains("eventos", interests);
    }

    [Fact]
    public async Task RemoveInterest_WhenExists_ReturnsSuccess()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-interest-3");
        SetAuthHeader(client, token);

        // Adicionar interesse
        await client.PostAsJsonAsync(
            "api/v1/users/me/interests",
            new AddInterestRequest("cultura"));

        // Remover interesse
        var response = await client.DeleteAsync("api/v1/users/me/interests/cultura");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task CreateVoting_WhenValid_ReturnsSuccess()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-voting-1");
        SetAuthHeader(client, token);

        // Criar membership como resident para poder criar votação
        var userId = await GetUserIdAsync(client);
        await CreateMembershipAsync(factory, userId, ActiveTerritoryId, MembershipRole.Resident);

        var response = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/votings",
            new CreateVotingRequest(
                "ThemePrioritization",
                "Priorizar temas",
                "Qual tema deve ter prioridade?",
                new[] { "Meio Ambiente", "Eventos" },
                "AllMembers",
                null,
                null));

        if (response.StatusCode != HttpStatusCode.Created)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Assert.Fail($"Expected Created but got {response.StatusCode}. Error: {errorContent}");
        }
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Vote_WhenVotingIsOpen_ReturnsSuccess()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-voting-2");
        SetAuthHeader(client, token);

        // Criar membership como resident
        var userId = await GetUserIdAsync(client);
        await CreateMembershipAsync(factory, userId, ActiveTerritoryId, MembershipRole.Resident);

        // Criar votação
        var createResponse = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/votings",
            new CreateVotingRequest(
                "ThemePrioritization",
                "Test Voting",
                "Test Description",
                new[] { "Option1", "Option2" },
                "AllMembers",
                null,
                null));

        createResponse.EnsureSuccessStatusCode();
        var voting = await createResponse.Content.ReadFromJsonAsync<VotingResponse>();
        Assert.NotNull(voting);

        // Abrir votação diretamente no dataStore (não há endpoint para isso)
        var dataStore = factory.GetDataStore();
        var domainVoting = dataStore.Votings.FirstOrDefault(v => v.Id == voting.Id);
        if (domainVoting != null)
        {
            domainVoting.Open();
        }

        // Votar (endpoint está em /api/v1/territories/{territoryId}/votings/{votingId}/vote)
        var voteResponse = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/votings/{voting.Id}/vote",
            new VoteRequest("Option1"));

        // Endpoint retorna NoContent (204) quando sucesso
        Assert.Equal(HttpStatusCode.NoContent, voteResponse.StatusCode);
    }

    [Fact]
    public async Task GetVotingResults_WhenVotingExists_ReturnsResults()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-voting-3");
        SetAuthHeader(client, token);

        // Criar membership como resident
        var userId = await GetUserIdAsync(client);
        await CreateMembershipAsync(factory, userId, ActiveTerritoryId, MembershipRole.Resident);

        // Criar votação
        var createResponse = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/votings",
            new CreateVotingRequest(
                "ThemePrioritization",
                "Test Voting",
                "Test Description",
                new[] { "Option1", "Option2" },
                "AllMembers",
                null,
                null));

        createResponse.EnsureSuccessStatusCode();
        var voting = await createResponse.Content.ReadFromJsonAsync<VotingResponse>();
        Assert.NotNull(voting);

        // Obter resultados (endpoint está em /api/v1/territories/{territoryId}/votings/{votingId}/results)
        var resultsResponse = await client.GetAsync($"api/v1/territories/{ActiveTerritoryId}/votings/{voting.Id}/results");
        resultsResponse.EnsureSuccessStatusCode();

        var results = await resultsResponse.Content.ReadFromJsonAsync<VotingResultsResponse>();
        Assert.NotNull(results);
    }

    [Fact]
    public async Task ListVotings_WhenTerritoryHasVotings_ReturnsList()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-voting-4");
        SetAuthHeader(client, token);

        // Criar membership como resident
        var userId = await GetUserIdAsync(client);
        await CreateMembershipAsync(factory, userId, ActiveTerritoryId, MembershipRole.Resident);

        // Criar votação
        var createResponse = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/votings",
            new CreateVotingRequest(
                "ThemePrioritization",
                "Test Voting",
                "Test Description",
                new[] { "Option1", "Option2" },
                "AllMembers",
                null,
                null));

        createResponse.EnsureSuccessStatusCode();

        // Listar votações
        var response = await client.GetAsync($"api/v1/territories/{ActiveTerritoryId}/votings");
        response.EnsureSuccessStatusCode();

        var votings = await response.Content.ReadFromJsonAsync<IReadOnlyList<VotingResponse>>();
        Assert.NotNull(votings);
        Assert.True(votings.Count > 0);
    }

    [Fact]
    public async Task UserProfile_IncludesInterests()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-profile-1");
        SetAuthHeader(client, token);

        // Adicionar interesse
        await client.PostAsJsonAsync(
            "api/v1/users/me/interests",
            new AddInterestRequest("tecnologia"));

        // Obter perfil
        var response = await client.GetAsync("api/v1/users/me/profile");
        response.EnsureSuccessStatusCode();

        var profile = await response.Content.ReadFromJsonAsync<UserProfileResponse>();
        Assert.NotNull(profile);
        Assert.NotNull(profile.Interests);
        Assert.Contains("tecnologia", profile.Interests);
    }
}
