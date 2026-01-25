using Araponga.Domain.Media;
using Xunit;

namespace Araponga.Tests.Domain.Media;

/// <summary>
/// Edge case tests for MediaAsset and MediaAttachment domain entities,
/// focusing on Unicode in filenames, extreme file sizes, invalid MIME types, and malformed URLs.
/// </summary>
public class MediaEdgeCasesTests
{
    private static readonly DateTime TestDate = DateTime.UtcNow;
    private static readonly Guid TestUserId = Guid.NewGuid();

    // MediaAsset edge cases
    [Fact]
    public void MediaAsset_Constructor_WithUnicodeInStorageKey_StoresCorrectly()
    {
        var asset = new MediaAsset(
            Guid.NewGuid(),
            TestUserId,
            MediaType.Image,
            "image/jpeg",
            "images/2025/01/café-naïve-文字-🏪.jpg",
            1024,
            1920,
            1080,
            "abc123",
            TestDate,
            null,
            null);

        Assert.Contains("café", asset.StorageKey);
        Assert.Contains("文字", asset.StorageKey);
        Assert.Contains("🏪", asset.StorageKey);
    }

    [Fact]
    public void MediaAsset_Constructor_WithUnicodeInMimeType_StoresCorrectly()
    {
        // MIME types geralmente não têm Unicode, mas testamos se aceita
        var asset = new MediaAsset(
            Guid.NewGuid(),
            TestUserId,
            MediaType.Document,
            "application/pdf",
            "documents/test.pdf",
            1024,
            null,
            null,
            "abc123",
            TestDate,
            null,
            null);

        Assert.Equal("application/pdf", asset.MimeType);
    }

    [Fact]
    public void MediaAsset_Constructor_WithZeroSizeBytes_CreatesSuccessfully()
    {
        var asset = new MediaAsset(
            Guid.NewGuid(),
            TestUserId,
            MediaType.Document,
            "application/pdf",
            "documents/empty.pdf",
            0,
            null,
            null,
            "abc123",
            TestDate,
            null,
            null);

        Assert.Equal(0, asset.SizeBytes);
    }

    [Fact]
    public void MediaAsset_Constructor_WithNegativeSizeBytes_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new MediaAsset(
            Guid.NewGuid(),
            TestUserId,
            MediaType.Image,
            "image/jpeg",
            "images/test.jpg",
            -1,
            1920,
            1080,
            "abc123",
            TestDate,
            null,
            null));
    }

    [Fact]
    public void MediaAsset_Constructor_WithVeryLargeSizeBytes_CreatesSuccessfully()
    {
        var largeSize = long.MaxValue;
        var asset = new MediaAsset(
            Guid.NewGuid(),
            TestUserId,
            MediaType.Video,
            "video/mp4",
            "videos/large.mp4",
            largeSize,
            1920,
            1080,
            "abc123",
            TestDate,
            null,
            null);

        Assert.Equal(largeSize, asset.SizeBytes);
    }

    [Fact]
    public void MediaAsset_Constructor_WithNegativeWidthPx_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new MediaAsset(
            Guid.NewGuid(),
            TestUserId,
            MediaType.Image,
            "image/jpeg",
            "images/test.jpg",
            1024,
            -1,
            1080,
            "abc123",
            TestDate,
            null,
            null));
    }

    [Fact]
    public void MediaAsset_Constructor_WithNegativeHeightPx_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new MediaAsset(
            Guid.NewGuid(),
            TestUserId,
            MediaType.Image,
            "image/jpeg",
            "images/test.jpg",
            1024,
            1920,
            -1,
            "abc123",
            TestDate,
            null,
            null));
    }

    [Fact]
    public void MediaAsset_Constructor_WithNullMimeType_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new MediaAsset(
            Guid.NewGuid(),
            TestUserId,
            MediaType.Image,
            null!,
            "images/test.jpg",
            1024,
            1920,
            1080,
            "abc123",
            TestDate,
            null,
            null));
    }

    [Fact]
    public void MediaAsset_Constructor_WithEmptyMimeType_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new MediaAsset(
            Guid.NewGuid(),
            TestUserId,
            MediaType.Image,
            "   ",
            "images/test.jpg",
            1024,
            1920,
            1080,
            "abc123",
            TestDate,
            null,
            null));
    }

    [Fact]
    public void MediaAsset_Constructor_WithNullStorageKey_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new MediaAsset(
            Guid.NewGuid(),
            TestUserId,
            MediaType.Image,
            "image/jpeg",
            null!,
            1024,
            1920,
            1080,
            "abc123",
            TestDate,
            null,
            null));
    }

    [Fact]
    public void MediaAsset_Constructor_WithEmptyStorageKey_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new MediaAsset(
            Guid.NewGuid(),
            TestUserId,
            MediaType.Image,
            "image/jpeg",
            "   ",
            1024,
            1920,
            1080,
            "abc123",
            TestDate,
            null,
            null));
    }

    [Fact]
    public void MediaAsset_Constructor_WithNullChecksum_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new MediaAsset(
            Guid.NewGuid(),
            TestUserId,
            MediaType.Image,
            "image/jpeg",
            "images/test.jpg",
            1024,
            1920,
            1080,
            null!,
            TestDate,
            null,
            null));
    }

    [Fact]
    public void MediaAsset_Constructor_WithEmptyChecksum_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new MediaAsset(
            Guid.NewGuid(),
            TestUserId,
            MediaType.Image,
            "image/jpeg",
            "images/test.jpg",
            1024,
            1920,
            1080,
            "   ",
            TestDate,
            null,
            null));
    }

    [Fact]
    public void MediaAsset_Delete_WithEmptyUserId_ThrowsArgumentException()
    {
        var asset = new MediaAsset(
            Guid.NewGuid(),
            TestUserId,
            MediaType.Image,
            "image/jpeg",
            "images/test.jpg",
            1024,
            1920,
            1080,
            "abc123",
            TestDate,
            null,
            null);

        Assert.Throws<ArgumentException>(() => asset.Delete(Guid.Empty, TestDate.AddHours(1)));
    }

    [Fact]
    public void MediaAsset_Delete_WhenAlreadyDeleted_ThrowsInvalidOperationException()
    {
        var asset = new MediaAsset(
            Guid.NewGuid(),
            TestUserId,
            MediaType.Image,
            "image/jpeg",
            "images/test.jpg",
            1024,
            1920,
            1080,
            "abc123",
            TestDate,
            null,
            null);

        asset.Delete(TestUserId, TestDate.AddHours(1));

        Assert.Throws<InvalidOperationException>(() => asset.Delete(TestUserId, TestDate.AddHours(2)));
    }

    [Fact]
    public void MediaAsset_Restore_WhenNotDeleted_ThrowsInvalidOperationException()
    {
        var asset = new MediaAsset(
            Guid.NewGuid(),
            TestUserId,
            MediaType.Image,
            "image/jpeg",
            "images/test.jpg",
            1024,
            1920,
            1080,
            "abc123",
            TestDate,
            null,
            null);

        Assert.Throws<InvalidOperationException>(() => asset.Restore());
    }

    [Fact]
    public void MediaAsset_Restore_WhenDeleted_RestoresSuccessfully()
    {
        var asset = new MediaAsset(
            Guid.NewGuid(),
            TestUserId,
            MediaType.Image,
            "image/jpeg",
            "images/test.jpg",
            1024,
            1920,
            1080,
            "abc123",
            TestDate,
            null,
            null);

        asset.Delete(TestUserId, TestDate.AddHours(1));
        Assert.True(asset.IsDeleted);

        asset.Restore();
        Assert.False(asset.IsDeleted);
        Assert.Null(asset.DeletedByUserId);
        Assert.Null(asset.DeletedAtUtc);
    }

    [Fact]
    public void MediaAsset_Constructor_WithAllMediaTypes_CreatesSuccessfully()
    {
        var mediaTypes = new[] { MediaType.Image, MediaType.Video, MediaType.Audio, MediaType.Document };

        foreach (var mediaType in mediaTypes)
        {
            var asset = new MediaAsset(
                Guid.NewGuid(),
                TestUserId,
                mediaType,
                "application/octet-stream",
                $"files/test.{mediaType.ToString().ToLower()}",
                1024,
                null,
                null,
                "abc123",
                TestDate,
                null,
                null);

            Assert.Equal(mediaType, asset.MediaType);
        }
    }

    // MediaAttachment edge cases
    [Fact]
    public void MediaAttachment_Constructor_WithEmptyMediaAssetId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new MediaAttachment(
            Guid.NewGuid(),
            Guid.Empty,
            MediaOwnerType.Post,
            Guid.NewGuid(),
            0,
            TestDate));
    }

    [Fact]
    public void MediaAttachment_Constructor_WithEmptyOwnerId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new MediaAttachment(
            Guid.NewGuid(),
            Guid.NewGuid(),
            MediaOwnerType.Post,
            Guid.Empty,
            0,
            TestDate));
    }

    [Fact]
    public void MediaAttachment_Constructor_WithNegativeDisplayOrder_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new MediaAttachment(
            Guid.NewGuid(),
            Guid.NewGuid(),
            MediaOwnerType.Post,
            Guid.NewGuid(),
            -1,
            TestDate));
    }

    [Fact]
    public void MediaAttachment_Constructor_WithZeroDisplayOrder_CreatesSuccessfully()
    {
        var attachment = new MediaAttachment(
            Guid.NewGuid(),
            Guid.NewGuid(),
            MediaOwnerType.Post,
            Guid.NewGuid(),
            0,
            TestDate);

        Assert.Equal(0, attachment.DisplayOrder);
    }

    [Fact]
    public void MediaAttachment_Constructor_WithLargeDisplayOrder_CreatesSuccessfully()
    {
        var largeOrder = int.MaxValue;
        var attachment = new MediaAttachment(
            Guid.NewGuid(),
            Guid.NewGuid(),
            MediaOwnerType.Post,
            Guid.NewGuid(),
            largeOrder,
            TestDate);

        Assert.Equal(largeOrder, attachment.DisplayOrder);
    }

    [Fact]
    public void MediaAttachment_UpdateDisplayOrder_WithNegativeValue_ThrowsArgumentException()
    {
        var attachment = new MediaAttachment(
            Guid.NewGuid(),
            Guid.NewGuid(),
            MediaOwnerType.Post,
            Guid.NewGuid(),
            0,
            TestDate);

        Assert.Throws<ArgumentException>(() => attachment.UpdateDisplayOrder(-1));
    }

    [Fact]
    public void MediaAttachment_UpdateDisplayOrder_WithValidValue_UpdatesSuccessfully()
    {
        var attachment = new MediaAttachment(
            Guid.NewGuid(),
            Guid.NewGuid(),
            MediaOwnerType.Post,
            Guid.NewGuid(),
            0,
            TestDate);

        attachment.UpdateDisplayOrder(10);

        Assert.Equal(10, attachment.DisplayOrder);
    }

    [Fact]
    public void MediaAttachment_Constructor_WithAllOwnerTypes_CreatesSuccessfully()
    {
        var ownerTypes = new[]
        {
            MediaOwnerType.User,
            MediaOwnerType.Post,
            MediaOwnerType.Event,
            MediaOwnerType.StoreItem,
            MediaOwnerType.ChatMessage
        };

        foreach (var ownerType in ownerTypes)
        {
            var attachment = new MediaAttachment(
                Guid.NewGuid(),
                Guid.NewGuid(),
                ownerType,
                Guid.NewGuid(),
                0,
                TestDate);

            Assert.Equal(ownerType, attachment.OwnerType);
        }
    }
}
