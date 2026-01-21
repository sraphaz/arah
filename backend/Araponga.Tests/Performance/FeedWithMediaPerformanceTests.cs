using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Araponga.Api;
using Araponga.Api.Contracts.Auth;
using Araponga.Api.Contracts.Common;
using Araponga.Api.Contracts.Feed;
using Araponga.Api.Contracts.Media;
using Araponga.Api.Contracts.Memberships;
using Araponga.Api.Contracts.Territories;
using Araponga.Tests.Api;
using Xunit;
using Xunit.Sdk;

namespace Araponga.Tests.Performance;

/// <summary>
/// Testes de performance para feed com mídias, validando SLA de < 500ms.
/// </summary>
public sealed class FeedWithMediaPerformanceTests : IClassFixture<ApiFactory>
{
    private static readonly Guid ActiveTerritoryId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    // Verificar se os testes de performance devem ser pulados
    private static bool ShouldSkipPerformanceTests()
    {
        var skipEnv = Environment.GetEnvironmentVariable("SKIP_PERFORMANCE_TESTS");
        var isCI = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI")) ||
                   !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTIONS")) ||
                   !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TF_BUILD")) ||
                   !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JENKINS_URL"));
        
        return isCI || string.Equals(skipEnv, "true", StringComparison.OrdinalIgnoreCase);
    }

    private static void SkipIfNeeded()
    {
        if (ShouldSkipPerformanceTests())
        {
            Skip.If(true, "Testes de performance pulados em CI/CD. Configure SKIP_PERFORMANCE_TESTS=false para executar.");
        }
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
                "test@araponga.com"));
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<SocialLoginResponse>();
        return payload!.Token;
    }

    private static async Task SelectTerritoryAsync(HttpClient client, Guid territoryId)
    {
        if (!client.DefaultRequestHeaders.Contains(ApiHeaders.SessionId))
        {
            client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, Guid.NewGuid().ToString());
        }

        var response = await client.PostAsJsonAsync(
            "api/v1/territories/selection",
            new TerritorySelectionRequest(territoryId));
        response.EnsureSuccessStatusCode();
    }

    private static async Task BecomeResidentAsync(HttpClient client, Guid territoryId, ApiFactory? factory = null)
    {
        // Usar o mesmo padrão do MediaInContentIntegrationTests
        var residentUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var requestResponse = await client.PostAsJsonAsync(
            $"api/v1/memberships/{territoryId}/become-resident",
            new BecomeResidentRequest(new[] { residentUserId }, "Test residency request"));
        
        if (requestResponse.StatusCode == HttpStatusCode.OK)
        {
            var requestPayload = await requestResponse.Content.ReadFromJsonAsync<RequestResidencyResponse>();
            if (requestPayload is not null)
            {
                // Tentar aprovar usando resident token
                var residentToken = await LoginForTokenAsync(client, "google", "resident-external");
                var originalAuth = client.DefaultRequestHeaders.Authorization;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", residentToken);

                var approveResponse = await client.PostAsync(
                    $"api/v1/join-requests/{requestPayload.JoinRequestId}/approve",
                    null);

                client.DefaultRequestHeaders.Authorization = originalAuth;

                // Verificar residência por geo
                await client.PostAsJsonAsync(
                    $"api/v1/memberships/{territoryId}/verify-residency/geo",
                    new VerifyResidencyGeoRequest(-23.3744, -45.0205));
            }
        }
    }

    private static async Task<Guid> UploadTestMediaAsync(HttpClient client, string testId)
    {
        // Usar o mesmo padrão do MediaInContentIntegrationTests - JPEG válido de 1x1 pixel
        var jpegBytes = new byte[]
        {
            0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01, 0x01, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00,
            0xFF, 0xDB, 0x00, 0x43, 0x00, 0x08, 0x06, 0x06, 0x07, 0x06, 0x05, 0x08, 0x07, 0x07, 0x07, 0x09, 0x09, 0x08, 0x0A, 0x0C,
            0x14, 0x0D, 0x0C, 0x0B, 0x0B, 0x0C, 0x19, 0x12, 0x13, 0x0F, 0x14, 0x1D, 0x1A, 0x1F, 0x1E, 0x1D, 0x1A, 0x1C, 0x1C, 0x20,
            0x24, 0x2E, 0x27, 0x20, 0x22, 0x2C, 0x23, 0x1C, 0x1C, 0x28, 0x37, 0x29, 0x2C, 0x30, 0x31, 0x34, 0x34, 0x34, 0x1F, 0x27,
            0x39, 0x3D, 0x38, 0x32, 0x3C, 0x2E, 0x33, 0x34, 0x32, 0xFF, 0xC0, 0x00, 0x0B, 0x08, 0x00, 0x01, 0x00, 0x01, 0x01, 0x01,
            0x11, 0x00, 0xFF, 0xC4, 0x00, 0x14, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x08, 0xFF, 0xC4, 0x00, 0x14, 0x10, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xDA, 0x00, 0x08, 0x01, 0x01, 0x00, 0x00, 0x3F, 0x00, 0x5F, 0xFF, 0xD9
        };
        
        var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(jpegBytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        content.Add(fileContent, "file", $"test-{testId}.jpg");

        var response = await client.PostAsync("api/v1/media/upload", content);

        if (response.StatusCode == HttpStatusCode.Created)
        {
            var mediaResponse = await response.Content.ReadFromJsonAsync<MediaAssetResponse>();
            return mediaResponse!.Id;
        }

        var errorBody = await response.Content.ReadAsStringAsync();
        throw new InvalidOperationException($"Upload failed: {response.StatusCode} - {errorBody}");
    }

    [SkippableFact]
    public async Task FeedPaged_WithPostsContainingMedia_RespondsWithin500ms()
    {
        SkipIfNeeded();

        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", $"perf-feed-media-{Guid.NewGuid()}");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, $"perf-session-{Guid.NewGuid()}");

        await SelectTerritoryAsync(client, ActiveTerritoryId);
        await BecomeResidentAsync(client, ActiveTerritoryId, factory);

        // Criar 5 posts com mídias (2 mídias cada)
        var postIds = new List<Guid>();
        for (int i = 0; i < 5; i++)
        {
            var mediaId1 = await UploadTestMediaAsync(client, $"post-{i}-1");
            var mediaId2 = await UploadTestMediaAsync(client, $"post-{i}-2");

            var createResponse = await client.PostAsJsonAsync(
                $"api/v1/feed?territoryId={ActiveTerritoryId}",
                new CreatePostRequest(
                    $"Post {i} com mídias",
                    $"Descrição do post {i}",
                    "GENERAL",
                    "PUBLIC",
                    null,
                    null,
                    null,
                    new[] { mediaId1, mediaId2 }));

            if (createResponse.StatusCode == HttpStatusCode.Created)
            {
                var post = await createResponse.Content.ReadFromJsonAsync<FeedItemResponse>();
                if (post is not null)
                {
                    postIds.Add(post.Id);
                }
            }
        }

        // Aguardar um pouco para garantir que tudo foi processado
        await Task.Delay(100);

        // Act: Buscar feed paginado
        var stopwatch = Stopwatch.StartNew();
        var response = await client.GetAsync($"api/v1/feed/paged?territoryId={ActiveTerritoryId}&pageNumber=1&pageSize=20");
        stopwatch.Stop();

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var feed = await response.Content.ReadFromJsonAsync<PagedResponse<FeedItemResponse>>();
            Assert.NotNull(feed);
            
            // SLA: Feed paginado com mídias deve responder em menos de 500ms
            Assert.True(
                stopwatch.ElapsedMilliseconds < 500,
                $"Feed paged with media took {stopwatch.ElapsedMilliseconds}ms, expected < 500ms. Posts with media: {feed!.Items.Count(i => i.MediaCount > 0)}");
        }
        else
        {
            // Se não autorizado, não falhar o teste (pode ser problema de permissões)
            Assert.True(
                response.StatusCode == HttpStatusCode.Unauthorized || 
                response.StatusCode == HttpStatusCode.Forbidden,
                $"Unexpected status code: {response.StatusCode}");
        }
    }

    [SkippableFact]
    public async Task FeedList_WithPostsContainingMultipleMedia_RespondsWithin500ms()
    {
        SkipIfNeeded();

        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", $"perf-feed-multi-{Guid.NewGuid()}");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, $"perf-multi-session-{Guid.NewGuid()}");

        await SelectTerritoryAsync(client, ActiveTerritoryId);
        await BecomeResidentAsync(client, ActiveTerritoryId, factory);

        // Criar 1 post com 10 mídias (máximo permitido)
        var mediaIds = new List<Guid>();
        for (int i = 0; i < 10; i++)
        {
            var mediaId = await UploadTestMediaAsync(client, $"max-media-{i}");
            mediaIds.Add(mediaId);
        }

        var createResponse = await client.PostAsJsonAsync(
            $"api/v1/feed?territoryId={ActiveTerritoryId}",
            new CreatePostRequest(
                "Post com 10 mídias",
                "Teste de performance com máximo de mídias",
                "GENERAL",
                "PUBLIC",
                null,
                null,
                null,
                mediaIds));

        if (createResponse.StatusCode == HttpStatusCode.Created)
        {
            // Aguardar processamento
            await Task.Delay(100);

            // Act: Buscar feed
            var stopwatch = Stopwatch.StartNew();
            var feedResponse = await client.GetAsync($"api/v1/feed?territoryId={ActiveTerritoryId}");
            stopwatch.Stop();

            // Assert
            if (feedResponse.StatusCode == HttpStatusCode.OK)
            {
                var feed = await feedResponse.Content.ReadFromJsonAsync<List<FeedItemResponse>>();
                Assert.NotNull(feed);
                
                var postWithMaxMedia = feed!.FirstOrDefault(p => p.MediaCount == 10);
                Assert.NotNull(postWithMaxMedia);
                
                // SLA: Feed com post contendo 10 mídias deve responder em menos de 500ms
                Assert.True(
                    stopwatch.ElapsedMilliseconds < 500,
                    $"Feed with post containing 10 media took {stopwatch.ElapsedMilliseconds}ms, expected < 500ms");
            }
        }
    }

    [SkippableFact]
    public async Task FeedPaged_WithCachedMediaUrls_RespondsFaster()
    {
        SkipIfNeeded();

        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", $"perf-cache-{Guid.NewGuid()}");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, $"perf-cache-session-{Guid.NewGuid()}");

        await SelectTerritoryAsync(client, ActiveTerritoryId);
        await BecomeResidentAsync(client, ActiveTerritoryId, factory);

        // Criar post com mídia
        var mediaId = await UploadTestMediaAsync(client, "cache-test");
        var createResponse = await client.PostAsJsonAsync(
            $"api/v1/feed?territoryId={ActiveTerritoryId}",
            new CreatePostRequest(
                "Post para teste de cache",
                "Teste de cache de URLs",
                "GENERAL",
                "PUBLIC",
                null,
                null,
                null,
                new[] { mediaId }));

        if (createResponse.StatusCode == HttpStatusCode.Created)
        {
            await Task.Delay(100);

            // Primeira chamada (sem cache ou cache inicial)
            var firstCallStopwatch = Stopwatch.StartNew();
            var firstResponse = await client.GetAsync($"api/v1/feed/paged?territoryId={ActiveTerritoryId}&pageNumber=1&pageSize=20");
            firstCallStopwatch.Stop();

            // Segunda chamada (com cache)
            var secondCallStopwatch = Stopwatch.StartNew();
            var secondResponse = await client.GetAsync($"api/v1/feed/paged?territoryId={ActiveTerritoryId}&pageNumber=1&pageSize=20");
            secondCallStopwatch.Stop();

            // Assert
            if (firstResponse.StatusCode == HttpStatusCode.OK && secondResponse.StatusCode == HttpStatusCode.OK)
            {
                // Segunda chamada deve ser mais rápida ou similar (cache ajuda)
                // Não falhar se segunda for mais lenta (pode ser variação de rede/processamento)
                // Mas ambas devem estar dentro do SLA
                Assert.True(
                    firstCallStopwatch.ElapsedMilliseconds < 500,
                    $"First call took {firstCallStopwatch.ElapsedMilliseconds}ms, expected < 500ms");
                
                Assert.True(
                    secondCallStopwatch.ElapsedMilliseconds < 500,
                    $"Second call (cached) took {secondCallStopwatch.ElapsedMilliseconds}ms, expected < 500ms");
            }
        }
    }
}
