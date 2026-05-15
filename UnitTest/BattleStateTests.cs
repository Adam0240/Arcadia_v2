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

        pokemon.Health += selectedMove.MovePower;

        Assert.Equal(60, pokemon.Health);
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
        Pokemon squirtle = Assert.Single(pokemon.Where(p => p.Name == "SQUIRTLE"));
        Pokemon magikarp = Assert.Single(pokemon.Where(p => p.Name == "MAGIKARP"));

        Assert.Equal("WATERGUN", squirtle.Moves[0].MoveName);
        Assert.Equal(6, squirtle.Moves[0].MovePower);
        Assert.Equal("SPLASH", magikarp.Moves[0].MoveName);
        Assert.Equal(0, magikarp.Moves[0].MovePower);
    }
}
