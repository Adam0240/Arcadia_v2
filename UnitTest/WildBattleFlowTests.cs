using Arcadia_v2;

namespace UnitTest;

public class WildBattleFlowTests
{
    [Fact]
    public void HandleWildBattle_FullPartyInvalidReleaseAnswer_RePromptsAndLetsWildPokemonRunAway()
    {
        GameState gameState = GameSetup.CreateForLoad();
        gameState.MainPlayer.MoveTo(gameState.GameMap.GetRoom("Route 1"));
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

    private static void AddPokemonUntilPartyIsFull(Player player)
    {
        IReadOnlyList<Pokemon> pokemon = GameData.CreatePokemon();

        for (int i = player.PokemonInventory.Count; i < 6; ++i)
        {
            player.AddPokemon(pokemon[i + 1]);
        }
    }
}
