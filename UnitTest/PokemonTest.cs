using Arcadia_v2;

namespace UnitTest
{
    public class PokemonTest
    {
        // Checks that a Pokemon stores all constructor values exactly as provided.
        [Fact]
        public void Constructor_SetsAllPokemonProperties()
        {
            Move[] moves = { MoveData.Tackle, MoveData.QuickAttack };
            Pokemon pokemon = new(25, "PIKACHU", PokemonType.Electric, 12, 35, 35, 5, moves);

            Assert.Equal(25, pokemon.Id);
            Assert.Equal("PIKACHU", pokemon.Name);
            Assert.Equal(PokemonType.Electric, pokemon.Type);
            Assert.Equal(12, pokemon.Speed);
            Assert.Equal(35, pokemon.BaseHealth);
            Assert.Equal(35, pokemon.Health);
            Assert.Equal(5, pokemon.Level);
        }

        // Checks that a Pokemon copies all provided moves into its move list in order.
        [Fact]
        public void Constructor_CopiesMovesIntoMoveList()
        {
            Move[] moves = { MoveData.Ember, MoveData.FireFang, MoveData.QuickAttack };
            Pokemon pokemon = new(7, "CHARMANDER", PokemonType.Fire, 7, 40, 40, 0, moves);

            Assert.Equal(3, pokemon.Moves.Count);
            Assert.Same(MoveData.Ember, pokemon.Moves[0]);
            Assert.Same(MoveData.FireFang, pokemon.Moves[1]);
            Assert.Same(MoveData.QuickAttack, pokemon.Moves[2]);
        }

        // Checks that a Pokemon cannot be created without any moves.
        [Fact]
        public void Constructor_EmptyMoveCollection_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                new Pokemon(0, "ZERO", PokemonType.Normal, 0, 0, 0, 0, Array.Empty<Move>()));
        }

        // Checks that a Pokemon cannot be created with more than four moves.
        [Fact]
        public void Constructor_MoreThanFourMoves_ThrowsArgumentException()
        {
            Move[] moves = { MoveData.Tackle, MoveData.QuickAttack, MoveData.Bite, MoveData.Ember, MoveData.WaterGun };

            Assert.Throws<ArgumentException>(() =>
                new Pokemon(0, "OVERFLOW", PokemonType.Normal, 0, 10, 10, 1, moves));
        }

        // Checks that changing the current health property updates the Pokemon health value.
        [Fact]
        public void Health_Setter_UpdatesHealth()
        {
            Pokemon pokemon = new(1, "UMBREON", PokemonType.Dark, 9, 75, 75, 0, new[] { MoveData.Bite });

            pokemon.Health = 40;

            Assert.Equal(40, pokemon.Health);
        }

        // Checks that the factory creates the expected number of Pokemon entries.
        [Fact]
        public void PokeFactory_CreatePokemon_ReturnsExpectedCount()
        {
            IReadOnlyList<Pokemon> pokemon = PokeFactory.CreatePokemon();

            Assert.Equal(20, pokemon.Count);
        }

        // Checks that the factory creates Espeon with the corrected psychic move.
        [Fact]
        public void PokeFactory_CreatePokemon_EspeonIncludesPsychicMove()
        {
            Pokemon espeon = PokeFactory.CreatePokemon().Single(pokemon => pokemon.Name == "ESPEON");

            Assert.Contains(MoveData.Psychic, espeon.Moves);
        }

        // Checks that the factory creates Pikachu with the expected electric Pokemon type and move set entry.
        [Fact]
        public void PokeFactory_CreatePokemon_PikachuHasExpectedTypeAndMove()
        {
            Pokemon pikachu = PokeFactory.CreatePokemon().Single(pokemon => pokemon.Name == "PIKACHU");

            Assert.Equal(PokemonType.Electric, pikachu.Type);
            Assert.Contains(MoveData.Thunderbolt, pikachu.Moves);
        }
    }
}
