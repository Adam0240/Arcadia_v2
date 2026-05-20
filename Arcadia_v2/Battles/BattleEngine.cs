#nullable enable

using System;

namespace Arcadia_v2
{
    public enum BattleMoveResultType
    {
        Damage,
        Healing,
        NoEffect
    }

    public readonly record struct BattleMoveResult(
        BattleMoveResultType ResultType,
        string MoveName,
        int Amount,
        int TargetHealth);

    // Owns low-level battle rules that should not depend on input or output.
    public static class BattleEngine
    {
        public static BattleMoveResult UseMove(Animal attacker, Animal defender, Move move)
        {
            ArgumentNullException.ThrowIfNull(attacker);
            ArgumentNullException.ThrowIfNull(defender);
            ArgumentNullException.ThrowIfNull(move);

            return move.Effect switch
            {
                MoveEffect.Healing => RestoreHealth(attacker, move),
                _ => ApplyDamage(defender, move)
            };
        }

        public static BattleMoveResult RestoreHealth(Animal animal, int healingPower)
        {
            ArgumentNullException.ThrowIfNull(animal);

            if (healingPower < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(healingPower), "Healing power cannot be negative.");
            }

            if (animal.CurrentHealth >= animal.BaseHealth)
            {
                return new BattleMoveResult(BattleMoveResultType.NoEffect, string.Empty, 0, animal.CurrentHealth);
            }

            int originalHealth = animal.CurrentHealth;
            animal.CurrentHealth = Math.Min(animal.BaseHealth, animal.CurrentHealth + healingPower);
            int restoredHealth = animal.CurrentHealth - originalHealth;

            return new BattleMoveResult(BattleMoveResultType.Healing, string.Empty, restoredHealth, animal.CurrentHealth);
        }

        public static BattleMoveResult ApplyDamage(Animal defender, int damage)
        {
            ArgumentNullException.ThrowIfNull(defender);

            if (damage < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(damage), "Damage cannot be negative.");
            }

            defender.CurrentHealth = Math.Max(0, defender.CurrentHealth - damage);
            return new BattleMoveResult(BattleMoveResultType.Damage, string.Empty, damage, defender.CurrentHealth);
        }

        public static bool IsHealingMove(Move move)
        {
            ArgumentNullException.ThrowIfNull(move);
            return move.Effect == MoveEffect.Healing;
        }

        public static bool IsFainted(Animal animal)
        {
            ArgumentNullException.ThrowIfNull(animal);
            return animal.CurrentHealth <= 0;
        }

        public static bool IsBattleOver(Animal firstAnimal, Animal secondAnimal)
        {
            return IsFainted(firstAnimal) || IsFainted(secondAnimal);
        }

        public static bool HasUsableAnimals(GenericPlayer player)
        {
            ArgumentNullException.ThrowIfNull(player);
            return GetNextHealthyAnimalIndex(player) >= 0;
        }

        public static int GetNextHealthyAnimalIndex(GenericPlayer player, int startIndex = 0)
        {
            ArgumentNullException.ThrowIfNull(player);

            for (int i = startIndex; i < player.AnimalInventory.Count; ++i)
            {
                if (!IsFainted(player.AnimalInventory[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        public static bool CanAutoSwapTwoAnimalParty(GenericPlayer player)
        {
            ArgumentNullException.ThrowIfNull(player);
            return player.AnimalInventory.Count == 2;
        }

        public static int GetOnlyOtherAnimalIndex(GenericPlayer player, Animal activeAnimal)
        {
            ArgumentNullException.ThrowIfNull(player);
            ArgumentNullException.ThrowIfNull(activeAnimal);

            if (!CanAutoSwapTwoAnimalParty(player))
            {
                return -1;
            }

            if (ReferenceEquals(player.AnimalInventory[0], activeAnimal))
            {
                return 1;
            }

            if (ReferenceEquals(player.AnimalInventory[1], activeAnimal))
            {
                return 0;
            }

            return -1;
        }

        public static bool TryAutoSwitchTwoAnimalParty(Player player, Animal activeAnimal)
        {
            ArgumentNullException.ThrowIfNull(player);
            ArgumentNullException.ThrowIfNull(activeAnimal);

            int otherAnimalIndex = GetOnlyOtherAnimalIndex(player, activeAnimal);

            if (otherAnimalIndex == -1 || IsFainted(player.AnimalInventory[otherAnimalIndex]))
            {
                return false;
            }

            int activeAnimalIndex = otherAnimalIndex == 0 ? 1 : 0;
            player.SwapAnimalPositions(activeAnimalIndex, otherAnimalIndex);
            return true;
        }

        public static bool TryCatchWildAnimal(Player player, Animal wildAnimal)
        {
            ArgumentNullException.ThrowIfNull(player);
            ArgumentNullException.ThrowIfNull(wildAnimal);

            if (player.AnimalInventory.Count >= 6)
            {
                return false;
            }

            player.AddAnimal(wildAnimal);
            player.CurrentRoom.RemoveEncounterAnimal(wildAnimal);
            return true;
        }

        public static void ReleaseAnimalAndCatchWildAnimal(Player player, Animal animalToRelease, Animal wildAnimal)
        {
            ArgumentNullException.ThrowIfNull(player);
            ArgumentNullException.ThrowIfNull(animalToRelease);
            ArgumentNullException.ThrowIfNull(wildAnimal);

            animalToRelease.CurrentHealth = animalToRelease.BaseHealth;
            player.CurrentRoom.AddEncounterAnimal(animalToRelease);
            player.RemoveAnimal(animalToRelease);

            player.AddAnimal(wildAnimal);
            player.CurrentRoom.RemoveEncounterAnimal(wildAnimal);
        }

        public static void LetWildAnimalRunAway(Player player, Animal wildAnimal)
        {
            ArgumentNullException.ThrowIfNull(player);
            ArgumentNullException.ThrowIfNull(wildAnimal);

            player.CurrentRoom.RemoveEncounterAnimal(wildAnimal);
        }

        private static BattleMoveResult RestoreHealth(Animal animal, Move move)
        {
            BattleMoveResult result = RestoreHealth(animal, move.Power);
            return result with { MoveName = move.Name };
        }

        private static BattleMoveResult ApplyDamage(Animal defender, Move move)
        {
            BattleMoveResult result = ApplyDamage(defender, move.Power);
            return result with { MoveName = move.Name };
        }
    }
}
