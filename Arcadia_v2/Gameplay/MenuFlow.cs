#nullable enable

using Arcadia_v2.Commands;
using Arcadia_v2.Saves;
using CommandReader = Arcadia_v2.Commands.Commands;

namespace Arcadia_v2
{
    // Handles menu actions, including healing, star fragment display, swapping, and sanctuary interaction.
    public static class MenuFlow
    {
        public static void HandleMenu(
            IGameIO io,
            GameState gameState,
            GameSaveService saveService)
        {
            Player mainPlayer = gameState.MainPlayer;
            bool hasGrowthOptions = GrowthFlow.HasGrowthOptions(mainPlayer);
            MenuCommandType menuCommand = CommandReader.ReadMenuCommand(io, hasGrowthOptions);

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
                    io.WriteLine(mainPlayer.GetStarFragmentDisplay());
                    break;

                case MenuCommandType.Swap:
                    PartyFlow.SwapAnimals(mainPlayer, io);
                    break;

                case MenuCommandType.Sanctuary:
                    SanctuaryFlow.HandleSanctuaryInteraction(
                        io,
                        gameState);

                    break;

                case MenuCommandType.Bond:
                    io.WriteLine(mainPlayer.GetBondDisplay());
                    break;

                case MenuCommandType.Grow:
                    if (!hasGrowthOptions)
                    {
                        io.WriteLine("No animals are ready to grow up.");
                        break;
                    }

                    GrowthFlow.HandleGrowth(io, mainPlayer);
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
