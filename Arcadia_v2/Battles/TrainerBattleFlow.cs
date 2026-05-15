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

        public static void Run(IGameIO io, Player main, CompPlayer opponent)
        {
            // Gym leaders rebuild a fresh runtime team here so earlier attempts cannot carry over damaged state.
            opponent.PrepareForBattle();
            BattleState battleState = BattleState.CreateTrainerBattle(main, opponent);

            io.WriteLine($"{main.Name} vs {opponent.Name}\n");
            io.WriteLine($"You sent out {battleState.PlayerPokemon.Name}");
            io.WriteLine($"{opponent.Name} sent out {battleState.OpponentPokemon.Name}");

            bool isPlayerTurn = true;

            while (!battleState.IsOver)
            {
                PrintBattleStatus(io, battleState);

                if (isPlayerTurn)
                {
                    HandlePlayerTurn(io, battleState);
                    HandleOpponentFaintedPokemon(io, battleState, opponent);
                }
                else
                {
                    HandleOpponentTurn(io, battleState, main);
                }

                isPlayerTurn = !isPlayerTurn;
            }

            FinishTrainerBattle(io, battleState, main, opponent);
        }

        // Prints the current health of both active Pokemon.
        private static void PrintBattleStatus(IGameIO io, BattleState battleState)
        {
            BattleHelpers.PrintBattleStatus(io, "opponents", battleState.PlayerPokemon, battleState.OpponentPokemon);
        }

        // Handles the player's turn by resolving the selected move against the opponent.
        private static void HandlePlayerTurn(IGameIO io, BattleState battleState)
        {
            BattleHelpers.HandlePlayerTurn(io, battleState.PlayerPokemon, battleState.OpponentPokemon, string.Empty);
        }

        // Handles the opponent's turn by choosing one random move from the opponent's active Pokemon.
        private static void HandleOpponentTurn(IGameIO io, BattleState battleState, Player main)
        {
            Pokemon opponentPokemon = battleState.OpponentPokemon;

            BattleHelpers.HandleOpponentTurn(io, opponentPokemon, battleState.PlayerPokemon, $"{opponentPokemon.Name} Move", string.Empty);
            BattleHelpers.HandlePlayerFaintedPokemon(io, main, "Would you like to switch Pokemon?");
            battleState.UseFirstPlayerPokemon();
        }

        // Handles the opponent sending out another Pokemon after the active one faints.
        private static void HandleOpponentFaintedPokemon(IGameIO io, BattleState battleState, CompPlayer opponent)
        {
            if (!BattleEngine.IsFainted(battleState.OpponentPokemon))
            {
                return;
            }

            if (!battleState.TrySwitchOpponentToNextHealthyPokemon(battleState.OpponentActiveIndex + 1))
            {
                return;
            }

            io.WriteLine($"\n{opponent.Name} sent out {battleState.OpponentPokemon.Name}\n");
        }

        // Handles the final result of the trainer battle.
        private static void FinishTrainerBattle(IGameIO io, BattleState battleState, Player main, CompPlayer opponent)
        {
            if (BattleEngine.IsFainted(battleState.PlayerPokemon))
            {
                io.WriteLine($"{battleState.PlayerPokemon.Name} fainted.");
                return;
            }

            if (BattleEngine.IsFainted(battleState.OpponentPokemon))
            {
                io.WriteLine($"{opponent.Name} defeated.");
                io.WriteLine("Congratulations! You defeated me. Please take this badge to honor your victory.");

                opponent.Defeated = true;
                main.AddBadge(opponent.Badges[0]);
            }
        }
    }
}
