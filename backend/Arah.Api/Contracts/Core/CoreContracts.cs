namespace Arah.Api.Contracts.Core;

public sealed record RegisterCoreInstanceRequest(
    string Mode,
    string BaseUrl,
    string Version);

public sealed record RegisterCoreInstanceResponse(
    CoreInstanceResponse Instance,
    string PrivateKeyPem,
    string InstanceAuthToken);

public sealed record PublishCoreReleaseRequest(
    string Version,
    string Channel,
    int SchemaVersion);

public sealed record CoreInstanceResponse(
    Guid Id,
    string Mode,
    string BaseUrl,
    string Version,
    string PublicKeyPem,
    string Status,
    DateTimeOffset RegisteredAtUtc,
    DateTimeOffset? LastHeartbeatUtc,
    IReadOnlyDictionary<string, string>? LastServices = null,
    long? LastUptimeSeconds = null);

public sealed record CoreHeartbeatRequest(
    IReadOnlyDictionary<string, string> Services,
    long UptimeSeconds);

public sealed record CoreReleaseResponse(
    Guid Id,
    string Version,
    string Channel,
    int SchemaVersion,
    DateTimeOffset PublishedAtUtc);

public sealed record PublishDirectoryTerritoryRequest(
    Guid TerritoryId,
    Guid InstanceId,
    string Name);

public sealed record DirectoryTerritoryResponse(
    Guid TerritoryId,
    Guid InstanceId,
    string Name,
    DateTimeOffset PublishedAtUtc);

public sealed record ProvisionFederatedIdentityRequest(
    Guid HomeInstanceId,
    string DisplayName);

public sealed record FederatedIdentityResponse(
    Guid GlobalUserId,
    Guid HomeInstanceId,
    string DisplayName,
    DateTimeOffset CreatedAtUtc);
