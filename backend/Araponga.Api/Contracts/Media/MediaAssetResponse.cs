namespace Araponga.Api.Contracts.Media;

/// <summary>
/// Response contract para MediaAsset.
/// </summary>
public sealed record MediaAssetResponse(
    Guid Id,
    Guid UploadedByUserId,
    string MediaType,
    string MimeType,
    string StorageKey,
    long SizeBytes,
    int? WidthPx,
    int? HeightPx,
    string Checksum,
    DateTime CreatedAtUtc,
    bool IsDeleted);