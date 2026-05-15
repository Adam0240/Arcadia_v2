#nullable enable

using System;
using System.Linq;
using Arcadia_v2.Map;

namespace Arcadia_v2
{
    // Centralizes room console output so the room model no longer owns presentation behavior.
    public static class RoomDisplay
    {
        public static void Print(IGameIO io, Room room)
        {
            ArgumentNullException.ThrowIfNull(room);
            ArgumentNullException.ThrowIfNull(io);

            string nearbyPokemon = room.EncounterPokemon.Count == 0
                ? "None"
                : string.Join(", ", room.EncounterPokemon.Select(pokemon => pokemon.Name));

            io.WriteLine(room.Name);
            io.WriteLine(room.Description);
            io.WriteLine($"Pokemon Nearby: {nearbyPokemon}");
        }
    }
}
