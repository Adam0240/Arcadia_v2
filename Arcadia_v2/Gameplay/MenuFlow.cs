#nullable enable

using Arcadia_v2.Commands;
using Arcadia_v2.Saves;
using CommandReader = Arcadia_v2.Commands.Commands;

namespace Arcadia_v2
{
    // Handles menu actions, including healing, badge display, swapping, and gym interaction.
    public static class MenuFlow
    {
        public static void HandleMenu(
            IGameIO io,
            GameState gameState,
            GameSaveService saveService)
        {
            MenuCommandType menuCommand = CommandReader.ReadMenuCommand(io);
            Player mainPlayer = gameState.MainPlayer;

            switch (menuCommand)
            {
                case MenuCommandType.Heal:
                    if (mainPlayer.CurrentRoom.IsTown)
                    {
                        foreach (Animal partyAnimal in mainPlayer.AnimalInventory)
                        {
                            partyAnimal.Health = partyAnimal.BaseHealth;
                        }

                        io.WriteLine("\nAll your animals have been fully restored!\n");
                    }
                    else
                    {
                        io.WriteLine("Can only heal if your in a town!.");
                    }

                    break;

                case MenuCommandType.Bag:
                    io.WriteLine(mainPlayer.GetBadgeDisplay());
                    break;

                case MenuCommandType.Swap:
                    PartyFlow.SwapAnimals(mainPlayer, io);
                    break;

                case MenuCommandType.Gym:
                    GymFlow.HandleGymInteraction(
                        io,
                        gameState);

                    break;

                case MenuCommandType.Save:
                    PrintSaveCommandResult(io, saveService.Save(gameState));
                    break;

                case MenuCommandType.Invalid:
                    io.WriteLine("Invalid menu option.");
                    break;
            }
        }

        private static void PrintSaveCommandResult(IGameIO io, SaveCommandResult result)
        {
            io.WriteLine(result.Message);
        }
    }
}
