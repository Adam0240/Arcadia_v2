#nullable enable

using System;
using System.Collections.Generic;

namespace Arcadia_v2
{
    // Handles wild Pokemon battles, including player moves, wild Pokemon moves,
    // switching after fainting, and catch/release flow.
    public static class WildBattleFlow
    {
        public static void HandleWildBattle(Player mainPlayer, List<Pokemon> mainPokemon)
        {
            if (!mainPlayer.CurrentRoom.HasEncounterPokemon())
            {
                Console.WriteLine("No pokemon nearby");
                return;
            }

            Pokemon playerPokemon = mainPlayer.PokemonInventory[0];
            Pokemon wildPokemon = mainPlayer.CurrentRoom.EncounterPokemon[0];

            Console.WriteLine($"A wild {wildPokemon.Name} attacked!");

            bool isPlayerTurn = true;

            while (!IsBattleOver(playerPokemon, wildPokemon))
            {
                PrintBattleStatus(playerPokemon, wildPokemon);

                if (isPlayerTurn)
                {
                    HandlePlayerTurn(playerPokemon, wildPokemon);
                }
                else
                {
                    HandleWildPokemonTurn(mainPlayer, wildPokemon);
                    playerPokemon = mainPlayer.PokemonInventory[0];
                }

                isPlayerTurn = !isPlayerTurn;
            }

            FinishWildBattle(mainPlayer, playerPokemon, wildPokemon);
        }

        // Prints the current health for the player's active Pokemon and the wild Pokemon.
        private static void PrintBattleStatus(Pokemon playerPokemon, Pokemon wildPokemon)
        {
            BattleHelpers.PrintBattleStatus("wild", playerPokemon, wildPokemon);
        }

        // Handles the player's turn by reading a move and applying the selected move's effect.
        private static void HandlePlayerTurn(Pokemon playerPokemon, Pokemon wildPokemon)
        {
            BattleHelpers.HandlePlayerTurn(playerPokemon, wildPokemon, "The wild ");
        }

        // Handles the wild Pokemon's turn by selecting one random move and applying damage.
        private static void HandleWildPokemonTurn(Player mainPlayer, Pokemon wildPokemon)
        {
            Pokemon playerPokemon = mainPlayer.PokemonInventory[0];

            BattleHelpers.HandleOpponentTurn(wildPokemon, playerPokemon, $"{wildPokemon.Name} Move", string.Empty);
            BattleHelpers.HandlePlayerFaintedPokemon(mainPlayer, "Would you like to switch Pokemon? (YES/NO)");
        }

        // Finishes the wild battle after either the player's Pokemon or the wild Pokemon faints.
        private static void FinishWildBattle(Player mainPlayer, Pokemon playerPokemon, Pokemon wildPokemon)
        {
            if (playerPokemon.Health <= 0)
            {
                Console.WriteLine($"{playerPokemon.Name} fainted.");
                return;
            }

            if (wildPokemon.Health <= 0)
            {
                Console.WriteLine($"{wildPokemon.Name} fainted.");
                HandleCatchChoice(mainPlayer, wildPokemon);
            }
        }

        // Handles the player's choice to catch or leave the fainted wild Pokemon.
        private static void HandleCatchChoice(Player mainPlayer, Pokemon wildPokemon)
        {
            while (true)
            {
                Console.WriteLine($"Would you like to catch {wildPokemon.Name}? -- (yes or no)");
                string answer = Program.ReadUpperTrimmedInput();

                if (BattleHelpers.IsYes(answer))
                {
                    CatchPokemon(mainPlayer, wildPokemon);
                    return;
                }

                if (BattleHelpers.IsNo(answer))
                {
                    LetWildPokemonRunAway(mainPlayer, wildPokemon);
                    return;
                }

                Console.WriteLine("Invalid input.");
            }
        }

        // Catches the wild Pokemon if the player has space, otherwise asks whether to release one.
        private static void CatchPokemon(Player mainPlayer, Pokemon wildPokemon)
        {
            if (mainPlayer.PokemonInventory.Count < 6)
            {
                AddCaughtPokemon(mainPlayer, wildPokemon);
                return;
            }

            Console.WriteLine($"\n{mainPlayer.Name}'s PokeInventory is full.");
            Console.WriteLine("You can only have 6 Pokemon with you at a time.");
            Console.WriteLine("Would you like to release a Pokemon? -- (yes or no)");

            string answer = Program.ReadUpperTrimmedInput();

            if (BattleHelpers.IsYes(answer))
            {
                ReleasePokemonAndCatchWildPokemon(mainPlayer, wildPokemon);
                return;
            }

            if (BattleHelpers.IsNo(answer))
            {
                LetWildPokemonRunAway(mainPlayer, wildPokemon);
                return;
            }

            Console.WriteLine("Invalid input.");
        }

        // Releases one Pokemon from the player's party, then catches the wild Pokemon.
        private static void ReleasePokemonAndCatchWildPokemon(Player mainPlayer, Pokemon wildPokemon)
        {
            Console.WriteLine(mainPlayer.GetPokemonInventoryDisplay());

            Pokemon pokemonToRelease = AskForPokemonToRelease(mainPlayer);

            Console.WriteLine($"You are releasing: {pokemonToRelease.Name}");

            pokemonToRelease.Health = 20;

            mainPlayer.CurrentRoom.AddEncounterPokemon(pokemonToRelease);
            mainPlayer.RemovePokemon(pokemonToRelease);

            AddCaughtPokemon(mainPlayer, wildPokemon);
        }

        // Keeps asking until the player enters the name of a Pokemon currently in their party.
        private static Pokemon AskForPokemonToRelease(Player mainPlayer)
        {
            while (true)
            {
                Console.WriteLine("Who would you like to release?");
                string pokemonName = Program.ReadUpperTrimmedInput();

                Pokemon? pokemonToRelease = FindPokemonByName(mainPlayer, pokemonName);

                if (pokemonToRelease != null)
                {
                    return pokemonToRelease;
                }

                Console.WriteLine($"{pokemonName} is not a valid Pokemon name.");
            }
        }

        // Finds a Pokemon in the player's party by name.
        private static Pokemon? FindPokemonByName(Player mainPlayer, string pokemonName)
        {
            foreach (Pokemon pokemon in mainPlayer.PokemonInventory)
            {
                if (pokemon.Name == pokemonName)
                {
                    return pokemon;
                }
            }

            return null;
        }

        // Adds the wild Pokemon to the player's party and removes it from the room.
        private static void AddCaughtPokemon(Player mainPlayer, Pokemon wildPokemon)
        {
            mainPlayer.AddPokemon(wildPokemon);
            mainPlayer.CurrentRoom.RemoveEncounterPokemon(wildPokemon);

            Console.WriteLine($"You caught {wildPokemon.Name}!");
        }

        // Removes the wild Pokemon from the room after the player chooses not to catch it.
        private static void LetWildPokemonRunAway(Player mainPlayer, Pokemon wildPokemon)
        {
            Console.WriteLine($"{wildPokemon.Name} ran away!");
            mainPlayer.CurrentRoom.RemoveEncounterPokemon(wildPokemon);
        }

        // Checks whether either active Pokemon has fainted.
        private static bool IsBattleOver(Pokemon playerPokemon, Pokemon wildPokemon)
        {
            return BattleHelpers.IsBattleOver(playerPokemon, wildPokemon);
        }

    }
}
