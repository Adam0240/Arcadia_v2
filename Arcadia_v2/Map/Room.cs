using System.Collections.Generic;

namespace Arcadia_v2.Map
{
    // Defines a room object that stores its own details and links to neighboring rooms.
    public class Room
    {
        private readonly List<Arcadia_v2.Pokemon> mEncounterPokemon = new();

        // Exposes the room's name as a read-only property.
        public string Name { get; }
        public string Description { get; }
        public bool IsTown { get; init; }
        public bool IsFinalRoom { get; init; }
        public int RequiredBadgesToEnter { get; init; }
        public bool RequiresChampionDefeatToEnter { get; init; }

        // Stores references to adjacent rooms so movement can happen by direction.
        public Room? North { get; set; }
        public Room? South { get; set; }
        public Room? East { get; set; }
        public Room? West { get; set; }

        // Room encounter state stays on the room because wild battles depend on room-local Pokemon lists.
        public IReadOnlyList<Arcadia_v2.Pokemon> EncounterPokemon => mEncounterPokemon;

        // Creates a new room with a name and description.
        public Room(string name, string description)
        {
            Name = name;
            Description = description;
        }

        // Adds a cloned wild Pokemon entry to the room so encounter state stays local to that room.
        public void SetRoomPokemon(Arcadia_v2.Pokemon pokemon)
        {
            ArgumentNullException.ThrowIfNull(pokemon);
            mEncounterPokemon.Add(pokemon.Clone());
        }

        // Adds a Pokemon instance to the room encounter list without cloning when gameplay releases an owned Pokemon into the room.
        public void AddEncounterPokemon(Arcadia_v2.Pokemon pokemon)
        {
            ArgumentNullException.ThrowIfNull(pokemon);
            mEncounterPokemon.Add(pokemon);
        }

        // Removes a Pokemon from the room encounter list and reports whether it was found.
        public bool RemoveEncounterPokemon(Arcadia_v2.Pokemon pokemon)
        {
            ArgumentNullException.ThrowIfNull(pokemon);
            return mEncounterPokemon.Remove(pokemon);
        }

        // Replaces room encounter state from persisted save data.
        public void RestoreEncounterPokemon(IEnumerable<Arcadia_v2.Pokemon> encounterPokemon)
        {
            ArgumentNullException.ThrowIfNull(encounterPokemon);

            mEncounterPokemon.Clear();

            foreach (Arcadia_v2.Pokemon pokemon in encounterPokemon)
            {
                AddEncounterPokemon(pokemon);
            }
        }

        // Reports whether the room currently has any encounter Pokemon available.
        public bool HasEncounterPokemon()
        {
            return mEncounterPokemon.Count > 0;
        }
    }
}
