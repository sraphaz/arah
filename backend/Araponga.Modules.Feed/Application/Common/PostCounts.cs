namespace Araponga.Application.Common;

public sealed record PostCounts
{
    private int _likeCount;
    private int _shareCount;

    public int LikeCount
    {
        get => _likeCount;
        init => _likeCount = value > int.MaxValue ? int.MaxValue : value;
    }

    public int ShareCount
    {
        get => _shareCount;
        init => _shareCount = value > int.MaxValue ? int.MaxValue : value;
    }

    public PostCounts(int likeCount, int shareCount)
    {
        LikeCount = likeCount;
        ShareCount = shareCount;
    }
}
