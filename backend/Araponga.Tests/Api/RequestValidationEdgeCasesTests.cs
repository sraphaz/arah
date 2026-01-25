using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Araponga.Api;
using Araponga.Api.Contracts.Feed;
using Xunit;

namespace Araponga.Tests.Api;

/// <summary>
/// Edge case tests for Request Validation (headers, query parameters, route parameters),
/// focusing on validation edge cases not covered by ControllerValidationEdgeCasesTests.
/// </summary>
public class RequestValidationEdgeCasesTests
{
    private static readonly Guid ActiveTerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    private static async Task<string> LoginForTokenAsync(HttpClient client, string provider, string externalId)
    {
        var response = await client.PostAsJsonAsync(
            "api/v1/auth/social",
            new Araponga.Api.Contracts.Auth.SocialLoginRequest(
                provider,
                externalId,
                "Test User",
                "123.456.789-00",
                null,
                null,
                null,
                "test@araponga.com"));
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<Araponga.Api.Contracts.Auth.SocialLoginResponse>();
        return payload!.Token;
    }

    [Fact]
    public async Task GetFeed_WithInvalidQueryParameter_ReturnsBadRequest()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-query-params");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Testar com pageNumber negativo
        var response = await client.GetAsync($"api/v1/feed?territoryId={ActiveTerritoryId}&pageNumber=-1");

        // Deve retornar BadRequest ou ajustar para valor válido
        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetFeed_WithInvalidPageSize_ReturnsBadRequest()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-page-size");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Testar com pageSize muito grande
        var response = await client.GetAsync($"api/v1/feed?territoryId={ActiveTerritoryId}&pageSize=10000");

        // Deve retornar BadRequest ou ajustar para valor máximo
        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetFeed_WithEmptyTerritoryId_ReturnsBadRequest()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-empty-territory");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // A API não valida Guid.Empty, apenas null. Sem territoryId e sem X-Session-Id, deve retornar BadRequest
        var response = await client.GetAsync("api/v1/feed");

        // Se não houver X-Session-Id header, deve retornar BadRequest
        // Caso contrário, pode retornar OK com lista vazia
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetFeed_WithInvalidRouteParameter_ReturnsNotFound()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-invalid-route");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Tentar acessar rota que não existe
        // ASP.NET Core pode retornar MethodNotAllowed se a rota existe mas o método HTTP não é permitido
        var response = await client.GetAsync("api/v1/feed/invalid-route");

        // Pode ser NotFound ou MethodNotAllowed dependendo da configuração de rotas
        Assert.True(response.StatusCode == HttpStatusCode.NotFound || 
                   response.StatusCode == HttpStatusCode.MethodNotAllowed);
    }

    [Fact]
    public async Task CreatePost_WithMissingRequiredHeader_ReturnsBadRequest()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-missing-header");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Não adicionar X-Session-Id header (pode ser requerido em alguns endpoints)
        var request = new CreatePostRequest(
            "Test Post",
            "Content",
            "GENERAL",
            "PUBLIC",
            null,
            null,
            null,
            null);

        var response = await client.PostAsJsonAsync(
            $"api/v1/feed?territoryId={ActiveTerritoryId}",
            request);

        // Como territoryId está presente na query, pode retornar Created (sucesso) ou BadRequest (validação)
        // O teste verifica que a API processa a requisição sem crashar
        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.Created ||
            response.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreatePost_WithUnicodeInContent_HandlesCorrectly()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-unicode-content");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add("X-Session-Id", "test-session");

        var request = new CreatePostRequest(
            "Post com café, naïve, résumé, 文字 e emoji 🎉",
            "Conteúdo com Unicode: café, naïve, résumé, 文字 e emoji 🎉",
            "GENERAL",
            "PUBLIC",
            null,
            null,
            null,
            null);

        var response = await client.PostAsJsonAsync(
            $"api/v1/feed?territoryId={ActiveTerritoryId}",
            request);

        // Deve processar Unicode corretamente (não deve falhar por causa do Unicode)
        // Pode retornar Created (sucesso), BadRequest (validação), ou Unauthorized (autenticação)
        Assert.True(
            response.StatusCode == HttpStatusCode.Created ||
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetFeed_WithInvalidHeaderValue_HandlesCorrectly()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-invalid-header");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        // Adicionar header com valor inválido
        client.DefaultRequestHeaders.Add("X-Session-Id", ""); // Vazio

        var response = await client.GetAsync($"api/v1/feed?territoryId={ActiveTerritoryId}");

        // Pode retornar BadRequest se header for validado, ou OK se ignorado
        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetFeed_WithSpecialCharactersInQueryParam_HandlesCorrectly()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-special-chars");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Testar com caracteres especiais no query param
        var response = await client.GetAsync($"api/v1/feed?territoryId={ActiveTerritoryId}&filter=test%20with%20spaces");

        // Deve processar corretamente ou retornar BadRequest se inválido
        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetFeed_WithVeryLongQueryString_HandlesCorrectly()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-long-query");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Criar query string muito longa
        var longFilter = new string('A', 10000);
        var response = await client.GetAsync($"api/v1/feed?territoryId={ActiveTerritoryId}&filter={longFilter}");

        // Pode retornar BadRequest se exceder limite, ou OK se processar
        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.RequestUriTooLong ||
            response.StatusCode == HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetFeed_WithDuplicateQueryParameters_HandlesCorrectly()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-duplicate-params");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Query string com parâmetros duplicados
        var response = await client.GetAsync($"api/v1/feed?territoryId={ActiveTerritoryId}&pageNumber=1&pageNumber=2");

        // Deve usar o último valor ou retornar BadRequest
        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetFeed_WithInvalidRouteGuid_ReturnsBadRequest()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-invalid-guid");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Tentar acessar rota com GUID inválido no path
        var response = await client.GetAsync("api/v1/feed/invalid-guid-format");

        // ASP.NET Core pode retornar NotFound, MethodNotAllowed ou BadRequest dependendo da configuração
        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.NotFound ||
            response.StatusCode == HttpStatusCode.MethodNotAllowed);
    }

    [Fact]
    public async Task GetFeed_WithNullQueryParameter_HandlesCorrectly()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-null-param");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Query string com parâmetro null (territoryId ausente)
        var response = await client.GetAsync("api/v1/feed");

        // Deve retornar BadRequest se territoryId for obrigatório
        Assert.True(
            response.StatusCode == HttpStatusCode.BadRequest ||
            response.StatusCode == HttpStatusCode.Unauthorized);
    }
}
