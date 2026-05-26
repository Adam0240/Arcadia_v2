#nullable enable

using System;
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
            Run(new ConsoleGameIO());
        }

        public static void Run(IGameIO io)
        {
            ArgumentNullException.ThrowIfNull(io);

            GameSaveService saveService = new(new SqliteGameSaveRepository());
            saveService.Initialize();
            GameState gameState = StartupFlow.Run(io, saveService);

            bool isRunning = true;

            while (isRunning)
            {
                MainCommandInput mainCommandInput = CommandReader.ReadMainCommandInput(io);

                isRunning = HandleMainCommand(io, mainCommandInput, gameState, saveService);

                if (isRunning)
                {
                    isRunning = HandleEndRoom(io, gameState.MainPlayer);
                }
            }

            PauseBeforeExit(io);
        }

        // Routes the player's main command to the correct command handler.
        private static bool HandleMainCommand(
            IGameIO io,
            MainCommandInput mainCommandInput,
            GameState gameState,
            GameSaveService saveService)
        {
            return mainCommandInput.MainCommand switch
            {
                MainCommandType.Go => HandleGoCommand(io, gameState, mainCommandInput.Remainder),
                MainCommandType.Action => HandleActionCommand(io, gameState, saveService, mainCommandInput.Remainder),
                _ => HandleInvalidCommand(io)
            };
        }

        // Handles movement commands and returns false if the player quits.
        private static bool HandleGoCommand(IGameIO io, GameState gameState, string directionInput)
        {
            DirectionCommandType directionCommand = string.IsNullOrEmpty(directionInput)
                ? CommandReader.ReadDirectionCommand(io)
                : Parser.ParseDirectionCommand(directionInput);

            if (directionCommand == DirectionCommandType.Quit)
            {
                io.WriteLine("Goodbye");
                return false;
            }

            string direction = Parser.ToUpperCase(directionCommand.ToString());

            MovementFlow.HandleMovement(
                io,
                gameState,
                directionCommand,
                direction);

            return true;
        }

        // Handles action commands such as wild battle, animal list, and menu.
        private static bool HandleActionCommand(
            IGameIO io,
            GameState gameState,
            GameSaveService saveService,
            string actionInput)
        {
            ActionCommandType actionCommand = string.IsNullOrEmpty(actionInput)
                ? CommandReader.ReadActionCommand(io)
                : Parser.ParseActionCommand(actionInput);

            switch (actionCommand)
            {
                case ActionCommandType.Battle:
                    WildBattleFlow.HandleWildBattle(
                        io,
                        gameState);
                    break;

                case ActionCommandType.AnimalInventory:
                    io.WriteLine(gameState.MainPlayer.GetAnimalInventoryDisplay());
                    break;

                case ActionCommandType.Menu:
                    MenuFlow.HandleMenu(
                        io,
                        gameState,
                        saveService);
                    break;

                case ActionCommandType.Invalid:
                    string actionName = Parser.ToUpperCase(actionCommand.ToString());
                    io.WriteLine($"\nSorry, {actionName} is invalid.");
                    break;
            }

            return true;
        }

        // Handles the special behavior that occurs in the final room.
        private static bool HandleEndRoom(IGameIO io, Player mainPlayer)
        {
            if (!mainPlayer.CurrentRoom.IsFinalRoom)
            {
                return true;
            }

            RoomDisplay.Print(io, mainPlayer.CurrentRoom);

            if (mainPlayer.CurrentRoom.HasEncounterAnimals())
            {
                PrintFinalChallenge(io);
                return true;
            }

            return AskPlayerToStay(io);
        }

        // Prints the final challenge text before the last encounter.
        private static void PrintFinalChallenge(IGameIO io)
        {
            io.WriteLine("\n\nCosmic Voice: I knew you would eventually find your way here.");
            io.WriteLine("Your potential was clear to me the first time you were in my presence.");
            io.WriteLine("You have proven you're the best trainer in Arcadia. But are you stronger than the god of this region?");
            io.WriteLine("Face me to find out if you truly are the best.\n");
        }

        // Asks whether the player wants to remain in this world after clearing the final encounter.
        private static bool AskPlayerToStay(IGameIO io)
        {
            io.WriteLine("\nYou have defeated all the strongest trainers in this region.\n");
            io.WriteLine("Do you wish to stay in this world?");

            string answer = Program.ReadUpperTrimmedInput(io);

            if (BattleHelpers.IsNo(answer))
            {
                io.WriteLine("Goodbye, may you find as much joy back home.");
                return false;
            }

            if (BattleHelpers.IsYes(answer))
            {
                io.WriteLine("You're welcome to stay. Type 'quit' to leave.");
                return true;
            }

            io.WriteLine("Invalid input");
            return true;
        }

        // Handles invalid main commands.
        private static bool HandleInvalidCommand(IGameIO io)
        {
            io.WriteLine("Invalid input.");
            return true;
        }

        // Keeps the console window open before the program exits.
        private static void PauseBeforeExit(IGameIO io)
        {
            io.WriteLine("Press Enter to continue...");
            io.ReadLine();
        }
    }
}
