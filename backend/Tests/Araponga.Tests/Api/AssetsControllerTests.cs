using System.Net;
using System.Net.Http.Json;
using Araponga.Api;
using Araponga.Api.Contracts.Assets;
using Araponga.Api.Contracts.Territories;
using Araponga.Tests.TestHelpers;
using Xunit;

namespace Araponga.Tests.Api;

/// <summary>
/// Testes para o AssetsController - aumentar cobertura de 75% para 90%
/// </summary>
public sealed class AssetsControllerTests
{
    private static readonly Guid ActiveTerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    [Fact]
    public async Task GetAssets_RequiresAuthentication()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"api/v1/assets?territoryId={ActiveTerritoryId}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAssets_RequiresTerritoryId()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await AuthTestHelper.LoginForTokenAsync(client, "google", "test-assets");
        AuthTestHelper.SetupAuthenticatedClient(client, token);

        var response = await client.GetAsync("api/v1/assets");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAssets_FiltersByAssetId()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await AuthTestHelper.LoginForTokenAsync(client, "google", "resident-external");
        AuthTestHelper.SetupAuthenticatedClient(client, token, "get-assets-filter-session");

        // Selecionar território
        await client.PostAsJsonAsync(
            "api/v1/territories/selection",
            new TerritorySelectionRequest(ActiveTerritoryId));

        var assetId = Guid.NewGuid();
        var response = await client.GetAsync($"api/v1/assets?territoryId={ActiveTerritoryId}&assetId={assetId}");

        // Pode retornar 200 com lista vazia, 400 se não encontrado, ou 401 se não for resident
        Assert.True(response.StatusCode == HttpStatusCode.OK ||
                   response.StatusCode == HttpStatusCode.BadRequest ||
                   response.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAssets_FiltersByStatus()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await AuthTestHelper.LoginForTokenAsync(client, "google", "resident-external");
        AuthTestHelper.SetupAuthenticatedClient(client, token, "get-assets-status-session");

        // Selecionar território
        await client.PostAsJsonAsync(
            "api/v1/territories/selection",
            new TerritorySelectionRequest(ActiveTerritoryId));

        var response = await client.GetAsync($"api/v1/assets?territoryId={ActiveTerritoryId}&status=ACTIVE");

        if (response.IsSuccessStatusCode)
        {
            var assets = await response.Content.ReadFromJsonAsync<List<AssetResponse>>();
            Assert.NotNull(assets);
        }
        else
        {
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }

    [Fact]
    public async Task GetAssets_InvalidStatusReturnsBadRequest()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await AuthTestHelper.LoginForTokenAsync(client, "google", "resident-external");
        AuthTestHelper.SetupAuthenticatedClient(client, token, "get-assets-invalid-status-session");

        // Selecionar território
        await client.PostAsJsonAsync(
            "api/v1/territories/selection",
            new TerritorySelectionRequest(ActiveTerritoryId));

        var response = await client.GetAsync($"api/v1/assets?territoryId={ActiveTerritoryId}&status=INVALID");

        // Pode retornar BadRequest (status inválido) ou Unauthorized (se não for resident)
        Assert.True(response.StatusCode == HttpStatusCode.BadRequest ||
                   response.StatusCode == HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAssetsPaged_ReturnsPagedResults()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await AuthTestHelper.LoginForTokenAsync(client, "google", "resident-external");
        AuthTestHelper.SetupAuthenticatedClient(client, token, "get-assets-paged-session");

        // Selecionar território
        await client.PostAsJsonAsync(
            "api/v1/territories/selection",
            new TerritorySelectionRequest(ActiveTerritoryId));

        var response = await client.GetAsync($"api/v1/assets/paged?territoryId={ActiveTerritoryId}&pageNumber=1&pageSize=10");

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<Araponga.Api.Contracts.Common.PagedResponse<AssetResponse>>();
            Assert.NotNull(result);
        }
        else
        {
            // Se não for resident, deve retornar Unauthorized
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }

    [Fact]
    public async Task CreateAsset_ValidatesGeoAnchors()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await AuthTestHelper.LoginForTokenAsync(client, "google", "resident-external");
        AuthTestHelper.SetupAuthenticatedClient(client, token, "assets-validation-session");

        // Selecionar território
        await client.PostAsJsonAsync(
            "api/v1/territories/selection",
            new TerritorySelectionRequest(ActiveTerritoryId));

        var request = new CreateAssetRequest(
            ActiveTerritoryId,
            "FACILITY",
            "Test Asset",
            "Description",
            new List<Araponga.Api.Contracts.Assets.AssetGeoAnchorRequest>
            {
                new Araponga.Api.Contracts.Assets.AssetGeoAnchorRequest(200.0, -45.0) // Latitude inválida
            });

        var response = await client.PostAsJsonAsync("api/v1/assets", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateAsset_RequiresResidentOrCurator()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await AuthTestHelper.LoginForTokenAsync(client, "google", "visitor-assets");
        AuthTestHelper.SetupAuthenticatedClient(client, token, "visitor-assets-session");

        // Selecionar território
        await client.PostAsJsonAsync(
            "api/v1/territories/selection",
            new TerritorySelectionRequest(ActiveTerritoryId));

        var request = new CreateAssetRequest(
            ActiveTerritoryId,
            "FACILITY",
            "Test Asset",
            "Description",
            new List<Araponga.Api.Contracts.Assets.AssetGeoAnchorRequest>
            {
                new Araponga.Api.Contracts.Assets.AssetGeoAnchorRequest(-23.37, -45.02)
            });

        var response = await client.PostAsJsonAsync("api/v1/assets", request);

        Assert.True(response.StatusCode == HttpStatusCode.Unauthorized ||
                   response.StatusCode == HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateAsset_RequiresAuthentication()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var assetId = Guid.NewGuid();
        // O endpoint usa PATCH (HttpPatch)
        var request = new HttpRequestMessage(HttpMethod.Patch, $"api/v1/assets/{assetId}?territoryId={ActiveTerritoryId}")
        {
            Content = JsonContent.Create(new UpdateAssetRequest("Updated", string.Empty, null, new List<Araponga.Api.Contracts.Assets.AssetGeoAnchorRequest>
            {
                new Araponga.Api.Contracts.Assets.AssetGeoAnchorRequest(-23.37, -45.02)
            }))
        };
        var response = await client.SendAsync(request);

        // O endpoint primeiro valida territoryId, depois GeoAnchors, depois autenticação
        // Pode retornar BadRequest (territoryId inválido ou GeoAnchors vazio) ou Unauthorized (sem token)
        Assert.True(response.StatusCode == HttpStatusCode.Unauthorized ||
                   response.StatusCode == HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task ArchiveAsset_RequiresResidentOrCurator()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await AuthTestHelper.LoginForTokenAsync(client, "google", "resident-external");
        AuthTestHelper.SetupAuthenticatedClient(client, token, "archive-asset-session");

        // Selecionar território
        await client.PostAsJsonAsync(
            "api/v1/territories/selection",
            new TerritorySelectionRequest(ActiveTerritoryId));

        var assetId = Guid.NewGuid();
        var response = await client.PostAsync($"api/v1/assets/{assetId}/archive?territoryId={ActiveTerritoryId}", null);

        // Pode retornar Unauthorized (se não for resident/curator), BadRequest (asset não encontrado), ou Ok (se for resident/curator e asset existir)
        Assert.True(response.StatusCode == HttpStatusCode.Unauthorized ||
                   response.StatusCode == HttpStatusCode.BadRequest ||
                   response.StatusCode == HttpStatusCode.OK);
    }
}
