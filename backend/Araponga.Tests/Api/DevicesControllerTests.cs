using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Araponga.Api;
using Araponga.Api.Contracts.Devices;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Api;

public sealed class DevicesControllerTests
{
    private static readonly Guid UserId1 = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

    private static async Task<string> LoginForTokenAsync(HttpClient client, string provider, string userId)
    {
        var response = await client.PostAsJsonAsync(
            "api/v1/auth/social",
            new Araponga.Api.Contracts.Auth.SocialLoginRequest(
                provider,
                userId,
                "Test User",
                null,
                null,
                null,
                null,
                null));
        var loginResponse = await response.Content.ReadFromJsonAsync<Araponga.Api.Contracts.Auth.SocialLoginResponse>();
        return loginResponse?.Token ?? string.Empty;
    }

    [Fact]
    public async Task RegisterDevice_RequiresAuthentication()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var request = new RegisterDeviceRequest("token123", "IOS", "iPhone");
        var response = await client.PostAsJsonAsync("api/v1/users/me/devices", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RegisterDevice_WhenValid_CreatesDevice()
    {
        using var factory = new ApiFactory();
        var dataStore = factory.GetDataStore();
        using var client = factory.CreateClient();

        // Garantir que o usuário existe
        var externalId = UserId1.ToString();
        var token = await LoginForTokenAsync(client, "google", externalId);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new RegisterDeviceRequest("token123", "IOS", "iPhone");
        var response = await client.PostAsJsonAsync("api/v1/users/me/devices", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var device = await response.Content.ReadFromJsonAsync<DeviceResponse>();
        Assert.NotNull(device);
        Assert.Equal("IOS", device.Platform);
        Assert.Equal("iPhone", device.DeviceName);
        Assert.True(device.IsActive);
    }

    [Fact]
    public async Task RegisterDevice_WhenTokenIsEmpty_ReturnsBadRequest()
    {
        using var factory = new ApiFactory();
        var dataStore = factory.GetDataStore();
        using var client = factory.CreateClient();

        var externalId = UserId1.ToString();
        var token = await LoginForTokenAsync(client, "google", externalId);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var request = new RegisterDeviceRequest("", "IOS", "iPhone");
        var response = await client.PostAsJsonAsync("api/v1/users/me/devices", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ListDevices_RequiresAuthentication()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("api/v1/users/me/devices");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ListDevices_WhenValid_ReturnsDevices()
    {
        using var factory = new ApiFactory();
        var dataStore = factory.GetDataStore();
        using var client = factory.CreateClient();

        var externalId = UserId1.ToString();
        var token = await LoginForTokenAsync(client, "google", externalId);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Registrar um dispositivo primeiro
        var registerRequest = new RegisterDeviceRequest("token123", "IOS", "iPhone");
        await client.PostAsJsonAsync("api/v1/users/me/devices", registerRequest);

        // Listar dispositivos
        var response = await client.GetAsync("api/v1/users/me/devices");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var devices = await response.Content.ReadFromJsonAsync<List<DeviceResponse>>();
        Assert.NotNull(devices);
        Assert.True(devices.Count >= 1);
    }

    [Fact]
    public async Task GetDevice_RequiresAuthentication()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var deviceId = Guid.NewGuid();
        var response = await client.GetAsync($"api/v1/users/me/devices/{deviceId}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetDevice_WhenDeviceNotFound_ReturnsNotFound()
    {
        using var factory = new ApiFactory();
        var dataStore = factory.GetDataStore();
        using var client = factory.CreateClient();

        var externalId = UserId1.ToString();
        var token = await LoginForTokenAsync(client, "google", externalId);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var deviceId = Guid.NewGuid();
        var response = await client.GetAsync($"api/v1/users/me/devices/{deviceId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDevice_WhenValid_ReturnsDevice()
    {
        using var factory = new ApiFactory();
        var dataStore = factory.GetDataStore();
        using var client = factory.CreateClient();

        var externalId = UserId1.ToString();
        var token = await LoginForTokenAsync(client, "google", externalId);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Registrar um dispositivo primeiro
        var registerRequest = new RegisterDeviceRequest("token123", "IOS", "iPhone");
        var registerResponse = await client.PostAsJsonAsync("api/v1/users/me/devices", registerRequest);
        
        // Pode retornar Created ou OK dependendo da implementação
        Assert.True(
            registerResponse.StatusCode == HttpStatusCode.Created ||
            registerResponse.StatusCode == HttpStatusCode.OK,
            $"Expected Created or OK, but got {registerResponse.StatusCode}");

        DeviceResponse? registeredDevice = null;
        if (registerResponse.StatusCode == HttpStatusCode.Created)
        {
            registeredDevice = await registerResponse.Content.ReadFromJsonAsync<DeviceResponse>();
        }
        else
        {
            // Se retornou OK, pode ser que o dispositivo já existia, buscar da lista
            var listResponse = await client.GetAsync("api/v1/users/me/devices");
            if (listResponse.IsSuccessStatusCode)
            {
                var devices = await listResponse.Content.ReadFromJsonAsync<List<DeviceResponse>>();
                registeredDevice = devices?.FirstOrDefault();
            }
        }

        Assert.NotNull(registeredDevice);

        // Buscar o dispositivo
        var response = await client.GetAsync($"api/v1/users/me/devices/{registeredDevice.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var device = await response.Content.ReadFromJsonAsync<DeviceResponse>();
        Assert.NotNull(device);
        Assert.Equal(registeredDevice.Id, device.Id);
        Assert.Equal("IOS", device.Platform);
    }

    [Fact]
    public async Task UnregisterDevice_RequiresAuthentication()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var deviceId = Guid.NewGuid();
        var response = await client.DeleteAsync($"api/v1/users/me/devices/{deviceId}");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UnregisterDevice_WhenDeviceNotFound_ReturnsNotFound()
    {
        using var factory = new ApiFactory();
        var dataStore = factory.GetDataStore();
        using var client = factory.CreateClient();

        var externalId = UserId1.ToString();
        var token = await LoginForTokenAsync(client, "google", externalId);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var deviceId = Guid.NewGuid();
        var response = await client.DeleteAsync($"api/v1/users/me/devices/{deviceId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UnregisterDevice_WhenValid_DeletesDevice()
    {
        using var factory = new ApiFactory();
        var dataStore = factory.GetDataStore();
        using var client = factory.CreateClient();

        var externalId = UserId1.ToString();
        var token = await LoginForTokenAsync(client, "google", externalId);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Registrar um dispositivo primeiro
        var registerRequest = new RegisterDeviceRequest("token123", "IOS", "iPhone");
        var registerResponse = await client.PostAsJsonAsync("api/v1/users/me/devices", registerRequest);
        
        DeviceResponse? registeredDevice = null;
        if (registerResponse.StatusCode == HttpStatusCode.Created)
        {
            registeredDevice = await registerResponse.Content.ReadFromJsonAsync<DeviceResponse>();
        }
        else
        {
            // Se retornou OK, buscar da lista
            var listResponse = await client.GetAsync("api/v1/users/me/devices");
            if (listResponse.IsSuccessStatusCode)
            {
                var devices = await listResponse.Content.ReadFromJsonAsync<List<DeviceResponse>>();
                registeredDevice = devices?.FirstOrDefault();
            }
        }

        Assert.NotNull(registeredDevice);

        // Remover o dispositivo
        var response = await client.DeleteAsync($"api/v1/users/me/devices/{registeredDevice.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verificar que o dispositivo foi removido
        var getResponse = await client.GetAsync($"api/v1/users/me/devices/{registeredDevice.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
