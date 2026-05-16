using Arcadia_v2;

namespace UnitTest;

public class WildBattleFlowTests
{
    [Fact]
    public void HandleWildBattle_CatchPokemon_AddsPokemonToPartyAndRemovesEncounter()
    {
        GameState gameState = CreateRouteOneWildBattle();
        Pokemon wildPokemon = gameState.MainPlayer.CurrentRoom.EncounterPokemon[0];
        wildPokemon.Health = 1;
        FakeGameIO io = new("tackle", "yes");

        WildBattleFlow.HandleWildBattle(io, gameState);

        Assert.Contains(wildPokemon, gameState.MainPlayer.PokemonInventory);
        Assert.DoesNotContain(wildPokemon, gameState.MainPlayer.CurrentRoom.EncounterPokemon);
        Assert.Contains($"You caught {wildPokemon.Name}!", io.OutputText);
    }

    [Fact]
    public void HandleWildBattle_FullPartyInvalidReleaseAnswer_RePromptsAndLetsWildPokemonRunAway()
    {
        GameState gameState = CreateRouteOneWildBattle();
        AddPokemonUntilPartyIsFull(gameState.MainPlayer);

        Pokemon wildPokemon = gameState.MainPlayer.CurrentRoom.EncounterPokemon[0];
        wildPokemon.Health = 1;

        FakeGameIO io = new("tackle", "yes", "maybe", "no");

        WildBattleFlow.HandleWildBattle(io, gameState);

        int releasePromptCount = io.OutputText.Split("Would you like to release a Pokemon? -- (yes or no)").Length - 1;
        Assert.Equal(2, releasePromptCount);
        Assert.Contains("Invalid input.", io.OutputText);
        Assert.Contains($"{wildPokemon.Name} ran away!", io.OutputText);
        Assert.DoesNotContain(wildPokemon, gameState.MainPlayer.CurrentRoom.EncounterPokemon);
    }

    [Fact]
    public void HandleWildBattle_FullPartyReleasePokemon_CatchesWildPokemonAndReturnsReleasedPokemonToRoom()
    {
        GameState gameState = CreateRouteOneWildBattle();
        AddPokemonUntilPartyIsFull(gameState.MainPlayer);
        Pokemon releasedPokemon = gameState.MainPlayer.PokemonInventory[0];
        Pokemon wildPokemon = gameState.MainPlayer.CurrentRoom.EncounterPokemon[0];
        wildPokemon.Health = 1;
        FakeGameIO io = new("tackle", "yes", "yes", releasedPokemon.Name);

        WildBattleFlow.HandleWildBattle(io, gameState);

        Assert.DoesNotContain(releasedPokemon, gameState.MainPlayer.PokemonInventory);
        Assert.Contains(wildPokemon, gameState.MainPlayer.PokemonInventory);
        Assert.Contains(releasedPokemon, gameState.MainPlayer.CurrentRoom.EncounterPokemon);
        Assert.DoesNotContain(wildPokemon, gameState.MainPlayer.CurrentRoom.EncounterPokemon);
        Assert.Equal(20, releasedPokemon.Health);
        Assert.Contains($"You caught {wildPokemon.Name}!", io.OutputText);
    }

    [Fact]
    public void HandleWildBattle_NoCatch_RemovesEncounterPokemon()
    {
        GameState gameState = CreateRouteOneWildBattle();
        Pokemon wildPokemon = gameState.MainPlayer.CurrentRoom.EncounterPokemon[0];
        wildPokemon.Health = 1;
        FakeGameIO io = new("tackle", "no");

        WildBattleFlow.HandleWildBattle(io, gameState);

        Assert.DoesNotContain(wildPokemon, gameState.MainPlayer.PokemonInventory);
        Assert.DoesNotContain(wildPokemon, gameState.MainPlayer.CurrentRoom.EncounterPokemon);
        Assert.Contains($"{wildPokemon.Name} ran away!", io.OutputText);
    }

    [Fact]
    public void HandleWildBattle_WhenAllPlayerPokemonAreFainted_PrintsPartyFaintedMessage()
    {
        GameState gameState = CreateRouteOneWildBattle();
        foreach (Pokemon pokemon in gameState.MainPlayer.PokemonInventory)
        {
            pokemon.Health = 0;
        }

        FakeGameIO io = new();

        WildBattleFlow.HandleWildBattle(io, gameState);

        Assert.Contains("All pokemon in your party are fainted.", io.OutputText);
        Assert.DoesNotContain("A wild", io.OutputText);
    }

    private static GameState CreateRouteOneWildBattle()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom("Route 1"));
        return gameState;
    }

    private static void AddPokemonUntilPartyIsFull(Player player)
    {
        IReadOnlyList<Pokemon> pokemon = GameData.CreatePokemon();

        for (int i = player.PokemonInventory.Count; i < 6; ++i)
        {
            player.AddPokemon(pokemon[i + 1]);
        }
    }
}
