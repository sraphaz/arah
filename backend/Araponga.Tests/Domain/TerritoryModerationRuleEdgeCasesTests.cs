using System.Text.Json;
using Araponga.Domain.Governance;
using Xunit;

namespace Araponga.Tests.Domain;

/// <summary>
/// Edge case tests for the TerritoryModerationRule domain entity, focusing on
/// JSON validation, rule lifecycle, and configuration management.
/// </summary>
public class TerritoryModerationRuleEdgeCasesTests
{
    private static readonly Guid TestTerritoryId = Guid.NewGuid();
    private static readonly DateTime TestDate = DateTime.UtcNow;

    // Constructor validation tests
    [Fact]
    public void Constructor_WithValidData_CreatesSuccessfully()
    {
        var ruleId = Guid.NewGuid();
        var ruleJson = JsonSerializer.Serialize(new { allowedContentTypes = new[] { "post", "image" } });

        var rule = new TerritoryModerationRule(
            ruleId,
            TestTerritoryId,
            null,
            RuleType.ContentType,
            ruleJson,
            true,
            TestDate,
            TestDate);

        Assert.Equal(ruleId, rule.Id);
        Assert.True(rule.IsActive);
        Assert.Null(rule.CreatedByVotingId);
    }

    [Fact]
    public void Constructor_WithNullRuleJson_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new TerritoryModerationRule(
                Guid.NewGuid(),
                TestTerritoryId,
                null,
                RuleType.ContentType,
                null!,
                true,
                TestDate,
                TestDate));
    }

    [Fact]
    public void Constructor_WithEmptyRuleJson_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            new TerritoryModerationRule(
                Guid.NewGuid(),
                TestTerritoryId,
                null,
                RuleType.ContentType,
                "   ",
                true,
                TestDate,
                TestDate));
    }

    [Fact]
    public void Constructor_WithInvalidJsonRuleJson_ThrowsArgumentException()
    {
        var invalidJson = "{ not valid json }";

        Assert.Throws<ArgumentException>(() =>
            new TerritoryModerationRule(
                Guid.NewGuid(),
                TestTerritoryId,
                null,
                RuleType.ContentType,
                invalidJson,
                true,
                TestDate,
                TestDate));
    }

    [Fact]
    public void Constructor_WithValidComplexJson_CreatesSuccessfully()
    {
        var complexJson = JsonSerializer.Serialize(new
        {
            prohibitedWords = new[] { "spam", "abuse" },
            severity = "high",
            autoModerate = true,
            exceptions = new[] { "moderator", "admin" }
        });

        var rule = new TerritoryModerationRule(
            Guid.NewGuid(),
            TestTerritoryId,
            null,
            RuleType.ProhibitedWords,
            complexJson,
            true,
            TestDate,
            TestDate);

        Assert.NotNull(rule.RuleJson);
        Assert.Contains("prohibitedWords", rule.RuleJson);
    }

    [Fact]
    public void Constructor_WithEmptyJsonObject_CreatesSuccessfully()
    {
        var emptyJson = JsonSerializer.Serialize(new { });

        var rule = new TerritoryModerationRule(
            Guid.NewGuid(),
            TestTerritoryId,
            null,
            RuleType.ContentType,
            emptyJson,
            true,
            TestDate,
            TestDate);

        Assert.NotNull(rule.RuleJson);
    }

    [Fact]
    public void Constructor_WithJsonArray_CreatesSuccessfully()
    {
        var arrayJson = JsonSerializer.Serialize(new[] { "item1", "item2", "item3" });

        var rule = new TerritoryModerationRule(
            Guid.NewGuid(),
            TestTerritoryId,
            null,
            RuleType.ProhibitedWords,
            arrayJson,
            true,
            TestDate,
            TestDate);

        Assert.NotNull(rule.RuleJson);
    }

    // RuleType coverage tests
    [Fact]
    public void Constructor_WithAllRuleTypes_CreatesSuccessfully()
    {
        var ruleTypes = new[]
        {
            RuleType.ContentType,
            RuleType.ProhibitedWords,
            RuleType.Behavior,
            RuleType.MarketplacePolicy,
            RuleType.EventPolicy
        };

        var ruleJson = JsonSerializer.Serialize(new { });

        foreach (var ruleType in ruleTypes)
        {
            var rule = new TerritoryModerationRule(
                Guid.NewGuid(),
                TestTerritoryId,
                null,
                ruleType,
                ruleJson,
                true,
                TestDate,
                TestDate);

            Assert.Equal(ruleType, rule.RuleType);
        }
    }

    // CreatedByVotingId tests
    [Fact]
    public void Constructor_WithVotingId_StoresCorrectly()
    {
        var votingId = Guid.NewGuid();
        var ruleJson = JsonSerializer.Serialize(new { });

        var rule = new TerritoryModerationRule(
            Guid.NewGuid(),
            TestTerritoryId,
            votingId,
            RuleType.ContentType,
            ruleJson,
            true,
            TestDate,
            TestDate);

        Assert.Equal(votingId, rule.CreatedByVotingId);
    }

    [Fact]
    public void Constructor_WithoutVotingId_AllowsNull()
    {
        var ruleJson = JsonSerializer.Serialize(new { });

        var rule = new TerritoryModerationRule(
            Guid.NewGuid(),
            TestTerritoryId,
            null,
            RuleType.ContentType,
            ruleJson,
            true,
            TestDate,
            TestDate);

        Assert.Null(rule.CreatedByVotingId);
    }

    // Activation/Deactivation tests
    [Fact]
    public void Activate_FromInactiveState_ActivatesSuccessfully()
    {
        var rule = CreateInactiveRule();

        rule.Activate();

        Assert.True(rule.IsActive);
    }

    [Fact]
    public void Activate_FromActiveState_RemainActive()
    {
        var rule = CreateActiveRule();
        var wasActive = rule.IsActive;

        rule.Activate();

        Assert.True(rule.IsActive);
        Assert.Equal(wasActive, rule.IsActive);
    }

    [Fact]
    public void Deactivate_FromActiveState_DeactivatesSuccessfully()
    {
        var rule = CreateActiveRule();

        rule.Deactivate();

        Assert.False(rule.IsActive);
    }

    [Fact]
    public void Deactivate_FromInactiveState_RemainInactive()
    {
        var rule = CreateInactiveRule();

        rule.Deactivate();

        Assert.False(rule.IsActive);
    }

    // UpdateRule tests
    [Fact]
    public void UpdateRule_WithValidJson_UpdatesSuccessfully()
    {
        var rule = CreateActiveRule();
        var newJson = JsonSerializer.Serialize(new { updated = true, timestamp = DateTime.UtcNow });

        rule.UpdateRule(newJson);

        Assert.Equal(newJson, rule.RuleJson);
    }

    [Fact]
    public void UpdateRule_WithNullJson_ThrowsArgumentException()
    {
        var rule = CreateActiveRule();

        Assert.Throws<ArgumentException>(() => rule.UpdateRule(null!));
    }

    [Fact]
    public void UpdateRule_WithEmptyJson_ThrowsArgumentException()
    {
        var rule = CreateActiveRule();

        Assert.Throws<ArgumentException>(() => rule.UpdateRule("   "));
    }

    [Fact]
    public void UpdateRule_WithInvalidJson_ThrowsArgumentException()
    {
        var rule = CreateActiveRule();
        var invalidJson = "{ invalid json format ]";

        Assert.Throws<ArgumentException>(() => rule.UpdateRule(invalidJson));
    }

    [Fact]
    public void UpdateRule_WithComplexJson_UpdatesSuccessfully()
    {
        var rule = CreateActiveRule();
        var complexJson = JsonSerializer.Serialize(new
        {
            version = 2,
            rules = new[] { "rule1", "rule2", "rule3" },
            metadata = new { author = "curator", approved = true },
            conditions = new[] { new { field = "content", @operator = "contains", value = "spam" } }
        });

        rule.UpdateRule(complexJson);

        Assert.Equal(complexJson, rule.RuleJson);
    }

    // Timestamp update tests
    [Fact]
    public void Activate_UpdatesTimestamp()
    {
        var rule = CreateInactiveRule();
        var originalTimestamp = rule.UpdatedAtUtc;

        System.Threading.Thread.Sleep(10);
        rule.Activate();

        Assert.True(rule.UpdatedAtUtc > originalTimestamp);
    }

    [Fact]
    public void Deactivate_UpdatesTimestamp()
    {
        var rule = CreateActiveRule();
        var originalTimestamp = rule.UpdatedAtUtc;

        System.Threading.Thread.Sleep(10);
        rule.Deactivate();

        Assert.True(rule.UpdatedAtUtc > originalTimestamp);
    }

    [Fact]
    public void UpdateRule_UpdatesTimestamp()
    {
        var rule = CreateActiveRule();
        var originalTimestamp = rule.UpdatedAtUtc;

        System.Threading.Thread.Sleep(10);
        rule.UpdateRule(JsonSerializer.Serialize(new { updated = true }));

        Assert.True(rule.UpdatedAtUtc > originalTimestamp);
    }

    // Large JSON tests
    [Fact]
    public void Constructor_WithLargeJson_CreatesSuccessfully()
    {
        var largeList = Enumerable.Range(1, 1000).Select(i => $"word{i}").ToList();
        var largeJson = JsonSerializer.Serialize(new { prohibitedWords = largeList });

        var rule = new TerritoryModerationRule(
            Guid.NewGuid(),
            TestTerritoryId,
            null,
            RuleType.ProhibitedWords,
            largeJson,
            true,
            TestDate,
            TestDate);

        Assert.NotNull(rule.RuleJson);
        Assert.Contains("word1000", rule.RuleJson);
    }

    [Fact]
    public void UpdateRule_WithLargeJson_UpdatesSuccessfully()
    {
        var rule = CreateActiveRule();
        var largeList = Enumerable.Range(1, 1000).Select(i => $"word{i}").ToList();
        var largeJson = JsonSerializer.Serialize(new { prohibitedWords = largeList });

        rule.UpdateRule(largeJson);

        Assert.Contains("word1000", rule.RuleJson);
    }

    // Unicode and special characters in JSON
    [Fact]
    public void Constructor_WithUnicodeInJson_StoresCorrectly()
    {
        var unicodeJson = JsonSerializer.Serialize(new
        {
            description = "Regra para país: Português (Portugal), Français (France), Español (España)",
            keywords = new[] { "café", "naïve", "résumé", "文字" }
        });

        var rule = new TerritoryModerationRule(
            Guid.NewGuid(),
            TestTerritoryId,
            null,
            RuleType.Behavior,
            unicodeJson,
            true,
            TestDate,
            TestDate);

        // JSON serialization escapes Unicode, so we verify by deserializing
        var deserialized = JsonSerializer.Deserialize<JsonElement>(rule.RuleJson);
        Assert.True(deserialized.TryGetProperty("description", out var desc));
        Assert.Contains("país", desc.GetString()!);
        Assert.True(deserialized.TryGetProperty("keywords", out var keywords));
        var keywordsArray = keywords.EnumerateArray().Select(k => k.GetString()).ToList();
        Assert.Contains("café", keywordsArray);
        Assert.Contains("文字", keywordsArray);
    }

    [Fact]
    public void Constructor_WithEmojiInJson_StoresCorrectly()
    {
        var emojiJson = JsonSerializer.Serialize(new
        {
            restriction = "No spam 🚫, be nice 😊, respect community 🤝",
            icons = new[] { "🎯", "⚡", "✅", "❌" }
        });

        var rule = new TerritoryModerationRule(
            Guid.NewGuid(),
            TestTerritoryId,
            null,
            RuleType.EventPolicy,
            emojiJson,
            true,
            TestDate,
            TestDate);

        // JSON serialization escapes emojis, so we verify by deserializing
        var deserialized = JsonSerializer.Deserialize<JsonElement>(rule.RuleJson);
        Assert.True(deserialized.TryGetProperty("restriction", out var restriction));
        Assert.Contains("🚫", restriction.GetString()!);
        Assert.True(deserialized.TryGetProperty("icons", out var icons));
        var iconsArray = icons.EnumerateArray().Select(i => i.GetString()).ToList();
        Assert.Contains("🎯", iconsArray);
        Assert.Contains("✅", iconsArray);
    }

    // Multiple updates sequence
    [Fact]
    public void MultipleOperations_MaintainIntegrity()
    {
        var rule = CreateActiveRule();
        var initialJson = rule.RuleJson;

        rule.Deactivate();
        Assert.False(rule.IsActive);

        var newJson = JsonSerializer.Serialize(new { version = 2 });
        rule.UpdateRule(newJson);
        Assert.Equal(newJson, rule.RuleJson);
        Assert.False(rule.IsActive);

        rule.Activate();
        Assert.True(rule.IsActive);
        Assert.Equal(newJson, rule.RuleJson);
    }

    // Helper methods
    private static TerritoryModerationRule CreateActiveRule()
    {
        var ruleJson = JsonSerializer.Serialize(new { isActive = true });
        return new TerritoryModerationRule(
            Guid.NewGuid(),
            TestTerritoryId,
            null,
            RuleType.ContentType,
            ruleJson,
            true,
            TestDate,
            TestDate);
    }

    private static TerritoryModerationRule CreateInactiveRule()
    {
        var ruleJson = JsonSerializer.Serialize(new { isActive = false });
        return new TerritoryModerationRule(
            Guid.NewGuid(),
            TestTerritoryId,
            null,
            RuleType.ContentType,
            ruleJson,
            false,
            TestDate,
            TestDate);
    }
}
