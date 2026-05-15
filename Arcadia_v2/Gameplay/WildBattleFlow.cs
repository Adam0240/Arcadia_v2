#nullable enable

namespace Arcadia_v2
{
    // Handles wild Pokemon battles, including player moves, wild Pokemon moves,
    // switching after fainting, and catch/release flow.
    public static class WildBattleFlow
    {
        public static void HandleWildBattle(IGameIO io, GameState gameState)
        {
            Player mainPlayer = gameState.MainPlayer;

            if (!mainPlayer.CurrentRoom.HasEncounterPokemon())
            {
                io.WriteLine("No pokemon nearby");
                return;
            }

            Pokemon wildPokemon = mainPlayer.CurrentRoom.EncounterPokemon[0];
            BattleState battleState = BattleState.CreateWildBattle(mainPlayer, wildPokemon);

            io.WriteLine($"A wild {wildPokemon.Name} attacked!");

            bool isPlayerTurn = true;

            while (!battleState.IsOver)
            {
                PrintBattleStatus(io, battleState);

                if (isPlayerTurn)
                {
                    HandlePlayerTurn(io, battleState);
                }
                else
                {
                    HandleWildPokemonTurn(io, mainPlayer, battleState);
                }

                isPlayerTurn = !isPlayerTurn;
            }

            FinishWildBattle(io, mainPlayer, battleState);
        }

        // Prints the current health for the player's active Pokemon and the wild Pokemon.
        private static void PrintBattleStatus(IGameIO io, BattleState battleState)
        {
            BattleHelpers.PrintBattleStatus(io, "wild", battleState.PlayerPokemon, battleState.OpponentPokemon);
        }

        // Handles the player's turn by reading a move and applying the selected move's effect.
        private static void HandlePlayerTurn(IGameIO io, BattleState battleState)
        {
            BattleHelpers.HandlePlayerTurn(io, battleState.PlayerPokemon, battleState.OpponentPokemon, "The wild ");
        }

        // Handles the wild Pokemon's turn by selecting one random move and applying damage.
        private static void HandleWildPokemonTurn(IGameIO io, Player mainPlayer, BattleState battleState)
        {
            Pokemon wildPokemon = battleState.OpponentPokemon;

            BattleHelpers.HandleOpponentTurn(io, wildPokemon, battleState.PlayerPokemon, $"{wildPokemon.Name} Move", string.Empty);
            BattleHelpers.HandlePlayerFaintedPokemon(io, mainPlayer, "Would you like to switch Pokemon? (YES/NO)");
            battleState.UseFirstPlayerPokemon();
        }

        // Finishes the wild battle after either the player's Pokemon or the wild Pokemon faints.
        private static void FinishWildBattle(IGameIO io, Player mainPlayer, BattleState battleState)
        {
            if (BattleEngine.IsFainted(battleState.PlayerPokemon))
            {
                io.WriteLine($"{battleState.PlayerPokemon.Name} fainted.");
                return;
            }

            if (BattleEngine.IsFainted(battleState.OpponentPokemon))
            {
                io.WriteLine($"{battleState.OpponentPokemon.Name} fainted.");
                HandleCatchChoice(io, mainPlayer, battleState.OpponentPokemon);
            }
        }

        // Handles the player's choice to catch or leave the fainted wild Pokemon.
        private static void HandleCatchChoice(IGameIO io, Player mainPlayer, Pokemon wildPokemon)
        {
            while (true)
            {
                io.WriteLine($"Would you like to catch {wildPokemon.Name}? -- (yes or no)");
                string answer = Program.ReadUpperTrimmedInput(io);

                if (BattleHelpers.IsYes(answer))
                {
                    CatchPokemon(io, mainPlayer, wildPokemon);
                    return;
                }

                if (BattleHelpers.IsNo(answer))
                {
                    LetWildPokemonRunAway(io, mainPlayer, wildPokemon);
                    return;
                }

                io.WriteLine("Invalid input.");
            }
        }

        // Catches the wild Pokemon if the player has space, otherwise asks whether to release one.
        private static void CatchPokemon(IGameIO io, Player mainPlayer, Pokemon wildPokemon)
        {
            if (mainPlayer.PokemonInventory.Count < 6)
            {
                AddCaughtPokemon(io, mainPlayer, wildPokemon);
                return;
            }

            io.WriteLine($"\n{mainPlayer.Name}'s PokeInventory is full.");
            io.WriteLine("You can only have 6 Pokemon with you at a time.");

            while (true)
            {
                io.WriteLine("Would you like to release a Pokemon? -- (yes or no)");
                string answer = Program.ReadUpperTrimmedInput(io);

                if (BattleHelpers.IsYes(answer))
                {
                    ReleasePokemonAndCatchWildPokemon(io, mainPlayer, wildPokemon);
                    return;
                }

                if (BattleHelpers.IsNo(answer))
                {
                    LetWildPokemonRunAway(io, mainPlayer, wildPokemon);
                    return;
                }

                io.WriteLine("Invalid input.");
            }
        }

        // Releases one Pokemon from the player's party, then catches the wild Pokemon.
        private static void ReleasePokemonAndCatchWildPokemon(IGameIO io, Player mainPlayer, Pokemon wildPokemon)
        {
            io.WriteLine(mainPlayer.GetPokemonInventoryDisplay());

            Pokemon pokemonToRelease = AskForPokemonToRelease(io, mainPlayer);

            io.WriteLine($"You are releasing: {pokemonToRelease.Name}");

            pokemonToRelease.Health = 20;

            mainPlayer.CurrentRoom.AddEncounterPokemon(pokemonToRelease);
            mainPlayer.RemovePokemon(pokemonToRelease);

            AddCaughtPokemon(io, mainPlayer, wildPokemon);
        }

        // Keeps asking until the player enters the name of a Pokemon currently in their party.
        private static Pokemon AskForPokemonToRelease(IGameIO io, Player mainPlayer)
        {
            while (true)
            {
                io.WriteLine("Who would you like to release?");
                string pokemonName = Program.ReadUpperTrimmedInput(io);

                Pokemon? pokemonToRelease = FindPokemonByName(mainPlayer, pokemonName);

                if (pokemonToRelease != null)
                {
                    return pokemonToRelease;
                }

                io.WriteLine($"{pokemonName} is not a valid Pokemon name.");
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
        private static void AddCaughtPokemon(IGameIO io, Player mainPlayer, Pokemon wildPokemon)
        {
            mainPlayer.AddPokemon(wildPokemon);
            mainPlayer.CurrentRoom.RemoveEncounterPokemon(wildPokemon);

            io.WriteLine($"You caught {wildPokemon.Name}!");
        }

        // Removes the wild Pokemon from the room after the player chooses not to catch it.
        private static void LetWildPokemonRunAway(IGameIO io, Player mainPlayer, Pokemon wildPokemon)
        {
            io.WriteLine($"{wildPokemon.Name} ran away!");
            mainPlayer.CurrentRoom.RemoveEncounterPokemon(wildPokemon);
        }

    }
}
