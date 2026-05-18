#nullable enable

using System;
using System.Collections.Generic;

namespace Arcadia_v2
{
    public enum AnimalElement
    {
        Nature,
        Mystic,
        Thunder,
        Draconic,
        Nuclear
    }

    // Represents an animal's current battle-relevant state.
    public class Animal
    {
        private int mHealth;
        private readonly List<Move> mMoves = new();

        public int Id { get; }
        public string Name { get; }
        public AnimalElement Element { get; }
        public int Speed { get; }
        public int BaseHealth { get; }
        public int Health
        {
            get => mHealth;
            set
            {
                if (value < 0 || value > BaseHealth)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Health must be between 0 and base health.");
                }

                mHealth = value;
            }
        }
        public int Level { get; }

        public IReadOnlyList<Move> Moves => mMoves;

        public Animal(
            int id,
            string name,
            AnimalElement element,
            int speed,
            int baseHealth,
            int health,
            int level,
            IEnumerable<Move> moves)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Animal name cannot be empty.", nameof(name));
            }

            if (speed < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(speed), "Speed cannot be negative.");
            }

            if (baseHealth < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(baseHealth), "Base health cannot be negative.");
            }

            if (level < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(level), "Level cannot be negative.");
            }

            ArgumentNullException.ThrowIfNull(moves);
            mMoves.AddRange(moves);

            // Animals must always have at least one move and can never exceed the four-move battle limit.
            if (mMoves.Count < 1 || mMoves.Count > 4)
            {
                throw new ArgumentException("Animal must have between 1 and 4 moves.", nameof(moves));
            }

            if (mMoves.Any(move => move == null))
            {
                throw new ArgumentException("Animal moves cannot contain null values.", nameof(moves));
            }

            Id = id;
            Name = name;
            Element = element;
            Speed = speed;
            BaseHealth = baseHealth;
            Health = health;
            Level = level;
        }

        // Creates a separate Animal instance with the same current data and move set.
        public Animal Clone()
        {
            return new Animal(
                id: Id,
                name: Name,
                element: Element,
                speed: Speed,
                baseHealth: BaseHealth,
                health: Health,
                level: Level,
                moves: Moves);
        }

    }
}
