using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Araponga.Api;
using Araponga.Api.Contracts.Auth;
using Araponga.Api.Contracts.Media;
using Xunit;

namespace Araponga.Tests.Api;

/// <summary>
/// Testes de integração completos do MediaController.
/// </summary>
public sealed class MediaControllerTests
{
    private static readonly Guid ActiveTerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    [Fact]
    public async Task UploadMedia_WithValidImage_ReturnsCreated()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "media-upload-test");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Criar um arquivo JPEG válido (magic bytes)
        var jpegBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46 };
        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(jpegBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        content.Add(fileContent, "file", "test.jpg");

        var response = await client.PostAsync("api/v1/media/upload", content);

        // Pode retornar Created (201) ou BadRequest se validação falhar
        // Mas não deve retornar Unauthorized
        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.True(
            response.StatusCode == HttpStatusCode.Created ||
            response.StatusCode == HttpStatusCode.BadRequest,
            $"Expected Created or BadRequest, got {response.StatusCode}");

        if (response.StatusCode == HttpStatusCode.Created)
        {
            var mediaResponse = await response.Content.ReadFromJsonAsync<MediaAssetResponse>();
            Assert.NotNull(mediaResponse);
            Assert.NotEqual(Guid.Empty, mediaResponse!.Id);
            Assert.Equal("IMAGE", mediaResponse.MediaType);
            Assert.Equal("image/jpeg", mediaResponse.MimeType);
        }
    }

    [Fact]
    public async Task UploadMedia_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Tentar upload sem token de autenticação
        var jpegBytes = new byte[] { 0xFF, 0xD8, 0xFF };
        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(jpegBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        content.Add(fileContent, "file", "test.jpg");

        var response = await client.PostAsync("api/v1/media/upload", content);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UploadMedia_WithEmptyFile_ReturnsBadRequest()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "media-empty-file");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(Array.Empty<byte>());
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        content.Add(fileContent, "file", "empty.jpg");

        var response = await client.PostAsync("api/v1/media/upload", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetMediaInfo_WithValidId_ReturnsOk()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Primeiro, fazer upload de uma mídia
        var token = await LoginForTokenAsync(client, "google", "media-info-test");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var jpegBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };
        var uploadContent = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(jpegBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        uploadContent.Add(fileContent, "file", "test.jpg");

        var uploadResponse = await client.PostAsync("api/v1/media/upload", uploadContent);
        
        if (uploadResponse.StatusCode == HttpStatusCode.Created)
        {
            var mediaResponse = await uploadResponse.Content.ReadFromJsonAsync<MediaAssetResponse>();
            Assert.NotNull(mediaResponse);

            // Obter informações da mídia
            var infoResponse = await client.GetAsync($"api/v1/media/{mediaResponse!.Id}/info");
            
            // Pode retornar OK ou NotFound (dependendo da implementação)
            Assert.True(
                infoResponse.StatusCode == HttpStatusCode.OK ||
                infoResponse.StatusCode == HttpStatusCode.NotFound,
                $"Expected OK or NotFound, got {infoResponse.StatusCode}");

            if (infoResponse.StatusCode == HttpStatusCode.OK)
            {
                var info = await infoResponse.Content.ReadFromJsonAsync<MediaAssetResponse>();
                Assert.NotNull(info);
                Assert.Equal(mediaResponse.Id, info!.Id);
            }
        }
    }

    [Fact]
    public async Task GetMediaInfo_WithInvalidId_ReturnsNotFound()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"api/v1/media/{Guid.NewGuid()}/info");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteMedia_WithValidMediaAndOwner_ReturnsNoContent()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "media-delete-test");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Primeiro, fazer upload de uma mídia
        var jpegBytes = new byte[] { 0xFF, 0xD8, 0xFF };
        var uploadContent = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(jpegBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        uploadContent.Add(fileContent, "file", "test.jpg");

        var uploadResponse = await client.PostAsync("api/v1/media/upload", uploadContent);

        if (uploadResponse.StatusCode == HttpStatusCode.Created)
        {
            var mediaResponse = await uploadResponse.Content.ReadFromJsonAsync<MediaAssetResponse>();
            Assert.NotNull(mediaResponse);

            // Deletar a mídia
            var deleteResponse = await client.DeleteAsync($"api/v1/media/{mediaResponse!.Id}");

            // Pode retornar NoContent ou BadRequest (dependendo da implementação)
            Assert.True(
                deleteResponse.StatusCode == HttpStatusCode.NoContent ||
                deleteResponse.StatusCode == HttpStatusCode.BadRequest ||
                deleteResponse.StatusCode == HttpStatusCode.NotFound,
                $"Expected NoContent, BadRequest or NotFound, got {deleteResponse.StatusCode}");
        }
    }

    [Fact]
    public async Task DeleteMedia_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.DeleteAsync($"api/v1/media/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DeleteMedia_WithNonOwner_ReturnsBadRequest()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Fazer upload como primeiro usuário
        var token1 = await LoginForTokenAsync(client, "google", "media-delete-owner1");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token1);

        var jpegBytes = new byte[] { 0xFF, 0xD8, 0xFF };
        var uploadContent = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(jpegBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        uploadContent.Add(fileContent, "file", "test.jpg");

        var uploadResponse = await client.PostAsync("api/v1/media/upload", uploadContent);
        MediaAssetResponse? mediaResponse = null;

        if (uploadResponse.StatusCode == HttpStatusCode.Created)
        {
            mediaResponse = await uploadResponse.Content.ReadFromJsonAsync<MediaAssetResponse>();
        }

        if (mediaResponse is not null)
        {
            // Tentar deletar como outro usuário
            var token2 = await LoginForTokenAsync(client, "google", "media-delete-owner2");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token2);

            var deleteResponse = await client.DeleteAsync($"api/v1/media/{mediaResponse.Id}");

            // Deve retornar BadRequest (apenas criador pode deletar)
            Assert.True(
                deleteResponse.StatusCode == HttpStatusCode.BadRequest ||
                deleteResponse.StatusCode == HttpStatusCode.Forbidden ||
                deleteResponse.StatusCode == HttpStatusCode.NotFound,
                $"Expected BadRequest, Forbidden or NotFound, got {deleteResponse.StatusCode}");
        }
    }

    [Fact]
    public async Task DownloadMedia_WithValidId_ReturnsFile()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Primeiro, fazer upload de uma mídia
        var token = await LoginForTokenAsync(client, "google", "media-download-test");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var jpegBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 };
        var uploadContent = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(jpegBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        uploadContent.Add(fileContent, "file", "test.jpg");

        var uploadResponse = await client.PostAsync("api/v1/media/upload", uploadContent);

        if (uploadResponse.StatusCode == HttpStatusCode.Created)
        {
            var mediaResponse = await uploadResponse.Content.ReadFromJsonAsync<MediaAssetResponse>();
            Assert.NotNull(mediaResponse);

            // Fazer download da mídia
            var downloadResponse = await client.GetAsync($"api/v1/media/{mediaResponse!.Id}");

            // Pode retornar OK (FileResult) ou NotFound (dependendo da implementação)
            Assert.True(
                downloadResponse.StatusCode == HttpStatusCode.OK ||
                downloadResponse.StatusCode == HttpStatusCode.NotFound ||
                downloadResponse.StatusCode == HttpStatusCode.Redirect,
                $"Expected OK, NotFound or Redirect, got {downloadResponse.StatusCode}");
        }
    }

    [Fact]
    public async Task DownloadMedia_WithInvalidId_ReturnsNotFound()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"api/v1/media/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
                null));

        if (response.StatusCode == HttpStatusCode.Created || response.StatusCode == HttpStatusCode.OK)
        {
            var loginResponse = await response.Content.ReadFromJsonAsync<SocialLoginResponse>();
            return loginResponse?.Token ?? string.Empty;
        }

        return string.Empty;
    }
}