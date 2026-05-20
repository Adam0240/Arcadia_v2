#nullable enable

using Arcadia_v2.Map;
using System.Collections.Generic;

namespace Arcadia_v2
{
    // Base player state shared by human and computer-controlled players.
    // Players store their own state and receive a starting room from the map.
    public abstract class GenericPlayer
    {
        private readonly List<string> mBadges = new();
        private readonly List<Animal> mAnimalInventory = new();
        private Room mCurrentRoom;

        public string Name { get; private set; }
        public Room CurrentRoom => mCurrentRoom;
        public IReadOnlyList<string> Badges => mBadges;
        public IReadOnlyList<Animal> AnimalInventory => mAnimalInventory;

        // Creates a player with a valid name and required starting room.
        protected GenericPlayer(string name, Room startingRoom)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Player name cannot be empty.", nameof(name));
            }

            ArgumentNullException.ThrowIfNull(startingRoom);

            Name = name;
            mCurrentRoom = startingRoom;
        }

        // Adds a badge once and rejects invalid badge names.
        public void AddBadge(string badge)
        {
            if (string.IsNullOrWhiteSpace(badge))
            {
                throw new ArgumentException("Badge name cannot be empty.", nameof(badge));
            }

            if (!mBadges.Contains(badge))
            {
                mBadges.Add(badge);
            }
        }

        // Returns a display string so presentation can happen outside the player model.
        public string GetBadgeDisplay()
        {
            if (mBadges.Count == 0)
            {
                return "You have no badges!";
            }

            return "Badges:\n" + string.Join("\n", mBadges);
        }

        // Displays every animal in the player's inventory along with each animal's current health.
        public string GetAnimalInventoryDisplay()
        {
            if (mAnimalInventory.Count == 0)
            {
                return "Inventory is Empty! :'( ";
            }

            List<string> inventoryLines = new List<string> { "Inventory List:" };

            foreach (Animal animal in mAnimalInventory)
            {
                inventoryLines.Add($"{animal.Name} Health: {animal.CurrentHealth}");
            }

            return string.Join("\n", inventoryLines);
        }

        // Adds an animal to the player's inventory.
        public void AddAnimal(Animal animal)
        {
            ArgumentNullException.ThrowIfNull(animal);
            mAnimalInventory.Add(animal);
        }

        // Removes an animal from the player's inventory and reports whether it was found.
        public bool RemoveAnimal(Animal animal)
        {
            ArgumentNullException.ThrowIfNull(animal);
            return mAnimalInventory.Remove(animal);
        }

        // Returns the animal at the requested party index so callers do not need direct mutable list access.
        public Animal GetAnimalAt(int index)
        {
            return mAnimalInventory[index];
        }

        // Swaps two animal positions in the active party while keeping ownership inside the player model.
        public void SwapAnimalPositions(int firstIndex, int secondIndex)
        {
            Animal temp = mAnimalInventory[firstIndex];
            mAnimalInventory[firstIndex] = mAnimalInventory[secondIndex];
            mAnimalInventory[secondIndex] = temp;
        }

        // Clears the current animal inventory before replacing it.
        protected void ClearAnimalInventory()
        {
            mAnimalInventory.Clear();
        }

        // Replaces badges from persisted state while preserving normal validation and uniqueness rules.
        public void RestoreBadges(IEnumerable<string> badges)
        {
            ArgumentNullException.ThrowIfNull(badges);

            mBadges.Clear();

            foreach (string badge in badges)
            {
                AddBadge(badge);
            }
        }

        // Restores the persisted player or trainer name.
        public void RestoreName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Player name cannot be empty.", nameof(name));
            }

            Name = name;
        }

        // Replaces the active animal inventory from persisted state.
        public void RestoreAnimalInventory(IEnumerable<Animal> animalInventory)
        {
            ArgumentNullException.ThrowIfNull(animalInventory);

            mAnimalInventory.Clear();

            foreach (Animal animal in animalInventory)
            {
                AddAnimal(animal);
            }
        }

        // Moves the player to a new room while ensuring the destination is valid.
        public void MoveTo(Room room)
        {
            ArgumentNullException.ThrowIfNull(room);
            mCurrentRoom = room;
        }
    }

    public class Player : GenericPlayer
    {
        // Creates a standard player at the provided starting room.
        public Player(string name, Room startingRoom) : base(name, startingRoom)
        {
        }
    }

    public class CompPlayer : GenericPlayer
    {
        public bool Defeated { get; set; }
        private readonly List<Animal> mBattleTeamTemplate = new();

        public IReadOnlyList<Animal> BattleTeamTemplate => mBattleTeamTemplate;

        // Creates a computer-controlled player at the provided starting room.
        public CompPlayer(string name, Room startingRoom) : base(name, startingRoom)
        {
        }

        // Replaces the trainer's template team with fresh animal copies, then rebuilds the active battle team.
        public void SetBattleTeam(IEnumerable<Animal> templateAnimals)
        {
            ArgumentNullException.ThrowIfNull(templateAnimals);

            mBattleTeamTemplate.Clear();

            foreach (Animal animal in templateAnimals)
            {
                ArgumentNullException.ThrowIfNull(animal);
                mBattleTeamTemplate.Add(animal.Clone());
            }

            PrepareForBattle();
        }

        // Rebuilds the active animal inventory from the stored template team so battle damage does not carry over.
        public void PrepareForBattle()
        {
            ClearAnimalInventory();

            foreach (Animal animal in mBattleTeamTemplate)
            {
                AddAnimal(animal.Clone());
            }
        }
    }
}
