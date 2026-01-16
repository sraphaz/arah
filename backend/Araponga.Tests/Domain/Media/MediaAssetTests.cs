using Araponga.Domain.Media;
using Xunit;

namespace Araponga.Tests.Domain.Media;

public sealed class MediaAssetTests
{
    [Fact]
    public void MediaAsset_Create_WithValidInputs_Success()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var mediaAsset = new MediaAsset(
            id,
            userId,
            MediaType.Image,
            "image/jpeg",
            "images/2025/01/test.jpg",
            1024,
            1920,
            1080,
            "abc123",
            now,
            null,
            null);

        Assert.Equal(id, mediaAsset.Id);
        Assert.Equal(userId, mediaAsset.UploadedByUserId);
        Assert.Equal(MediaType.Image, mediaAsset.MediaType);
        Assert.Equal("image/jpeg", mediaAsset.MimeType);
        Assert.Equal(1024, mediaAsset.SizeBytes);
        Assert.Equal(1920, mediaAsset.WidthPx);
        Assert.Equal(1080, mediaAsset.HeightPx);
        Assert.Equal("abc123", mediaAsset.Checksum);
        Assert.Equal(now, mediaAsset.CreatedAtUtc);
        Assert.False(mediaAsset.IsDeleted);
        Assert.Null(mediaAsset.DeletedByUserId);
        Assert.Null(mediaAsset.DeletedAtUtc);
    }

    [Fact]
    public void MediaAsset_Create_WithEmptyUploadedByUserId_Throws()
    {
        Assert.Throws<ArgumentException>(() => new MediaAsset(
            Guid.NewGuid(),
            Guid.Empty,
            MediaType.Image,
            "image/jpeg",
            "images/test.jpg",
            1024,
            1920,
            1080,
            "abc123",
            DateTime.UtcNow,
            null,
            null));
    }

    [Fact]
    public void MediaAsset_Create_WithNullMimeType_Throws()
    {
        Assert.Throws<ArgumentException>(() => new MediaAsset(
            Guid.NewGuid(),
            Guid.NewGuid(),
            MediaType.Image,
            null!,
            "images/test.jpg",
            1024,
            1920,
            1080,
            "abc123",
            DateTime.UtcNow,
            null,
            null));
    }

    [Fact]
    public void MediaAsset_Create_WithEmptyStorageKey_Throws()
    {
        Assert.Throws<ArgumentException>(() => new MediaAsset(
            Guid.NewGuid(),
            Guid.NewGuid(),
            MediaType.Image,
            "image/jpeg",
            "",
            1024,
            1920,
            1080,
            "abc123",
            DateTime.UtcNow,
            null,
            null));
    }

    [Fact]
    public void MediaAsset_Create_WithNegativeSize_Throws()
    {
        Assert.Throws<ArgumentException>(() => new MediaAsset(
            Guid.NewGuid(),
            Guid.NewGuid(),
            MediaType.Image,
            "image/jpeg",
            "images/test.jpg",
            -1,
            1920,
            1080,
            "abc123",
            DateTime.UtcNow,
            null,
            null));
    }

    [Fact]
    public void MediaAsset_Create_WithNegativeWidth_Throws()
    {
        Assert.Throws<ArgumentException>(() => new MediaAsset(
            Guid.NewGuid(),
            Guid.NewGuid(),
            MediaType.Image,
            "image/jpeg",
            "images/test.jpg",
            1024,
            -1,
            1080,
            "abc123",
            DateTime.UtcNow,
            null,
            null));
    }

    [Fact]
    public void MediaAsset_Create_WithNullChecksum_Throws()
    {
        Assert.Throws<ArgumentException>(() => new MediaAsset(
            Guid.NewGuid(),
            Guid.NewGuid(),
            MediaType.Image,
            "image/jpeg",
            "images/test.jpg",
            1024,
            1920,
            1080,
            null!,
            DateTime.UtcNow,
            null,
            null));
    }

    [Fact]
    public void MediaAsset_Delete_Success()
    {
        var mediaAsset = CreateValidMediaAsset();
        var deletedByUserId = Guid.NewGuid();
        var deletedAtUtc = DateTime.UtcNow;

        mediaAsset.Delete(deletedByUserId, deletedAtUtc);

        Assert.True(mediaAsset.IsDeleted);
        Assert.Equal(deletedByUserId, mediaAsset.DeletedByUserId);
        Assert.Equal(deletedAtUtc, mediaAsset.DeletedAtUtc);
    }

    [Fact]
    public void MediaAsset_Delete_WithEmptyUserId_Throws()
    {
        var mediaAsset = CreateValidMediaAsset();

        Assert.Throws<ArgumentException>(() => mediaAsset.Delete(Guid.Empty, DateTime.UtcNow));
    }

    [Fact]
    public void MediaAsset_Delete_WhenAlreadyDeleted_Throws()
    {
        var mediaAsset = CreateValidMediaAsset();
        mediaAsset.Delete(Guid.NewGuid(), DateTime.UtcNow);

        Assert.Throws<InvalidOperationException>(() => mediaAsset.Delete(Guid.NewGuid(), DateTime.UtcNow));
    }

    [Fact]
    public void MediaAsset_Restore_Success()
    {
        var mediaAsset = CreateValidMediaAsset();
        mediaAsset.Delete(Guid.NewGuid(), DateTime.UtcNow);

        mediaAsset.Restore();

        Assert.False(mediaAsset.IsDeleted);
        Assert.Null(mediaAsset.DeletedByUserId);
        Assert.Null(mediaAsset.DeletedAtUtc);
    }

    [Fact]
    public void MediaAsset_Restore_WhenNotDeleted_Throws()
    {
        var mediaAsset = CreateValidMediaAsset();

        Assert.Throws<InvalidOperationException>(() => mediaAsset.Restore());
    }

    [Fact]
    public void MediaAsset_Create_WithAllMediaTypes_Success()
    {
        var userId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var image = new MediaAsset(Guid.NewGuid(), userId, MediaType.Image, "image/jpeg", "test.jpg", 1024, 100, 100, "hash", now, null, null);
        var video = new MediaAsset(Guid.NewGuid(), userId, MediaType.Video, "video/mp4", "test.mp4", 2048, 1920, 1080, "hash", now, null, null);
        var audio = new MediaAsset(Guid.NewGuid(), userId, MediaType.Audio, "audio/mp3", "test.mp3", 512, null, null, "hash", now, null, null);
        var document = new MediaAsset(Guid.NewGuid(), userId, MediaType.Document, "application/pdf", "test.pdf", 1024, null, null, "hash", now, null, null);

        Assert.Equal(MediaType.Image, image.MediaType);
        Assert.Equal(MediaType.Video, video.MediaType);
        Assert.Equal(MediaType.Audio, audio.MediaType);
        Assert.Equal(MediaType.Document, document.MediaType);
    }

    private static MediaAsset CreateValidMediaAsset()
    {
        return new MediaAsset(
            Guid.NewGuid(),
            Guid.NewGuid(),
            MediaType.Image,
            "image/jpeg",
            "images/test.jpg",
            1024,
            1920,
            1080,
            "abc123",
            DateTime.UtcNow,
            null,
            null);
    }
}