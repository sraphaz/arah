using Araponga.Domain.Feed;
using Xunit;

namespace Araponga.Tests.Domain;

/// <summary>
/// Comprehensive edge case tests for CommunityPost entity.
/// Tests tag deduplication, editing, visibility, special characters, and boundary conditions.
/// </summary>
public sealed class CommunityPostEdgeCasesTests
{
    private static readonly Guid ValidTerritoryId = Guid.NewGuid();
    private static readonly Guid ValidAuthorId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithValidData_CreatesPost()
    {
        // Act
        var post = new CommunityPost(
            Guid.NewGuid(),
            ValidTerritoryId,
            ValidAuthorId,
            "Test Post",
            "This is test content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);
        
        // Assert
        Assert.NotNull(post);
        Assert.NotEqual(Guid.Empty, post.Id);
    }

    [Fact]
    public void Constructor_WithMultipleTags_DeduplicatesAndNormalizes()
    {
        // Arrange
        var tags = new[] { "Python", "python", "PYTHON", "C#", "c#" };
        
        // Act
        var post = new CommunityPost(
            Guid.NewGuid(),
            ValidTerritoryId,
            ValidAuthorId,
            "Tech Post",
            "Content about tech",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow,
            tags: tags);
        
        // Assert
        Assert.Equal(2, post.Tags.Count);
        Assert.Contains("python", post.Tags);
        Assert.Contains("c#", post.Tags);
    }

    [Fact]
    public void Constructor_WithEmptyTags_RemovesEmpty()
    {
        // Arrange
        var tags = new[] { "valid-tag", "  ", "", "another-tag", "   \t  " };
        
        // Act
        var post = new CommunityPost(
            Guid.NewGuid(),
            ValidTerritoryId,
            ValidAuthorId,
            "Post with Tags",
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow,
            tags: tags);
        
        // Assert
        Assert.Equal(2, post.Tags.Count);
        Assert.Contains("valid-tag", post.Tags);
        Assert.Contains("another-tag", post.Tags);
    }

    [Fact]
    public void Constructor_WithExceeding10Tags_LimitsTo10()
    {
        // Arrange
        var tags = Enumerable.Range(1, 15).Select(i => $"tag{i}").ToArray();
        
        // Act
        var post = new CommunityPost(
            Guid.NewGuid(),
            ValidTerritoryId,
            ValidAuthorId,
            "Post with Many Tags",
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow,
            tags: tags);
        
        // Assert
        Assert.Equal(10, post.Tags.Count);
    }

    [Fact]
    public void Constructor_WithSpecialCharactersInTags_Normalizes()
    {
        // Arrange
        var tags = new[] { "C#", "C++", "Node.JS", "ASP.NET" };
        
        // Act
        var post = new CommunityPost(
            Guid.NewGuid(),
            ValidTerritoryId,
            ValidAuthorId,
            "Code Post",
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow,
            tags: tags);
        
        // Assert
        Assert.Equal(4, post.Tags.Count);
        Assert.Contains("c#", post.Tags);
        Assert.Contains("c++", post.Tags);
        Assert.Contains("node.js", post.Tags);
        Assert.Contains("asp.net", post.Tags);
    }

    [Fact]
    public void Constructor_WithNullTags_CreatesEmptyList()
    {
        // Act
        var post = new CommunityPost(
            Guid.NewGuid(),
            ValidTerritoryId,
            ValidAuthorId,
            "No Tags Post",
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow,
            tags: null);
        
        // Assert
        Assert.Empty(post.Tags);
    }

    [Fact]
    public void Constructor_WithTitleContainingSpecialChars_TrimsSuccessfully()
    {
        // Arrange
        var title = "  🎨 Design Tips for UI/UX Developers 🚀  ";
        
        // Act
        var post = new CommunityPost(
            Guid.NewGuid(),
            ValidTerritoryId,
            ValidAuthorId,
            title,
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);
        
        // Assert
        Assert.Equal("🎨 Design Tips for UI/UX Developers 🚀", post.Title);
    }

    [Fact]
    public void Constructor_WithLongContent_Succeeds()
    {
        // Arrange
        var longContent = new string('A', 5000);
        
        // Act
        var post = new CommunityPost(
            Guid.NewGuid(),
            ValidTerritoryId,
            ValidAuthorId,
            "Long Post",
            longContent,
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);
        
        // Assert
        Assert.Equal(longContent, post.Content);
    }

    [Fact]
    public void Constructor_WithMultilineContent_PreservesFormatting()
    {
        // Arrange
        var multilineContent = "Line 1\n\nLine 3 with paragraph\n- Item 1\n- Item 2";
        
        // Act
        var post = new CommunityPost(
            Guid.NewGuid(),
            ValidTerritoryId,
            ValidAuthorId,
            "Formatted Post",
            multilineContent,
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);
        
        // Assert
        Assert.Equal(multilineContent, post.Content);
    }

    [Fact]
    public void Constructor_WithUnicodeContent_Succeeds()
    {
        // Arrange
        var unicodeContent = "José wrote about São Paulo's 🏙️ architecture. 北京很大。";
        
        // Act
        var post = new CommunityPost(
            Guid.NewGuid(),
            ValidTerritoryId,
            ValidAuthorId,
            "International Post",
            unicodeContent,
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);
        
        // Assert
        Assert.Equal(unicodeContent, post.Content);
    }

    [Fact]
    public void Constructor_WithEmptyTerritoryId_ThrowsException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new CommunityPost(
                Guid.NewGuid(),
                Guid.Empty,
                ValidAuthorId,
                "Title",
                "Content",
                PostType.General,
                PostVisibility.Public,
                PostStatus.Published,
                null,
                DateTime.UtcNow));
        
        Assert.Contains("Territory", ex.Message);
    }

    [Fact]
    public void Constructor_WithEmptyAuthorId_ThrowsException()
    {
        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() =>
            new CommunityPost(
                Guid.NewGuid(),
                ValidTerritoryId,
                Guid.Empty,
                "Title",
                "Content",
                PostType.General,
                PostVisibility.Public,
                PostStatus.Published,
                null,
                DateTime.UtcNow));
        
        Assert.Contains("Author", ex.Message);
    }

    [Fact]
    public void Constructor_WithAllPostTypes_Succeeds()
    {
        // Test all post types
        var postTypes = new[] { PostType.General, PostType.Alert };
        
        foreach (var postType in postTypes)
        {
            // Act
            var post = new CommunityPost(
                Guid.NewGuid(),
                ValidTerritoryId,
                ValidAuthorId,
                "Test Post",
                "Content",
                postType,
                PostVisibility.Public,
                PostStatus.Published,
                null,
                DateTime.UtcNow);
            
            // Assert
            Assert.Equal(postType, post.Type);
        }
    }

    [Fact]
    public void Constructor_WithAllVisibilities_Succeeds()
    {
        // Test all visibilities
        var visibilities = new[] { PostVisibility.Public, PostVisibility.ResidentsOnly };
        
        foreach (var visibility in visibilities)
        {
            // Act
            var post = new CommunityPost(
                Guid.NewGuid(),
                ValidTerritoryId,
                ValidAuthorId,
                "Test Post",
                "Content",
                PostType.General,
                visibility,
                PostStatus.Published,
                null,
                DateTime.UtcNow);
            
            // Assert
            Assert.Equal(visibility, post.Visibility);
        }
    }

    [Fact]
    public void Constructor_WithAllStatuses_Succeeds()
    {
        // Test all statuses
        var statuses = new[] { PostStatus.Published, PostStatus.PendingApproval, PostStatus.Rejected, PostStatus.Hidden };
        
        foreach (var status in statuses)
        {
            // Act
            var post = new CommunityPost(
                Guid.NewGuid(),
                ValidTerritoryId,
                ValidAuthorId,
                "Test Post",
                "Content",
                PostType.General,
                PostVisibility.Public,
                status,
                null,
                DateTime.UtcNow);
            
            // Assert
            Assert.Equal(status, post.Status);
        }
    }

    [Fact]
    public void Edit_UpdatesTitleAndContent_UpdatesTimestampAndCount()
    {
        // Arrange
        var post = new CommunityPost(
            Guid.NewGuid(),
            ValidTerritoryId,
            ValidAuthorId,
            "Original Title",
            "Original content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);
        
        var newTitle = "Updated Title";
        var newContent = "Updated content with more details";
        
        // Act
        post.Edit(newTitle, newContent);
        
        // Assert
        Assert.Equal(newTitle, post.Title);
        Assert.Equal(newContent, post.Content);
        Assert.Equal(1, post.EditCount);
        Assert.NotNull(post.EditedAtUtc);
        Assert.True(post.EditedAtUtc > post.CreatedAtUtc);
    }

    [Fact]
    public void Edit_WithNewTags_UpdatesTags()
    {
        // Arrange
        var originalTags = new[] { "old-tag" };
        var post = new CommunityPost(
            Guid.NewGuid(),
            ValidTerritoryId,
            ValidAuthorId,
            "Post",
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow,
            tags: originalTags);
        
        var newTags = new[] { "new-tag-1", "new-tag-2" };
        
        // Act
        post.Edit("Title", "Content", newTags);
        
        // Assert
        Assert.Equal(2, post.Tags.Count);
        Assert.Contains("new-tag-1", post.Tags);
        Assert.Contains("new-tag-2", post.Tags);
        Assert.DoesNotContain("old-tag", post.Tags);
    }

    [Fact]
    public void Edit_MultipleTimesIncrementEditCount()
    {
        // Arrange
        var post = new CommunityPost(
            Guid.NewGuid(),
            ValidTerritoryId,
            ValidAuthorId,
            "Title",
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);
        
        // Act & Assert
        post.Edit("Edit 1", "Content 1");
        Assert.Equal(1, post.EditCount);
        
        post.Edit("Edit 2", "Content 2");
        Assert.Equal(2, post.EditCount);
        
        post.Edit("Edit 3", "Content 3");
        Assert.Equal(3, post.EditCount);
    }

    [Fact]
    public void Edit_WithTrimmableTitle_TrimsSuccessfully()
    {
        // Arrange
        var post = new CommunityPost(
            Guid.NewGuid(),
            ValidTerritoryId,
            ValidAuthorId,
            "Title",
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow);
        
        var newTitle = "   Updated Title with Spaces   ";
        
        // Act
        post.Edit(newTitle, "Content");
        
        // Assert
        Assert.Equal("Updated Title with Spaces", post.Title);
    }

    [Fact]
    public void Edit_WithoutProvidingTags_PreservesTags()
    {
        // Arrange
        var originalTags = new[] { "python", "coding" };
        var post = new CommunityPost(
            Guid.NewGuid(),
            ValidTerritoryId,
            ValidAuthorId,
            "Post",
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow,
            tags: originalTags);
        
        // Act
        post.Edit("New Title", "New content", null);
        
        // Assert
        Assert.Equal(2, post.Tags.Count);
        Assert.Contains("python", post.Tags);
        Assert.Contains("coding", post.Tags);
    }

    [Fact]
    public void Constructor_WithReference_StoresReference()
    {
        // Arrange
        var referenceId = Guid.NewGuid();
        
        // Act
        var post = new CommunityPost(
            Guid.NewGuid(),
            ValidTerritoryId,
            ValidAuthorId,
            "Referenced Post",
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow,
            referenceType: "comment",
            referenceId: referenceId);
        
        // Assert
        Assert.Equal("comment", post.ReferenceType);
        Assert.Equal(referenceId, post.ReferenceId);
    }

    [Fact]
    public void Constructor_WithMapEntity_StoresMapEntity()
    {
        // Arrange
        var mapEntityId = Guid.NewGuid();
        
        // Act
        var post = new CommunityPost(
            Guid.NewGuid(),
            ValidTerritoryId,
            ValidAuthorId,
            "Map Post",
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            mapEntityId,
            DateTime.UtcNow);
        
        // Assert
        Assert.Equal(mapEntityId, post.MapEntityId);
    }

    [Fact]
    public void Constructor_WithEditedAtUtc_PreservesEditTimestamp()
    {
        // Arrange
        var createdAt = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var editedAt = new DateTime(2024, 1, 2, 15, 30, 0, DateTimeKind.Utc);
        
        // Act
        var post = new CommunityPost(
            Guid.NewGuid(),
            ValidTerritoryId,
            ValidAuthorId,
            "Title",
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            createdAt,
            editedAtUtc: editedAt);
        
        // Assert
        Assert.Equal(editedAt, post.EditedAtUtc);
    }

    [Fact]
    public void Constructor_WithEditCount_PreservesEditCount()
    {
        // Arrange
        var editCount = 5;
        
        // Act
        var post = new CommunityPost(
            Guid.NewGuid(),
            ValidTerritoryId,
            ValidAuthorId,
            "Title",
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow,
            editCount: editCount);
        
        // Assert
        Assert.Equal(5, post.EditCount);
    }

    [Fact]
    public void Constructor_WithLargeEditCount_PreservesEditCount()
    {
        // Arrange
        var editCount = 1000;
        
        // Act
        var post = new CommunityPost(
            Guid.NewGuid(),
            ValidTerritoryId,
            ValidAuthorId,
            "Title",
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow,
            editCount: editCount);
        
        // Assert
        Assert.Equal(1000, post.EditCount);
    }

    [Fact]
    public void Edit_WhenEditCountAtMax_DoesNotIncrement()
    {
        // Arrange
        var post = new CommunityPost(
            Guid.NewGuid(),
            ValidTerritoryId,
            ValidAuthorId,
            "Title",
            "Content",
            PostType.General,
            PostVisibility.Public,
            PostStatus.Published,
            null,
            DateTime.UtcNow,
            editCount: int.MaxValue);
        
        // Act
        post.Edit("New Title", "New Content");
        
        // Assert
        Assert.Equal(int.MaxValue, post.EditCount);
    }
}
