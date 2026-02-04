using Araponga.Domain.Users;
using Xunit;

namespace Araponga.Tests.Domain.Users;

public sealed class UserInterestEdgeCasesTests
{
    [Fact]
    public void UserInterest_WithNullInterestTag_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new UserInterest(
                Guid.NewGuid(),
                Guid.NewGuid(),
                null!,
                DateTime.UtcNow));

        Assert.Contains("Interest tag", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UserInterest_WithEmptyInterestTag_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new UserInterest(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "",
                DateTime.UtcNow));

        Assert.Contains("Interest tag", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UserInterest_WithWhitespaceInterestTag_ThrowsArgumentException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new UserInterest(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "   ",
                DateTime.UtcNow));

        Assert.Contains("Interest tag", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UserInterest_WithInterestTagTooLong_ThrowsArgumentException()
    {
        var longTag = new string('a', 51);

        var ex = Assert.Throws<ArgumentException>(() =>
            new UserInterest(
                Guid.NewGuid(),
                Guid.NewGuid(),
                longTag,
                DateTime.UtcNow));

        Assert.Contains("Interest tag", ex.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("50", ex.Message);
    }

    [Fact]
    public void UserInterest_WithInterestTagExactly50Characters_DoesNotThrow()
    {
        var tag = new string('a', 50);

        var interest = new UserInterest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            tag,
            DateTime.UtcNow);

        Assert.Equal(tag, interest.InterestTag);
    }

    [Fact]
    public void UserInterest_NormalizesInterestTag_ToLowercase()
    {
        var interest = new UserInterest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "MEIO AMBIENTE",
            DateTime.UtcNow);

        Assert.Equal("meio ambiente", interest.InterestTag);
    }

    [Fact]
    public void UserInterest_NormalizesInterestTag_TrimsWhitespace()
    {
        var interest = new UserInterest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "  meio ambiente  ",
            DateTime.UtcNow);

        Assert.Equal("meio ambiente", interest.InterestTag);
    }

    [Fact]
    public void UserInterest_NormalizesInterestTag_TrimsAndLowercases()
    {
        var interest = new UserInterest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "  MEIO AMBIENTE  ",
            DateTime.UtcNow);

        Assert.Equal("meio ambiente", interest.InterestTag);
    }

    [Fact]
    public void UserInterest_WithUnicodeInInterestTag_AcceptsUnicode()
    {
        var interest = new UserInterest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "educação",
            DateTime.UtcNow);

        Assert.Equal("educação", interest.InterestTag);
    }

    [Fact]
    public void UserInterest_WithSpecialCharacters_AcceptsSpecialCharacters()
    {
        var interest = new UserInterest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "cultura & arte",
            DateTime.UtcNow);

        Assert.Equal("cultura & arte", interest.InterestTag);
    }

    [Fact]
    public void UserInterest_WithNumbers_AcceptsNumbers()
    {
        var interest = new UserInterest(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "eventos 2024",
            DateTime.UtcNow);

        Assert.Equal("eventos 2024", interest.InterestTag);
    }
}
