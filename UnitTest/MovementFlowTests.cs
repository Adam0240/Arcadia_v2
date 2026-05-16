using Arcadia_v2;
using Arcadia_v2.Commands;

namespace UnitTest;

public class MovementFlowTests
{
    [Fact]
    public void HandleMovement_FromRoute4ToNewNucleon_WithOneBadge_AllowsMovement()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom("Route 4"));
        gameState.MainPlayer.AddBadge("Grass Badge");
        FakeGameIO io = new();

        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.North, "north");

        Assert.Equal("New Nucleon", gameState.MainPlayer.CurrentRoom.Name);
        Assert.DoesNotContain("2 badge(s)", io.OutputText);
    }

    [Fact]
    public void HandleMovement_FromRoute5ToNewNucleon_WithOneBadge_BlocksMovement()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom("Route 5"));
        gameState.MainPlayer.AddBadge("Grass Badge");
        FakeGameIO io = new();

        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.West, "west");

        Assert.Equal("Route 5", gameState.MainPlayer.CurrentRoom.Name);
        Assert.Contains("You need to obtain 2 badge(s) before this way unlocks!", io.OutputText);
    }
}
