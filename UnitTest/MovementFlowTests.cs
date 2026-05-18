using Arcadia_v2;
using Arcadia_v2.Commands;

namespace UnitTest;

public class MovementFlowTests
{
    // Checks that the Road 6 gate opens once the player has the required three badges.
    [Fact]
    public void HandleMovement_FromIkenaToRoadSix_WithThreeBadges_AllowsMovement()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.AddBadge("Grass Badge");
        gameState.MainPlayer.AddBadge("Water Badge");
        gameState.MainPlayer.AddBadge("Rock Badge");
        FakeGameIO io = new();

        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.North, "north");
        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.East, "east");

        Assert.Equal("Road 6", gameState.MainPlayer.CurrentRoom.Name);
        Assert.DoesNotContain("3 badge(s)", io.OutputText);
    }

    // Checks that the Road 6 gate blocks movement and reports the badge requirement when the player only has two badges.
    [Fact]
    public void HandleMovement_FromIkenaToRoadSix_WithTwoBadges_BlocksMovement()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.AddBadge("Grass Badge");
        gameState.MainPlayer.AddBadge("Water Badge");
        FakeGameIO io = new();

        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.North, "north");
        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.East, "east");

        Assert.Equal("Ikena", gameState.MainPlayer.CurrentRoom.Name);
        Assert.Contains("You need to obtain 3 badge(s) before this way unlocks!", io.OutputText);
    }

    // Checks that the Road 5 gate blocks movement when the player does not have a Mystic animal on the team.
    [Fact]
    public void HandleMovement_FromIkenaToRoadFive_WithoutMysticAnimal_BlocksMovement()
    {
        GameState gameState = GameSetup.CreateForLoad();
        FakeGameIO io = new();

        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.North, "north");
        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.South, "south");

        Assert.Equal("Ikena", gameState.MainPlayer.CurrentRoom.Name);
        Assert.Contains("You need a Mystic animal on your team before this way unlocks!", io.OutputText);
    }

    // Checks that the Road 5 gate opens when the player has a Mystic animal on the team.
    [Fact]
    public void HandleMovement_FromIkenaToRoadFive_WithMysticAnimal_AllowsMovement()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.AddAnimal(GameData.CreateAnimals()[6]);
        FakeGameIO io = new();

        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.North, "north");
        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.South, "south");

        Assert.Equal("Road 5", gameState.MainPlayer.CurrentRoom.Name);
        Assert.DoesNotContain("Mystic animal", io.OutputText);
    }

    // Checks that the Nucleon gate remains locked until the player has four badges.
    [Fact]
    public void HandleMovement_FromRoadFiveToNucleon_WithThreeBadges_BlocksMovement()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom("Road 5"));
        gameState.MainPlayer.AddBadge("Grass Badge");
        gameState.MainPlayer.AddBadge("Water Badge");
        gameState.MainPlayer.AddBadge("Rock Badge");
        FakeGameIO io = new();

        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.East, "east");

        Assert.Equal("Road 5", gameState.MainPlayer.CurrentRoom.Name);
        Assert.Contains("You need to obtain 4 badge(s) before this way unlocks!", io.OutputText);
    }

    // Checks that Guardian's Tower remains locked until the champion has been defeated.
    [Fact]
    public void HandleMovement_FromRoadEightToGuardiansTower_BeforeChampionDefeat_BlocksMovement()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom("Road 8"));
        FakeGameIO io = new();

        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.North, "north");

        Assert.Equal("Road 8", gameState.MainPlayer.CurrentRoom.Name);
        Assert.Contains("You must become the Champion of the region to proceed.", io.OutputText);
    }
}
