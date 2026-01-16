using Araponga.Domain.Media;
using Xunit;

namespace Araponga.Tests.Domain.Media;

public sealed class MediaAttachmentTests
{
    [Fact]
    public void MediaAttachment_Create_WithValidInputs_Success()
    {
        var id = Guid.NewGuid();
        var mediaAssetId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var attachment = new MediaAttachment(
            id,
            mediaAssetId,
            MediaOwnerType.Post,
            ownerId,
            0,
            now);

        Assert.Equal(id, attachment.Id);
        Assert.Equal(mediaAssetId, attachment.MediaAssetId);
        Assert.Equal(MediaOwnerType.Post, attachment.OwnerType);
        Assert.Equal(ownerId, attachment.OwnerId);
        Assert.Equal(0, attachment.DisplayOrder);
        Assert.Equal(now, attachment.CreatedAtUtc);
    }

    [Fact]
    public void MediaAttachment_Create_WithAllOwnerTypes_Success()
    {
        var mediaAssetId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var userAttachment = new MediaAttachment(Guid.NewGuid(), mediaAssetId, MediaOwnerType.User, Guid.NewGuid(), 0, now);
        var postAttachment = new MediaAttachment(Guid.NewGuid(), mediaAssetId, MediaOwnerType.Post, Guid.NewGuid(), 0, now);
        var eventAttachment = new MediaAttachment(Guid.NewGuid(), mediaAssetId, MediaOwnerType.Event, Guid.NewGuid(), 0, now);
        var storeItemAttachment = new MediaAttachment(Guid.NewGuid(), mediaAssetId, MediaOwnerType.StoreItem, Guid.NewGuid(), 0, now);
        var chatMessageAttachment = new MediaAttachment(Guid.NewGuid(), mediaAssetId, MediaOwnerType.ChatMessage, Guid.NewGuid(), 0, now);

        Assert.Equal(MediaOwnerType.User, userAttachment.OwnerType);
        Assert.Equal(MediaOwnerType.Post, postAttachment.OwnerType);
        Assert.Equal(MediaOwnerType.Event, eventAttachment.OwnerType);
        Assert.Equal(MediaOwnerType.StoreItem, storeItemAttachment.OwnerType);
        Assert.Equal(MediaOwnerType.ChatMessage, chatMessageAttachment.OwnerType);
    }

    [Fact]
    public void MediaAttachment_Create_WithEmptyMediaAssetId_Throws()
    {
        Assert.Throws<ArgumentException>(() => new MediaAttachment(
            Guid.NewGuid(),
            Guid.Empty,
            MediaOwnerType.Post,
            Guid.NewGuid(),
            0,
            DateTime.UtcNow));
    }

    [Fact]
    public void MediaAttachment_Create_WithEmptyOwnerId_Throws()
    {
        Assert.Throws<ArgumentException>(() => new MediaAttachment(
            Guid.NewGuid(),
            Guid.NewGuid(),
            MediaOwnerType.Post,
            Guid.Empty,
            0,
            DateTime.UtcNow));
    }

    [Fact]
    public void MediaAttachment_Create_WithNegativeDisplayOrder_Throws()
    {
        Assert.Throws<ArgumentException>(() => new MediaAttachment(
            Guid.NewGuid(),
            Guid.NewGuid(),
            MediaOwnerType.Post,
            Guid.NewGuid(),
            -1,
            DateTime.UtcNow));
    }

    [Fact]
    public void MediaAttachment_UpdateDisplayOrder_Success()
    {
        var attachment = CreateValidAttachment();

        attachment.UpdateDisplayOrder(5);

        Assert.Equal(5, attachment.DisplayOrder);
    }

    [Fact]
    public void MediaAttachment_UpdateDisplayOrder_WithNegativeValue_Throws()
    {
        var attachment = CreateValidAttachment();

        Assert.Throws<ArgumentException>(() => attachment.UpdateDisplayOrder(-1));
    }

    private static MediaAttachment CreateValidAttachment()
    {
        return new MediaAttachment(
            Guid.NewGuid(),
            Guid.NewGuid(),
            MediaOwnerType.Post,
            Guid.NewGuid(),
            0,
            DateTime.UtcNow);
    }
}