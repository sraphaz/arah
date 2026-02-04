using Araponga.Domain.Governance;
using Araponga.Domain.Territories;
using Araponga.Domain.Users;
using Xunit;

namespace Araponga.Tests.Domain;

/// <summary>
/// Edge case tests for the Voting domain entity, focusing on voting creation,
/// status transitions, option validation, and boundary conditions.
/// </summary>
public class VotingEdgeCasesTests
{
    private static readonly DateTime TestDate = DateTime.UtcNow;
    private static readonly Guid TestTerritoryId = Guid.NewGuid();
    private static readonly Guid TestUserId = Guid.NewGuid();

    // Constructor validation tests
    [Fact]
    public void Constructor_WithValidData_CreatesSuccessfully()
    {
        var votingId = Guid.NewGuid();
        var options = new[] { "Option1", "Option2" };

        var voting = new Voting(
            votingId,
            TestTerritoryId,
            TestUserId,
            VotingType.ThemePrioritization,
            "Test Voting",
            "This is a test voting",
            options,
            VotingVisibility.AllMembers,
            VotingStatus.Draft,
            null,
            null,
            TestDate,
            TestDate);

        Assert.Equal(votingId, voting.Id);
        Assert.Equal("Test Voting", voting.Title);
        Assert.Equal(VotingStatus.Draft, voting.Status);
    }

    [Fact]
    public void Constructor_WithNullTitle_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Voting(
                Guid.NewGuid(),
                TestTerritoryId,
                TestUserId,
                VotingType.ThemePrioritization,
                null!,
                "Description",
                new[] { "Option1", "Option2" },
                VotingVisibility.AllMembers,
                VotingStatus.Draft,
                null,
                null,
                TestDate,
                TestDate));
    }

    [Fact]
    public void Constructor_WithEmptyTitle_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Voting(
                Guid.NewGuid(),
                TestTerritoryId,
                TestUserId,
                VotingType.ThemePrioritization,
                "   ",
                "Description",
                new[] { "Option1", "Option2" },
                VotingVisibility.AllMembers,
                VotingStatus.Draft,
                null,
                null,
                TestDate,
                TestDate));
    }

    [Fact]
    public void Constructor_WithNullDescription_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Voting(
                Guid.NewGuid(),
                TestTerritoryId,
                TestUserId,
                VotingType.ThemePrioritization,
                "Title",
                null!,
                new[] { "Option1", "Option2" },
                VotingVisibility.AllMembers,
                VotingStatus.Draft,
                null,
                null,
                TestDate,
                TestDate));
    }

    [Fact]
    public void Constructor_WithEmptyDescription_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Voting(
                Guid.NewGuid(),
                TestTerritoryId,
                TestUserId,
                VotingType.ThemePrioritization,
                "Title",
                "   ",
                new[] { "Option1", "Option2" },
                VotingVisibility.AllMembers,
                VotingStatus.Draft,
                null,
                null,
                TestDate,
                TestDate));
    }

    [Fact]
    public void Constructor_WithLessThanTwoOptions_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Voting(
                Guid.NewGuid(),
                TestTerritoryId,
                TestUserId,
                VotingType.ThemePrioritization,
                "Title",
                "Description",
                new[] { "OnlyOne" },
                VotingVisibility.AllMembers,
                VotingStatus.Draft,
                null,
                null,
                TestDate,
                TestDate));
    }

    [Fact]
    public void Constructor_WithNullOptions_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Voting(
                Guid.NewGuid(),
                TestTerritoryId,
                TestUserId,
                VotingType.ThemePrioritization,
                "Title",
                "Description",
                null!,
                VotingVisibility.AllMembers,
                VotingStatus.Draft,
                null,
                null,
                TestDate,
                TestDate));
    }

    [Fact]
    public void Constructor_WithMaximumOptions_CreatesSuccessfully()
    {
        var options = Enumerable.Range(1, 10).Select(i => $"Option{i}").ToArray();

        var voting = new Voting(
            Guid.NewGuid(),
            TestTerritoryId,
            TestUserId,
            VotingType.ThemePrioritization,
            "Title",
            "Description",
            options,
            VotingVisibility.AllMembers,
            VotingStatus.Draft,
            null,
            null,
            TestDate,
            TestDate);

        Assert.Equal(10, voting.Options.Count);
    }

    [Fact]
    public void Constructor_WithUnicodeInTitle_TrimsAndStores()
    {
        var voting = new Voting(
            Guid.NewGuid(),
            TestTerritoryId,
            TestUserId,
            VotingType.ThemePrioritization,
            "  Título com Ünícödé  ",
            "Description",
            new[] { "Option1", "Option2" },
            VotingVisibility.AllMembers,
            VotingStatus.Draft,
            null,
            null,
            TestDate,
            TestDate);

        Assert.Equal("Título com Ünícödé", voting.Title);
    }

    [Fact]
    public void Constructor_WithUnicodeInDescription_TrimsAndStores()
    {
        var voting = new Voting(
            Guid.NewGuid(),
            TestTerritoryId,
            TestUserId,
            VotingType.ThemePrioritization,
            "Title",
            "  Descríção com émóji 🎉  ",
            new[] { "Option1", "Option2" },
            VotingVisibility.AllMembers,
            VotingStatus.Draft,
            null,
            null,
            TestDate,
            TestDate);

        Assert.Equal("Descríção com émóji 🎉", voting.Description);
    }

    // Status transition tests
    [Fact]
    public void Open_FromDraftStatus_TransitionsToOpen()
    {
        var voting = CreateVotingInDraft();

        voting.Open();

        Assert.Equal(VotingStatus.Open, voting.Status);
    }

    [Fact]
    public void Open_FromOpenStatus_ThrowsInvalidOperationException()
    {
        var voting = CreateVotingInDraft();
        voting.Open();

        Assert.Throws<InvalidOperationException>(() => voting.Open());
    }

    [Fact]
    public void Close_FromOpenStatus_TransitionsToClose()
    {
        var voting = CreateVotingInDraft();
        voting.Open();

        voting.Close();

        Assert.Equal(VotingStatus.Closed, voting.Status);
    }

    [Fact]
    public void Close_FromDraftStatus_ThrowsInvalidOperationException()
    {
        var voting = CreateVotingInDraft();

        Assert.Throws<InvalidOperationException>(() => voting.Close());
    }

    [Fact]
    public void Approve_FromClosedStatus_TransitionsToApproved()
    {
        var voting = CreateVotingInDraft();
        voting.Open();
        voting.Close();

        voting.Approve();

        Assert.Equal(VotingStatus.Approved, voting.Status);
    }

    [Fact]
    public void Approve_FromOpenStatus_ThrowsInvalidOperationException()
    {
        var voting = CreateVotingInDraft();
        voting.Open();

        Assert.Throws<InvalidOperationException>(() => voting.Approve());
    }

    [Fact]
    public void Reject_FromClosedStatus_TransitionsToRejected()
    {
        var voting = CreateVotingInDraft();
        voting.Open();
        voting.Close();

        voting.Reject();

        Assert.Equal(VotingStatus.Rejected, voting.Status);
    }

    [Fact]
    public void Reject_FromOpenStatus_ThrowsInvalidOperationException()
    {
        var voting = CreateVotingInDraft();
        voting.Open();

        Assert.Throws<InvalidOperationException>(() => voting.Reject());
    }

    // Voting types tests
    [Fact]
    public void Constructor_WithAllVotingTypes_CreatesSuccessfully()
    {
        var votingTypes = new[]
        {
            VotingType.ThemePrioritization,
            VotingType.ModerationRule,
            VotingType.TerritoryCharacterization,
            VotingType.FeatureFlag,
            VotingType.CommunityPolicy
        };

        foreach (var votingType in votingTypes)
        {
            var voting = new Voting(
                Guid.NewGuid(),
                TestTerritoryId,
                TestUserId,
                votingType,
                "Title",
                "Description",
                new[] { "Option1", "Option2" },
                VotingVisibility.AllMembers,
                VotingStatus.Draft,
                null,
                null,
                TestDate,
                TestDate);

            Assert.Equal(votingType, voting.Type);
        }
    }

    // Visibility tests
    [Fact]
    public void Constructor_WithAllVisibilities_CreatesSuccessfully()
    {
        var visibilities = new[]
        {
            VotingVisibility.AllMembers,
            VotingVisibility.ResidentsOnly,
            VotingVisibility.CuratorsOnly
        };

        foreach (var visibility in visibilities)
        {
            var voting = new Voting(
                Guid.NewGuid(),
                TestTerritoryId,
                TestUserId,
                VotingType.ThemePrioritization,
                "Title",
                "Description",
                new[] { "Option1", "Option2" },
                visibility,
                VotingStatus.Draft,
                null,
                null,
                TestDate,
                TestDate);

            Assert.Equal(visibility, voting.Visibility);
        }
    }

    // DateTime handling tests
    [Fact]
    public void Constructor_WithStartAndEndDates_StoresCorrectly()
    {
        var startDate = TestDate.AddDays(1);
        var endDate = TestDate.AddDays(7);

        var voting = new Voting(
            Guid.NewGuid(),
            TestTerritoryId,
            TestUserId,
            VotingType.ThemePrioritization,
            "Title",
            "Description",
            new[] { "Option1", "Option2" },
            VotingVisibility.AllMembers,
            VotingStatus.Draft,
            startDate,
            endDate,
            TestDate,
            TestDate);

        Assert.Equal(startDate, voting.StartsAtUtc);
        Assert.Equal(endDate, voting.EndsAtUtc);
    }

    [Fact]
    public void Constructor_WithoutStartAndEndDates_AllowsNull()
    {
        var voting = new Voting(
            Guid.NewGuid(),
            TestTerritoryId,
            TestUserId,
            VotingType.ThemePrioritization,
            "Title",
            "Description",
            new[] { "Option1", "Option2" },
            VotingVisibility.AllMembers,
            VotingStatus.Draft,
            null,
            null,
            TestDate,
            TestDate);

        Assert.Null(voting.StartsAtUtc);
        Assert.Null(voting.EndsAtUtc);
    }

    // UpdatedAtUtc update tests
    [Fact]
    public void Open_UpdatesTimestamp()
    {
        var voting = CreateVotingInDraft();
        var originalTimestamp = voting.UpdatedAtUtc;

        System.Threading.Thread.Sleep(10);
        voting.Open();

        Assert.True(voting.UpdatedAtUtc > originalTimestamp);
    }

    [Fact]
    public void Close_UpdatesTimestamp()
    {
        var voting = CreateVotingInDraft();
        voting.Open();
        var originalTimestamp = voting.UpdatedAtUtc;

        System.Threading.Thread.Sleep(10);
        voting.Close();

        Assert.True(voting.UpdatedAtUtc > originalTimestamp);
    }

    [Fact]
    public void Approve_UpdatesTimestamp()
    {
        var voting = CreateVotingInDraft();
        voting.Open();
        voting.Close();
        var originalTimestamp = voting.UpdatedAtUtc;

        System.Threading.Thread.Sleep(10);
        voting.Approve();

        Assert.True(voting.UpdatedAtUtc > originalTimestamp);
    }

    [Fact]
    public void Reject_UpdatesTimestamp()
    {
        var voting = CreateVotingInDraft();
        voting.Open();
        voting.Close();
        var originalTimestamp = voting.UpdatedAtUtc;

        System.Threading.Thread.Sleep(10);
        voting.Reject();

        Assert.True(voting.UpdatedAtUtc > originalTimestamp);
    }

    // Helper method
    private static Voting CreateVotingInDraft()
    {
        return new Voting(
            Guid.NewGuid(),
            TestTerritoryId,
            TestUserId,
            VotingType.ThemePrioritization,
            "Test Voting",
            "This is a test voting",
            new[] { "Option1", "Option2" },
            VotingVisibility.AllMembers,
            VotingStatus.Draft,
            null,
            null,
            TestDate,
            TestDate);
    }
}

/// <summary>
/// Edge case tests for the Vote domain entity, focusing on vote creation,
/// option validation, and data integrity.
/// </summary>
public class VoteEdgeCasesTests
{
    private static readonly DateTime TestDate = DateTime.UtcNow;
    private static readonly Guid TestVotingId = Guid.NewGuid();
    private static readonly Guid TestUserId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidData_CreatesSuccessfully()
    {
        var voteId = Guid.NewGuid();

        var vote = new Vote(voteId, TestVotingId, TestUserId, "Option1", TestDate);

        Assert.Equal(voteId, vote.Id);
        Assert.Equal(TestVotingId, vote.VotingId);
        Assert.Equal(TestUserId, vote.UserId);
        Assert.Equal("Option1", vote.SelectedOption);
    }

    [Fact]
    public void Constructor_WithNullOption_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Vote(Guid.NewGuid(), TestVotingId, TestUserId, null!, TestDate));
    }

    [Fact]
    public void Constructor_WithEmptyOption_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new Vote(Guid.NewGuid(), TestVotingId, TestUserId, "   ", TestDate));
    }

    [Fact]
    public void Constructor_WithWhitespaceOption_TrimsAndStores()
    {
        var vote = new Vote(Guid.NewGuid(), TestVotingId, TestUserId, "  Option1  ", TestDate);

        Assert.Equal("Option1", vote.SelectedOption);
    }

    [Fact]
    public void Constructor_WithUnicodeOption_TrimsAndStores()
    {
        var vote = new Vote(Guid.NewGuid(), TestVotingId, TestUserId, "  Ópçãõ 🎉  ", TestDate);

        Assert.Equal("Ópçãõ 🎉", vote.SelectedOption);
    }

    [Fact]
    public void Constructor_WithLongOption_StoresSuccessfully()
    {
        var longOption = new string('a', 500);
        var vote = new Vote(Guid.NewGuid(), TestVotingId, TestUserId, longOption, TestDate);

        Assert.Equal(longOption, vote.SelectedOption);
    }

    [Fact]
    public void Constructor_PreservesAllGuids()
    {
        var voteId = Guid.NewGuid();
        var votingId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var createdDate = TestDate.AddHours(5);

        var vote = new Vote(voteId, votingId, userId, "MyOption", createdDate);

        Assert.Equal(voteId, vote.Id);
        Assert.Equal(votingId, vote.VotingId);
        Assert.Equal(userId, vote.UserId);
        Assert.Equal(createdDate, vote.CreatedAtUtc);
    }
}

/// <summary>
/// Edge case tests for the TerritoryCharacterization domain entity.
/// </summary>
public class TerritoryCharacterizationEdgeCasesTests
{
    private static readonly Guid TestTerritoryId = Guid.NewGuid();
    private static readonly DateTime TestDate = DateTime.UtcNow;

    [Fact]
    public void Constructor_WithValidTags_CreatesSuccessfully()
    {
        var tags = new[] { "tag1", "tag2" };

        var characterization = new TerritoryCharacterization(
            TestTerritoryId,
            tags,
            TestDate);

        Assert.Equal(2, characterization.Tags.Count);
    }

    [Fact]
    public void Constructor_WithNullTags_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new TerritoryCharacterization(TestTerritoryId, null!, TestDate));
    }

    [Fact]
    public void Constructor_NormalizesToLowercase()
    {
        var tags = new[] { "TAG1", "Tag2", "tAg3" };

        var characterization = new TerritoryCharacterization(
            TestTerritoryId,
            tags,
            TestDate);

        Assert.All(characterization.Tags, tag => Assert.Equal(tag, tag.ToLowerInvariant()));
    }

    [Fact]
    public void Constructor_RemovesDuplicates()
    {
        var tags = new[] { "tag1", "TAG1", "tag1", "tag2" };

        var characterization = new TerritoryCharacterization(
            TestTerritoryId,
            tags,
            TestDate);

        Assert.Equal(2, characterization.Tags.Count);
        Assert.Contains("tag1", characterization.Tags);
        Assert.Contains("tag2", characterization.Tags);
    }

    [Fact]
    public void Constructor_TrimsWhitespace()
    {
        var tags = new[] { "  tag1  ", "\ttag2\t", "\ntag3\n" };

        var characterization = new TerritoryCharacterization(
            TestTerritoryId,
            tags,
            TestDate);

        Assert.All(characterization.Tags, tag => Assert.Equal(tag, tag.Trim()));
    }

    [Fact]
    public void Constructor_FiltersEmptyOrWhitespaceTags()
    {
        var tags = new[] { "tag1", "", "   ", "tag2", "\t", "\n" };

        var characterization = new TerritoryCharacterization(
            TestTerritoryId,
            tags,
            TestDate);

        Assert.Equal(2, characterization.Tags.Count);
        Assert.All(characterization.Tags, tag => Assert.False(string.IsNullOrWhiteSpace(tag)));
    }

    [Fact]
    public void Constructor_WithUnicodeTags_NormalizesCorrectly()
    {
        var tags = new[] { "  Técnológia  ", "  Íncêntívos  ", "  Ámbiénte  " };

        var characterization = new TerritoryCharacterization(
            TestTerritoryId,
            tags,
            TestDate);

        Assert.Equal(3, characterization.Tags.Count);
    }

    [Fact]
    public void UpdateTags_WithValidTags_UpdatesSuccessfully()
    {
        var characterization = new TerritoryCharacterization(
            TestTerritoryId,
            new[] { "old1", "old2" },
            TestDate);

        characterization.UpdateTags(new[] { "new1", "new2", "new3" });

        Assert.Equal(3, characterization.Tags.Count);
        Assert.Contains("new1", characterization.Tags);
    }

    [Fact]
    public void UpdateTags_WithNullTags_ThrowsArgumentNullException()
    {
        var characterization = new TerritoryCharacterization(
            TestTerritoryId,
            new[] { "tag1" },
            TestDate);

        Assert.Throws<ArgumentNullException>(() =>
            characterization.UpdateTags(null!));
    }

    [Fact]
    public void UpdateTags_UpdatesTimestamp()
    {
        var characterization = new TerritoryCharacterization(
            TestTerritoryId,
            new[] { "tag1" },
            TestDate);
        var originalTimestamp = characterization.UpdatedAtUtc;

        System.Threading.Thread.Sleep(10);
        characterization.UpdateTags(new[] { "tag2" });

        Assert.True(characterization.UpdatedAtUtc > originalTimestamp);
    }

    [Fact]
    public void UpdateTags_AppliesSameNormalizationRules()
    {
        var characterization = new TerritoryCharacterization(
            TestTerritoryId,
            new[] { "tag1" },
            TestDate);

        characterization.UpdateTags(new[] { "  TAG2  ", "TAG2", "", "  tag3  " });

        Assert.Equal(2, characterization.Tags.Count);
        Assert.All(characterization.Tags, tag => Assert.Equal(tag, tag.ToLowerInvariant().Trim()));
    }

    [Fact]
    public void Constructor_WithEmptyTagsList_CreatesWithEmptyTags()
    {
        var characterization = new TerritoryCharacterization(
            TestTerritoryId,
            Array.Empty<string>(),
            TestDate);

        Assert.Empty(characterization.Tags);
    }

    [Fact]
    public void Constructor_WithAllWhitespaceTags_CreatesWithEmptyTags()
    {
        var tags = new[] { "   ", "\t", "\n", "  \t  " };

        var characterization = new TerritoryCharacterization(
            TestTerritoryId,
            tags,
            TestDate);

        Assert.Empty(characterization.Tags);
    }
}

/// <summary>
/// Edge case tests for the UserInterest domain entity.
/// </summary>
public class UserInterestEdgeCasesTests
{
    private static readonly Guid TestUserId = Guid.NewGuid();
    private static readonly DateTime TestDate = DateTime.UtcNow;

    [Fact]
    public void Constructor_WithValidData_CreatesSuccessfully()
    {
        var interestId = Guid.NewGuid();

        var interest = new UserInterest(interestId, TestUserId, "tecnologia", TestDate);

        Assert.Equal(interestId, interest.Id);
        Assert.Equal(TestUserId, interest.UserId);
        Assert.Equal("tecnologia", interest.InterestTag);
    }

    [Fact]
    public void Constructor_WithNullTag_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new UserInterest(Guid.NewGuid(), TestUserId, null!, TestDate));
    }

    [Fact]
    public void Constructor_WithEmptyTag_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new UserInterest(Guid.NewGuid(), TestUserId, "   ", TestDate));
    }

    [Fact]
    public void Constructor_NormalizesToLowercase()
    {
        var interest = new UserInterest(Guid.NewGuid(), TestUserId, "TECNOLOGIA", TestDate);

        Assert.Equal("tecnologia", interest.InterestTag);
    }

    [Fact]
    public void Constructor_TrimsWhitespace()
    {
        var interest = new UserInterest(Guid.NewGuid(), TestUserId, "  tecnologia  ", TestDate);

        Assert.Equal("tecnologia", interest.InterestTag);
    }

    [Fact]
    public void Constructor_WithMaximumLengthTag_CreatesSuccessfully()
    {
        var maxTag = new string('a', 50);
        var interest = new UserInterest(Guid.NewGuid(), TestUserId, maxTag, TestDate);

        Assert.Equal(50, interest.InterestTag.Length);
    }

    [Fact]
    public void Constructor_WithExceedingLengthTag_ThrowsArgumentException()
    {
        var exceedingTag = new string('a', 51);

        Assert.Throws<ArgumentException>(() =>
            new UserInterest(Guid.NewGuid(), TestUserId, exceedingTag, TestDate));
    }

    [Fact]
    public void Constructor_WithUnicodeTag_NormalizesCorrectly()
    {
        var interest = new UserInterest(Guid.NewGuid(), TestUserId, "  Téc Nología  ", TestDate);

        Assert.Equal("téc nología", interest.InterestTag);
    }

    [Fact]
    public void Constructor_WithMixedCaseAndWhitespace_NormalizesCorrectly()
    {
        var interest = new UserInterest(Guid.NewGuid(), TestUserId, "  PrOdUçÃo Sústenável  ", TestDate);

        Assert.Equal("produção sústenável", interest.InterestTag);
        Assert.True(interest.InterestTag.Length <= 50);
    }

    [Fact]
    public void Constructor_WithSpecialCharacters_StoresCorrectly()
    {
        var interest = new UserInterest(Guid.NewGuid(), TestUserId, "tech-2024 & inovação", TestDate);

        Assert.Equal("tech-2024 & inovação", interest.InterestTag);
    }

    [Fact]
    public void Constructor_WithEmoji_StoresCorrectly()
    {
        var interest = new UserInterest(Guid.NewGuid(), TestUserId, "tech 🚀 inovação", TestDate);

        Assert.Equal("tech 🚀 inovação", interest.InterestTag);
    }

    [Fact]
    public void Constructor_PreservesAllGuids()
    {
        var interestId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var createdDate = TestDate.AddHours(3);

        var interest = new UserInterest(interestId, userId, "tag", createdDate);

        Assert.Equal(interestId, interest.Id);
        Assert.Equal(userId, interest.UserId);
        Assert.Equal(createdDate, interest.CreatedAtUtc);
    }
}
