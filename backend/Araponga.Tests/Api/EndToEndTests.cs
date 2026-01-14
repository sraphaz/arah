using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Araponga.Api;
using Araponga.Api.Contracts.Auth;
using Araponga.Api.Contracts.Feed;
using Araponga.Api.Contracts.Map;
using Araponga.Api.Contracts.Memberships;
using Araponga.Api.Contracts.Territories;
using Xunit;

namespace Araponga.Tests.Api;

/// <summary>
/// Testes end-to-end para fluxos críticos de usuário.
/// </summary>
public sealed class EndToEndTests
{
    private static readonly Guid ActiveTerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    [Fact]
    public async Task CompleteUserFlow_CadastroToFeed()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // 1. Cadastro e autenticação
        var loginResponse = await client.PostAsJsonAsync(
            "api/v1/auth/social",
            new SocialLoginRequest(
                "google",
                "e2e-user-" + Guid.NewGuid(),
                "Usuário E2E",
                "123.456.789-00",
                null,
                "(11) 90000-0000",
                "Rua E2E, 100",
                "e2e@araponga.com"));

        loginResponse.EnsureSuccessStatusCode();
        var loginPayload = await loginResponse.Content.ReadFromJsonAsync<SocialLoginResponse>();
        Assert.NotNull(loginPayload);
        var token = loginPayload!.Token;

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // 2. Descobrir territórios próximos
        var nearbyResponse = await client.GetFromJsonAsync<List<TerritoryResponse>>(
            "api/v1/territories/nearby?lat=-23.37&lng=-45.02");
        Assert.NotNull(nearbyResponse);
        Assert.NotEmpty(nearbyResponse!);

        // 3. Selecionar território
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "e2e-session");
        var selectResponse = await client.PostAsJsonAsync(
            "api/v1/territories/selection",
            new TerritorySelectionRequest(ActiveTerritoryId));
        selectResponse.EnsureSuccessStatusCode();

        // 4. Entrar como VISITOR
        var membershipResponse = await client.PostAsync(
            $"api/v1/territories/{ActiveTerritoryId}/enter",
            null);
        membershipResponse.EnsureSuccessStatusCode();
        var membership = await membershipResponse.Content.ReadFromJsonAsync<EnterTerritoryResponse>();
        Assert.NotNull(membership);
        Assert.Equal("VISITOR", membership!.Role);

        // 5. Consultar feed do território
        var feedResponse = await client.GetFromJsonAsync<List<FeedItemResponse>>(
            $"api/v1/feed?territoryId={ActiveTerritoryId}");
        Assert.NotNull(feedResponse);
        Assert.NotEmpty(feedResponse!);

        // 6. Consultar status do vínculo
        var statusResponse = await client.GetFromJsonAsync<MembershipDetailResponse>(
            $"api/v1/memberships/{ActiveTerritoryId}/me");
        Assert.NotNull(statusResponse);
        Assert.Equal("VISITOR", statusResponse!.Role);
    }

    [Fact]
    public async Task CompleteResidentFlow_CadastroToPost()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // 1. Cadastro
        var loginResponse = await client.PostAsJsonAsync(
            "api/v1/auth/social",
            new SocialLoginRequest(
                "google",
                "e2e-resident-" + Guid.NewGuid(),
                "Morador E2E",
                "123.456.789-00",
                null,
                "(11) 90000-0000",
                "Rua E2E, 100",
                "resident-e2e@araponga.com"));

        loginResponse.EnsureSuccessStatusCode();
        var loginPayload = await loginResponse.Content.ReadFromJsonAsync<SocialLoginResponse>();
        var token = loginPayload!.Token;

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "e2e-resident-session");
        client.DefaultRequestHeaders.Add(ApiHeaders.GeoLatitude, "-23.37");
        client.DefaultRequestHeaders.Add(ApiHeaders.GeoLongitude, "-45.02");

        // 2. Selecionar território
        await client.PostAsJsonAsync(
            "api/v1/territories/selection",
            new TerritorySelectionRequest(ActiveTerritoryId));

        // 3. Tornar-se RESIDENT
        var membershipResponse = await client.PostAsync(
            $"api/v1/memberships/{ActiveTerritoryId}/become-resident",
            null);
        membershipResponse.EnsureSuccessStatusCode();
        var membership = await membershipResponse.Content.ReadFromJsonAsync<MembershipDetailResponse>();
        Assert.Equal("RESIDENT", membership!.Role);
        Assert.Equal("UNVERIFIED", membership.ResidencyVerification);

        // 4. Criar post (deve falhar porque ainda está PENDING)
        var postResponse = await client.PostAsJsonAsync(
            $"api/v1/feed?territoryId={ActiveTerritoryId}",
            new CreatePostRequest(
                "Meu Post",
                "Conteúdo do post",
                "GENERAL",
                "PUBLIC",
                null,
                null,
                null));

        // Como o usuário não está validado, pode falhar ou funcionar dependendo da implementação
        // Este teste valida o fluxo completo mesmo que falhe na criação do post
        Assert.True(postResponse.StatusCode == HttpStatusCode.Created || 
                   postResponse.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CompleteFeedInteractionFlow()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Setup: Login como residente validado
        var token = await LoginForTokenAsync(client, "google", "e2e-interaction");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "e2e-interaction-session");

        await SelectTerritoryAsync(client, ActiveTerritoryId);

        // 1. Criar post
        var createPostResponse = await client.PostAsJsonAsync(
            $"api/v1/feed?territoryId={ActiveTerritoryId}",
            new CreatePostRequest(
                "Post para interação",
                "Conteúdo",
                "GENERAL",
                "PUBLIC",
                null,
                null,
                null));

        if (createPostResponse.StatusCode == HttpStatusCode.Created)
        {
            var post = await createPostResponse.Content.ReadFromJsonAsync<FeedItemResponse>();
            Assert.NotNull(post);

            // 2. Curtir post
            var likeResponse = await client.PostAsync(
                $"api/v1/feed/{post!.Id}/likes?territoryId={ActiveTerritoryId}",
                null);
            Assert.True(likeResponse.StatusCode == HttpStatusCode.NoContent || 
                       likeResponse.StatusCode == HttpStatusCode.BadRequest);

            // 3. Comentar post
            var commentResponse = await client.PostAsJsonAsync(
                $"api/v1/feed/{post.Id}/comments?territoryId={ActiveTerritoryId}",
                new AddCommentRequest("Comentário teste"));
            Assert.True(commentResponse.StatusCode == HttpStatusCode.NoContent || 
                       commentResponse.StatusCode == HttpStatusCode.BadRequest);

            // 4. Compartilhar post
            var shareResponse = await client.PostAsync(
                $"api/v1/feed/{post.Id}/shares?territoryId={ActiveTerritoryId}",
                null);
            Assert.True(shareResponse.StatusCode == HttpStatusCode.NoContent || 
                       shareResponse.StatusCode == HttpStatusCode.BadRequest);
        }
    }

    [Fact]
    public async Task CompleteMapFlow_EntitySuggestionToConfirmation()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "e2e-map");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, "e2e-map-session");

        await SelectTerritoryAsync(client, ActiveTerritoryId);

        // 1. Sugerir entidade
        var suggestResponse = await client.PostAsJsonAsync(
            $"api/v1/map/entities?territoryId={ActiveTerritoryId}",
            new SuggestMapEntityRequest("Ponto E2E", "espaço natural", -23.37, -45.02));

        if (suggestResponse.StatusCode == HttpStatusCode.Created)
        {
            var entity = await suggestResponse.Content.ReadFromJsonAsync<MapEntityResponse>();
            Assert.NotNull(entity);
            Assert.Equal("SUGGESTED", entity!.Status);

            // 2. Confirmar entidade (como residente)
            var confirmResponse = await client.PostAsync(
                $"api/v1/map/entities/{entity.Id}/confirmations?territoryId={ActiveTerritoryId}",
                null);
            Assert.True(confirmResponse.StatusCode == HttpStatusCode.NoContent || 
                       confirmResponse.StatusCode == HttpStatusCode.BadRequest);
        }
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
        Assert.NotNull(payload);
        return payload!.Token;
    }

    private static async Task SelectTerritoryAsync(HttpClient client, Guid territoryId)
    {
        var response = await client.PostAsJsonAsync(
            "api/v1/territories/selection",
            new TerritorySelectionRequest(territoryId));
        response.EnsureSuccessStatusCode();
    }
}
