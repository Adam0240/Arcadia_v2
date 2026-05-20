using Arcadia_v2;
using Arcadia_v2.Saves;

namespace UnitTest;

public class MenuFlowTests
{
    // Checks that using the heal menu option in a town restores the active party to full currentHealth.
    [Fact]
    public void HandleMenu_HealInTown_RestoresPartyHealth()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom("Ikena"));
        Animal firstAnimal = gameState.MainPlayer.AnimalInventory[0];
        firstAnimal.CurrentHealth = 1;
        FakeGameIO io = new("heal");

        MenuFlow.HandleMenu(io, gameState, new GameSaveService(new FakeGameSaveRepository()));

        Assert.Equal(firstAnimal.BaseHealth, firstAnimal.CurrentHealth);
        Assert.Contains("All your animals have been fully restored!", io.OutputText);
    }

    // Checks that the save menu option writes save data and reports success through the injected IO.
    [Fact]
    public void HandleMenu_Save_WritesSaveResultToInjectedIo()
    {
        GameState gameState = GameSetup.CreateForLoad();
        FakeGameSaveRepository repository = new();
        FakeGameIO io = new("save");

        MenuFlow.HandleMenu(io, gameState, new GameSaveService(repository));

        Assert.NotNull(repository.SaveJsonValue);
        Assert.Contains("Game saved.", io.OutputText);
    }

    // Checks that the bag menu option prints the player's badge list.
    [Fact]
    public void HandleMenu_Bag_PrintsBadgeDisplay()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.AddBadge("Grass Badge");
        FakeGameIO io = new("bag");

        MenuFlow.HandleMenu(io, gameState, new GameSaveService(new FakeGameSaveRepository()));

        Assert.Contains("Badges:", io.OutputText);
        Assert.Contains("Grass Badge", io.OutputText);
    }

    // Checks that the swap menu option dispatches into the party swap flow.
    [Fact]
    public void HandleMenu_Swap_SwapsTwoAnimalParty()
    {
        GameState gameState = GameSetup.CreateForLoad();
        FakeGameIO io = new("s");

        MenuFlow.HandleMenu(io, gameState, new GameSaveService(new FakeGameSaveRepository()));

        Assert.Equal("Nature Lion", gameState.MainPlayer.AnimalInventory[0].Name);
        Assert.Equal("Nature Cat", gameState.MainPlayer.AnimalInventory[1].Name);
        Assert.Contains("You are swapping: Nature Cat and Nature Lion .", io.OutputText);
    }

    // Checks that the gym menu option dispatches to the gym interaction flow.
    [Fact]
    public void HandleMenu_Gym_DispatchesToGymFlow()
    {
        GameState gameState = GameSetup.CreateForLoad();
        FakeGameIO io = new("gym");

        MenuFlow.HandleMenu(io, gameState, new GameSaveService(new FakeGameSaveRepository()));

        Assert.Contains("No gym in area.", io.OutputText);
    }

    // Checks that an invalid menu command prints the invalid-option message.
    [Fact]
    public void HandleMenu_InvalidCommand_WritesInvalidMenuMessageToInjectedIo()
    {
        GameState gameState = GameSetup.CreateForLoad();
        FakeGameIO io = new("invalid");

        MenuFlow.HandleMenu(io, gameState, new GameSaveService(new FakeGameSaveRepository()));

        Assert.Contains("Invalid menu option.", io.OutputText);
    }

}
