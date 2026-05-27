using Arcadia_Mobile.Map;
using Arcadia_Mobile.Services;

namespace UnitTest.MobileUnitTest;

public class MobileGameSessionTests
{
    // Checks that the mobile session starts in the same first room used by the prototype map.
    [Fact]
    public void Constructor_StartsAtMaiaStable()
    {
        MobileGameSession session = new(new GameMap());

        Assert.Equal(RoomId.MaiaStable, session.CurrentRoom.Id);
        Assert.Equal("Maia's Stable", session.CurrentRoom.Name);
    }

    // Checks that touch navigation moves through the adapted mobile room graph.
    [Fact]
    public void Move_NorthFromMaiaStable_UpdatesCurrentRoomToIkena()
    {
        MobileGameSession session = new(new GameMap());

        MoveResult result = session.Move(RoomDirection.North);

        Assert.True(result.Moved);
        Assert.Equal(RoomId.Ikena, session.CurrentRoom.Id);
        Assert.Equal("Moved to Ikena.", result.Message);
    }

    // Checks that blocked directions report a message without changing rooms.
    [Fact]
    public void Move_InvalidDirection_DoesNotChangeCurrentRoom()
    {
        MobileGameSession session = new(new GameMap());

        MoveResult result = session.Move(RoomDirection.South);

        Assert.False(result.Moved);
        Assert.Equal(RoomId.MaiaStable, session.CurrentRoom.Id);
        Assert.Equal("You cannot travel that way from here.", result.Message);
    }

    // Checks that room interaction text comes from gameplay room data, not page-only hardcoding.
    [Fact]
    public void Interact_ReturnsCurrentRoomInteractionText()
    {
        MobileGameSession session = new(new GameMap());

        string interactionText = session.Interact();

        Assert.Contains("stable is ready", interactionText);
    }
}
