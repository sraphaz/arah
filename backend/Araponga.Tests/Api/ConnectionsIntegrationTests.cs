using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Araponga.Application.Models;
using Araponga.Api;
using Araponga.Api.Contracts.Auth;
using Araponga.Infrastructure.InMemory;
using Xunit;

namespace Araponga.Tests.Api;

/// <summary>
/// Testes de integração da API de conexões (círculo de amigos) e do fluxo de notificações (Outbox).
/// </summary>
public sealed class ConnectionsIntegrationTests
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

    private static async Task<(string Token, Guid UserId)> LoginAndGetUserIdAsync(HttpClient client, string externalId)
    {
        var response = await client.PostAsJsonAsync(
            "api/v1/auth/social",
            new SocialLoginRequest(
                "google",
                externalId,
                "Test User",
                "123.456.789-00",
                null,
                null,
                null,
                "test@araponga.com"));
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<SocialLoginResponse>();
        return (payload!.Token, payload.User.Id);
    }

    [Fact]
    public async Task RequestConnection_ThenAccept_EnqueuesConnectionRequestAndAcceptedNotifications()
    {
        using var factory = new ApiFactory();
        using var client = factory.CreateClient();

        client.DefaultRequestHeaders.Add(ApiHeaders.SessionId, Guid.NewGuid().ToString());

        var (tokenA, userAId) = await LoginAndGetUserIdAsync(client, "conn-integration-user-a");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenA);

        var (tokenB, userBId) = await LoginAndGetUserIdAsync(client, "conn-integration-user-b");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenA);
        var requestResponse = await client.PostAsJsonAsync(
            "api/v1/connections/request",
            new { TargetUserId = userBId });
        requestResponse.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, requestResponse.StatusCode);

        var dataStore = factory.GetDataStore();
        var requestNotifications = dataStore.OutboxMessages
            .Where(m => m.Type == "notification.dispatch")
            .ToList();
        Assert.NotEmpty(requestNotifications);

        var requestPayload = JsonSerializer.Deserialize<NotificationDispatchPayload>(
            requestNotifications[^1].PayloadJson,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.NotNull(requestPayload);
        Assert.Equal("connection.request", requestPayload.Kind);
        Assert.Single(requestPayload.Recipients);
        Assert.Equal(userBId, requestPayload.Recipients.First());

        var createdResponse = await requestResponse.Content.ReadFromJsonAsync<ConnectionCreatedResponse>();
        var connectionId = createdResponse!.Id;

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenB);
        var acceptResponse = await client.PostAsync(
            $"api/v1/connections/{connectionId}/accept",
            null);
        acceptResponse.EnsureSuccessStatusCode();

        var acceptedNotifications = dataStore.OutboxMessages
            .Where(m => m.Type == "notification.dispatch")
            .ToList();
        Assert.True(acceptedNotifications.Count >= 2, "Expected at least connection.request and connection.accepted");

        var acceptedPayload = JsonSerializer.Deserialize<NotificationDispatchPayload>(
            acceptedNotifications[^1].PayloadJson,
            new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.NotNull(acceptedPayload);
        Assert.Equal("connection.accepted", acceptedPayload.Kind);
        Assert.Single(acceptedPayload.Recipients);
        Assert.Equal(userAId, acceptedPayload.Recipients.First());
    }

    private sealed record ConnectionCreatedResponse(
        Guid Id,
        Guid RequesterUserId,
        Guid TargetUserId,
        string Status,
        Guid? TerritoryId,
        DateTime RequestedAtUtc);
}
