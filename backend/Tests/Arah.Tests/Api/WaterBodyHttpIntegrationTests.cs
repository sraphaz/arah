using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Arah.Api;
using Arah.Api.Contracts.Assets;
using Arah.Api.Contracts.Common;
using Arah.Api.Contracts.Territories;
using Arah.Modules.Assets.Domain;
using Arah.Tests.TestHelpers;
using Xunit;

namespace Arah.Tests.Api;

/// <summary>
/// HTTP smoke para NaturalAsset ponto (WA-N1). AC-WA-1 parcial, AC-WA-2.
/// </summary>
public sealed class WaterBodyHttpIntegrationTests
{
    private static readonly Guid ActiveTerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    [Fact]
    public async Task Create_List_Get_Publish_PointNaturalAsset()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var residentToken = await AuthTestHelper.LoginForTokenAsync(client, "google", "resident-external");
        AuthTestHelper.SetupAuthenticatedClient(client, residentToken, "wa-n1-natural-asset");
        await client.PostAsJsonAsync(
            "api/v1/territories/selection",
            new TerritorySelectionRequest(ActiveTerritoryId));

        var create = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/natural-assets",
            new CreateNaturalAssetRequest(
                NaturalAssetType.Spring,
                "Nascente do Córrego",
                "Cuidar da água",
                -23.382,
                -45.032,
                WaterPointWaterType.Well));

        Assert.Equal(HttpStatusCode.OK, create.StatusCode);
        var created = await create.Content.ReadFromJsonAsync<NaturalAssetResponse>();
        Assert.NotNull(created);
        Assert.Equal("PENDING", created!.Status);
        Assert.Equal(NaturalAssetType.Spring, created.Type);
        Assert.Equal(WaterPointWaterType.Well, created.WaterType);

        var list = await client.GetFromJsonAsync<PagedResponse<NaturalAssetResponse>>(
            $"api/v1/territories/{ActiveTerritoryId}/natural-assets?pageNumber=1&pageSize=10");
        Assert.NotNull(list);
        Assert.Contains(list!.Items, a => a.Id == created.Id);

        var get = await client.GetFromJsonAsync<NaturalAssetResponse>(
            $"api/v1/territories/{ActiveTerritoryId}/natural-assets/{created.Id}");
        Assert.NotNull(get);
        Assert.Equal(created.Id, get!.Id);

        var curatorToken = await AuthTestHelper.LoginForTokenAsync(client, "google", "curator-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", curatorToken);

        var publish = await client.PostAsync(
            $"api/v1/territories/{ActiveTerritoryId}/natural-assets/{created.Id}/publish",
            null);
        Assert.Equal(HttpStatusCode.OK, publish.StatusCode);
        var published = await publish.Content.ReadFromJsonAsync<NaturalAssetResponse>();
        Assert.NotNull(published);
        Assert.Equal("PUBLISHED", published!.Status);
    }

    [Fact]
    public async Task Create_River_ReturnsBadRequest()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await AuthTestHelper.LoginForTokenAsync(client, "google", "resident-external");
        AuthTestHelper.SetupAuthenticatedClient(client, token, "wa-n1-river-reject");
        await client.PostAsJsonAsync(
            "api/v1/territories/selection",
            new TerritorySelectionRequest(ActiveTerritoryId));

        var response = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/natural-assets",
            new CreateNaturalAssetRequest("RIVER", "Rio", null, -23.382, -45.032));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Create_RequiresAuthentication()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/natural-assets",
            new CreateNaturalAssetRequest(NaturalAssetType.Spring, "X", null, -23.382, -45.032));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Get_MissingAsset_ReturnsNotFound()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await AuthTestHelper.LoginForTokenAsync(client, "google", "resident-external");
        AuthTestHelper.SetupAuthenticatedClient(client, token, "wa-n1-get-404");
        await client.PostAsJsonAsync(
            "api/v1/territories/selection",
            new TerritorySelectionRequest(ActiveTerritoryId));

        var missingId = Guid.Parse("99999999-9999-9999-9999-999999999999");
        var response = await client.GetAsync(
            $"api/v1/territories/{ActiveTerritoryId}/natural-assets/{missingId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
