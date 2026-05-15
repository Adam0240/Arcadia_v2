using Arcadia_v2;
using Arcadia_v2.Map;

namespace UnitTest;

public class BattleStateTests
{
    // Verifies that battle damage stops at zero HP instead of allowing negative health values.
    [Fact]
    public void ApplyDamage_ReducesHealthWithoutGoingNegative()
    {
        Pokemon target = new Pokemon(99, "TESTMON", PokemonType.Normal, 5, 20, 5, 1, new[] { MoveData.Tackle });

        Program.ApplyDamage(target, 10);

        Assert.Equal(0, target.Health);
    }

    // Verifies that negative damage is rejected so the damage helper cannot be used as accidental healing.
    [Fact]
    public void ApplyDamage_NegativeDamage_ThrowsArgumentOutOfRangeException()
    {
        Pokemon target = new Pokemon(99, "TESTMON", PokemonType.Normal, 5, 20, 5, 1, new[] { MoveData.Tackle });

        Assert.Throws<ArgumentOutOfRangeException>(() => Program.ApplyDamage(target, -1));
    }

    [Fact]
    public void BattleEngine_UseAttackMove_AppliesDamageAndClampsHealthAtZero()
    {
        Pokemon attacker = new Pokemon(98, "ATTACKMON", PokemonType.Normal, 5, 20, 20, 1, new[] { MoveData.Tackle });
        Pokemon defender = new Pokemon(99, "DEFENDMON", PokemonType.Normal, 5, 20, 3, 1, new[] { MoveData.Tackle });

        BattleMoveResult result = BattleEngine.UseMove(attacker, defender, new Move("STRONGHIT", MoveType.Normal, 10));

        Assert.Equal(BattleMoveResultType.Damage, result.ResultType);
        Assert.Equal("STRONGHIT", result.MoveName);
        Assert.Equal(10, result.Amount);
        Assert.Equal(0, result.TargetHealth);
        Assert.Equal(0, defender.Health);
    }

    // Verifies that healing moves use the selected move's power instead of always using the first move slot.
    [Fact]
    public void HealingMoves_UseSelectedMovePower()
    {
        Pokemon pokemon = new Pokemon(
            99,
            "HEALMON",
            PokemonType.Psychic,
            7,
            75,
            50,
            1,
            new[] { MoveData.Tackle, MoveData.Moonlight });

        Move selectedMove = pokemon.Moves[1];

        BattleMoveResult result = BattleEngine.UseMove(pokemon, pokemon, selectedMove);

        Assert.Equal(BattleMoveResultType.Healing, result.ResultType);
        Assert.Equal("MOONLIGHT", result.MoveName);
        Assert.Equal(10, result.Amount);
        Assert.Equal(60, pokemon.Health);
    }

    [Fact]
    public void BattleEngine_UseHealingMoveAtFullHealth_ReturnsNoEffect()
    {
        Pokemon pokemon = new Pokemon(99, "HEALMON", PokemonType.Psychic, 7, 30, 30, 1, new[] { MoveData.Moonlight });

        BattleMoveResult result = BattleEngine.UseMove(pokemon, pokemon, MoveData.Moonlight);

        Assert.Equal(BattleMoveResultType.NoEffect, result.ResultType);
        Assert.Equal("MOONLIGHT", result.MoveName);
        Assert.Equal(0, result.Amount);
        Assert.Equal(30, result.TargetHealth);
        Assert.Equal(30, pokemon.Health);
    }

    [Fact]
    public void HealingMoves_ExactAmountToFull_RestoresHealth()
    {
        Pokemon pokemon = new Pokemon(99, "HEALMON", PokemonType.Psychic, 7, 30, 25, 1, new[] { MoveData.Moonlight });
        FakeGameIO io = new();

        BattleHelpers.UseHealingMove(io, pokemon, 5);

        Assert.Equal(30, pokemon.Health);
        Assert.Contains("Health Restored", io.OutputText);
        Assert.DoesNotContain("Nothing happened", io.OutputText);
    }

    [Fact]
    public void HandlePlayerFaintedPokemon_InvalidThenNo_RePromptsWithoutSwapping()
    {
        Player player = CreateTwoPokemonPlayer();
        player.PokemonInventory[0].Health = 0;
        FakeGameIO io = new("maybe", "no");

        BattleHelpers.HandlePlayerFaintedPokemon(io, player, "Would you like to switch Pokemon?");

        int promptCount = io.OutputText.Split("Would you like to switch Pokemon?").Length - 1;
        Assert.Equal(2, promptCount);
        Assert.Contains("Invalid input.", io.OutputText);
        Assert.Equal("UMBREON", player.PokemonInventory[0].Name);
        Assert.Equal("ESPEON", player.PokemonInventory[1].Name);
    }

    [Fact]
    public void HandlePlayerFaintedPokemon_InvalidThenYes_RePromptsAndSwapsPokemon()
    {
        Player player = CreateTwoPokemonPlayer();
        player.PokemonInventory[0].Health = 0;
        FakeGameIO io = new("maybe", "yes", "umbreon", "espeon");

        BattleHelpers.HandlePlayerFaintedPokemon(io, player, "Would you like to switch Pokemon?");

        int promptCount = io.OutputText.Split("Would you like to switch Pokemon?").Length - 1;
        Assert.Equal(2, promptCount);
        Assert.Contains("Invalid input.", io.OutputText);
        Assert.Equal("ESPEON", player.PokemonInventory[0].Name);
        Assert.Equal("UMBREON", player.PokemonInventory[1].Name);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void BattleEngine_IsFainted_ReturnsTrueForZeroOrNegativeHealth(int health)
    {
        Pokemon pokemon = new Pokemon(99, "TESTMON", PokemonType.Normal, 5, 20, health, 1, new[] { MoveData.Tackle });

        Assert.True(BattleEngine.IsFainted(pokemon));
    }

    [Fact]
    public void BattleEngine_HasUsablePokemon_ReturnsFalseWhenNoPartyPokemonHaveHealth()
    {
        Player player = CreateTwoPokemonPlayer();
        player.PokemonInventory[0].Health = 0;
        player.PokemonInventory[1].Health = -1;

        Assert.False(BattleEngine.HasUsablePokemon(player));
        Assert.Equal(-1, BattleEngine.GetNextHealthyPokemonIndex(player));
    }

    [Fact]
    public void BattleEngine_GetNextHealthyPokemonIndex_ReturnsFirstHealthyPokemonAtOrAfterStartIndex()
    {
        Player player = CreateTwoPokemonPlayer();
        player.AddPokemon(new Pokemon(3, "FLAREON", PokemonType.Fire, 7, 10, 10, 1, new[] { MoveData.Ember }));
        player.PokemonInventory[0].Health = 0;
        player.PokemonInventory[1].Health = 0;

        Assert.True(BattleEngine.HasUsablePokemon(player));
        Assert.Equal(2, BattleEngine.GetNextHealthyPokemonIndex(player));
        Assert.Equal(2, BattleEngine.GetNextHealthyPokemonIndex(player, startIndex: 1));
    }

    [Fact]
    public void BattleState_CreateWildBattle_UsesCurrentPlayerLeadAndWildPokemon()
    {
        Player player = CreateTwoPokemonPlayer();
        Pokemon wildPokemon = new Pokemon(3, "PIDGEY", PokemonType.Flying, 7, 10, 10, 1, new[] { MoveData.Peck });

        BattleState battleState = BattleState.CreateWildBattle(player, wildPokemon);

        Assert.Equal("UMBREON", battleState.PlayerPokemon.Name);
        Assert.Equal("PIDGEY", battleState.OpponentPokemon.Name);
        Assert.False(battleState.IsOver);
    }

    [Fact]
    public void BattleState_TrySwitchOpponentToNextHealthyPokemon_UpdatesActiveOpponent()
    {
        Player player = CreateTwoPokemonPlayer();
        CompPlayer opponent = new("Opponent", new Map().StartRoom);
        opponent.SetBattleTeam(new[]
        {
            new Pokemon(3, "PIDGEY", PokemonType.Flying, 7, 10, 0, 1, new[] { MoveData.Peck }),
            new Pokemon(4, "PIKACHU", PokemonType.Electric, 7, 10, 10, 1, new[] { MoveData.Spark })
        });
        BattleState battleState = BattleState.CreateTrainerBattle(player, opponent);

        bool switched = battleState.TrySwitchOpponentToNextHealthyPokemon(startIndex: 1);

        Assert.True(switched);
        Assert.Equal(1, battleState.OpponentActiveIndex);
        Assert.Equal("PIKACHU", battleState.OpponentPokemon.Name);
    }

    // Verifies that cloning creates a separate Pokemon and move list so later mutations do not leak back to the original.
    [Fact]
    public void PokemonClone_CreatesIndependentMoveObjects()
    {
        Pokemon original = new Pokemon(
            99,
            "CLONEMON",
            PokemonType.Normal,
            8,
            30,
            30,
            4,
            new[] { new Move("TACKLE", MoveType.Normal, 5) });

        Pokemon clone = original.Clone();
        clone.Health = 1;

        Assert.Equal("CLONEMON", original.Name);
        Assert.Equal("TACKLE", original.Moves[0].MoveName);
        Assert.Equal(5, original.Moves[0].MovePower);
        Assert.Equal(30, original.Health);
    }

    // Verifies that a trainer can rebuild a fresh battle roster from templates after a previous battle changed live HP values.
    [Fact]
    public void PrepareForBattle_RebuildsFreshTrainerRosterFromTemplate()
    {
        List<Pokemon> pokemon = GameData.CreatePokemon();
        Room startingRoom = new Map().StartRoom;
        CompPlayer gymLeader = new CompPlayer("Trainer", startingRoom);
        gymLeader.SetBattleTeam(new[] { pokemon[3], pokemon[14] });

        gymLeader.PokemonInventory[0].Health = 1;
        gymLeader.PokemonInventory[1].Health = 2;
        Pokemon firstBattleLead = gymLeader.PokemonInventory[0];

        gymLeader.PrepareForBattle();

        Assert.Equal(gymLeader.BattleTeamTemplate[0].BaseHealth, gymLeader.PokemonInventory[0].Health);
        Assert.Equal(gymLeader.BattleTeamTemplate[1].BaseHealth, gymLeader.PokemonInventory[1].Health);
        Assert.NotSame(firstBattleLead, gymLeader.PokemonInventory[0]);
    }

    // Verifies that the corrected Pokemon data keeps Squirtle's WATERGUN and Magikarp's SPLASH assignments intact.
    [Fact]
        public void CreatePokemon_UsesCorrectWaterStarterAndMagikarpMoves()
        {
            List<Pokemon> pokemon = GameData.CreatePokemon();
            Pokemon squirtle = Assert.Single(pokemon, p => p.Name == "SQUIRTLE");
            Pokemon magikarp = Assert.Single(pokemon, p => p.Name == "MAGIKARP");

        Assert.Equal("WATERGUN", squirtle.Moves[0].MoveName);
        Assert.Equal(6, squirtle.Moves[0].MovePower);
        Assert.Equal("SPLASH", magikarp.Moves[0].MoveName);
        Assert.Equal(0, magikarp.Moves[0].MovePower);
    }

    private static Player CreateTwoPokemonPlayer()
    {
        Player player = new Player("Trainer", new Map().StartRoom);
        player.AddPokemon(new Pokemon(1, "UMBREON", PokemonType.Dark, 7, 10, 10, 1, new[] { MoveData.Bite }));
        player.AddPokemon(new Pokemon(2, "ESPEON", PokemonType.Psychic, 7, 10, 10, 1, new[] { MoveData.Psychic }));
        return player;
    }

}
