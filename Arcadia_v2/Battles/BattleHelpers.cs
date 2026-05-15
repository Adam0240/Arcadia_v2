#nullable enable

using System;

namespace Arcadia_v2
{
    // Shared battle utilities used by both wild and trainer battle flows.
    public static class BattleHelpers
    {
        public static void PrintBattleStatus(IGameIO io, string opponentLabel, Pokemon playerPokemon, Pokemon opponentPokemon)
        {
            io.WriteLine($"Your {playerPokemon.Name}'s health is at: {playerPokemon.Health}");
            io.WriteLine($"The {opponentLabel} {opponentPokemon.Name}'s health is at: {opponentPokemon.Health}\n");
        }

        public static void PrintMoveList(IGameIO io, Pokemon pokemon, Func<Move, string> getMoveName)
        {
            for (int i = 0; i < pokemon.Moves.Count; ++i)
            {
                io.Write($"{getMoveName(pokemon.Moves[i])} -- ");
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
            return BattleEngine.IsHealingMove(moveName);
        }

        public static void HandlePlayerTurn(IGameIO io, Pokemon playerPokemon, Pokemon opponentPokemon, string defenderLabel)
        {
            while (true)
            {
                io.WriteLine("Your moves.");
                PrintMoveList(io, playerPokemon, move => move.Name);

                io.WriteLine("\n\nEnter your move.");
                string attackMove = Program.ReadUpperTrimmedInput(io);

                Move? selectedMove = FindMoveByName(playerPokemon, attackMove, move => move.Name);

                if (selectedMove == null)
                {
                    io.WriteLine($"{attackMove} is an invalid move.");
                    continue;
                }

                io.WriteLine($"You used {selectedMove.Name}");

                BattleMoveResult result = BattleEngine.UseMove(playerPokemon, opponentPokemon, selectedMove);
                PrintMoveResult(io, string.Empty, playerPokemon, defenderLabel, opponentPokemon, result);
                return;
            }
        }

        public static void HandleOpponentTurn(IGameIO io, Pokemon opponentPokemon, Pokemon playerPokemon, string moveHeader, string defenderLabel)
        {
            if (BattleEngine.IsBattleOver(playerPokemon, opponentPokemon))
            {
                return;
            }

            Move selectedMove = GetRandomMove(opponentPokemon);

            io.WriteLine(moveHeader);
            BattleMoveResult result = BattleEngine.UseMove(opponentPokemon, playerPokemon, selectedMove);
            PrintMoveResult(io, string.Empty, opponentPokemon, defenderLabel, playerPokemon, result);
        }

        public static void HandlePlayerFaintedPokemon(IGameIO io, Player mainPlayer, string prompt)
        {
            Pokemon playerPokemon = mainPlayer.PokemonInventory[0];

            if (!BattleEngine.IsFainted(playerPokemon))
            {
                return;
            }

            io.WriteLine($"{playerPokemon.Name} fainted.");

            while (true)
            {
                io.WriteLine(prompt);
                string answer = Program.ReadUpperTrimmedInput(io);

                if (IsYes(answer))
                {
                    PartyFlow.SwapPokemon(mainPlayer, io);
                    return;
                }

                if (IsNo(answer))
                {
                    return;
                }

                io.WriteLine("Invalid input.");
            }
        }

        public static void UseHealingMove(IGameIO io, Pokemon pokemon, int healingPower)
        {
            BattleMoveResult result = BattleEngine.RestoreHealth(pokemon, healingPower);
            PrintHealingResult(io, result);
        }

        public static void UseAttackMove(IGameIO io, string attackerLabel, Pokemon attacker, string defenderLabel, Pokemon defender, int movePower, string moveName)
        {
            io.WriteLine($"{attackerLabel}{attacker.Name} used {moveName}");
            io.WriteLine($"{defenderLabel}{defender.Name} took {movePower} damage.");

            BattleEngine.ApplyDamage(defender, movePower);
        }

        private static void PrintMoveResult(
            IGameIO io,
            string attackerLabel,
            Pokemon attacker,
            string defenderLabel,
            Pokemon defender,
            BattleMoveResult result)
        {
            if (result.ResultType is BattleMoveResultType.Healing or BattleMoveResultType.NoEffect)
            {
                PrintHealingResult(io, result);
                return;
            }

            io.WriteLine($"{attackerLabel}{attacker.Name} used {result.MoveName}");
            io.WriteLine($"{defenderLabel}{defender.Name} took {result.Amount} damage.");
        }

        private static void PrintHealingResult(IGameIO io, BattleMoveResult result)
        {
            if (result.ResultType == BattleMoveResultType.NoEffect)
            {
                io.WriteLine("Nothing happened");
                return;
            }

            io.WriteLine("Health Restored");
            io.WriteLine(result.TargetHealth.ToString());
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
