using Arcadia_v2;
using Arcadia_v2.Commands;

namespace UnitTest;

public class MovementFlowTests
{
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

    [Fact]
    public void HandleMovement_FromIkenaToRoadFive_WithoutWaterType_BlocksMovement()
    {
        GameState gameState = GameSetup.CreateForLoad();
        FakeGameIO io = new();

        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.North, "north");
        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.South, "south");

        Assert.Equal("Ikena", gameState.MainPlayer.CurrentRoom.Name);
        Assert.Contains("You need a Water-type Pokemon on your team before this way unlocks!", io.OutputText);
    }

    [Fact]
    public void HandleMovement_FromIkenaToRoadFive_WithWaterType_AllowsMovement()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.AddPokemon(GameData.CreatePokemon()[6]);
        FakeGameIO io = new();

        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.North, "north");
        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.South, "south");

        Assert.Equal("Road 5", gameState.MainPlayer.CurrentRoom.Name);
        Assert.DoesNotContain("Water-type Pokemon", io.OutputText);
    }

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
