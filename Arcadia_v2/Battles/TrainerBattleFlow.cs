#nullable enable

using System;

namespace Arcadia_v2
{
    // Runs the trainer-versus-trainer battle loop used for gyms and the champion.
    public static class TrainerBattleFlow
    {
        public static void Run(Player main, CompPlayer opponent)
        {
            var mainPokemonInventory = main.PokemonInventory;
            var opponentPokemonInventory = opponent.PokemonInventory;

            // Gym leaders rebuild a fresh runtime team here so earlier attempts cannot carry over damaged state.
            opponent.PrepareForBattle();

            Console.WriteLine($"{main.Name} vs {opponent.Name}\n");
            Console.WriteLine($"You sent out {mainPokemonInventory[0].Name}");
            Console.WriteLine($"{opponent.Name} sent out {opponentPokemonInventory[0].Name}");

            bool isPlayerTurn = true;

            while (!IsBattleOver(mainPokemonInventory[0], opponentPokemonInventory[0]))
            {
                PrintBattleStatus(mainPokemonInventory[0], opponentPokemonInventory[0]);

                if (isPlayerTurn)
                {
                    HandlePlayerTurn(main, opponent);
                    HandleOpponentFaintedPokemon(opponent);
                }
                else
                {
                    HandleOpponentTurn(main, opponent);
                }

                isPlayerTurn = !isPlayerTurn;
            }

            FinishTrainerBattle(main, opponent);
        }

        // Prints the current health of both active Pokemon.
        private static void PrintBattleStatus(Pokemon playerPokemon, Pokemon opponentPokemon)
        {
            BattleHelpers.PrintBattleStatus("opponents", playerPokemon, opponentPokemon);
        }

        // Handles the player's turn by resolving the selected move against the opponent.
        private static void HandlePlayerTurn(Player main, CompPlayer opponent)
        {
            Pokemon playerPokemon = main.PokemonInventory[0];
            Pokemon opponentPokemon = opponent.PokemonInventory[0];
            BattleHelpers.HandlePlayerTurn(playerPokemon, opponentPokemon, string.Empty);
        }

        // Handles the opponent's turn by choosing one random move from the opponent's active Pokemon.
        private static void HandleOpponentTurn(Player main, CompPlayer opponent)
        {
            Pokemon playerPokemon = main.PokemonInventory[0];
            Pokemon opponentPokemon = opponent.PokemonInventory[0];

            BattleHelpers.HandleOpponentTurn(opponentPokemon, playerPokemon, $"{opponentPokemon.Name} Move", string.Empty);
            BattleHelpers.HandlePlayerFaintedPokemon(main, "Would you like to switch Pokemon?");
        }

        // Handles the opponent sending out another Pokemon after the active one faints.
        private static void HandleOpponentFaintedPokemon(CompPlayer opponent)
        {
            var opponentPokemonInventory = opponent.PokemonInventory;

            if (opponentPokemonInventory[0].Health > 0)
            {
                return;
            }

            int nextPokemonIndex = GetNextAvailablePokemonIndex(opponent);

            if (nextPokemonIndex == -1)
            {
                return;
            }

            opponent.SwapPokemonPositions(0, nextPokemonIndex);
            Console.WriteLine($"\n{opponent.Name} sent out {opponentPokemonInventory[0].Name}\n");
        }

        // Finds the next Pokemon on the opponent's team that still has health remaining.
        private static int GetNextAvailablePokemonIndex(CompPlayer opponent)
        {
            var opponentPokemonInventory = opponent.PokemonInventory;

            for (int i = 1; i < opponentPokemonInventory.Count; i++)
            {
                if (opponentPokemonInventory[i].Health > 0)
                {
                    return i;
                }
            }

            return -1;
        }

        // Checks whether either active Pokemon has fainted.
        private static bool IsBattleOver(Pokemon playerPokemon, Pokemon opponentPokemon)
        {
            return BattleHelpers.IsBattleOver(playerPokemon, opponentPokemon);
        }

        // Handles the final result of the trainer battle.
        private static void FinishTrainerBattle(Player main, CompPlayer opponent)
        {
            Pokemon playerPokemon = main.PokemonInventory[0];
            Pokemon opponentPokemon = opponent.PokemonInventory[0];

            if (playerPokemon.Health <= 0)
            {
                Console.WriteLine($"{playerPokemon.Name} fainted.");
                return;
            }

            if (opponentPokemon.Health <= 0)
            {
                Console.WriteLine($"{opponent.Name} defeated.");
                Console.WriteLine("Congratulations! You defeated me. Please take this badge to honor your victory.");

                opponent.Defeated = true;
                main.AddBadge(opponent.Badges[0]);
            }
        }
    }
}
