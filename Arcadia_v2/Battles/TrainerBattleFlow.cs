#nullable enable

using System;

namespace Arcadia_v2
{
    // Runs the trainer-versus-trainer battle loop used for gyms and the champion.
    public static class TrainerBattleFlow
    {
        public static void Run(Player main, CompPlayer opponent)
        {
            Run(new ConsoleGameIO(), main, opponent);
        }

        public static void Run(IGameIO io, GameState gameState, CompPlayer opponent)
        {
            Run(io, gameState.MainPlayer, opponent);
        }

        public static void Run(IGameIO io, GameState gameState, CompPlayer opponent, IBattleMoveSelector moveSelector)
        {
            Run(io, gameState.MainPlayer, opponent, moveSelector);
        }

        public static void Run(IGameIO io, Player main, CompPlayer opponent)
        {
            Run(io, main, opponent, RandomBattleMoveSelector.Instance);
        }

        public static void Run(IGameIO io, Player main, CompPlayer opponent, IBattleMoveSelector moveSelector)
        {
            if (!BattleEngine.HasUsableAnimals(main))
            {
                io.WriteLine("All animals in your party are fainted.");
                return;
            }

            // Gym leaders rebuild a fresh runtime team here so earlier attempts cannot carry over damaged state.
            opponent.PrepareForBattle();
            BattleState battleState = BattleState.CreateTrainerBattle(main, opponent);

            io.WriteLine($"{main.Name} vs {opponent.Name}\n");
            io.WriteLine($"You sent out {battleState.PlayerAnimal.Name}");
            io.WriteLine($"{opponent.Name} sent out {battleState.OpponentAnimal.Name}");

            bool isPlayerTurn = true;

            while (!battleState.IsOver)
            {
                PrintBattleStatus(io, battleState);

                if (isPlayerTurn)
                {
                    HandlePlayerTurn(io, battleState);
                    HandleOpponentFaintedAnimal(io, battleState, opponent);
                }
                else
                {
                    HandleOpponentTurn(io, battleState, main, moveSelector);
                }

                isPlayerTurn = !isPlayerTurn;
            }

            FinishTrainerBattle(io, battleState, main, opponent);
        }

        // Prints the current health of both active animals.
        private static void PrintBattleStatus(IGameIO io, BattleState battleState)
        {
            BattleHelpers.PrintBattleStatus(io, "opponents", battleState.PlayerAnimal, battleState.OpponentAnimal);
        }

        // Handles the player's turn by resolving the selected move against the opponent.
        private static void HandlePlayerTurn(IGameIO io, BattleState battleState)
        {
            BattleHelpers.HandlePlayerTurn(io, battleState.PlayerAnimal, battleState.OpponentAnimal, string.Empty);
        }

        // Handles the opponent's turn by choosing one random move from the opponent's active animal.
        private static void HandleOpponentTurn(IGameIO io, BattleState battleState, Player main, IBattleMoveSelector moveSelector)
        {
            Animal opponentAnimal = battleState.OpponentAnimal;

            BattleHelpers.HandleOpponentTurn(io, opponentAnimal, battleState.PlayerAnimal, $"{opponentAnimal.Name} Move", string.Empty, moveSelector);

            if (BattleHelpers.HandlePlayerFaintedAnimal(io, main, battleState.PlayerAnimal, "Would you like to switch animals?"))
            {
                battleState.UseFirstHealthyPlayerAnimal();
            }
        }

        // Handles the opponent sending out another animal after the active one faints.
        private static void HandleOpponentFaintedAnimal(IGameIO io, BattleState battleState, CompPlayer opponent)
        {
            if (!BattleEngine.IsFainted(battleState.OpponentAnimal))
            {
                return;
            }

            if (!battleState.TrySwitchOpponentToNextHealthyAnimal(battleState.OpponentActiveIndex + 1))
            {
                return;
            }

            io.WriteLine($"\n{opponent.Name} sent out {battleState.OpponentAnimal.Name}\n");
        }

        // Handles the final result of the trainer battle.
        private static void FinishTrainerBattle(IGameIO io, BattleState battleState, Player main, CompPlayer opponent)
        {
            if (BattleEngine.IsFainted(battleState.PlayerAnimal))
            {
                io.WriteLine($"{battleState.PlayerAnimal.Name} fainted.");
                return;
            }

            if (BattleEngine.IsFainted(battleState.OpponentAnimal))
            {
                io.WriteLine($"{opponent.Name} defeated.");
                io.WriteLine("Congratulations! You defeated me. Please take this badge to honor your victory.");

                opponent.Defeated = true;
                main.AddBadge(opponent.Badges[0]);
            }
        }
    }
}
