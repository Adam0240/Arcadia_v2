using System.Collections.Generic;
namespace Arcadia_v2.Map
{
    public enum RoomId
    {
        Custom = 0,
        MaiaStable,
        Ikena,
        Road1,
        Road2,
        OakPass,
        Road3,
        Road4,
        NewNucleon,
        Road5,
        Road6,
        Road7,
        Wyrmrest,
        Mountains,
        RadioactiveWay,
        Nucleon,
        FinalTrials,
        GuardiansTower,
        Road8,
        TheEnd
    }

    // Defines a room object that stores its own details and links to neighboring rooms.
    public class Room
    {
        private readonly List<Arcadia_v2.Animal> mEncounterAnimals = new();

        // Exposes the room's name as a read-only property.
        public RoomId Id { get; }
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

        // Room encounter state stays on the room because wild battles depend on room-local animal lists.
        public IReadOnlyList<Arcadia_v2.Animal> EncounterAnimals => mEncounterAnimals;

        // Creates a new room with a name and description.
        public Room(string name, string description)
            : this(RoomId.Custom, name, description)
        {
        }

        public Room(RoomId id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        // Adds a cloned wild animal entry to the room so encounter state stays local to that room.
        public void SetRoomAnimal(Arcadia_v2.Animal animal)
        {
            ArgumentNullException.ThrowIfNull(animal);
            mEncounterAnimals.Add(animal.Clone());
        }

        // Adds an animal instance to the room encounter list without cloning when gameplay releases an owned animal into the room.
        public void AddEncounterAnimal(Arcadia_v2.Animal animal)
        {
            ArgumentNullException.ThrowIfNull(animal);
            mEncounterAnimals.Add(animal);
        }

        // Removes an animal from the room encounter list and reports whether it was found.
        public bool RemoveEncounterAnimal(Arcadia_v2.Animal animal)
        {
            ArgumentNullException.ThrowIfNull(animal);
            return mEncounterAnimals.Remove(animal);
        }

        // Replaces room encounter state from persisted save data.
        public void RestoreEncounterAnimals(IEnumerable<Arcadia_v2.Animal> encounterAnimals)
        {
            ArgumentNullException.ThrowIfNull(encounterAnimals);

            mEncounterAnimals.Clear();

            foreach (Arcadia_v2.Animal animal in encounterAnimals)
            {
                AddEncounterAnimal(animal);
            }
        }

        // Reports whether the room currently has any encounter animals available.
        public bool HasEncounterAnimals()
        {
            return mEncounterAnimals.Count > 0;
        }
    }
}
