namespace Araponga.Domain.Feed;

public sealed class PostAsset
{
    public PostAsset(Guid postId, Guid assetId)
    {
        PostId = postId;
        AssetId = assetId;
    }

    public Guid PostId { get; }
    public Guid AssetId { get; }
}
