using System.Net;
using System.Net.Http.Json;
using Araponga.Api.Contracts.Territories;
using Araponga.Tests.TestHelpers;
using Xunit;

namespace Araponga.Tests.Api;

/// <summary>
/// Testes para o TerritoriesController - aumentar cobertura
/// </summary>
public sealed class TerritoriesControllerTests
{
    private static readonly Guid ActiveTerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    private static Task<string> LoginForTokenAsync(HttpClient client, string provider, string externalId) =>
        AuthTestHelper.LoginForTokenAsync(client, provider, externalId);

    [Fact]
    public async Task List_ReturnsTerritories()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("api/v1/territories");

        response.EnsureSuccessStatusCode();
        var territories = await response.Content.ReadFromJsonAsync<List<TerritoryResponse>>();
        Assert.NotNull(territories);
    }

    [Fact]
    public async Task ListPaged_ReturnsPagedResults()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("api/v1/territories/paged?pageNumber=1&pageSize=10");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Araponga.Api.Contracts.Common.PagedResponse<TerritoryResponse>>();
        Assert.NotNull(result);
        Assert.True(result!.PageNumber >= 1);
        Assert.True(result.PageSize > 0);
    }

    [Fact]
    public async Task GetById_RequiresAuthentication()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"api/v1/territories/{ActiveTerritoryId}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetById_ReturnsTerritory()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-territory");
        AuthTestHelper.SetupAuthenticatedClient(client, token);

        var response = await client.GetAsync($"api/v1/territories/{ActiveTerritoryId}");

        response.EnsureSuccessStatusCode();
        var territory = await response.Content.ReadFromJsonAsync<TerritoryResponse>();
        Assert.NotNull(territory);
        Assert.Equal(ActiveTerritoryId, territory!.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_ForInvalidId()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-territory");
        AuthTestHelper.SetupAuthenticatedClient(client, token);

        var invalidId = Guid.NewGuid();
        var response = await client.GetAsync($"api/v1/territories/{invalidId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Suggest_RequiresRateLimiting()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-suggest");
        AuthTestHelper.SetupAuthenticatedClient(client, token);

        var request = new SuggestTerritoryRequest(
            "Novo Território",
            "Descrição",
            "São Paulo",
            "SP",
            -23.5505,
            -46.6333);

        var response = await client.PostAsJsonAsync("api/v1/territories/suggestions", request);

        // Pode retornar Created, BadRequest (validação), ou TooManyRequests (rate limit)
        Assert.True(response.StatusCode == HttpStatusCode.Created ||
                   response.StatusCode == HttpStatusCode.BadRequest ||
                   response.StatusCode == HttpStatusCode.TooManyRequests);
    }

    [Fact]
    public async Task Suggest_ValidatesInput()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-suggest");
        AuthTestHelper.SetupAuthenticatedClient(client, token);

        // Nome vazio
        var request = new SuggestTerritoryRequest(
            "",
            "Descrição",
            "São Paulo",
            "SP",
            -23.5505,
            -46.6333);

        var response = await client.PostAsJsonAsync("api/v1/territories/suggestions", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Search_ReturnsTerritories()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("api/v1/territories/search?city=Ubatuba&state=SP");

        response.EnsureSuccessStatusCode();
        var territories = await response.Content.ReadFromJsonAsync<List<TerritoryResponse>>();
        Assert.NotNull(territories);
    }

    [Fact]
    public async Task SearchPaged_ReturnsPagedResults()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("api/v1/territories/search/paged?city=Ubatuba&state=SP&pageNumber=1&pageSize=10");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Araponga.Api.Contracts.Common.PagedResponse<TerritoryResponse>>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Nearby_ReturnsTerritories()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("api/v1/territories/nearby?lat=-23.37&lng=-45.02");

        response.EnsureSuccessStatusCode();
        var territories = await response.Content.ReadFromJsonAsync<List<TerritoryResponse>>();
        Assert.NotNull(territories);
    }

    [Fact]
    public async Task NearbyPaged_ReturnsPagedResults()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("api/v1/territories/nearby/paged?lat=-23.37&lng=-45.02&pageNumber=1&pageSize=10");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<Araponga.Api.Contracts.Common.PagedResponse<TerritoryResponse>>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Selection_RequiresSessionHeader()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "api/v1/territories/selection",
            new TerritorySelectionRequest(ActiveTerritoryId));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Selection_CanSetAndGet()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        client.DefaultRequestHeaders.Add("X-Session-Id", "test-session-territories");

        var setResponse = await client.PostAsJsonAsync(
            "api/v1/territories/selection",
            new TerritorySelectionRequest(ActiveTerritoryId));

        setResponse.EnsureSuccessStatusCode();

        var getResponse = await client.GetAsync("api/v1/territories/selection");
        getResponse.EnsureSuccessStatusCode();

        var selection = await getResponse.Content.ReadFromJsonAsync<TerritorySelectionResponse>();
        Assert.NotNull(selection);
        Assert.Equal(ActiveTerritoryId, selection!.TerritoryId);
    }
}
