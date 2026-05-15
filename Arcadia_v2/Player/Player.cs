#nullable enable

using Arcadia_v2.Map;
using System.Collections.Generic;

namespace Arcadia_v2
{
    // Reference version of a cleaner player design for the rebuilt project.
    // Players no longer inherit from the map; they store only their own state and receive a starting room.
    public abstract class GenericPlayer
    {
        private readonly List<string> mBadges = new();
        private readonly List<Pokemon> mPokemonInventory = new();
        private Room mCurrentRoom;

        public string Name { get; private set; }
        public Room CurrentRoom => mCurrentRoom;
        public IReadOnlyList<string> Badges => mBadges;
        public IReadOnlyList<Pokemon> PokemonInventory => mPokemonInventory;

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

        // Displays every Pokemon in the player's inventory along with each Pokemon's current health.
        public string GetPokemonInventoryDisplay()
        {
            if (mPokemonInventory.Count == 0)
            {
                return "Inventory is Empty! :'( ";
            }

            List<string> inventoryLines = new List<string> { "Inventory List:" };

            foreach (Pokemon pokemon in mPokemonInventory)
            {
                inventoryLines.Add($"{pokemon.Name} Health: {pokemon.Health}");
            }

            return string.Join("\n", inventoryLines);
        }

        // Adds a Pokemon to the player's inventory.
        public void AddPokemon(Pokemon pokemon)
        {
            ArgumentNullException.ThrowIfNull(pokemon);
            mPokemonInventory.Add(pokemon);
        }

        // Removes a Pokemon from the player's inventory and reports whether it was found.
        public bool RemovePokemon(Pokemon pokemon)
        {
            ArgumentNullException.ThrowIfNull(pokemon);
            return mPokemonInventory.Remove(pokemon);
        }

        // Returns the Pokemon at the requested party index so callers do not need direct mutable list access.
        public Pokemon GetPokemonAt(int index)
        {
            return mPokemonInventory[index];
        }

        // Swaps two Pokemon positions in the active party while keeping ownership inside the player model.
        public void SwapPokemonPositions(int firstIndex, int secondIndex)
        {
            Pokemon temp = mPokemonInventory[firstIndex];
            mPokemonInventory[firstIndex] = mPokemonInventory[secondIndex];
            mPokemonInventory[secondIndex] = temp;
        }

        // Clears the current Pokemon inventory so it can be rebuilt from a fresh source.
        protected void ClearPokemonInventory()
        {
            mPokemonInventory.Clear();
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

        // Replaces the active Pokemon inventory from persisted state.
        public void RestorePokemonInventory(IEnumerable<Pokemon> pokemonInventory)
        {
            ArgumentNullException.ThrowIfNull(pokemonInventory);

            mPokemonInventory.Clear();

            foreach (Pokemon pokemon in pokemonInventory)
            {
                AddPokemon(pokemon);
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
        private readonly List<Pokemon> mBattleTeamTemplate = new();

        public IReadOnlyList<Pokemon> BattleTeamTemplate => mBattleTeamTemplate;

        // Creates a computer-controlled player at the provided starting room.
        public CompPlayer(string name, Room startingRoom) : base(name, startingRoom)
        {
        }

        // Replaces the trainer's template team with fresh Pokemon copies, then rebuilds the active battle team.
        public void SetBattleTeam(IEnumerable<Pokemon> templatePokemon)
        {
            ArgumentNullException.ThrowIfNull(templatePokemon);

            mBattleTeamTemplate.Clear();

            foreach (Pokemon pokemon in templatePokemon)
            {
                ArgumentNullException.ThrowIfNull(pokemon);
                mBattleTeamTemplate.Add(pokemon.Clone());
            }

            PrepareForBattle();
        }

        // Rebuilds the active Pokemon inventory from the stored template team so battle damage does not carry over.
        public void PrepareForBattle()
        {
            ClearPokemonInventory();

            foreach (Pokemon pokemon in mBattleTeamTemplate)
            {
                AddPokemon(pokemon.Clone());
            }
        }
    }
}
