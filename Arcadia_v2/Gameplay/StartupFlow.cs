#nullable enable

using System;
using Arcadia_v2.Saves;

namespace Arcadia_v2
{
    public static class StartupFlow
    {
        public static GameState Run(GameSaveService saveService)
        {
            ArgumentNullException.ThrowIfNull(saveService);

            while (true)
            {
                bool hasSave = saveService.HasSave();
                PrintStartupMenu(hasSave);

                string choice = Program.ReadUpperTrimmedInput();

                if (!hasSave && choice == "1")
                {
                    return GameSetup.Initialize();
                }

                if (choice == "2")
                {
                    GameState gameState = GameSetup.CreateForLoad();
                    SaveCommandResult loadResult = saveService.Load(gameState);
                    Console.WriteLine(loadResult.Message);

                    if (loadResult.Succeeded)
                    {
                        RoomDisplay.Print(gameState.MainPlayer.CurrentRoom);
                        return gameState;
                    }

                    continue;
                }

                if (choice == "3")
                {
                    HandleDelete(saveService);
                    continue;
                }

                Console.WriteLine("Invalid input");
            }
        }

        private static void PrintStartupMenu(bool hasSave)
        {
            if (!hasSave)
            {
                Console.WriteLine("1. New Game");
            }

            Console.WriteLine("2. Load Game");
            Console.WriteLine("3. Delete Game");
        }

        private static void HandleDelete(GameSaveService saveService)
        {
            Console.WriteLine("Are you sure you want to delete?");
            string answer = Program.ReadUpperTrimmedInput();

            if (BattleHelpers.IsYes(answer))
            {
                SaveCommandResult deleteResult = saveService.Delete();
                Console.WriteLine(deleteResult.Message);
                return;
            }

            if (BattleHelpers.IsNo(answer))
            {
                return;
            }

            Console.WriteLine("Invalid input");
        }
    }
}
