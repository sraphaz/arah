using Araponga.Domain.Chat;
using Xunit;

namespace Araponga.Tests.Domain.Chat;

/// <summary>
/// Edge case tests for ChatConversation and ChatMessage domain entities,
/// focusing on empty/null messages, Unicode, invalid timestamps, and status transitions.
/// </summary>
public class ChatEdgeCasesTests
{
    private static readonly DateTime TestDate = DateTime.UtcNow;
    private static readonly Guid TestTerritoryId = Guid.NewGuid();
    private static readonly Guid TestUserId = Guid.NewGuid();

    // ChatConversation edge cases
    [Fact]
    public void ChatConversation_Constructor_WithValidData_CreatesSuccessfully()
    {
        var conversation = new ChatConversation(
            Guid.NewGuid(),
            ConversationKind.TerritoryPublic,
            ConversationStatus.Active,
            TestTerritoryId,
            "Test Conversation",
            TestUserId,
            TestDate,
            null,
            null,
            null,
            null,
            null,
            null);

        Assert.Equal("Test Conversation", conversation.Name);
        Assert.Equal(ConversationStatus.Active, conversation.Status);
    }

    [Fact]
    public void ChatConversation_Constructor_WithEmptyId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ChatConversation(
            Guid.Empty,
            ConversationKind.TerritoryPublic,
            ConversationStatus.Active,
            TestTerritoryId,
            "Test",
            TestUserId,
            TestDate,
            null,
            null,
            null,
            null,
            null,
            null));
    }

    [Fact]
    public void ChatConversation_Constructor_WithEmptyCreatedByUserId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ChatConversation(
            Guid.NewGuid(),
            ConversationKind.TerritoryPublic,
            ConversationStatus.Active,
            TestTerritoryId,
            "Test",
            Guid.Empty,
            TestDate,
            null,
            null,
            null,
            null,
            null,
            null));
    }

    [Fact]
    public void ChatConversation_Constructor_WithTerritoryKindButNullTerritoryId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ChatConversation(
            Guid.NewGuid(),
            ConversationKind.TerritoryPublic,
            ConversationStatus.Active,
            null,
            "Test",
            TestUserId,
            TestDate,
            null,
            null,
            null,
            null,
            null,
            null));
    }

    [Fact]
    public void ChatConversation_Constructor_WithDirectKindButNotNullTerritoryId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ChatConversation(
            Guid.NewGuid(),
            ConversationKind.Direct,
            ConversationStatus.Active,
            TestTerritoryId,
            null,
            TestUserId,
            TestDate,
            null,
            null,
            null,
            null,
            null,
            null));
    }

    [Fact]
    public void ChatConversation_Constructor_WithNameExceedingMaxLength_ThrowsArgumentException()
    {
        var longName = new string('A', ChatConversation.MaxNameLength + 1);

        Assert.Throws<ArgumentException>(() => new ChatConversation(
            Guid.NewGuid(),
            ConversationKind.TerritoryPublic,
            ConversationStatus.Active,
            TestTerritoryId,
            longName,
            TestUserId,
            TestDate,
            null,
            null,
            null,
            null,
            null,
            null));
    }

    [Fact]
    public void ChatConversation_Constructor_WithNameExactlyMaxLength_CreatesSuccessfully()
    {
        var exactName = new string('A', ChatConversation.MaxNameLength);
        var conversation = new ChatConversation(
            Guid.NewGuid(),
            ConversationKind.TerritoryPublic,
            ConversationStatus.Active,
            TestTerritoryId,
            exactName,
            TestUserId,
            TestDate,
            null,
            null,
            null,
            null,
            null,
            null);

        Assert.Equal(exactName, conversation.Name);
    }

    [Fact]
    public void ChatConversation_Constructor_WithUnicodeName_TrimsAndStores()
    {
        var conversation = new ChatConversation(
            Guid.NewGuid(),
            ConversationKind.TerritoryPublic,
            ConversationStatus.Active,
            TestTerritoryId,
            "  Conversa Café & Cia 🎉  ",
            TestUserId,
            TestDate,
            null,
            null,
            null,
            null,
            null,
            null);

        Assert.Equal("Conversa Café & Cia 🎉", conversation.Name);
    }

    [Fact]
    public void ChatConversation_Rename_WithNameExceedingMaxLength_ThrowsArgumentException()
    {
        var conversation = new ChatConversation(
            Guid.NewGuid(),
            ConversationKind.TerritoryPublic,
            ConversationStatus.Active,
            TestTerritoryId,
            "Original",
            TestUserId,
            TestDate,
            null,
            null,
            null,
            null,
            null,
            null);

        var longName = new string('A', ChatConversation.MaxNameLength + 1);

        Assert.Throws<ArgumentException>(() => conversation.Rename(longName));
    }

    [Fact]
    public void ChatConversation_Rename_WithNullName_ThrowsArgumentException()
    {
        var conversation = new ChatConversation(
            Guid.NewGuid(),
            ConversationKind.TerritoryPublic,
            ConversationStatus.Active,
            TestTerritoryId,
            "Original",
            TestUserId,
            TestDate,
            null,
            null,
            null,
            null,
            null,
            null);

        Assert.Throws<ArgumentException>(() => conversation.Rename(null!));
    }

    [Fact]
    public void ChatConversation_Approve_WithEmptyApprovedByUserId_ThrowsArgumentException()
    {
        var conversation = new ChatConversation(
            Guid.NewGuid(),
            ConversationKind.Group,
            ConversationStatus.PendingApproval,
            TestTerritoryId,
            "Group",
            TestUserId,
            TestDate,
            null,
            null,
            null,
            null,
            null,
            null);

        Assert.Throws<ArgumentException>(() => conversation.Approve(Guid.Empty, TestDate.AddHours(1)));
    }

    [Fact]
    public void ChatConversation_Approve_WhenNotPendingApproval_ThrowsInvalidOperationException()
    {
        var conversation = new ChatConversation(
            Guid.NewGuid(),
            ConversationKind.Group,
            ConversationStatus.Active,
            TestTerritoryId,
            "Group",
            TestUserId,
            TestDate,
            TestUserId,
            TestDate,
            null,
            null,
            null,
            null);

        Assert.Throws<InvalidOperationException>(() => conversation.Approve(TestUserId, TestDate.AddHours(1)));
    }

    [Fact]
    public void ChatConversation_Approve_WhenNotGroup_ThrowsInvalidOperationException()
    {
        var conversation = new ChatConversation(
            Guid.NewGuid(),
            ConversationKind.TerritoryPublic,
            ConversationStatus.PendingApproval,
            TestTerritoryId,
            "Public",
            TestUserId,
            TestDate,
            null,
            null,
            null,
            null,
            null,
            null);

        Assert.Throws<InvalidOperationException>(() => conversation.Approve(TestUserId, TestDate.AddHours(1)));
    }

    [Fact]
    public void ChatConversation_Lock_WhenDisabled_ThrowsInvalidOperationException()
    {
        var conversation = new ChatConversation(
            Guid.NewGuid(),
            ConversationKind.TerritoryPublic,
            ConversationStatus.Disabled,
            TestTerritoryId,
            "Test",
            TestUserId,
            TestDate,
            null,
            null,
            null,
            null,
            TestDate,
            TestUserId);

        Assert.Throws<InvalidOperationException>(() => conversation.Lock(TestUserId, TestDate.AddHours(1)));
    }

    [Fact]
    public void ChatConversation_Unlock_WhenNotLocked_ThrowsInvalidOperationException()
    {
        var conversation = new ChatConversation(
            Guid.NewGuid(),
            ConversationKind.TerritoryPublic,
            ConversationStatus.Active,
            TestTerritoryId,
            "Test",
            TestUserId,
            TestDate,
            null,
            null,
            null,
            null,
            null,
            null);

        Assert.Throws<InvalidOperationException>(() => conversation.Unlock());
    }

    [Fact]
    public void ChatConversation_Disable_WhenAlreadyDisabled_DoesNotThrow()
    {
        var conversation = new ChatConversation(
            Guid.NewGuid(),
            ConversationKind.TerritoryPublic,
            ConversationStatus.Disabled,
            TestTerritoryId,
            "Test",
            TestUserId,
            TestDate,
            null,
            null,
            null,
            null,
            TestDate,
            TestUserId);

        conversation.Disable(TestUserId, TestDate.AddHours(1));

        // Should not throw
        Assert.True(true);
    }

    // ChatMessage edge cases
    [Fact]
    public void ChatMessage_Constructor_WithValidTextMessage_CreatesSuccessfully()
    {
        var message = new ChatMessage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            MessageContentType.Text,
            "Hello World",
            null,
            TestDate,
            null,
            null,
            null);

        Assert.Equal("Hello World", message.Text);
        Assert.Equal(MessageContentType.Text, message.ContentType);
    }

    [Fact]
    public void ChatMessage_Constructor_WithEmptyId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ChatMessage(
            Guid.Empty,
            Guid.NewGuid(),
            TestUserId,
            MessageContentType.Text,
            "Hello",
            null,
            TestDate,
            null,
            null,
            null));
    }

    [Fact]
    public void ChatMessage_Constructor_WithEmptyConversationId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ChatMessage(
            Guid.NewGuid(),
            Guid.Empty,
            TestUserId,
            MessageContentType.Text,
            "Hello",
            null,
            TestDate,
            null,
            null,
            null));
    }

    [Fact]
    public void ChatMessage_Constructor_WithEmptySenderUserId_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ChatMessage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.Empty,
            MessageContentType.Text,
            "Hello",
            null,
            TestDate,
            null,
            null,
            null));
    }

    [Fact]
    public void ChatMessage_Constructor_WithTextTypeButNullText_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ChatMessage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            MessageContentType.Text,
            null!,
            null,
            TestDate,
            null,
            null,
            null));
    }

    [Fact]
    public void ChatMessage_Constructor_WithTextTypeButEmptyText_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new ChatMessage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            MessageContentType.Text,
            "   ",
            null,
            TestDate,
            null,
            null,
            null));
    }

    [Fact]
    public void ChatMessage_Constructor_WithTextExceedingMaxLength_ThrowsArgumentException()
    {
        var longText = new string('A', ChatMessage.MaxTextLength + 1);

        Assert.Throws<ArgumentException>(() => new ChatMessage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            MessageContentType.Text,
            longText,
            null,
            TestDate,
            null,
            null,
            null));
    }

    [Fact]
    public void ChatMessage_Constructor_WithTextExactlyMaxLength_CreatesSuccessfully()
    {
        var exactText = new string('A', ChatMessage.MaxTextLength);
        var message = new ChatMessage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            MessageContentType.Text,
            exactText,
            null,
            TestDate,
            null,
            null,
            null);

        Assert.Equal(exactText, message.Text);
    }

    [Fact]
    public void ChatMessage_Constructor_WithUnicodeText_TrimsAndStores()
    {
        var message = new ChatMessage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            MessageContentType.Text,
            "  Mensagem com café, naïve, résumé, 文字 e emoji 🎉  ",
            null,
            TestDate,
            null,
            null,
            null);

        Assert.Contains("café", message.Text!);
        Assert.Contains("文字", message.Text!);
        Assert.Contains("🎉", message.Text!);
    }

    [Fact]
    public void ChatMessage_Constructor_WithPayloadExceedingMaxLength_ThrowsArgumentException()
    {
        var longPayload = new string('A', ChatMessage.MaxPayloadLength + 1);

        Assert.Throws<ArgumentException>(() => new ChatMessage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            MessageContentType.System,
            null,
            longPayload,
            TestDate,
            null,
            null,
            null));
    }

    [Fact]
    public void ChatMessage_Edit_WithNonTextContentType_ThrowsInvalidOperationException()
    {
        var message = new ChatMessage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            MessageContentType.System,
            null,
            "{}",
            TestDate,
            null,
            null,
            null);

        Assert.Throws<InvalidOperationException>(() => message.Edit("New text", TestDate.AddHours(1)));
    }

    [Fact]
    public void ChatMessage_Edit_WhenDeleted_ThrowsInvalidOperationException()
    {
        var message = new ChatMessage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            MessageContentType.Text,
            "Original",
            null,
            TestDate,
            null,
            TestDate.AddHours(1),
            TestUserId);

        Assert.Throws<InvalidOperationException>(() => message.Edit("New text", TestDate.AddHours(2)));
    }

    [Fact]
    public void ChatMessage_Delete_WhenAlreadyDeleted_DoesNotThrow()
    {
        var message = new ChatMessage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            MessageContentType.Text,
            "Original",
            null,
            TestDate,
            null,
            TestDate.AddHours(1),
            TestUserId);

        message.Delete(TestUserId, TestDate.AddHours(2));

        // Should not throw
        Assert.True(true);
    }

    [Fact]
    public void ChatMessage_Delete_WithEmptyDeletedByUserId_ThrowsArgumentException()
    {
        var message = new ChatMessage(
            Guid.NewGuid(),
            Guid.NewGuid(),
            TestUserId,
            MessageContentType.Text,
            "Original",
            null,
            TestDate,
            null,
            null,
            null);

        Assert.Throws<ArgumentException>(() => message.Delete(Guid.Empty, TestDate.AddHours(1)));
    }
}
