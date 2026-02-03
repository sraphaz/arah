using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Araponga.Api;
using Araponga.Api.Contracts.Auth;
using Araponga.Api.Contracts.Feed;
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
        var sharedStore = factory.GetSharedStore();

        var existing = sharedStore.Memberships.FirstOrDefault(m => m.UserId == userId && m.TerritoryId == territoryId);
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

        sharedStore.Memberships.Add(membership);
        return Task.CompletedTask;
    }

    private static Task CreateCuratorCapabilityAsync(ApiFactory factory, Guid userId, Guid territoryId)
    {
        var sharedStore = factory.GetSharedStore();

        var membership = sharedStore.Memberships.FirstOrDefault(m => m.UserId == userId && m.TerritoryId == territoryId);
        if (membership == null)
        {
            throw new InvalidOperationException("Membership must exist before creating capability");
        }

        var existing = sharedStore.MembershipCapabilities.FirstOrDefault(
            c => c.MembershipId == membership.Id &&
                 c.CapabilityType == MembershipCapabilityType.Curator &&
                 c.IsActive());
        if (existing != null)
        {
            return Task.CompletedTask;
        }

        var capability = new MembershipCapability(
            Guid.NewGuid(),
            membership.Id,
            MembershipCapabilityType.Curator,
            DateTime.UtcNow,
            userId,
            membership.Id,
            "Test curator capability");

        sharedStore.MembershipCapabilities.Add(capability);
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

        // Abrir votação diretamente no sharedStore (não há endpoint para isso)
        var sharedStore = factory.GetSharedStore();
        var domainVoting = sharedStore.Votings.FirstOrDefault(v => v.Id == voting.Id);
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

    /// <summary>
    /// Teste de integração para feed com filtro por interesses (filterByInterests).
    /// Requer que GET /api/v1/feed aceite filterByInterests e que InterestFilterService esteja integrado (Fase 14 / 14.5).
    /// </summary>
    [Fact]
    public async Task Feed_WithFilterByInterests_ReturnsOnlyMatchingPosts()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-feed-filter-1");
        SetAuthHeader(client, token);
        if (!client.DefaultRequestHeaders.Contains(ApiHeaders.SessionId))
        {
            client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "feed-filter-session");
        }

        var userId = await GetUserIdAsync(client);
        await CreateMembershipAsync(factory, userId, ActiveTerritoryId, MembershipRole.Resident);

        await client.PostAsJsonAsync("api/v1/users/me/interests", new AddInterestRequest("eventos"));

        var post1 = new CreatePostRequest(
            "Eventos na praça",
            "Conteúdo sobre eventos comunitários.",
            "GENERAL",
            "PUBLIC",
            null,
            null,
            null,
            null);
        var create1 = await client.PostAsJsonAsync($"api/v1/feed?territoryId={ActiveTerritoryId}", post1);
        if (create1.StatusCode != HttpStatusCode.Created)
        {
            var err = await create1.Content.ReadAsStringAsync();
            Assert.Fail($"Create post 1 failed: {create1.StatusCode}. {err}");
        }

        var post2 = new CreatePostRequest(
            "Outro assunto",
            "Conteúdo sem match com interesses.",
            "GENERAL",
            "PUBLIC",
            null,
            null,
            null,
            null);
        var create2 = await client.PostAsJsonAsync($"api/v1/feed?territoryId={ActiveTerritoryId}", post2);
        if (create2.StatusCode != HttpStatusCode.Created)
        {
            var err = await create2.Content.ReadAsStringAsync();
            Assert.Fail($"Create post 2 failed: {create2.StatusCode}. {err}");
        }

        var fullResponse = await client.GetAsync($"api/v1/feed?territoryId={ActiveTerritoryId}");
        fullResponse.EnsureSuccessStatusCode();
        var fullFeed = await fullResponse.Content.ReadFromJsonAsync<List<FeedItemResponse>>();
        Assert.NotNull(fullFeed);
        Assert.True(fullFeed!.Count >= 2, "Feed completo deve conter os dois posts criados.");

        var filteredResponse = await client.GetAsync(
            $"api/v1/feed?territoryId={ActiveTerritoryId}&filterByInterests=true");
        filteredResponse.EnsureSuccessStatusCode();
        var filteredFeed = await filteredResponse.Content.ReadFromJsonAsync<List<FeedItemResponse>>();
        Assert.NotNull(filteredFeed);

        Assert.True(
            filteredFeed!.Count <= fullFeed.Count,
            "Feed filtrado deve ser subconjunto do feed completo.");

        var lower = "eventos";
        foreach (var item in filteredFeed)
        {
            var t = (item.Title ?? "").ToLowerInvariant();
            var c = (item.Content ?? "").ToLowerInvariant();
            Assert.True(
                t.Contains(lower) || c.Contains(lower),
                $"Com filterByInterests=true, todos os itens devem conter 'eventos' em título ou conteúdo. Falhou: {item.Title} / {item.Content}");
        }
    }

    // ========== Testes de Segurança de Governança (Fase 14.5 - 14.3) ==========

    [Fact]
    public async Task Visitor_CannotVote_OnResidentsOnlyVoting()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Criar usuário resident que cria votação
        var residentToken = await LoginForTokenAsync(client, "google", "test-security-resident-1");
        SetAuthHeader(client, residentToken);
        var residentUserId = await GetUserIdAsync(client);
        await CreateMembershipAsync(factory, residentUserId, ActiveTerritoryId, MembershipRole.Resident);

        // Criar votação ResidentsOnly
        var createResponse = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/votings",
            new CreateVotingRequest(
                "ThemePrioritization",
                "Residents Only Voting",
                "Only residents can vote",
                new[] { "Option1", "Option2" },
                "ResidentsOnly", // Visibility
                null,
                null));

        createResponse.EnsureSuccessStatusCode();
        var voting = await createResponse.Content.ReadFromJsonAsync<VotingResponse>();
        Assert.NotNull(voting);

        // Abrir votação
        var sharedStore = factory.GetSharedStore();
        var domainVoting = sharedStore.Votings.FirstOrDefault(v => v.Id == voting.Id);
        if (domainVoting != null)
        {
            domainVoting.Open();
        }

        // Criar usuário visitor (sem membership ou com role Visitor)
        var visitorToken = await LoginForTokenAsync(client, "google", "test-security-visitor-1");
        SetAuthHeader(client, visitorToken);
        var visitorUserId = await GetUserIdAsync(client);
        await CreateMembershipAsync(factory, visitorUserId, ActiveTerritoryId, MembershipRole.Visitor);

        // Tentar votar (deve falhar)
        var voteResponse = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/votings/{voting.Id}/vote",
            new VoteRequest("Option1"));

        // Deve retornar Forbidden (403) ou BadRequest (400)
        Assert.True(
            voteResponse.StatusCode == HttpStatusCode.Forbidden || 
            voteResponse.StatusCode == HttpStatusCode.BadRequest,
            $"Expected Forbidden or BadRequest, but got {voteResponse.StatusCode}");
    }

    [Fact]
    public async Task Visitor_CannotVote_OnCuratorsOnlyVoting()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Criar curador que cria votação
        var curatorToken = await LoginForTokenAsync(client, "google", "test-security-curator-1");
        SetAuthHeader(client, curatorToken);
        var curatorUserId = await GetUserIdAsync(client);
        await CreateMembershipAsync(factory, curatorUserId, ActiveTerritoryId, MembershipRole.Resident);
        await CreateCuratorCapabilityAsync(factory, curatorUserId, ActiveTerritoryId);

        // Criar votação CuratorsOnly
        var createResponse = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/votings",
            new CreateVotingRequest(
                "ModerationRule",
                "Curators Only Voting",
                "Only curators can vote",
                new[] { "Option1", "Option2" },
                "CuratorsOnly", // Visibility
                null,
                null));

        createResponse.EnsureSuccessStatusCode();
        var voting = await createResponse.Content.ReadFromJsonAsync<VotingResponse>();
        Assert.NotNull(voting);

        // Abrir votação
        var sharedStore = factory.GetSharedStore();
        var domainVoting = sharedStore.Votings.FirstOrDefault(v => v.Id == voting.Id);
        if (domainVoting != null)
        {
            domainVoting.Open();
        }

        // Criar usuário visitor
        var visitorToken = await LoginForTokenAsync(client, "google", "test-security-visitor-2");
        SetAuthHeader(client, visitorToken);
        var visitorUserId = await GetUserIdAsync(client);
        await CreateMembershipAsync(factory, visitorUserId, ActiveTerritoryId, MembershipRole.Visitor);

        // Tentar votar (deve falhar)
        var voteResponse = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/votings/{voting.Id}/vote",
            new VoteRequest("Option1"));

        Assert.True(
            voteResponse.StatusCode == HttpStatusCode.Forbidden || 
            voteResponse.StatusCode == HttpStatusCode.BadRequest,
            $"Expected Forbidden or BadRequest, but got {voteResponse.StatusCode}");
    }

    [Fact]
    public async Task Resident_CannotCreate_ModerationRuleVoting()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-security-resident-2");
        SetAuthHeader(client, token);
        var userId = await GetUserIdAsync(client);
        await CreateMembershipAsync(factory, userId, ActiveTerritoryId, MembershipRole.Resident);

        // Tentar criar votação ModerationRule (apenas curador pode)
        var response = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/votings",
            new CreateVotingRequest(
                "ModerationRule", // Tipo que requer curador
                "Moderation Rule Voting",
                "Test description",
                new[] { "Option1", "Option2" },
                "AllMembers",
                null,
                null));

        // Deve retornar Forbidden (403) ou BadRequest (400)
        Assert.True(
            response.StatusCode == HttpStatusCode.Forbidden || 
            response.StatusCode == HttpStatusCode.BadRequest,
            $"Expected Forbidden or BadRequest, but got {response.StatusCode}");
    }

    [Fact]
    public async Task Resident_CannotCreate_FeatureFlagVoting()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-security-resident-3");
        SetAuthHeader(client, token);
        var userId = await GetUserIdAsync(client);
        await CreateMembershipAsync(factory, userId, ActiveTerritoryId, MembershipRole.Resident);

        // Tentar criar votação FeatureFlag (apenas curador pode)
        var response = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/votings",
            new CreateVotingRequest(
                "FeatureFlag", // Tipo que requer curador
                "Feature Flag Voting",
                "Test description",
                new[] { "Option1", "Option2" },
                "AllMembers",
                null,
                null));

        // Deve retornar Forbidden (403) ou BadRequest (400)
        Assert.True(
            response.StatusCode == HttpStatusCode.Forbidden || 
            response.StatusCode == HttpStatusCode.BadRequest,
            $"Expected Forbidden or BadRequest, but got {response.StatusCode}");
    }

    [Fact]
    public async Task User_CannotVoteTwice_OnSameVoting()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-security-double-vote");
        SetAuthHeader(client, token);
        var userId = await GetUserIdAsync(client);
        await CreateMembershipAsync(factory, userId, ActiveTerritoryId, MembershipRole.Resident);

        // Criar votação
        var createResponse = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/votings",
            new CreateVotingRequest(
                "ThemePrioritization",
                "Double Vote Test",
                "Test description",
                new[] { "Option1", "Option2" },
                "AllMembers",
                null,
                null));

        createResponse.EnsureSuccessStatusCode();
        var voting = await createResponse.Content.ReadFromJsonAsync<VotingResponse>();
        Assert.NotNull(voting);

        // Abrir votação
        var sharedStore = factory.GetSharedStore();
        var domainVoting = sharedStore.Votings.FirstOrDefault(v => v.Id == voting.Id);
        if (domainVoting != null)
        {
            domainVoting.Open();
        }

        // Primeiro voto (deve funcionar)
        var firstVote = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/votings/{voting.Id}/vote",
            new VoteRequest("Option1"));
        Assert.Equal(HttpStatusCode.NoContent, firstVote.StatusCode);

        // Segundo voto (deve falhar)
        var secondVote = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/votings/{voting.Id}/vote",
            new VoteRequest("Option2"));

        // Deve retornar BadRequest (400) ou Conflict (409)
        Assert.True(
            secondVote.StatusCode == HttpStatusCode.BadRequest || 
            secondVote.StatusCode == HttpStatusCode.Conflict,
            $"Expected BadRequest or Conflict, but got {secondVote.StatusCode}");
    }

    [Fact]
    public async Task User_CannotClose_OtherUsersVoting()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Criar usuário que cria votação
        var creatorToken = await LoginForTokenAsync(client, "google", "test-security-creator");
        SetAuthHeader(client, creatorToken);
        var creatorUserId = await GetUserIdAsync(client);
        await CreateMembershipAsync(factory, creatorUserId, ActiveTerritoryId, MembershipRole.Resident);

        // Criar votação
        var createResponse = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/votings",
            new CreateVotingRequest(
                "ThemePrioritization",
                "Close Test Voting",
                "Test description",
                new[] { "Option1", "Option2" },
                "AllMembers",
                null,
                null));

        createResponse.EnsureSuccessStatusCode();
        var voting = await createResponse.Content.ReadFromJsonAsync<VotingResponse>();
        Assert.NotNull(voting);

        // Criar outro usuário (sem ser curador)
        var otherToken = await LoginForTokenAsync(client, "google", "test-security-other-user");
        SetAuthHeader(client, otherToken);
        var otherUserId = await GetUserIdAsync(client);
        await CreateMembershipAsync(factory, otherUserId, ActiveTerritoryId, MembershipRole.Resident);

        // Tentar fechar votação alheia (deve falhar)
        var closeResponse = await client.PostAsync(
            $"api/v1/territories/{ActiveTerritoryId}/votings/{voting.Id}/close",
            null);

        // Deve retornar Forbidden (403) ou BadRequest (400)
        Assert.True(
            closeResponse.StatusCode == HttpStatusCode.Forbidden || 
            closeResponse.StatusCode == HttpStatusCode.BadRequest,
            $"Expected Forbidden or BadRequest, but got {closeResponse.StatusCode}");
    }

    [Fact]
    public async Task Curator_CanClose_AnyVoting()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Criar usuário que cria votação
        var creatorToken = await LoginForTokenAsync(client, "google", "test-security-creator-2");
        SetAuthHeader(client, creatorToken);
        var creatorUserId = await GetUserIdAsync(client);
        await CreateMembershipAsync(factory, creatorUserId, ActiveTerritoryId, MembershipRole.Resident);

        // Criar votação
        var createResponse = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/votings",
            new CreateVotingRequest(
                "ThemePrioritization",
                "Curator Close Test",
                "Test description",
                new[] { "Option1", "Option2" },
                "AllMembers",
                null,
                null));

        createResponse.EnsureSuccessStatusCode();
        var voting = await createResponse.Content.ReadFromJsonAsync<VotingResponse>();
        Assert.NotNull(voting);

        // Abrir votação para poder fechá-la (não há endpoint para isso)
        var sharedStore = factory.GetSharedStore();
        var domainVoting = sharedStore.Votings.FirstOrDefault(v => v.Id == voting.Id);
        if (domainVoting != null)
        {
            domainVoting.Open();
        }

        // Criar curador
        var curatorToken = await LoginForTokenAsync(client, "google", "test-security-curator-close");
        SetAuthHeader(client, curatorToken);
        var curatorUserId = await GetUserIdAsync(client);
        await CreateMembershipAsync(factory, curatorUserId, ActiveTerritoryId, MembershipRole.Resident);
        await CreateCuratorCapabilityAsync(factory, curatorUserId, ActiveTerritoryId);

        // Curador deve conseguir fechar votação alheia
        var closeResponse = await client.PostAsync(
            $"api/v1/territories/{ActiveTerritoryId}/votings/{voting.Id}/close",
            null);

        // Deve retornar sucesso (NoContent ou OK)
        Assert.True(
            closeResponse.StatusCode == HttpStatusCode.NoContent || 
            closeResponse.StatusCode == HttpStatusCode.OK,
            $"Expected NoContent or OK, but got {closeResponse.StatusCode}");
    }
}
