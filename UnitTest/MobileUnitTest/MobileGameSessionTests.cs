using Arcadia_Mobile.Map;
using Arcadia_Mobile.Services;

namespace UnitTest.MobileUnitTest;

public class MobileGameSessionTests
{
    // Checks that the mobile map now includes the full set of rooms from the current console reference map.
    [Fact]
    public void GameMap_Rooms_ContainsFullReferenceRoomCount()
    {
        GameMap map = new();

        Assert.Equal(19, map.Rooms.Count);
        Assert.Equal("Guardian Tower", map.GetRoom(RoomId.GuardiansTower).Name);
        Assert.Equal("The End", map.GetRoom(RoomId.TheEnd).Name);
    }

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

    // Checks that starting a new game resets the mobile session to the starting room.
    [Fact]
    public void StartNewGame_AfterMoving_ReturnsToStartRoom()
    {
        MobileGameSession session = new(new GameMap());
        session.Move(RoomDirection.North);

        session.StartNewGame();

        Assert.Equal(RoomId.MaiaStable, session.CurrentRoom.Id);
    }

    // Checks that room interaction text comes from gameplay room data, not page-only hardcoding.
    [Fact]
    public void Interact_ReturnsCurrentRoomInteractionText()
    {
        MobileGameSession session = new(new GameMap());

        string interactionText = session.Interact();

        Assert.Contains("stable is ready", interactionText);
    }

    // Checks that the western route from Ikena through Road 1 follows the mobile-adapted map graph.
    [Fact]
    public void GameMap_WesternRoute_ConnectsIkenaRoadOneRoadTwoAndOakPass()
    {
        GameMap map = new();
        Room ikena = map.GetRoom(RoomId.Ikena);
        Room road1 = map.GetRoom(RoomId.Road1);
        Room road2 = map.GetRoom(RoomId.Road2);
        Room oakPass = map.GetRoom(RoomId.OakPass);

        Assert.Same(road1, ikena.GetExit(RoomDirection.West));
        Assert.Same(road2, road1.GetExit(RoomDirection.South));
        Assert.Same(oakPass, road2.GetExit(RoomDirection.South));
        Assert.Same(road2, oakPass.GetExit(RoomDirection.North));
    }

    // Checks that the eastern branch from Ikena reaches Wyrmrest and continues toward Nucleon.
    [Fact]
    public void GameMap_EasternBranch_ConnectsIkenaToNucleonPath()
    {
        GameMap map = new();

        Assert.Equal(RoomId.Road6, map.GetRoom(RoomId.Ikena).GetExit(RoomDirection.East)?.Id);
        Assert.Equal(RoomId.Road7, map.GetRoom(RoomId.Road6).GetExit(RoomDirection.South)?.Id);
        Assert.Equal(RoomId.Wyrmrest, map.GetRoom(RoomId.Road7).GetExit(RoomDirection.South)?.Id);
        Assert.Equal(RoomId.Mountains, map.GetRoom(RoomId.Wyrmrest).GetExit(RoomDirection.South)?.Id);
        Assert.Equal(RoomId.RadioactiveWay, map.GetRoom(RoomId.Mountains).GetExit(RoomDirection.South)?.Id);
        Assert.Equal(RoomId.Nucleon, map.GetRoom(RoomId.RadioactiveWay).GetExit(RoomDirection.South)?.Id);
    }

    // Checks that final-route rooms are wired into the mobile map before their gameplay gates are added.
    [Fact]
    public void GameMap_FinalRoute_ConnectsGuardianTowerAndTheEnd()
    {
        GameMap map = new();

        Assert.Equal(RoomId.GuardiansTower, map.GetRoom(RoomId.Road8).GetExit(RoomDirection.North)?.Id);
        Assert.Equal(RoomId.FinalTrials, map.GetRoom(RoomId.GuardiansTower).GetExit(RoomDirection.East)?.Id);
        Assert.Equal(RoomId.Ikena, map.GetRoom(RoomId.GuardiansTower).GetExit(RoomDirection.South)?.Id);
        Assert.Equal(RoomId.TheEnd, map.GetRoom(RoomId.Ikena).GetExit(RoomDirection.North)?.Id);
        Assert.Equal(RoomId.Ikena, map.GetRoom(RoomId.TheEnd).GetExit(RoomDirection.South)?.Id);
    }
}
