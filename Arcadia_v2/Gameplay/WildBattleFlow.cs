#nullable enable

namespace Arcadia_v2
{
    // Handles wild animal battles, including player moves, wild animal moves,
    // switching after fainting, and catch/release flow.
    public static class WildBattleFlow
    {
        public static void HandleWildBattle(IGameIO io, GameState gameState)
        {
            HandleWildBattle(io, gameState, RandomBattleMoveSelector.Instance);
        }

        public static void HandleWildBattle(IGameIO io, GameState gameState, IBattleMoveSelector moveSelector)
        {
            Player mainPlayer = gameState.MainPlayer;

            if (!BattleEngine.HasUsableAnimals(mainPlayer))
            {
                io.WriteLine("All animals in your party are fainted.");
                return;
            }

            if (!mainPlayer.CurrentRoom.HasEncounterAnimals())
            {
                io.WriteLine("No animals nearby");
                return;
            }

            Animal wildAnimal = mainPlayer.CurrentRoom.EncounterAnimals[0];
            BattleState battleState = BattleState.CreateWildBattle(mainPlayer, wildAnimal);

            io.WriteLine($"A wild {wildAnimal.Name} attacked!");

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
                    HandleWildAnimalTurn(io, mainPlayer, battleState, moveSelector);
                }

                isPlayerTurn = !isPlayerTurn;
            }

            FinishWildBattle(io, mainPlayer, battleState);
        }

        // Prints the current health for the player's active animal and the wild animal.
        private static void PrintBattleStatus(IGameIO io, BattleState battleState)
        {
            BattleHelpers.PrintBattleStatus(io, "wild", battleState.PlayerAnimal, battleState.OpponentAnimal);
        }

        // Handles the player's turn by reading a move and applying the selected move's effect.
        private static void HandlePlayerTurn(IGameIO io, BattleState battleState)
        {
            BattleHelpers.HandlePlayerTurn(io, battleState.PlayerAnimal, battleState.OpponentAnimal, "The wild ");
        }

        // Handles the wild animal's turn by selecting one random move and applying damage.
        private static void HandleWildAnimalTurn(IGameIO io, Player mainPlayer, BattleState battleState, IBattleMoveSelector moveSelector)
        {
            Animal wildAnimal = battleState.OpponentAnimal;

            BattleHelpers.HandleOpponentTurn(io, wildAnimal, battleState.PlayerAnimal, $"{wildAnimal.Name} Move", string.Empty, moveSelector);

            if (BattleHelpers.HandlePlayerFaintedAnimal(io, mainPlayer, battleState.PlayerAnimal, "Would you like to switch animals? (YES/NO)"))
            {
                battleState.UseFirstHealthyPlayerAnimal();
            }
        }

        // Finishes the wild battle after either the player's animal or the wild animal faints.
        private static void FinishWildBattle(IGameIO io, Player mainPlayer, BattleState battleState)
        {
            if (BattleEngine.IsFainted(battleState.PlayerAnimal))
            {
                io.WriteLine($"{battleState.PlayerAnimal.Name} fainted.");
                return;
            }

            if (BattleEngine.IsFainted(battleState.OpponentAnimal))
            {
                io.WriteLine($"{battleState.OpponentAnimal.Name} fainted.");
                HandleCatchChoice(io, mainPlayer, battleState.OpponentAnimal);
            }
        }

        // Handles the player's choice to catch or leave the fainted wild animal.
        private static void HandleCatchChoice(IGameIO io, Player mainPlayer, Animal wildAnimal)
        {
            while (true)
            {
                io.WriteLine($"Would you like to catch {wildAnimal.Name}? -- (yes or no)");
                string answer = Program.ReadUpperTrimmedInput(io);

                if (BattleHelpers.IsYes(answer))
                {
                    CatchAnimal(io, mainPlayer, wildAnimal);
                    return;
                }

                if (BattleHelpers.IsNo(answer))
                {
                    LetWildAnimalRunAway(io, mainPlayer, wildAnimal);
                    return;
                }

                io.WriteLine("Invalid input.");
            }
        }

        // Catches the wild animal if the player has space, otherwise asks whether to release one.
        private static void CatchAnimal(IGameIO io, Player mainPlayer, Animal wildAnimal)
        {
            if (mainPlayer.AnimalInventory.Count < 6)
            {
                AddCaughtAnimal(io, mainPlayer, wildAnimal);
                return;
            }

            io.WriteLine($"\n{mainPlayer.Name}'s animal inventory is full.");
            io.WriteLine("You can only have 6 animals with you at a time.");

            while (true)
            {
                io.WriteLine("Would you like to release an animal? -- (yes or no)");
                string answer = Program.ReadUpperTrimmedInput(io);

                if (BattleHelpers.IsYes(answer))
                {
                    ReleaseAnimalAndCatchWildAnimal(io, mainPlayer, wildAnimal);
                    return;
                }

                if (BattleHelpers.IsNo(answer))
                {
                    LetWildAnimalRunAway(io, mainPlayer, wildAnimal);
                    return;
                }

                io.WriteLine("Invalid input.");
            }
        }

        // Releases one animal from the player's party, then catches the wild animal.
        private static void ReleaseAnimalAndCatchWildAnimal(IGameIO io, Player mainPlayer, Animal wildAnimal)
        {
            io.WriteLine(mainPlayer.GetAnimalInventoryDisplay());

            Animal animalToRelease = AskForAnimalToRelease(io, mainPlayer);

            io.WriteLine($"You are releasing: {animalToRelease.Name}");

            BattleEngine.ReleaseAnimalAndCatchWildAnimal(mainPlayer, animalToRelease, wildAnimal);

            io.WriteLine($"You caught {wildAnimal.Name}!");
        }

        // Keeps asking until the player enters the name of an animal currently in their party.
        private static Animal AskForAnimalToRelease(IGameIO io, Player mainPlayer)
        {
            while (true)
            {
                io.WriteLine("Who would you like to release?");
                string animalName = Program.ReadUpperTrimmedInput(io);

                Animal? animalToRelease = FindAnimalByName(mainPlayer, animalName);

                if (animalToRelease != null)
                {
                    return animalToRelease;
                }

                io.WriteLine($"{animalName} is not a valid animal name.");
            }
        }

        // Finds an animal in the player's party by name.
        private static Animal? FindAnimalByName(Player mainPlayer, string animalName)
        {
            foreach (Animal animal in mainPlayer.AnimalInventory)
            {
                if (animal.Name == animalName)
                {
                    return animal;
                }
            }

            return null;
        }

        // Adds the wild animal to the player's party and removes it from the room.
        private static void AddCaughtAnimal(IGameIO io, Player mainPlayer, Animal wildAnimal)
        {
            BattleEngine.TryCatchWildAnimal(mainPlayer, wildAnimal);

            io.WriteLine($"You caught {wildAnimal.Name}!");
        }

        // Removes the wild animal from the room after the player chooses not to catch it.
        private static void LetWildAnimalRunAway(IGameIO io, Player mainPlayer, Animal wildAnimal)
        {
            BattleEngine.LetWildAnimalRunAway(mainPlayer, wildAnimal);
            io.WriteLine($"{wildAnimal.Name} ran away!");
        }

    }
}
