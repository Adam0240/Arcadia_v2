using Arcadia_v2;
using Arcadia_v2.Saves;

namespace UnitTest;

public class MenuFlowTests
{
    // Checks that using the heal menu option in a town restores the active party to full health.
    [Fact]
    public void HandleMenu_HealInTown_RestoresPartyHealth()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom("Ikena"));
        Animal firstAnimal = gameState.MainPlayer.AnimalInventory[0];
        firstAnimal.Health = 1;
        FakeGameIO io = new("heal");

        MenuFlow.HandleMenu(io, gameState, new GameSaveService(new FakeGameSaveRepository()));

        Assert.Equal(firstAnimal.BaseHealth, firstAnimal.Health);
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
