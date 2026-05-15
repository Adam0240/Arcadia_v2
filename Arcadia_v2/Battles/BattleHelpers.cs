#nullable enable

using System;

namespace Arcadia_v2
{
    // Shared battle utilities used by both wild and trainer battle flows.
    public static class BattleHelpers
    {
        public static void PrintBattleStatus(string opponentLabel, Pokemon playerPokemon, Pokemon opponentPokemon)
        {
            Console.WriteLine($"Your {playerPokemon.Name}'s health is at: {playerPokemon.Health}");
            Console.WriteLine($"The {opponentLabel} {opponentPokemon.Name}'s health is at: {opponentPokemon.Health}\n");
        }

        public static void PrintMoveList(Pokemon pokemon, Func<Move, string> getMoveName)
        {
            for (int i = 0; i < pokemon.Moves.Count; ++i)
            {
                Console.Write($"{getMoveName(pokemon.Moves[i])} -- ");
            }
        }

        public static Move? FindMoveByName(Pokemon pokemon, string moveName, Func<Move, string> getMoveName)
        {
            foreach (Move move in pokemon.Moves)
            {
                if (getMoveName(move) == moveName)
                {
                    return move;
                }
            }

            return null;
        }

        public static Move GetRandomMove(Pokemon pokemon)
        {
            int moveIndex = Random.Shared.Next(pokemon.Moves.Count);
            return pokemon.Moves[moveIndex];
        }

        public static bool IsHealingMove(string moveName)
        {
            return moveName == "MOONLIGHT" || moveName == "SUNLIGHT";
        }

        public static void HandlePlayerTurn(Pokemon playerPokemon, Pokemon opponentPokemon, string defenderLabel)
        {
            while (true)
            {
                Console.WriteLine("Your moves.");
                PrintMoveList(playerPokemon, move => move.Name);

                Console.WriteLine("\n\nEnter your move.");
                string attackMove = Program.ReadUpperTrimmedInput();

                Move? selectedMove = FindMoveByName(playerPokemon, attackMove, move => move.Name);

                if (selectedMove == null)
                {
                    Console.WriteLine($"{attackMove} is an invalid move.");
                    continue;
                }

                Console.WriteLine($"You used {selectedMove.Name}");

                if (IsHealingMove(selectedMove.Name))
                {
                    UseHealingMove(playerPokemon, selectedMove.Power);
                    return;
                }

                UseAttackMove(string.Empty, playerPokemon, defenderLabel, opponentPokemon, selectedMove.Power, selectedMove.Name);
                return;
            }
        }

        public static void HandleOpponentTurn(Pokemon opponentPokemon, Pokemon playerPokemon, string moveHeader, string defenderLabel)
        {
            if (IsBattleOver(playerPokemon, opponentPokemon))
            {
                return;
            }

            Move selectedMove = GetRandomMove(opponentPokemon);

            Console.WriteLine(moveHeader);
            UseAttackMove(string.Empty, opponentPokemon, defenderLabel, playerPokemon, selectedMove.Power, selectedMove.Name);
        }

        public static void HandlePlayerFaintedPokemon(Player mainPlayer, string prompt)
        {
            Pokemon playerPokemon = mainPlayer.PokemonInventory[0];

            if (playerPokemon.Health > 0)
            {
                return;
            }

            Console.WriteLine($"{playerPokemon.Name} fainted.");
            Console.WriteLine(prompt);

            if (IsYes(Program.ReadUpperTrimmedInput()))
            {
                PartyFlow.SwapPokemon(mainPlayer);
            }
        }

        public static void UseHealingMove(Pokemon pokemon, int healingPower)
        {
            if (pokemon.Health >= pokemon.BaseHealth - healingPower)
            {
                Console.WriteLine("Nothing happened");
                return;
            }

            pokemon.Health = Math.Min(pokemon.BaseHealth, pokemon.Health + healingPower);

            Console.WriteLine("Health Restored");
            Console.WriteLine(pokemon.Health);
        }

        public static void UseAttackMove(string attackerLabel, Pokemon attacker, string defenderLabel, Pokemon defender, int movePower, string moveName)
        {
            Console.WriteLine($"{attackerLabel}{attacker.Name} used {moveName}");
            Console.WriteLine($"{defenderLabel}{defender.Name} took {movePower} damage.");

            Program.ApplyDamage(defender, movePower);
        }

        public static bool IsBattleOver(Pokemon playerPokemon, Pokemon opponentPokemon)
        {
            return playerPokemon.Health <= 0 || opponentPokemon.Health <= 0;
        }

        public static bool IsYes(string answer)
        {
            return answer == "YES" || answer == "Y";
        }

        public static bool IsNo(string answer)
        {
            return answer == "NO" || answer == "N";
        }
    }
}
