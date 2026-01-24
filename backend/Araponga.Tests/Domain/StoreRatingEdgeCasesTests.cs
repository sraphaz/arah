using Araponga.Domain.Marketplace;
using Xunit;

namespace Araponga.Tests.Domain;

/// <summary>
/// Edge case tests for StoreRating and StoreItemRating domain entities,
/// focusing on rating validation (1-5), comment length limits, and updates.
/// </summary>
public class StoreRatingEdgeCasesTests
{
    private static readonly DateTime TestDate = DateTime.UtcNow;
    private static readonly Guid TestStoreId = Guid.NewGuid();
    private static readonly Guid TestUserId = Guid.NewGuid();

    // StoreRating constructor validation tests
    [Fact]
    public void StoreRating_Constructor_WithValidData_CreatesSuccessfully()
    {
        var ratingId = Guid.NewGuid();

        var rating = new StoreRating(
            ratingId,
            TestStoreId,
            TestUserId,
            5,
            "Excelente loja!",
            TestDate,
            TestDate);

        Assert.Equal(ratingId, rating.Id);
        Assert.Equal(5, rating.Rating);
        Assert.Equal("Excelente loja!", rating.Comment);
    }

    [Fact]
    public void StoreRating_Constructor_WithNullId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new StoreRating(
                Guid.Empty,
                TestStoreId,
                TestUserId,
                5,
                null,
                TestDate,
                TestDate));
    }

    [Fact]
    public void StoreRating_Constructor_WithNullStoreId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new StoreRating(
                Guid.NewGuid(),
                Guid.Empty,
                TestUserId,
                5,
                null,
                TestDate,
                TestDate));
    }

    [Fact]
    public void StoreRating_Constructor_WithNullUserId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new StoreRating(
                Guid.NewGuid(),
                TestStoreId,
                Guid.Empty,
                5,
                null,
                TestDate,
                TestDate));
    }

    [Fact]
    public void StoreRating_Constructor_WithRatingBelow1_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new StoreRating(
                Guid.NewGuid(),
                TestStoreId,
                TestUserId,
                0,
                null,
                TestDate,
                TestDate));
    }

    [Fact]
    public void StoreRating_Constructor_WithRatingAbove5_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new StoreRating(
                Guid.NewGuid(),
                TestStoreId,
                TestUserId,
                6,
                null,
                TestDate,
                TestDate));
    }

    [Fact]
    public void StoreRating_Constructor_WithAllValidRatings_CreatesSuccessfully()
    {
        for (int rating = 1; rating <= 5; rating++)
        {
            var storeRating = new StoreRating(
                Guid.NewGuid(),
                TestStoreId,
                TestUserId,
                rating,
                null,
                TestDate,
                TestDate);

            Assert.Equal(rating, storeRating.Rating);
        }
    }

    [Fact]
    public void StoreRating_Constructor_WithCommentExceeding2000Chars_ThrowsArgumentException()
    {
        var longComment = new string('A', 2001);

        Assert.Throws<ArgumentException>(() =>
            new StoreRating(
                Guid.NewGuid(),
                TestStoreId,
                TestUserId,
                5,
                longComment,
                TestDate,
                TestDate));
    }

    [Fact]
    public void StoreRating_Constructor_WithCommentExactly2000Chars_CreatesSuccessfully()
    {
        var comment = new string('A', 2000);

        var rating = new StoreRating(
            Guid.NewGuid(),
            TestStoreId,
            TestUserId,
            5,
            comment,
            TestDate,
            TestDate);

        Assert.Equal(2000, rating.Comment!.Length);
    }

    [Fact]
    public void StoreRating_Constructor_WithWhitespaceComment_TrimsToNull()
    {
        var rating = new StoreRating(
            Guid.NewGuid(),
            TestStoreId,
            TestUserId,
            5,
            "   ",
            TestDate,
            TestDate);

        Assert.Null(rating.Comment);
    }

    [Fact]
    public void StoreRating_Constructor_WithUnicodeComment_TrimsAndStores()
    {
        var rating = new StoreRating(
            Guid.NewGuid(),
            TestStoreId,
            TestUserId,
            5,
            "  Excelente loja! Café, naïve, résumé, 文字 e emoji 🏪  ",
            TestDate,
            TestDate);

        Assert.Equal("Excelente loja! Café, naïve, résumé, 文字 e emoji 🏪", rating.Comment);
    }

    // StoreRating Update tests
    [Fact]
    public void StoreRating_Update_WithValidData_UpdatesSuccessfully()
    {
        var rating = CreateStoreRating(5, "Bom");
        var newDate = TestDate.AddHours(1);

        rating.Update(4, "Muito bom", newDate);

        Assert.Equal(4, rating.Rating);
        Assert.Equal("Muito bom", rating.Comment);
        Assert.Equal(newDate, rating.UpdatedAtUtc);
    }

    [Fact]
    public void StoreRating_Update_WithRatingBelow1_ThrowsArgumentException()
    {
        var rating = CreateStoreRating(5, null);

        Assert.Throws<ArgumentException>(() =>
            rating.Update(0, null, TestDate.AddHours(1)));
    }

    [Fact]
    public void StoreRating_Update_WithRatingAbove5_ThrowsArgumentException()
    {
        var rating = CreateStoreRating(5, null);

        Assert.Throws<ArgumentException>(() =>
            rating.Update(6, null, TestDate.AddHours(1)));
    }

    [Fact]
    public void StoreRating_Update_WithCommentExceeding2000Chars_ThrowsArgumentException()
    {
        var rating = CreateStoreRating(5, null);
        var longComment = new string('A', 2001);

        Assert.Throws<ArgumentException>(() =>
            rating.Update(5, longComment, TestDate.AddHours(1)));
    }

    [Fact]
    public void StoreRating_Update_WithWhitespaceComment_TrimsToNull()
    {
        var rating = CreateStoreRating(5, "Original");

        rating.Update(4, "   ", TestDate.AddHours(1));

        Assert.Null(rating.Comment);
    }

    [Fact]
    public void StoreRating_Update_UpdatesTimestamp()
    {
        var rating = CreateStoreRating(5, null);
        var originalTimestamp = rating.UpdatedAtUtc;

        System.Threading.Thread.Sleep(10);
        rating.Update(4, "Atualizado", DateTime.UtcNow);

        Assert.True(rating.UpdatedAtUtc > originalTimestamp);
    }

    // StoreItemRating constructor validation tests
    [Fact]
    public void StoreItemRating_Constructor_WithValidData_CreatesSuccessfully()
    {
        var ratingId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        var rating = new StoreItemRating(
            ratingId,
            itemId,
            TestUserId,
            5,
            "Excelente produto!",
            TestDate,
            TestDate);

        Assert.Equal(ratingId, rating.Id);
        Assert.Equal(itemId, rating.StoreItemId);
        Assert.Equal(5, rating.Rating);
    }

    [Fact]
    public void StoreItemRating_Constructor_WithNullId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new StoreItemRating(
                Guid.Empty,
                Guid.NewGuid(),
                TestUserId,
                5,
                null,
                TestDate,
                TestDate));
    }

    [Fact]
    public void StoreItemRating_Constructor_WithNullStoreItemId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new StoreItemRating(
                Guid.NewGuid(),
                Guid.Empty,
                TestUserId,
                5,
                null,
                TestDate,
                TestDate));
    }

    [Fact]
    public void StoreItemRating_Constructor_WithNullUserId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new StoreItemRating(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.Empty,
                5,
                null,
                TestDate,
                TestDate));
    }

    [Fact]
    public void StoreItemRating_Constructor_WithRatingBelow1_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new StoreItemRating(
                Guid.NewGuid(),
                Guid.NewGuid(),
                TestUserId,
                0,
                null,
                TestDate,
                TestDate));
    }

    [Fact]
    public void StoreItemRating_Constructor_WithRatingAbove5_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new StoreItemRating(
                Guid.NewGuid(),
                Guid.NewGuid(),
                TestUserId,
                6,
                null,
                TestDate,
                TestDate));
    }

    [Fact]
    public void StoreItemRating_Constructor_WithAllValidRatings_CreatesSuccessfully()
    {
        for (int rating = 1; rating <= 5; rating++)
        {
            var itemRating = new StoreItemRating(
                Guid.NewGuid(),
                Guid.NewGuid(),
                TestUserId,
                rating,
                null,
                TestDate,
                TestDate);

            Assert.Equal(rating, itemRating.Rating);
        }
    }

    [Fact]
    public void StoreItemRating_Constructor_WithCommentExceeding2000Chars_ThrowsArgumentException()
    {
        var longComment = new string('A', 2001);

        Assert.Throws<ArgumentException>(() =>
            new StoreItemRating(
                Guid.NewGuid(),
                Guid.NewGuid(),
                TestUserId,
                5,
                longComment,
                TestDate,
                TestDate));
    }

    [Fact]
    public void StoreItemRating_Constructor_WithCommentExactly2000Chars_CreatesSuccessfully()
    {
        var comment = new string('A', 2000);

        var rating = new StoreItemRating(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            5,
            comment,
            TestDate,
            TestDate);

        Assert.Equal(2000, rating.Comment!.Length);
    }

    [Fact]
    public void StoreItemRating_Constructor_WithWhitespaceComment_TrimsToNull()
    {
        var rating = new StoreItemRating(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            5,
            "   ",
            TestDate,
            TestDate);

        Assert.Null(rating.Comment);
    }

    [Fact]
    public void StoreItemRating_Constructor_WithUnicodeComment_TrimsAndStores()
    {
        var rating = new StoreItemRating(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            5,
            "  Excelente! Café, naïve, résumé, 文字 e emoji 🍃  ",
            TestDate,
            TestDate);

        Assert.Equal("Excelente! Café, naïve, résumé, 文字 e emoji 🍃", rating.Comment);
    }

    // StoreItemRating Update tests
    [Fact]
    public void StoreItemRating_Update_WithValidData_UpdatesSuccessfully()
    {
        var rating = CreateStoreItemRating(5, "Bom");
        var newDate = TestDate.AddHours(1);

        rating.Update(4, "Muito bom", newDate);

        Assert.Equal(4, rating.Rating);
        Assert.Equal("Muito bom", rating.Comment);
        Assert.Equal(newDate, rating.UpdatedAtUtc);
    }

    [Fact]
    public void StoreItemRating_Update_WithRatingBelow1_ThrowsArgumentException()
    {
        var rating = CreateStoreItemRating(5, null);

        Assert.Throws<ArgumentException>(() =>
            rating.Update(0, null, TestDate.AddHours(1)));
    }

    [Fact]
    public void StoreItemRating_Update_WithRatingAbove5_ThrowsArgumentException()
    {
        var rating = CreateStoreItemRating(5, null);

        Assert.Throws<ArgumentException>(() =>
            rating.Update(6, null, TestDate.AddHours(1)));
    }

    [Fact]
    public void StoreItemRating_Update_WithCommentExceeding2000Chars_ThrowsArgumentException()
    {
        var rating = CreateStoreItemRating(5, null);
        var longComment = new string('A', 2001);

        Assert.Throws<ArgumentException>(() =>
            rating.Update(5, longComment, TestDate.AddHours(1)));
    }

    [Fact]
    public void StoreItemRating_Update_WithWhitespaceComment_TrimsToNull()
    {
        var rating = CreateStoreItemRating(5, "Original");

        rating.Update(4, "   ", TestDate.AddHours(1));

        Assert.Null(rating.Comment);
    }

    [Fact]
    public void StoreItemRating_Update_UpdatesTimestamp()
    {
        var rating = CreateStoreItemRating(5, null);
        var originalTimestamp = rating.UpdatedAtUtc;

        System.Threading.Thread.Sleep(10);
        rating.Update(4, "Atualizado", DateTime.UtcNow);

        Assert.True(rating.UpdatedAtUtc > originalTimestamp);
    }

    // Helper methods
    private static StoreRating CreateStoreRating(int rating, string? comment)
    {
        return new StoreRating(
            Guid.NewGuid(),
            TestStoreId,
            TestUserId,
            rating,
            comment,
            TestDate,
            TestDate);
    }

    private static StoreItemRating CreateStoreItemRating(int rating, string? comment)
    {
        return new StoreItemRating(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            rating,
            comment,
            TestDate,
            TestDate);
    }
}
