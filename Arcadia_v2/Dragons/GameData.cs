#nullable enable

using System.Collections.Generic;

namespace Arcadia_v2
{
    // Provides fresh Pokemon roster data for game setup and save restoration.
    public static class GameData
    {
        public static List<Pokemon> CreatePokemon()
        {
            return new List<Pokemon>(PokeFactory.CreatePokemon());
        }
    }
}
