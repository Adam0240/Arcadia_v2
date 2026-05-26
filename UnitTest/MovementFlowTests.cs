using Arcadia_v2;
using Arcadia_v2.Commands;

namespace UnitTest;

public class MovementFlowTests
{
    // Checks that the Road 6 gate opens once the player has the required three star fragments.
    [Fact]
    public void HandleMovement_FromThunderSanctuaryToRoadSix_WithThreeStarFragments_AllowsMovement()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.AddStarFragment("Nature Star Fragment");
        gameState.MainPlayer.AddStarFragment("Mystic Star Fragment");
        gameState.MainPlayer.AddStarFragment("Thunder Star Fragment");
        FakeGameIO io = new();

        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.North, "north");
        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.East, "east");

        Assert.Equal("Road 6", gameState.MainPlayer.CurrentRoom.Name);
        Assert.DoesNotContain("3 star fragment(s)", io.OutputText);
    }

    // Checks that the Road 6 gate blocks movement and reports the star fragment requirement when the player only has two star fragments.
    [Fact]
    public void HandleMovement_FromNewNucleonToRoadSix_WithTwoStarFragments_BlocksMovement()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.AddStarFragment("Nature Star Fragment");
        gameState.MainPlayer.AddStarFragment("Mystic Star Fragment");
        FakeGameIO io = new();

        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.North, "north");
        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.East, "east");

        Assert.Equal("Ikena", gameState.MainPlayer.CurrentRoom.Name);
        Assert.Contains("You need to obtain 3 star fragment(s) before this way unlocks!", io.OutputText);
    }

    // Checks that the Road 5 gate blocks movement when the player does not have a Mystic animal on the team.
    [Fact]
    public void HandleMovement_FromThunderSanctuaryToRoadFive_WithoutMysticAnimal_BlocksMovement()
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
    public void HandleMovement_FromThunderSanctuaryToRoadFive_WithMysticAnimal_AllowsMovement()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.AddAnimal(GameData.CreateAnimals()[19]);
        FakeGameIO io = new();

        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.North, "north");
        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.South, "south");

        Assert.Equal("Road 5", gameState.MainPlayer.CurrentRoom.Name);
        Assert.DoesNotContain("Mystic animal", io.OutputText);
    }

    // Checks that the Nucleon gate remains locked until the player has four star fragments.
    [Fact]
    public void HandleMovement_FromRoadFiveToNucleon_WithThreeStarFragments_BlocksMovement()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom("Road 5"));
        gameState.MainPlayer.AddStarFragment("Nature Star Fragment");
        gameState.MainPlayer.AddStarFragment("Mystic Star Fragment");
        gameState.MainPlayer.AddStarFragment("Thunder Star Fragment");
        FakeGameIO io = new();

        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.East, "east");

        Assert.Equal("Road 5", gameState.MainPlayer.CurrentRoom.Name);
        Assert.Contains("You need to obtain 4 star fragment(s) before this way unlocks!", io.OutputText);
    }

    // Checks that Elemental Sanctuary remains locked until the Elemental Titan has been defeated.
    [Fact]
    public void HandleMovement_FromRoadEightToElementalSanctuary_BeforeElementalTitanDefeat_BlocksMovement()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom("Road 8"));
        FakeGameIO io = new();

        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.North, "north");

        Assert.Equal("Road 8", gameState.MainPlayer.CurrentRoom.Name);
        Assert.Contains("You must defeat the Elemental Titan to proceed.", io.OutputText);
    }

    // Checks that Elemental Sanctuary opens from Road 8 once the Elemental Titan has been defeated.
    [Fact]
    public void HandleMovement_FromRoadEightToElementalSanctuary_AfterElementalTitanDefeat_AllowsMovement()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom("Road 8"));
        gameState.ElementalTitan.Defeated = true;
        FakeGameIO io = new();

        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.North, "north");

        Assert.Equal("Guardian Tower", gameState.MainPlayer.CurrentRoom.Name);
        Assert.DoesNotContain("You must defeat the Elemental Titan to proceed.", io.OutputText);
    }

    // Checks that returning to Maia's Stable after the Elemental Titan adds the completed Elemental Star.
    [Fact]
    public void HandleMovement_ReturnToMaiaStableAfterElementalTitanDefeat_AddsElementalStar()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom("Road 1"));
        gameState.ElementalTitan.Defeated = true;
        gameState.MainPlayer.AddStarFragment("Cosmic Star Fragment");
        FakeGameIO io = new();

        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.West, "west");

        Assert.Equal("Maia's Stable", gameState.MainPlayer.CurrentRoom.Name);
        Assert.Contains("Elemental Star", gameState.MainPlayer.StarFragments);
        Assert.Contains("your star fragments have merged into a full star", io.OutputText);
        Assert.Contains("Cosmic Star Fragment", gameState.MainPlayer.StarFragments);
    }

    // Checks that the Elemental Star merge only happens once.
    [Fact]
    public void HandleMovement_ReturnToMaiaStableWithElementalStar_DoesNotAddDuplicate()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom("Road 1"));
        gameState.ElementalTitan.Defeated = true;
        gameState.MainPlayer.AddStarFragment("Cosmic Star Fragment");
        gameState.MainPlayer.AddStarFragment("Elemental Star");
        FakeGameIO io = new();

        MovementFlow.HandleMovement(io, gameState, DirectionCommandType.West, "west");

        Assert.Single(gameState.MainPlayer.StarFragments, fragment => fragment == "Elemental Star");
        Assert.DoesNotContain("your star fragments have merged into a full star", io.OutputText);
    }
}
