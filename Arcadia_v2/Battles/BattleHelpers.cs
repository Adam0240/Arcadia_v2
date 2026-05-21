#nullable enable

using Arcadia_v2.Creatures;
using System;

namespace Arcadia_v2
{
    public enum PlayerDefeatedAnimalResult
    {
        NotDefeated,
        Switched,
        DefeatedNoSwitch
    }

    // Shared battle utilities used by both wild and trainer battle flows.
    public static class BattleHelpers
    {
        public static void PrintBattleStatus(IGameIO io, string opponentLabel, Animal playerAnimal, Animal opponentAnimal)
        {
            io.WriteLine($"Your {playerAnimal.Name}'s health is at: {playerAnimal.Health}");
            io.WriteLine($"The {opponentLabel} {opponentAnimal.Name}'s health is at: {opponentAnimal.Health}\n");
        }

        public static void PrintMoveList(IGameIO io, Animal animal, Func<Move, string> getMoveName)
        {
            for (int i = 0; i < animal.Moves.Count; ++i)
            {
                io.Write($"{i + 1}. {getMoveName(animal.Moves[i])} -- ");
            }
        }

        public static Move? FindMoveByNumber(Animal animal, string moveNumber)
        {
            if (!int.TryParse(moveNumber, out int selectedMoveNumber))
            {
                return null;
            }

            int selectedMoveIndex = selectedMoveNumber - 1;
            return selectedMoveIndex >= 0 && selectedMoveIndex < animal.Moves.Count
                ? animal.Moves[selectedMoveIndex]
                : null;
        }

        public static void HandlePlayerTurn(IGameIO io, Animal playerAnimal, Animal opponentAnimal, string defenderLabel)
        {
            while (true)
            {
                io.WriteLine("Your moves.");
                PrintMoveList(io, playerAnimal, move => move.Name);

                io.WriteLine("\n\nEnter your move number.");
                string attackMove = Program.ReadUpperTrimmedInput(io);

                Move? selectedMove = FindMoveByNumber(playerAnimal, attackMove);

                if (selectedMove == null)
                {
                    io.WriteLine($"{attackMove} is an invalid move.");
                    continue;
                }

                io.WriteLine($"You used {selectedMove.Name}");

                BattleMoveResult result = BattleEngine.UseMove(playerAnimal, opponentAnimal, selectedMove);
                PrintMoveResult(io, string.Empty, playerAnimal, defenderLabel, opponentAnimal, result);
                return;
            }
        }

        public static void HandleOpponentTurn(IGameIO io, Animal opponentAnimal, Animal playerAnimal, string moveHeader, string defenderLabel)
        {
            HandleOpponentTurn(io, opponentAnimal, playerAnimal, moveHeader, defenderLabel, RandomBattleMoveSelector.Instance);
        }

        public static void HandleOpponentTurn(
            IGameIO io,
            Animal opponentAnimal,
            Animal playerAnimal,
            string moveHeader,
            string defenderLabel,
            IBattleMoveSelector moveSelector)
        {
            if (BattleEngine.IsBattleOver(playerAnimal, opponentAnimal))
            {
                return;
            }

            Move selectedMove = moveSelector.SelectMove(opponentAnimal);

            io.WriteLine(moveHeader);
            BattleMoveResult result = BattleEngine.UseMove(opponentAnimal, playerAnimal, selectedMove);
            PrintMoveResult(io, string.Empty, opponentAnimal, defenderLabel, playerAnimal, result);
        }

        public static PlayerDefeatedAnimalResult HandlePlayerDefeatedAnimal(IGameIO io, Player mainPlayer, string prompt)
        {
            return HandlePlayerDefeatedAnimal(io, mainPlayer, mainPlayer.AnimalInventory[0], prompt);
        }

        public static PlayerDefeatedAnimalResult HandlePlayerDefeatedAnimal(IGameIO io, Player mainPlayer, Animal playerAnimal, string prompt)
        {
            if (!BattleEngine.IsDefeated(playerAnimal))
            {
                return PlayerDefeatedAnimalResult.NotDefeated;
            }

            if (BattleEngine.CanAutoSwapTwoAnimalParty(mainPlayer))
            {
                return BattleEngine.TryAutoSwitchTwoAnimalParty(mainPlayer, playerAnimal)
                    ? PlayerDefeatedAnimalResult.Switched
                    : PlayerDefeatedAnimalResult.DefeatedNoSwitch;
            }

            while (true)
            {
                io.WriteLine(prompt);
                string answer = Program.ReadUpperTrimmedInput(io);

                if (IsYes(answer))
                {
                    PartyFlow.SwapAnimals(mainPlayer, io);
                    return PlayerDefeatedAnimalResult.Switched;
                }

                if (IsNo(answer))
                {
                    return PlayerDefeatedAnimalResult.DefeatedNoSwitch;
                }

                io.WriteLine("Invalid input.");
            }
        }

        public static void UseHealingMove(IGameIO io, Animal animal, int healingPower)
        {
            BattleMoveResult result = BattleEngine.RestoreHealth(animal, healingPower);
            PrintHealingResult(io, result);
        }

        public static void UseAttackMove(IGameIO io, string attackerLabel, Animal attacker, string defenderLabel, Animal defender, int movePower, string moveName)
        {
            io.WriteLine($"{attackerLabel}{attacker.Name} used {moveName}");
            io.WriteLine($"{defenderLabel}{defender.Name} took {movePower} damage.");

            BattleEngine.ApplyDamage(defender, movePower);
        }

        private static void PrintMoveResult(
            IGameIO io,
            string attackerLabel,
            Animal attacker,
            string defenderLabel,
            Animal defender,
            BattleMoveResult result)
        {
            io.WriteLine($"{attackerLabel}{attacker.Name} used {result.MoveName}");

            if (result.ResultType is BattleMoveResultType.Healing or BattleMoveResultType.NoEffect)
            {
                PrintHealingResult(io, result);
                return;
            }

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
