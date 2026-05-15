#nullable enable

using System;
using System.Linq;
using Arcadia_v2.Map;

namespace Arcadia_v2
{
    // Centralizes room console output so the room model no longer owns presentation behavior.
    public static class RoomDisplay
    {
        public static void Print(Room room)
        {
            ArgumentNullException.ThrowIfNull(room);

            string nearbyPokemon = room.EncounterPokemon.Count == 0
                ? "None"
                : string.Join(", ", room.EncounterPokemon.Select(pokemon => pokemon.Name));

            Console.WriteLine(room.Name);
            Console.WriteLine(room.Description);
            Console.WriteLine($"Pokemon Nearby: {nearbyPokemon}");
        }
    }
}
