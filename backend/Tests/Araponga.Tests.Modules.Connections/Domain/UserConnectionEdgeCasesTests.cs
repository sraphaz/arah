using Araponga.Domain.Connections;
using Xunit;

namespace Araponga.Tests.Modules.Connections.Domain;

public sealed class UserConnectionEdgeCasesTests
{
    private static readonly Guid UserA = Guid.NewGuid();
    private static readonly Guid UserB = Guid.NewGuid();
    private static readonly Guid TerritoryId = Guid.NewGuid();

    [Fact]
    public void CreatePending_WithSameUser_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            UserConnection.CreatePending(Guid.NewGuid(), UserA, UserA, null, DateTime.UtcNow));
    }

    [Fact]
    public void CreatePending_WithDifferentUsers_CreatesPending()
    {
        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var c = UserConnection.CreatePending(id, UserA, UserB, TerritoryId, now);

        Assert.Equal(id, c.Id);
        Assert.Equal(UserA, c.RequesterUserId);
        Assert.Equal(UserB, c.TargetUserId);
        Assert.Equal(ConnectionStatus.Pending, c.Status);
        Assert.Equal(TerritoryId, c.TerritoryId);
        Assert.Equal(now, c.RequestedAtUtc);
        Assert.Null(c.RespondedAtUtc);
        Assert.Null(c.RemovedAtUtc);
    }

    [Fact]
    public void Accept_WhenPending_ChangesToAccepted()
    {
        var c = UserConnection.CreatePending(Guid.NewGuid(), UserA, UserB, null, DateTime.UtcNow);
        var respondedAt = DateTime.UtcNow;

        c.Accept(respondedAt);

        Assert.Equal(ConnectionStatus.Accepted, c.Status);
        Assert.Equal(respondedAt, c.RespondedAtUtc);
    }

    [Fact]
    public void Accept_WhenNotPending_Throws()
    {
        var c = UserConnection.CreatePending(Guid.NewGuid(), UserA, UserB, null, DateTime.UtcNow);
        c.Accept(DateTime.UtcNow);

        Assert.Throws<InvalidOperationException>(() => c.Accept(DateTime.UtcNow));
    }

    [Fact]
    public void Reject_WhenPending_ChangesToRejected()
    {
        var c = UserConnection.CreatePending(Guid.NewGuid(), UserA, UserB, null, DateTime.UtcNow);
        var respondedAt = DateTime.UtcNow;

        c.Reject(respondedAt);

        Assert.Equal(ConnectionStatus.Rejected, c.Status);
        Assert.Equal(respondedAt, c.RespondedAtUtc);
    }

    [Fact]
    public void Remove_WhenAccepted_Succeeds()
    {
        var c = UserConnection.CreatePending(Guid.NewGuid(), UserA, UserB, null, DateTime.UtcNow);
        c.Accept(DateTime.UtcNow);
        var removedAt = DateTime.UtcNow;

        c.Remove(removedAt);

        Assert.Equal(ConnectionStatus.Removed, c.Status);
        Assert.Equal(removedAt, c.RemovedAtUtc);
    }

    [Fact]
    public void Remove_WhenPending_Throws()
    {
        var c = UserConnection.CreatePending(Guid.NewGuid(), UserA, UserB, null, DateTime.UtcNow);

        Assert.Throws<InvalidOperationException>(() => c.Remove(DateTime.UtcNow));
    }

    [Fact]
    public void GetOtherUserId_ReturnsCorrectUser()
    {
        var c = UserConnection.CreatePending(Guid.NewGuid(), UserA, UserB, null, DateTime.UtcNow);
        Assert.Equal(UserB, c.GetOtherUserId(UserA));
        Assert.Equal(UserA, c.GetOtherUserId(UserB));
    }

    [Fact]
    public void GetOtherUserId_WithNonParticipant_Throws()
    {
        var c = UserConnection.CreatePending(Guid.NewGuid(), UserA, UserB, null, DateTime.UtcNow);
        Assert.Throws<ArgumentException>(() => c.GetOtherUserId(Guid.NewGuid()));
    }

    [Fact]
    public void FromPersistence_ReconstitutesAllStatuses()
    {
        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var accepted = UserConnection.FromPersistence(id, UserA, UserB, ConnectionStatus.Accepted, null, now, now, null, now, now);
        Assert.Equal(ConnectionStatus.Accepted, accepted.Status);

        var removed = UserConnection.FromPersistence(id, UserA, UserB, ConnectionStatus.Removed, null, now, now, now, now, now);
        Assert.Equal(ConnectionStatus.Removed, removed.Status);
    }
}
