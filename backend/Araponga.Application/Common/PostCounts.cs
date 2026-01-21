namespace Araponga.Application.Common;

public sealed record PostCounts(int LikeCount, int ShareCount)
{
    public PostCounts(int likeCount, int shareCount)
        : this(
            likeCount > int.MaxValue ? int.MaxValue : likeCount,
            shareCount > int.MaxValue ? int.MaxValue : shareCount)
    {
    }
}
