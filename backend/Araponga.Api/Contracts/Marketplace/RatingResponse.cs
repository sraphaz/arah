namespace Araponga.Api.Contracts.Marketplace;

public sealed record RatingResponse(
    Guid Id,
    Guid UserId,
    int Rating,
    string? Comment,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    RatingResponseInfo? Response = null);

public sealed record RatingResponseInfo(
    Guid Id,
    string ResponseText,
    DateTime CreatedAtUtc);
