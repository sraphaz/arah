using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Araponga.Api;
using Araponga.Api.Contracts.Auth;
using Araponga.Api.Contracts.Users;
using Xunit;

namespace Araponga.Tests.Api;

/// <summary>
/// Testes de integração end-to-end para preferências de mídia do usuário.
/// </summary>
public sealed class UserMediaPreferencesIntegrationTests
{
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
    public async Task GetUserMediaPreferences_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("api/v1/user/media-preferences");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetUserMediaPreferences_WithAuthentication_ReturnsDefaultPreferences()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-preferences");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("api/v1/user/media-preferences");

        response.EnsureSuccessStatusCode();
        var preferences = await response.Content.ReadFromJsonAsync<UserMediaPreferencesResponse>();
        Assert.NotNull(preferences);
        
        // Verificar valores padrão
        Assert.True(preferences!.ShowImages);
        Assert.True(preferences.ShowVideos);
        Assert.True(preferences.ShowAudio);
        Assert.False(preferences.AutoPlayVideos);
        Assert.False(preferences.AutoPlayAudio);
    }

    [Fact]
    public async Task UpdateUserMediaPreferences_UpdatesSuccessfully()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-update-preferences");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var updateRequest = new UpdateUserMediaPreferencesRequest(
            ShowImages: false,
            ShowVideos: true,
            ShowAudio: false,
            AutoPlayVideos: true,
            AutoPlayAudio: false);

        var response = await client.PutAsJsonAsync(
            "api/v1/user/media-preferences",
            updateRequest);

        response.EnsureSuccessStatusCode();
        var updatedPreferences = await response.Content.ReadFromJsonAsync<UserMediaPreferencesResponse>();
        Assert.NotNull(updatedPreferences);
        Assert.False(updatedPreferences!.ShowImages);
        Assert.True(updatedPreferences.ShowVideos);
        Assert.False(updatedPreferences.ShowAudio);
        Assert.True(updatedPreferences.AutoPlayVideos);
        Assert.False(updatedPreferences.AutoPlayAudio);
    }

    [Fact]
    public async Task UpdateUserMediaPreferences_PartialUpdate_OnlyUpdatesSpecifiedFields()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-partial-update");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Obter preferências iniciais
        var getResponse = await client.GetAsync("api/v1/user/media-preferences");
        getResponse.EnsureSuccessStatusCode();
        var initialPreferences = await getResponse.Content.ReadFromJsonAsync<UserMediaPreferencesResponse>();

        // Atualizar apenas ShowImages
        var updateRequest = new UpdateUserMediaPreferencesRequest(
            ShowImages: false,
            null, null, null, null);

        var response = await client.PutAsJsonAsync(
            "api/v1/user/media-preferences",
            updateRequest);

        response.EnsureSuccessStatusCode();
        var updatedPreferences = await response.Content.ReadFromJsonAsync<UserMediaPreferencesResponse>();
        Assert.NotNull(updatedPreferences);
        
        // ShowImages deve estar atualizado
        Assert.False(updatedPreferences!.ShowImages);
        
        // Outros campos devem manter valores anteriores
        Assert.Equal(initialPreferences!.ShowVideos, updatedPreferences.ShowVideos);
        Assert.Equal(initialPreferences.ShowAudio, updatedPreferences.ShowAudio);
        Assert.Equal(initialPreferences.AutoPlayVideos, updatedPreferences.AutoPlayVideos);
        Assert.Equal(initialPreferences.AutoPlayAudio, updatedPreferences.AutoPlayAudio);
    }

    [Fact]
    public async Task GetUserMediaPreferences_AfterUpdate_ReturnsUpdatedPreferences()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-get-after-update");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Atualizar
        var updateRequest = new UpdateUserMediaPreferencesRequest(
            ShowImages: false,
            null, null, null, null);

        await client.PutAsJsonAsync(
            "api/v1/user/media-preferences",
            updateRequest);

        // Verificar que GET retorna valores atualizados
        var getResponse = await client.GetAsync("api/v1/user/media-preferences");
        getResponse.EnsureSuccessStatusCode();
        var preferences = await getResponse.Content.ReadFromJsonAsync<UserMediaPreferencesResponse>();
        Assert.NotNull(preferences);
        Assert.False(preferences!.ShowImages);
    }

    [Fact]
    public async Task UpdateUserMediaPreferences_DifferentUsers_HaveIndependentPreferences()
    {
        using var factory = new ApiFactory();

        // Usuário 1
        using var client1 = factory.CreateClient();
        var token1 = await LoginForTokenAsync(client1, "google", "user1-preferences");
        client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token1);

        var updateRequest1 = new UpdateUserMediaPreferencesRequest(
            ShowImages: false,
            null, null, null, null);
        await client1.PutAsJsonAsync("api/v1/user/media-preferences", updateRequest1);

        // Usuário 2
        using var client2 = factory.CreateClient();
        var token2 = await LoginForTokenAsync(client2, "google", "user2-preferences");
        client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token2);

        var updateRequest2 = new UpdateUserMediaPreferencesRequest(
            ShowImages: true,
            null, null, null, null);
        await client2.PutAsJsonAsync("api/v1/user/media-preferences", updateRequest2);

        // Verificar que cada usuário tem suas próprias preferências
        var response1 = await client1.GetAsync("api/v1/user/media-preferences");
        response1.EnsureSuccessStatusCode();
        var prefs1 = await response1.Content.ReadFromJsonAsync<UserMediaPreferencesResponse>();
        Assert.NotNull(prefs1);
        Assert.False(prefs1!.ShowImages);

        var response2 = await client2.GetAsync("api/v1/user/media-preferences");
        response2.EnsureSuccessStatusCode();
        var prefs2 = await response2.Content.ReadFromJsonAsync<UserMediaPreferencesResponse>();
        Assert.NotNull(prefs2);
        Assert.True(prefs2!.ShowImages);
    }
}
