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
        public static BattleMoveResult UseMove(Pokemon attacker, Pokemon defender, Move move)
        {
            ArgumentNullException.ThrowIfNull(attacker);
            ArgumentNullException.ThrowIfNull(defender);
            ArgumentNullException.ThrowIfNull(move);

            return IsHealingMove(move)
                ? RestoreHealth(attacker, move)
                : ApplyDamage(defender, move);
        }

        public static BattleMoveResult RestoreHealth(Pokemon pokemon, int healingPower)
        {
            ArgumentNullException.ThrowIfNull(pokemon);

            if (healingPower < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(healingPower), "Healing power cannot be negative.");
            }

            if (pokemon.Health >= pokemon.BaseHealth)
            {
                return new BattleMoveResult(BattleMoveResultType.NoEffect, string.Empty, 0, pokemon.Health);
            }

            int originalHealth = pokemon.Health;
            pokemon.Health = Math.Min(pokemon.BaseHealth, pokemon.Health + healingPower);
            int restoredHealth = pokemon.Health - originalHealth;

            return new BattleMoveResult(BattleMoveResultType.Healing, string.Empty, restoredHealth, pokemon.Health);
        }

        public static BattleMoveResult ApplyDamage(Pokemon defender, int damage)
        {
            ArgumentNullException.ThrowIfNull(defender);

            if (damage < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(damage), "Damage cannot be negative.");
            }

            defender.Health = Math.Max(0, defender.Health - damage);
            return new BattleMoveResult(BattleMoveResultType.Damage, string.Empty, damage, defender.Health);
        }

        public static bool IsHealingMove(Move move)
        {
            ArgumentNullException.ThrowIfNull(move);
            return IsHealingMove(move.Name);
        }

        public static bool IsHealingMove(string moveName)
        {
            return moveName == "MOONLIGHT" || moveName == "SUNLIGHT";
        }

        public static bool IsFainted(Pokemon pokemon)
        {
            ArgumentNullException.ThrowIfNull(pokemon);
            return pokemon.Health <= 0;
        }

        public static bool IsBattleOver(Pokemon firstPokemon, Pokemon secondPokemon)
        {
            return IsFainted(firstPokemon) || IsFainted(secondPokemon);
        }

        public static bool HasUsablePokemon(GenericPlayer player)
        {
            ArgumentNullException.ThrowIfNull(player);
            return GetNextHealthyPokemonIndex(player) >= 0;
        }

        public static int GetNextHealthyPokemonIndex(GenericPlayer player, int startIndex = 0)
        {
            ArgumentNullException.ThrowIfNull(player);

            for (int i = startIndex; i < player.PokemonInventory.Count; ++i)
            {
                if (!IsFainted(player.PokemonInventory[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        private static BattleMoveResult RestoreHealth(Pokemon pokemon, Move move)
        {
            BattleMoveResult result = RestoreHealth(pokemon, move.Power);
            return result with { MoveName = move.Name };
        }

        private static BattleMoveResult ApplyDamage(Pokemon defender, Move move)
        {
            BattleMoveResult result = ApplyDamage(defender, move.Power);
            return result with { MoveName = move.Name };
        }
    }
}
