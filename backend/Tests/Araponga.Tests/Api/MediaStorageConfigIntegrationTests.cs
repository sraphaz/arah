using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Araponga.Api;
using Araponga.Api.Contracts.Auth;
using Araponga.Api.Contracts.Media;
using Xunit;

namespace Araponga.Tests.Api;

/// <summary>
/// Testes de integração end-to-end para configuração de blob storage via painel administrativo.
/// </summary>
public sealed class MediaStorageConfigIntegrationTests
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
    public async Task GetActiveConfig_WithoutAuthentication_ReturnsNotFound_WhenNoActiveConfig()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("api/v1/admin/media-storage-config/active");

        // GetActive não requer autenticação, apenas retorna NotFound se não houver config ativa
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetActiveConfig_WithAuthentication_ReturnsNotFound_WhenNoActiveConfig()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-no-admin");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("api/v1/admin/media-storage-config/active");

        // GetActiveConfig não requer SystemAdmin, apenas autenticação
        // Se não houver configuração ativa, retorna NotFound
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ListConfigs_WithoutSystemAdmin_ReturnsForbid()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-no-admin");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync("api/v1/admin/media-storage-config");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateConfig_WithoutSystemAdmin_ReturnsForbid()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-no-admin");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateMediaStorageConfigRequest
        {
            Provider = "Local",
            Settings = new MediaStorageSettingsRequest
            {
                Local = new LocalStorageSettingsRequest { BasePath = "wwwroot/media" }
            },
            Description = "Test config"
        };

        var response = await client.PostAsJsonAsync("api/v1/admin/media-storage-config", request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateConfig_WithInvalidProvider_ReturnsBadRequest()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Login como SystemAdmin
        var token = await LoginForTokenAsync(client, "google", "admin-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateMediaStorageConfigRequest
        {
            Provider = "InvalidProvider",
            Settings = new MediaStorageSettingsRequest
            {
                Local = new LocalStorageSettingsRequest { BasePath = "wwwroot/media" }
            }
        };

        var response = await client.PostAsJsonAsync("api/v1/admin/media-storage-config", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateLocalConfig_WithValidSettings_ReturnsCreated()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Login como SystemAdmin
        var token = await LoginForTokenAsync(client, "google", "admin-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateMediaStorageConfigRequest
        {
            Provider = "Local",
            Settings = new MediaStorageSettingsRequest
            {
                EnableUrlCache = true,
                UrlCacheExpiration = TimeSpan.FromHours(24),
                Local = new LocalStorageSettingsRequest { BasePath = "wwwroot/media" }
            },
            Description = "Local storage for development"
        };

        var response = await client.PostAsJsonAsync("api/v1/admin/media-storage-config", request);

        response.EnsureSuccessStatusCode();
        var config = await response.Content.ReadFromJsonAsync<MediaStorageConfigResponse>();
        Assert.NotNull(config);
        Assert.Equal("Local", config!.Provider);
        Assert.False(config.IsActive); // Nova configuração é inativa por padrão
        Assert.NotNull(config.Settings);
        Assert.NotNull(config.Settings.Local);
        Assert.Equal("wwwroot/media", config.Settings.Local.BasePath);
        Assert.Equal("Local storage for development", config.Description);
    }

    [Fact]
    public async Task CreateS3Config_WithValidSettings_ReturnsCreated()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Login como SystemAdmin
        var token = await LoginForTokenAsync(client, "google", "admin-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateMediaStorageConfigRequest
        {
            Provider = "S3",
            Settings = new MediaStorageSettingsRequest
            {
                EnableUrlCache = true,
                UrlCacheExpiration = TimeSpan.FromHours(24),
                S3 = new S3StorageSettingsRequest
                {
                    BucketName = "test-bucket",
                    Region = "us-east-1",
                    AccessKeyId = "AKIAIOSFODNN7EXAMPLE",
                    Prefix = "media/"
                }
            },
            Description = "S3 storage for production"
        };

        var response = await client.PostAsJsonAsync("api/v1/admin/media-storage-config", request);

        response.EnsureSuccessStatusCode();
        var config = await response.Content.ReadFromJsonAsync<MediaStorageConfigResponse>();
        Assert.NotNull(config);
        Assert.Equal("S3", config!.Provider);
        Assert.NotNull(config.Settings);
        Assert.NotNull(config.Settings.S3);
        Assert.Equal("test-bucket", config.Settings.S3.BucketName);
        Assert.Equal("us-east-1", config.Settings.S3.Region);
        // AccessKeyId deve estar mascarado
        Assert.NotNull(config.Settings.S3.AccessKeyId);
        Assert.Contains("****", config.Settings.S3.AccessKeyId);
        Assert.Equal("media/", config.Settings.S3.Prefix);
    }

    [Fact]
    public async Task CreateAzureBlobConfig_WithValidSettings_ReturnsCreated()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Login como SystemAdmin
        var token = await LoginForTokenAsync(client, "google", "admin-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new CreateMediaStorageConfigRequest
        {
            Provider = "AzureBlob",
            Settings = new MediaStorageSettingsRequest
            {
                EnableUrlCache = true,
                UrlCacheExpiration = TimeSpan.FromHours(24),
                AzureBlob = new AzureBlobStorageSettingsRequest
                {
                    ConnectionString = "DefaultEndpointsProtocol=https;AccountName=testaccount;AccountKey=testkey;EndpointSuffix=core.windows.net",
                    ContainerName = "media",
                    Prefix = "uploads/"
                }
            },
            Description = "Azure Blob storage for production"
        };

        var response = await client.PostAsJsonAsync("api/v1/admin/media-storage-config", request);

        response.EnsureSuccessStatusCode();
        var config = await response.Content.ReadFromJsonAsync<MediaStorageConfigResponse>();
        Assert.NotNull(config);
        Assert.Equal("AzureBlob", config!.Provider);
        Assert.NotNull(config.Settings);
        Assert.NotNull(config.Settings.AzureBlob);
        Assert.Equal("media", config.Settings.AzureBlob.ContainerName);
        // ConnectionString deve estar mascarado
        Assert.NotNull(config.Settings.AzureBlob.ConnectionString);
        Assert.Contains("****", config.Settings.AzureBlob.ConnectionString);
        Assert.Equal("uploads/", config.Settings.AzureBlob.Prefix);
    }

    [Fact]
    public async Task ActivateConfig_WithoutSystemAdmin_ReturnsForbid()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-no-admin");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var configId = Guid.NewGuid();
        var response = await client.PostAsync(
            $"api/v1/admin/media-storage-config/{configId}/activate",
            null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ActivateConfig_WithSystemAdmin_ActivatesConfig()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Login como SystemAdmin
        var token = await LoginForTokenAsync(client, "google", "admin-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Primeiro, criar uma configuração
        var createRequest = new CreateMediaStorageConfigRequest
        {
            Provider = "Local",
            Settings = new MediaStorageSettingsRequest
            {
                Local = new LocalStorageSettingsRequest { BasePath = "wwwroot/media" }
            }
        };

        var createResponse = await client.PostAsJsonAsync("api/v1/admin/media-storage-config", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var createdConfig = await createResponse.Content.ReadFromJsonAsync<MediaStorageConfigResponse>();
        Assert.NotNull(createdConfig);
        Assert.False(createdConfig!.IsActive);

        // Ativar a configuração
        var activateResponse = await client.PostAsync(
            $"api/v1/admin/media-storage-config/{createdConfig.Id}/activate",
            null);

        activateResponse.EnsureSuccessStatusCode();
        var activatedConfig = await activateResponse.Content.ReadFromJsonAsync<MediaStorageConfigResponse>();
        Assert.NotNull(activatedConfig);
        Assert.True(activatedConfig!.IsActive);
    }

    [Fact]
    public async Task UpdateConfig_WithoutSystemAdmin_ReturnsForbid()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-no-admin");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var configId = Guid.NewGuid();
        var request = new UpdateMediaStorageConfigRequest
        {
            Settings = new MediaStorageSettingsRequest
            {
                EnableUrlCache = false
            },
            Description = "Updated config"
        };

        var response = await client.PutAsJsonAsync(
            $"api/v1/admin/media-storage-config/{configId}",
            request);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateConfig_WithSystemAdmin_UpdatesSuccessfully()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Login como SystemAdmin
        var token = await LoginForTokenAsync(client, "google", "admin-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Primeiro, criar uma configuração
        var createRequest = new CreateMediaStorageConfigRequest
        {
            Provider = "Local",
            Settings = new MediaStorageSettingsRequest
            {
                Local = new LocalStorageSettingsRequest { BasePath = "wwwroot/media" }
            },
            Description = "Original description"
        };

        var createResponse = await client.PostAsJsonAsync("api/v1/admin/media-storage-config", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var createdConfig = await createResponse.Content.ReadFromJsonAsync<MediaStorageConfigResponse>();
        Assert.NotNull(createdConfig);

        // Atualizar a configuração
        var updateRequest = new UpdateMediaStorageConfigRequest
        {
            Settings = new MediaStorageSettingsRequest
            {
                EnableUrlCache = false,
                Local = new LocalStorageSettingsRequest { BasePath = "wwwroot/media-updated" }
            },
            Description = "Updated description"
        };

        var updateResponse = await client.PutAsJsonAsync(
            $"api/v1/admin/media-storage-config/{createdConfig!.Id}",
            updateRequest);

        updateResponse.EnsureSuccessStatusCode();
        var updatedConfig = await updateResponse.Content.ReadFromJsonAsync<MediaStorageConfigResponse>();
        Assert.NotNull(updatedConfig);
        Assert.Equal("Updated description", updatedConfig!.Description);
        Assert.NotNull(updatedConfig.Settings);
        Assert.NotNull(updatedConfig.Settings.Local);
        Assert.Equal("wwwroot/media-updated", updatedConfig.Settings.Local.BasePath);
    }

    [Fact]
    public async Task GetConfigById_WithoutSystemAdmin_ReturnsForbid()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await LoginForTokenAsync(client, "google", "test-no-admin");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var configId = Guid.NewGuid();
        var response = await client.GetAsync($"api/v1/admin/media-storage-config/{configId}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetConfigById_WithNonExistentId_ReturnsNotFound()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Login como SystemAdmin
        var token = await LoginForTokenAsync(client, "google", "admin-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var configId = Guid.NewGuid();
        var response = await client.GetAsync($"api/v1/admin/media-storage-config/{configId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ListConfigs_WithSystemAdmin_ReturnsAllConfigs()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Login como SystemAdmin
        var token = await LoginForTokenAsync(client, "google", "admin-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Criar algumas configurações
        var config1 = new CreateMediaStorageConfigRequest
        {
            Provider = "Local",
            Settings = new MediaStorageSettingsRequest
            {
                Local = new LocalStorageSettingsRequest { BasePath = "wwwroot/media1" }
            }
        };
        await client.PostAsJsonAsync("api/v1/admin/media-storage-config", config1);

        var config2 = new CreateMediaStorageConfigRequest
        {
            Provider = "Local",
            Settings = new MediaStorageSettingsRequest
            {
                Local = new LocalStorageSettingsRequest { BasePath = "wwwroot/media2" }
            }
        };
        await client.PostAsJsonAsync("api/v1/admin/media-storage-config", config2);

        // Listar todas as configurações
        var response = await client.GetAsync("api/v1/admin/media-storage-config");
        response.EnsureSuccessStatusCode();
        var configs = await response.Content.ReadFromJsonAsync<List<MediaStorageConfigResponse>>();
        Assert.NotNull(configs);
        Assert.True(configs!.Count >= 2);
    }

    [Fact]
    public async Task ActivateConfig_DeactivatesOtherConfigs()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        // Login como SystemAdmin
        var token = await LoginForTokenAsync(client, "google", "admin-external");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Criar duas configurações
        var config1Request = new CreateMediaStorageConfigRequest
        {
            Provider = "Local",
            Settings = new MediaStorageSettingsRequest
            {
                Local = new LocalStorageSettingsRequest { BasePath = "wwwroot/media1" }
            }
        };
        var config1Response = await client.PostAsJsonAsync("api/v1/admin/media-storage-config", config1Request);
        config1Response.EnsureSuccessStatusCode();
        var config1 = await config1Response.Content.ReadFromJsonAsync<MediaStorageConfigResponse>();

        var config2Request = new CreateMediaStorageConfigRequest
        {
            Provider = "Local",
            Settings = new MediaStorageSettingsRequest
            {
                Local = new LocalStorageSettingsRequest { BasePath = "wwwroot/media2" }
            }
        };
        var config2Response = await client.PostAsJsonAsync("api/v1/admin/media-storage-config", config2Request);
        config2Response.EnsureSuccessStatusCode();
        var config2 = await config2Response.Content.ReadFromJsonAsync<MediaStorageConfigResponse>();

        // Ativar config1
        var activate1Response = await client.PostAsync(
            $"api/v1/admin/media-storage-config/{config1!.Id}/activate",
            null);
        activate1Response.EnsureSuccessStatusCode();

        // Ativar config2 (deve desativar config1)
        var activate2Response = await client.PostAsync(
            $"api/v1/admin/media-storage-config/{config2!.Id}/activate",
            null);
        activate2Response.EnsureSuccessStatusCode();

        // Verificar que config2 está ativa e config1 está inativa
        var get1Response = await client.GetAsync($"api/v1/admin/media-storage-config/{config1.Id}");
        get1Response.EnsureSuccessStatusCode();
        var updatedConfig1 = await get1Response.Content.ReadFromJsonAsync<MediaStorageConfigResponse>();
        Assert.False(updatedConfig1!.IsActive);

        var get2Response = await client.GetAsync($"api/v1/admin/media-storage-config/{config2.Id}");
        get2Response.EnsureSuccessStatusCode();
        var updatedConfig2 = await get2Response.Content.ReadFromJsonAsync<MediaStorageConfigResponse>();
        Assert.True(updatedConfig2!.IsActive);
    }
}
