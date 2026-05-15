#nullable enable

using System;
using System.Collections.Generic;

namespace Arcadia_v2
{
    public enum PokemonType
    {
        Normal,
        Dark,
        Psychic,
        Grass,
        Water,
        Fire,
        Ground,
        Electric,
        Flying,
        Dragon
    }

    // Reference version of a cleaner Pokemon model for the rebuilt project.
    public class Pokemon
    {
        public int Id { get; }
        public string Name { get; }
        public PokemonType Type { get; }
        public int Speed { get; }
        public int BaseHealth { get; }
        public int Health { get; set; }
        public int Level { get; }

        public List<Move> Moves { get; } = new List<Move>();

        public Pokemon(
            int id,
            string name,
            PokemonType type,
            int speed,
            int baseHealth,
            int health,
            int level,
            IEnumerable<Move> moves)
        {
            Id = id;
            Name = name;
            Type = type;
            Speed = speed;
            BaseHealth = baseHealth;
            Health = health;
            Level = level;
            Moves.AddRange(moves);

            // Pokemon must always have at least one move and can never exceed the four-move battle limit.
            if (Moves.Count < 1 || Moves.Count > 4)
            {
                throw new ArgumentException("Pokemon must have between 1 and 4 moves.", nameof(moves));
            }
        }

        // Creates a separate Pokemon instance with the same current data and move set.
        public Pokemon Clone()
        {
            return new Pokemon(Id, Name, Type, Speed, BaseHealth, Health, Level, Moves);
        }

        // Legacy split files still ask each Pokemon to choose a random move slot.
        public int RanNum()
        {
            return Random.Shared.Next(1, Moves.Count + 1);
        }
    }
}
