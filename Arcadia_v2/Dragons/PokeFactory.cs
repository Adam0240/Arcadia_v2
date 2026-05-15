#nullable enable

using System.Collections.Generic;

namespace Arcadia_v2
{
    // Reference version of a Pokemon factory for the rebuilt project.
    // This keeps hard-coded roster creation separate from the Pokemon model itself.
    public static class PokeFactory
    {
        private static Pokemon CreatePokemonEntry(
            int id,
            string name,
            PokemonType type,
            int speed,
            int baseHealth,
            int health,
            int level,
            params Move[] moves)
        {
            return new Pokemon(id, name, type, speed, baseHealth, health, level, moves);
        }

        public static IReadOnlyList<Pokemon> CreatePokemon()
        {
            List<Pokemon> pokemon = new List<Pokemon>
            {
                // Special Pokemon
                CreatePokemonEntry(0, "Null0", PokemonType.Normal, 0, 0, 0, 0, MoveData.Tackle, MoveData.Tackle, MoveData.Tackle, MoveData.Tackle),

                // Dark Pokemon
                CreatePokemonEntry(1, "UMBREON", PokemonType.Dark, 9, 75, 75, 0, MoveData.Moonlight, MoveData.DarkPulse, MoveData.Bite, MoveData.Tackle),

                // Psychic Pokemon
                CreatePokemonEntry(2, "ESPEON", PokemonType.Psychic, 7, 75, 75, 0, MoveData.Sunlight, MoveData.Confusion, MoveData.Psychic, MoveData.Moonblast),

                // Grass Pokemon
                CreatePokemonEntry(3, "BULBASAUR", PokemonType.Grass, 7, 40, 40, 0, MoveData.Tackle, MoveData.VineWhip, MoveData.RazerLeaf, MoveData.QuickAttack),
                CreatePokemonEntry(4, "VENUSAUR", PokemonType.Grass, 7, 80, 80, 0, MoveData.PetalBlizzard, MoveData.SolarBeam, MoveData.Earthquake, MoveData.Sunlight),
                CreatePokemonEntry(14, "VILEPLUME", PokemonType.Grass, 7, 80, 80, 0, MoveData.SeedBomb, MoveData.RazerLeaf, MoveData.Earthquake, MoveData.Psychic),

                // Water Pokemon
                CreatePokemonEntry(5, "SQUIRTLE", PokemonType.Water, 7, 40, 40, 0, MoveData.WaterGun, MoveData.QuickAttack, MoveData.WaterPulse, MoveData.Bite),
                CreatePokemonEntry(6, "BLASTOISE", PokemonType.Water, 7, 80, 80, 0, MoveData.Bite, MoveData.HydroPump, MoveData.Surf, MoveData.QuickAttack),
                CreatePokemonEntry(12, "MAGIKARP", PokemonType.Water, 7, 40, 40, 0, MoveData.Splash, MoveData.Tackle, MoveData.QuickAttack, MoveData.Growl),
                CreatePokemonEntry(13, "GYRADOS", PokemonType.Water, 7, 80, 80, 0, MoveData.Bite, MoveData.HydroPump, MoveData.Earthquake, MoveData.Surf),

                // Fire Pokemon
                CreatePokemonEntry(7, "CHARMANDER", PokemonType.Fire, 7, 40, 40, 0, MoveData.Tackle, MoveData.Ember, MoveData.FireFang, MoveData.QuickAttack),
                CreatePokemonEntry(16, "CHARMELEON", PokemonType.Fire, 7, 65, 65, 0, MoveData.Flamethrower, MoveData.QuickAttack, MoveData.Bite, MoveData.FlameWheel),
                CreatePokemonEntry(8, "CHARIZARD", PokemonType.Fire, 7, 80, 80, 0, MoveData.WingAttack, MoveData.Flamethrower, MoveData.FireBlitz, MoveData.QuickAttack),

                // Ground Pokemon
                CreatePokemonEntry(9, "GRAVELER", PokemonType.Ground, 7, 70, 70, 0, MoveData.QuickAttack, MoveData.RockSmash, MoveData.RollOut, MoveData.Earthquake),
                CreatePokemonEntry(10, "ONIX", PokemonType.Ground, 7, 75, 75, 0, MoveData.QuickAttack, MoveData.RockSmash, MoveData.RollOut, MoveData.Earthquake),

                // Electric Pokemon
                CreatePokemonEntry(11, "PIKACHU", PokemonType.Electric, 7, 75, 75, 0, MoveData.Spark, MoveData.Thunderbolt, MoveData.QuickAttack, MoveData.Surf),

                // Flying Pokemon
                CreatePokemonEntry(15, "PIDGEY", PokemonType.Flying, 7, 45, 45, 0, MoveData.Tackle, MoveData.Peck, MoveData.QuickAttack, MoveData.WingAttack),

                // Dragon Pokemon
                CreatePokemonEntry(17, "SALAMANCE", PokemonType.Dragon, 7, 90, 90, 0, MoveData.Flamethrower, MoveData.WingAttack, MoveData.Earthquake, MoveData.Surf),
                CreatePokemonEntry(18, "DRAGONITE", PokemonType.Dragon, 7, 90, 90, 0, MoveData.Flamethrower, MoveData.WingAttack, MoveData.Earthquake, MoveData.Surf),
                CreatePokemonEntry(19, "ARCEUS", PokemonType.Dragon, 7, 100, 100, 0, MoveData.Earthquake, MoveData.DarkPulse, MoveData.Moonblast, MoveData.HydroPump)
            };

            return pokemon;
        }
    }
}
