namespace Arah.Api.Contracts.Core;

public sealed record RegisterCoreInstanceRequest(
    string Mode,
    string BaseUrl,
    string Version);

public sealed record CoreInstanceResponse(
    Guid Id,
    string Mode,
    string BaseUrl,
    string Version,
    string Status,
    DateTimeOffset RegisteredAtUtc,
    DateTimeOffset? LastHeartbeatUtc);

public sealed record CoreHeartbeatRequest(
    IReadOnlyDictionary<string, string> Services,
    long UptimeSeconds);

public sealed record CoreReleaseResponse(
    Guid Id,
    string Version,
    string Channel,
    int SchemaVersion,
    DateTimeOffset PublishedAtUtc);
