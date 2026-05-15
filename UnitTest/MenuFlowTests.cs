using Arcadia_v2;
using Arcadia_v2.Saves;

namespace UnitTest;

public class MenuFlowTests
{
    [Fact]
    public void HandleMenu_HealInTown_RestoresPartyHealth()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom("Ikena"));
        Pokemon firstPokemon = gameState.MainPlayer.PokemonInventory[0];
        firstPokemon.Health = 1;
        FakeGameIO io = new("heal");

        MenuFlow.HandleMenu(io, gameState, new GameSaveService(new FakeGameSaveRepository()));

        Assert.Equal(firstPokemon.BaseHealth, firstPokemon.Health);
        Assert.Contains("All your Pokemon have been fully restored!", io.OutputText);
    }

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

    [Fact]
    public void HandleMenu_InvalidCommand_WritesInvalidMenuMessageToInjectedIo()
    {
        GameState gameState = GameSetup.CreateForLoad();
        FakeGameIO io = new("invalid");

        MenuFlow.HandleMenu(io, gameState, new GameSaveService(new FakeGameSaveRepository()));

        Assert.Contains("Invalid menu option.", io.OutputText);
    }

}
