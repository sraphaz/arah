namespace Arah.Application.Common;

public sealed record PostCounts
{
    private int _likeCount;
    private int _shareCount;
    private int _commentCount;

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

    public int CommentCount
    {
        get => _commentCount;
        init => _commentCount = value > int.MaxValue ? int.MaxValue : value;
    }

    public PostCounts(int likeCount, int shareCount, int commentCount = 0)
    {
        LikeCount = likeCount;
        ShareCount = shareCount;
        CommentCount = commentCount;
    }
}
