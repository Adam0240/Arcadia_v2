#nullable enable

using Arcadia_v2.Creatures;

namespace Arcadia_v2
{
    public sealed class RandomBattleMoveSelector : IBattleMoveSelector
    {
        public static RandomBattleMoveSelector Instance { get; } = new();

        private RandomBattleMoveSelector()
        {
        }

        public Move SelectMove(Animal animal)
        {
            ArgumentNullException.ThrowIfNull(animal);

            int moveIndex = Random.Shared.Next(animal.Moves.Count);
            return animal.Moves[moveIndex];
        }
    }
}
