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
        gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom("New Nucleon"));
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

    // Checks that the bag menu option prints the player's star fragment list.
    [Fact]
    public void HandleMenu_Bag_PrintsStarFragmentDisplay()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.AddStarFragment("Nature Star Fragment");
        FakeGameIO io = new("bag");

        MenuFlow.HandleMenu(io, gameState, new GameSaveService(new FakeGameSaveRepository()));

        Assert.Contains("Star Fragments:", io.OutputText);
        Assert.Contains("Nature Star Fragment", io.OutputText);
    }

    // Checks that the bond menu option prints every elemental bond meter.
    [Fact]
    public void HandleMenu_Bond_PrintsBondDisplay()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.AddBond(AnimalElement.Nature, 50);
        FakeGameIO io = new("bond");

        MenuFlow.HandleMenu(io, gameState, new GameSaveService(new FakeGameSaveRepository()));

        Assert.Contains("Bond:", io.OutputText);
        Assert.Contains("Nature 50%/100%", io.OutputText);
        Assert.Contains("Nuclear 0%/100%", io.OutputText);
    }

    // Checks that the swap menu option dispatches into the party swap flow.
    [Fact]
    public void HandleMenu_Swap_SwapsTwoAnimalParty()
    {
        GameState gameState = GameSetup.CreateForLoad();
        FakeGameIO io = new("s");

        MenuFlow.HandleMenu(io, gameState, new GameSaveService(new FakeGameSaveRepository()));

        Assert.Equal("N_DOG", gameState.MainPlayer.AnimalInventory[0].Name);
        Assert.Equal("N_CAT", gameState.MainPlayer.AnimalInventory[1].Name);
        Assert.Contains("You are swapping: N_CAT and N_DOG .", io.OutputText);
    }

    // Checks that choosing grow without a ready animal reports the unavailable state.
    [Fact]
    public void HandleMenu_GrowWithoutEligibleAnimal_PrintsUnavailableMessage()
    {
        GameState gameState = GameSetup.CreateForLoad();
        FakeGameIO io = new("grow");

        MenuFlow.HandleMenu(io, gameState, new GameSaveService(new FakeGameSaveRepository()));

        Assert.Contains("No animals are ready to grow up.", io.OutputText);
        Assert.DoesNotContain("An animal is growing up!", io.OutputText);
    }

    // Checks that choosing grow replaces an eligible base animal with its adult form and resets bond.
    [Fact]
    public void HandleMenu_GrowWithEligibleAnimal_ReplacesAnimalAndResetsBond()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.AddBond(AnimalElement.Nature, 100);
        FakeGameIO io = new("grow", "1");

        MenuFlow.HandleMenu(io, gameState, new GameSaveService(new FakeGameSaveRepository()));

        Assert.Equal("N_LION", gameState.MainPlayer.AnimalInventory[0].Name);
        Assert.Equal(0, gameState.MainPlayer.GetBond(AnimalElement.Nature));
        Assert.Contains("An animal is growing up!", io.OutputText);
        Assert.Contains("One of the following animals are ready to grow up", io.OutputText);
        Assert.Contains("1. N_CAT", io.OutputText);
        Assert.DoesNotContain("2. N_LION", io.OutputText);
        Assert.Contains("N_CAT grew into N_LION!", io.OutputText);
    }

    // Checks that the sanctuary menu option dispatches to the sanctuary interaction flow.
    [Fact]
    public void HandleMenu_Sanctuary_DispatchesToSanctuaryFlow()
    {
        GameState gameState = GameSetup.CreateForLoad();
        FakeGameIO io = new("sanctuary");

        MenuFlow.HandleMenu(io, gameState, new GameSaveService(new FakeGameSaveRepository()));

        Assert.Contains("No sanctuary in area.", io.OutputText);
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
