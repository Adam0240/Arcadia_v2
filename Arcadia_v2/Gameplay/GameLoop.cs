#nullable enable

using System;
using System.Collections.Generic;
using Arcadia_v2.Commands;
using Arcadia_v2.Saves;
using CommandReader = Arcadia_v2.Commands.Commands;

namespace Arcadia_v2
{
    // Runs the main gameplay loop and delegates command-specific behavior to smaller methods.
    public static class GameLoop
    {
        public static void Run()
        {
            GameSaveService saveService = new(new SqliteGameSaveRepository());
            saveService.Initialize();
            GameState gameState = StartupFlow.Run(saveService);

            bool isRunning = true;

            while (isRunning)
            {
                MainCommandInput mainCommandInput = CommandReader.ReadMainCommandInput();

                isRunning = HandleMainCommand(mainCommandInput, gameState, saveService);

                if (isRunning)
                {
                    isRunning = HandleEndRoom(gameState.MainPlayer);
                }
            }

            PauseBeforeExit();
        }

        // Routes the player's main command to the correct command handler.
        private static bool HandleMainCommand(
            MainCommandInput mainCommandInput,
            GameState gameState,
            GameSaveService saveService)
        {
            return mainCommandInput.MainCommand switch
            {
                MainCommandType.Go => HandleGoCommand(gameState, mainCommandInput.Remainder),
                MainCommandType.Action => HandleActionCommand(gameState, saveService, mainCommandInput.Remainder),
                _ => HandleInvalidCommand()
            };
        }

        // Handles movement commands and returns false if the player quits.
        private static bool HandleGoCommand(GameState gameState, string directionInput)
        {
            DirectionCommandType directionCommand = string.IsNullOrEmpty(directionInput)
                ? CommandReader.ReadDirectionCommand()
                : Parser.ParseDirectionCommand(directionInput);

            if (directionCommand == DirectionCommandType.Quit)
            {
                Console.WriteLine("Goodbye");
                return false;
            }

            string direction = Parser.ToUpperCase(directionCommand.ToString());
            int choice = CommandReader.GetDirectionChoice(directionCommand);

            MovementFlow.HandleMovement(
                gameState.MainPlayer,
                gameState.ArcadiaChampion,
                choice,
                direction);

            return true;
        }

        // Handles action commands such as wild battle, Pokemon list, and menu.
        private static bool HandleActionCommand(
            GameState gameState,
            GameSaveService saveService,
            string actionInput)
        {
            ActionCommandType actionCommand = string.IsNullOrEmpty(actionInput)
                ? CommandReader.ReadActionCommand()
                : Parser.ParseActionCommand(actionInput);
            int actionChoice = CommandReader.GetActionChoice(actionCommand);

            switch (actionChoice)
            {
                case 1:
                    WildBattleFlow.HandleWildBattle(
                        gameState.MainPlayer,
                        gameState.MainPokemon);
                    break;

                case 2:
                    Console.WriteLine(gameState.MainPlayer.GetPokemonInventoryDisplay());
                    break;

                case 3:
                    MenuFlow.HandleMenu(
                        gameState,
                        saveService,
                        gameState.MainPlayer,
                        gameState.MainPokemon,
                        gameState.GymLeader1,
                        gameState.GymLeader2,
                        gameState.GymLeader3,
                        gameState.GymLeader4,
                        gameState.ArcadiaChampion);
                    break;

                default:
                    string actionName = Parser.ToUpperCase(actionCommand.ToString());
                    Console.WriteLine($"\nSorry, {actionName} is invalid.");
                    break;
            }

            return true;
        }

        // Handles the special behavior that occurs in the final room.
        private static bool HandleEndRoom(Player mainPlayer)
        {
            if (!mainPlayer.CurrentRoom.IsFinalRoom)
            {
                return true;
            }

            RoomDisplay.Print(mainPlayer.CurrentRoom);

            if (mainPlayer.CurrentRoom.HasEncounterPokemon())
            {
                PrintArceusChallenge();
                return true;
            }

            return AskPlayerToStay();
        }

        // Prints the final challenge text before the Arceus encounter.
        private static void PrintArceusChallenge()
        {
            Console.WriteLine("\n\nArceus Voice: I knew you would eventually find your way here.");
            Console.WriteLine("Your potential was clear to me the first time you were in my presence.");
            Console.WriteLine("You have proven you're the best Pokemon Trainer in Arcadia. But are you stronger than the god of this region?");
            Console.WriteLine("Face me to find out if you truly are the best.\n");
        }

        // Asks whether the player wants to remain in the Pokemon world after clearing the final encounter.
        private static bool AskPlayerToStay()
        {
            Console.WriteLine("\nYou have defeated all the strongest trainers in this region.\n");
            Console.WriteLine("Do you wish to stay in this world?");

            string answer = Program.ReadUpperTrimmedInput();

            if (BattleHelpers.IsNo(answer))
            {
                Console.WriteLine("Goodbye, may you find as much joy back home.");
                return false;
            }

            if (BattleHelpers.IsYes(answer))
            {
                Console.WriteLine("You're welcome to stay. Type 'quit' to leave.");
                return true;
            }

            Console.WriteLine("Invalid input");
            return true;
        }

        // Handles invalid main commands.
        private static bool HandleInvalidCommand()
        {
            Console.WriteLine("Invalid input.");
            return true;
        }

        // Keeps the console window open before the program exits.
        private static void PauseBeforeExit()
        {
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
    }
}
