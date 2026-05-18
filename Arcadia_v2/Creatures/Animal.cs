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
        public int Id { get; }
        public string Name { get; }
        public AnimalElement Element { get; }
        public int Speed { get; }
        public int BaseHealth { get; }
        public int Health { get; set; }
        public int Level { get; }

        public List<Move> Moves { get; } = new List<Move>();

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
            Id = id;
            Name = name;
            Element = element;
            Speed = speed;
            BaseHealth = baseHealth;
            Health = health;
            Level = level;
            Moves.AddRange(moves);

            // Animals must always have at least one move and can never exceed the four-move battle limit.
            if (Moves.Count < 1 || Moves.Count > 4)
            {
                throw new ArgumentException("Animal must have between 1 and 4 moves.", nameof(moves));
            }
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

        // Returns a random one-based move slot for callers that need slot-style selection.
        public int RanNum()
        {
            return Random.Shared.Next(1, Moves.Count + 1);
        }
    }
}
