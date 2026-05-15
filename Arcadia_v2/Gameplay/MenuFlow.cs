#nullable enable

using System;
using System.Collections.Generic;
using Arcadia_v2.Commands;
using Arcadia_v2.Saves;
using CommandReader = Arcadia_v2.Commands.Commands;

namespace Arcadia_v2
{
    // Handles the legacy menu branch, including healing, badge display, swapping, and gym interaction.
    public static class MenuFlow
    {
        public static void HandleMenu(
            GameState gameState,
            GameSaveService saveService,
            Player mainPlayer,
            List<Pokemon> mainPokemon,
            CompPlayer gymLeader1,
            CompPlayer gymLeader2,
            CompPlayer gymLeader3,
            CompPlayer gymLeader4,
            CompPlayer arcadiaChampion)
        {
            MenuCommandType menuCommand = CommandReader.ReadMenuCommand();
            int menuOption = CommandReader.GetMenuChoice(menuCommand);
            string menuChoice = Parser.ToUpperCase(menuCommand.ToString());

            switch (menuOption)
            {
                case 1:
                    if (mainPlayer.CurrentRoom.IsTown)
                    {
                        foreach (Pokemon partyPokemon in mainPlayer.PokemonInventory)
                        {
                            partyPokemon.Health = partyPokemon.BaseHealth;
                        }

                        Console.WriteLine("\nAll your Pokemon have been fully restored!\n");
                    }
                    else
                    {
                        Console.WriteLine("Can only heal if your in a town!.");
                    }

                    break;

                case 2:
                    Console.WriteLine(mainPlayer.GetBadgeDisplay());
                    break;

                case 3:
                    PartyFlow.SwapPokemon(mainPlayer);
                    break;

                case 4:
                    GymFlow.HandleGymInteraction(
                        mainPlayer,
                        gymLeader1,
                        gymLeader2,
                        gymLeader3,
                        gymLeader4,
                        arcadiaChampion);

                    break;

                case 5:
                    PrintSaveCommandResult(saveService.Save(gameState));
                    break;

                case 0:
                    Console.WriteLine("Invalid menu option.");
                    break;
            }
        }

        private static void PrintSaveCommandResult(SaveCommandResult result)
        {
            Console.WriteLine(result.Message);
        }
    }
}
