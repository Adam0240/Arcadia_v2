#nullable enable

using System.Collections.Generic;

namespace Arcadia_v2
{
    // Legacy split files still request Pokemon data through the original GameData entry point.
    public static class GameData
    {
        public static List<Pokemon> CreatePokemon()
        {
            return new List<Pokemon>(PokeFactory.CreatePokemon());
        }
    }
}
