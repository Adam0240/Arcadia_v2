#nullable enable

using System;
using Arcadia_v2.Saves;

namespace Arcadia_v2
{
    public static class StartupFlow
    {
        public static GameState Run(IGameIO io, GameSaveService saveService)
        {
            ArgumentNullException.ThrowIfNull(saveService);
            ArgumentNullException.ThrowIfNull(io);

            while (true)
            {
                bool hasSave = saveService.HasSave();
                PrintStartupMenu(io, hasSave);

                StartupCommandType startupCommand = ParseStartupCommand(Program.ReadUpperTrimmedInput(io), hasSave);

                if (!hasSave && startupCommand == StartupCommandType.NewGame)
                {
                    return GameSetup.Initialize(io);
                }

                if (startupCommand == StartupCommandType.LoadGame)
                {
                    GameState gameState = GameSetup.CreateForLoad();
                    SaveCommandResult loadResult = saveService.Load(gameState);
                    io.WriteLine(loadResult.Message);

                    if (loadResult.Succeeded)
                    {
                        RoomDisplay.Print(io, gameState.MainPlayer.CurrentRoom);
                        return gameState;
                    }

                    continue;
                }

                if (startupCommand == StartupCommandType.DeleteGame)
                {
                    HandleDelete(io, saveService);
                    continue;
                }

                io.WriteLine("Invalid input");
            }
        }

        private static void PrintStartupMenu(IGameIO io, bool hasSave)
        {
            if (hasSave)
            {
                io.WriteLine("1. Load Game");
                io.WriteLine("2. Delete Game");
                return;
            }

            io.WriteLine("1. New Game");
        }

        private static StartupCommandType ParseStartupCommand(string choice, bool hasSave)
        {
            if (hasSave)
            {
                return choice switch
                {
                    "1" => StartupCommandType.LoadGame,
                    "2" => StartupCommandType.DeleteGame,
                    _ => StartupCommandType.Invalid
                };
            }

            return choice switch
            {
                "1" => StartupCommandType.NewGame,
                _ => StartupCommandType.Invalid
            };
        }

        private static void HandleDelete(IGameIO io, GameSaveService saveService)
        {
            while (true)
            {
                io.WriteLine("Are you sure you want to delete?");
                string answer = Program.ReadUpperTrimmedInput(io);

                if (BattleHelpers.IsYes(answer))
                {
                    SaveCommandResult deleteResult = saveService.Delete();
                    io.WriteLine(deleteResult.Message);
                    return;
                }

                if (BattleHelpers.IsNo(answer))
                {
                    return;
                }

                io.WriteLine("Invalid input");
            }
        }

        private enum StartupCommandType
        {
            Invalid,
            NewGame,
            LoadGame,
            DeleteGame
        }
    }
}
