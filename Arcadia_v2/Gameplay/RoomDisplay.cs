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

            string nearbyAnimals = room.EncounterAnimals.Count == 0
                ? "None"
                : string.Join(", ", room.EncounterAnimals.Select(animal => animal.Name));

            io.WriteLine(room.Name);
            io.WriteLine(room.Description);
            io.WriteLine($"Animals Nearby: {nearbyAnimals}");
        }
    }
}
