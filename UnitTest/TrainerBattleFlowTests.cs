using Arcadia_v2;
using Arcadia_v2.Map;

namespace UnitTest;

public class TrainerBattleFlowTests
{
    [Fact]
    public void Run_WhenOpponentPokemonFaints_SendsOutNextHealthyPokemon()
    {
        Player player = CreatePlayer();
        CompPlayer opponent = CreateOpponent();
        FakeGameIO io = new("stronghit", "stronghit");

        TrainerBattleFlow.Run(io, player, opponent);

        Assert.Contains("Rival sent out SECOND", io.OutputText);
    }

    [Fact]
    public void Run_WhenOpponentTeamFaints_CompletesBattleAndAwardsBadge()
    {
        Player player = CreatePlayer();
        CompPlayer opponent = CreateOpponent();
        FakeGameIO io = new("stronghit", "stronghit");

        TrainerBattleFlow.Run(io, player, opponent);

        Assert.True(opponent.Defeated);
        Assert.Contains("Test Badge", player.Badges);
        Assert.Contains("Rival defeated.", io.OutputText);
    }

    [Fact]
    public void Run_PreparesFreshTrainerTeamBeforeBattle()
    {
        Player player = CreatePlayer();
        CompPlayer opponent = CreateOpponent();
        opponent.PokemonInventory[0].Health = 1;
        FakeGameIO io = new("stronghit", "stronghit");

        TrainerBattleFlow.Run(io, player, opponent);

        Assert.Contains("The opponents FIRST's health is at: 5", io.OutputText);
    }

    [Fact]
    public void Run_WhenPlayerLeadIsFainted_StartsWithNextHealthyPokemon()
    {
        Player player = CreatePlayer();
        player.PokemonInventory[0].Health = 0;
        CompPlayer opponent = CreateOpponent();
        FakeGameIO io = new("stronghit", "stronghit");

        TrainerBattleFlow.Run(io, player, opponent);

        Assert.Contains("You sent out BACKUP", io.OutputText);
    }

    [Fact]
    public void Run_WhenAllPlayerPokemonAreFainted_PrintsPartyFaintedMessage()
    {
        Player player = CreatePlayer();
        player.PokemonInventory[0].Health = 0;
        player.PokemonInventory[1].Health = 0;
        CompPlayer opponent = CreateOpponent();
        FakeGameIO io = new();

        TrainerBattleFlow.Run(io, player, opponent);

        Assert.Contains("All pokemon in your party are fainted.", io.OutputText);
        Assert.DoesNotContain("You sent out", io.OutputText);
    }

    private static Player CreatePlayer()
    {
        Player player = new("Trainer", new Map().StartRoom);
        player.AddPokemon(new Pokemon(1, "LEAD", PokemonType.Normal, 5, 50, 50, 1, new[] { new Move("STRONGHIT", MoveType.Normal, 10) }));
        player.AddPokemon(new Pokemon(2, "BACKUP", PokemonType.Normal, 5, 50, 50, 1, new[] { new Move("STRONGHIT", MoveType.Normal, 10) }));
        return player;
    }

    private static CompPlayer CreateOpponent()
    {
        CompPlayer opponent = new("Rival", new Map().StartRoom);
        opponent.AddBadge("Test Badge");
        opponent.SetBattleTeam(new[]
        {
            new Pokemon(3, "FIRST", PokemonType.Normal, 5, 5, 5, 1, new[] { MoveData.Splash }),
            new Pokemon(4, "SECOND", PokemonType.Normal, 5, 5, 5, 1, new[] { MoveData.Splash })
        });
        return opponent;
    }
}
