using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Araponga.Api;
using Araponga.Api.Contracts.Auth;
using Araponga.Api.Contracts.Media;
using Araponga.Api.Contracts.Territories;
using Xunit;

namespace Araponga.Tests.Api;

/// <summary>
/// Testes de integração end-to-end para configuração de mídias por território.
/// </summary>
public sealed class MediaConfigIntegrationTests
{
    private static readonly Guid ActiveTerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid CuratorUserId = Guid.Parse("cccccccc-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

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

    [Fact]
    public async Task GetMediaConfig_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync(
            $"api/v1/territories/{ActiveTerritoryId}/media-config");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMediaConfig_WithAuthentication_ReturnsDefaultConfig()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-media-config");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync(
            $"api/v1/territories/{ActiveTerritoryId}/media-config");

        response.EnsureSuccessStatusCode();
        var config = await response.Content.ReadFromJsonAsync<TerritoryMediaConfigResponse>();
        Assert.NotNull(config);
        Assert.Equal(ActiveTerritoryId, config!.TerritoryId);
        Assert.NotNull(config.Posts);
        Assert.NotNull(config.Events);
        Assert.NotNull(config.Marketplace);
        Assert.NotNull(config.Chat);
        
        // Verificar valores padrão
        Assert.True(config.Posts.ImagesEnabled);
        Assert.True(config.Posts.VideosEnabled);
        Assert.True(config.Posts.AudioEnabled);
    }

    [Fact]
    public async Task UpdateMediaConfig_WithoutCurator_ReturnsUnauthorized()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-no-curator");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateRequest = new UpdateTerritoryMediaConfigRequest(
            new MediaContentConfigRequest(
                ImagesEnabled: false,
                null, null, null, null, null, null, null, null, null, null, null, null, null),
            null, null, null);

        var response = await client.PutAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/media-config",
            updateRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateMediaConfig_AsCurator_UpdatesSuccessfully()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Login como curador
        var token = await LoginForTokenAsync(client, "google", "curator-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateRequest = new UpdateTerritoryMediaConfigRequest(
            new MediaContentConfigRequest(
                ImagesEnabled: false,
                VideosEnabled: true,
                AudioEnabled: true,
                MaxMediaCount: 5,
                MaxVideoCount: null,
                MaxAudioCount: null,
                MaxImageSizeBytes: null,
                MaxVideoSizeBytes: null,
                MaxAudioSizeBytes: null,
                MaxVideoDurationSeconds: null,
                MaxAudioDurationSeconds: null,
                AllowedImageMimeTypes: null,
                AllowedVideoMimeTypes: null,
                AllowedAudioMimeTypes: null),
            null, null, null);

        var response = await client.PutAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/media-config",
            updateRequest);

        response.EnsureSuccessStatusCode();
        var updatedConfig = await response.Content.ReadFromJsonAsync<TerritoryMediaConfigResponse>();
        Assert.NotNull(updatedConfig);
        Assert.False(updatedConfig!.Posts.ImagesEnabled);
        Assert.True(updatedConfig.Posts.VideosEnabled);
        Assert.Equal(5, updatedConfig.Posts.MaxMediaCount);
    }

    [Fact]
    public async Task UpdateMediaConfig_ChatConfig_UpdatesSuccessfully()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "curator-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateRequest = new UpdateTerritoryMediaConfigRequest(
            null, null, null,
            new MediaChatConfigRequest(
                ImagesEnabled: false,
                AudioEnabled: true,
                VideosEnabled: false,
                MaxImageSizeBytes: 3 * 1024 * 1024,
                MaxAudioSizeBytes: 1 * 1024 * 1024,
                MaxAudioDurationSeconds: 30,
                AllowedImageMimeTypes: null,
                AllowedAudioMimeTypes: null));

        var response = await client.PutAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/media-config",
            updateRequest);

        response.EnsureSuccessStatusCode();
        var updatedConfig = await response.Content.ReadFromJsonAsync<TerritoryMediaConfigResponse>();
        Assert.NotNull(updatedConfig);
        Assert.False(updatedConfig!.Chat.ImagesEnabled);
        Assert.True(updatedConfig.Chat.AudioEnabled);
        Assert.False(updatedConfig.Chat.VideosEnabled); // Sempre false para chat
        Assert.Equal(3 * 1024 * 1024, updatedConfig.Chat.MaxImageSizeBytes);
        Assert.Equal(30, updatedConfig.Chat.MaxAudioDurationSeconds);
    }

    [Fact]
    public async Task UpdateMediaConfig_PartialUpdate_OnlyUpdatesSpecifiedFields()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "curator-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Obter config inicial
        var getResponse = await client.GetAsync(
            $"api/v1/territories/{ActiveTerritoryId}/media-config");
        getResponse.EnsureSuccessStatusCode();
        var initialConfig = await getResponse.Content.ReadFromJsonAsync<TerritoryMediaConfigResponse>();

        // Atualizar apenas Marketplace
        var updateRequest = new UpdateTerritoryMediaConfigRequest(
            null,
            null,
            new MediaContentConfigRequest(
                ImagesEnabled: false,
                null, null, null, null, null, null, null, null, null, null, null, null, null),
            null);

        var response = await client.PutAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/media-config",
            updateRequest);

        response.EnsureSuccessStatusCode();
        var updatedConfig = await response.Content.ReadFromJsonAsync<TerritoryMediaConfigResponse>();
        Assert.NotNull(updatedConfig);
        
        // Marketplace deve estar atualizado
        Assert.False(updatedConfig!.Marketplace.ImagesEnabled);
        
        // Posts e Events devem manter valores anteriores
        Assert.Equal(initialConfig!.Posts.ImagesEnabled, updatedConfig.Posts.ImagesEnabled);
        Assert.Equal(initialConfig.Events.ImagesEnabled, updatedConfig.Events.ImagesEnabled);
    }

    [Fact]
    public async Task GetMediaConfig_AfterUpdate_ReturnsUpdatedConfig()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "curator-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Atualizar
        var updateRequest = new UpdateTerritoryMediaConfigRequest(
            new MediaContentConfigRequest(
                ImagesEnabled: false,
                null, null, null, null, null, null, null, null, null, null, null, null, null),
            null, null, null);

        await client.PutAsJsonAsync(
            $"api/v1/territories/{ActiveTerritoryId}/media-config",
            updateRequest);

        // Verificar que GET retorna valores atualizados
        var getResponse = await client.GetAsync(
            $"api/v1/territories/{ActiveTerritoryId}/media-config");
        getResponse.EnsureSuccessStatusCode();
        var config = await getResponse.Content.ReadFromJsonAsync<TerritoryMediaConfigResponse>();
        Assert.NotNull(config);
        Assert.False(config!.Posts.ImagesEnabled);
    }
}
