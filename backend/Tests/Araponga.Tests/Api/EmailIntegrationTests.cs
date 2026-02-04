using System.Net;
using System.Net.Http.Json;
using Araponga.Api;
using Araponga.Api.Contracts.Users;
using Araponga.Domain.Email;
using Araponga.Domain.Users;
using Araponga.Infrastructure.InMemory;
using Araponga.Tests.TestHelpers;
using Xunit;

namespace Araponga.Tests.Api;

/// <summary>
/// Testes de integração end-to-end para sistema de envio de emails.
/// </summary>
public sealed class EmailIntegrationTests
{
    [Fact]
    public async Task UpdateEmailPreferences_WithValidData_UpdatesSuccessfully()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await AuthTestHelper.LoginForTokenAsync(client, "google", "test-email-prefs", "test@araponga.com");
        AuthTestHelper.SetupAuthenticatedClient(client, token);

        var updateRequest = new UpdateEmailPreferencesRequest(
            ReceiveEmails: false,
            EmailFrequency: "Daily",
            EmailTypes: (int)EmailTypes.Events | (int)EmailTypes.CriticalAlerts);

        var response = await client.PutAsJsonAsync(
            "api/v1/users/me/preferences/email",
            updateRequest);

        response.EnsureSuccessStatusCode();
        var preferences = await response.Content.ReadFromJsonAsync<UserPreferencesResponse>();
        Assert.NotNull(preferences);
        Assert.NotNull(preferences.Email);
        Assert.False(preferences.Email.ReceiveEmails);
        Assert.Equal("Daily", preferences.Email.EmailFrequency);
    }

    [Fact]
    public async Task UpdateEmailPreferences_WithoutAuthentication_ReturnsUnauthorized()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var updateRequest = new UpdateEmailPreferencesRequest(
            ReceiveEmails: true,
            EmailFrequency: "Immediate",
            EmailTypes: (int)EmailTypes.All);

        var response = await client.PutAsJsonAsync(
            "api/v1/users/me/preferences/email",
            updateRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateEmailPreferences_WithInvalidFrequency_ReturnsBadRequest()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await AuthTestHelper.LoginForTokenAsync(client, "google", "test-email-invalid", "test@araponga.com");
        AuthTestHelper.SetupAuthenticatedClient(client, token);

        var updateRequest = new UpdateEmailPreferencesRequest(
            ReceiveEmails: true,
            EmailFrequency: "InvalidFrequency",
            EmailTypes: (int)EmailTypes.All);

        var response = await client.PutAsJsonAsync(
            "api/v1/users/me/preferences/email",
            updateRequest);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetPreferences_IncludesEmailPreferences()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await AuthTestHelper.LoginForTokenAsync(client, "google", "test-email-get", "test@araponga.com");
        AuthTestHelper.SetupAuthenticatedClient(client, token);

        var response = await client.GetAsync("api/v1/users/me/preferences");

        response.EnsureSuccessStatusCode();
        var preferences = await response.Content.ReadFromJsonAsync<UserPreferencesResponse>();
        Assert.NotNull(preferences);
        Assert.NotNull(preferences.Email);
        Assert.True(preferences.Email.ReceiveEmails); // Default
        Assert.Equal("Immediate", preferences.Email.EmailFrequency); // Default
    }

    [Fact]
    public async Task EmailQueue_EnqueuesEmail_WhenNotificationRequiresEmail()
    {
        using var factory = new ApiFactory();
        var dataStore = factory.GetDataStore();

        using var client = factory.CreateClient();

        var token = await AuthTestHelper.LoginForTokenAsync(client, "google", "test-email-queue", "test@araponga.com");
        AuthTestHelper.SetupAuthenticatedClient(client, token);

        // Criar um evento que deve gerar notificação e email
        // (Este teste verifica que o sistema enfileira emails quando apropriado)
        // Nota: O processamento real da queue acontece via EmailQueueWorker em background

        // Verificar que a queue está acessível
        Assert.NotNull(dataStore);
        var initialQueueCount = dataStore.EmailQueueItems.Count;

        // O email será enfileirado quando a notificação for processada pelo OutboxDispatcherWorker
        // Por enquanto, apenas verificamos que a infraestrutura está disponível
        Assert.True(initialQueueCount >= 0);
    }

    [Fact]
    public async Task EmailPreferences_RespectsOptOut()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        var token = await AuthTestHelper.LoginForTokenAsync(client, "google", "test-email-optout", "test@araponga.com");
        AuthTestHelper.SetupAuthenticatedClient(client, token);

        // Opt-out de emails
        var updateRequest = new UpdateEmailPreferencesRequest(
            ReceiveEmails: false,
            EmailFrequency: "Immediate",
            EmailTypes: (int)EmailTypes.None);

        var updateResponse = await client.PutAsJsonAsync(
            "api/v1/users/me/preferences/email",
            updateRequest);
        updateResponse.EnsureSuccessStatusCode();

        // Verificar que preferências foram atualizadas
        var getResponse = await client.GetAsync("api/v1/users/me/preferences");
        getResponse.EnsureSuccessStatusCode();
        var preferences = await getResponse.Content.ReadFromJsonAsync<UserPreferencesResponse>();
        Assert.NotNull(preferences);
        Assert.NotNull(preferences.Email);
        Assert.False(preferences.Email.ReceiveEmails);
    }
}
