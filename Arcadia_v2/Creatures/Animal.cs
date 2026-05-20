#nullable enable

using System;
using System.Collections.Generic;

namespace Arcadia_v2
{
    public enum AnimalElement
    {
        Nature = 0,
        Mystic = 1,
        Thunder = 2,
        Draconic = 3,
        Cosmic = 4,
        Nuclear = 5
    }

    // Represents an animal's current battle-relevant state.
    public class Animal
    {
        private int mCurrentHealth;
        private readonly List<Move> mMoves = new();

        public int Id { get; }
        public string Name { get; }
        public AnimalElement Element { get; }
        public int Speed { get; }
        public int BaseHealth { get; }
        public int CurrentHealth
        {
            get => mCurrentHealth;
            set
            {
                if (value < 0 || value > BaseHealth)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Current health must be between 0 and base health.");
                }

                mCurrentHealth = value;
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
            int currentHealth,
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
            CurrentHealth = currentHealth;
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
                currentHealth: CurrentHealth,
                level: Level,
                moves: Moves);
        }

    }
}
