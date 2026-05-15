#nullable enable

using System;

namespace Arcadia_v2
{
    // Handles party-order interactions such as swapping active Pokemon positions.
    public static class PartyFlow
    {
        public static void SwapPokemon(Player main)
        {
            string pokeSwap = "";
            string swap1 = "";
            string pokeSwap2 = "";
            string swap2 = "";
            int a = 0;
            int b = 0;
            int pokeIntSize = main.PokemonInventory.Count;
            bool validPokemon = false;
            bool validPokemon2 = false;

            while (swap1 == "")
            {
                Console.WriteLine("Heres your Pokemon. Who would you like to trade positions with?");
                Console.WriteLine(main.GetPokemonInventoryDisplay());
                Console.WriteLine();

                pokeSwap = Program.ReadUpperTrimmedInput();

                for (int i = 0; i <= main.PokemonInventory.Count - 1; ++i)
                {
                    if (main.PokemonInventory[i].Name == pokeSwap)
                    {
                        validPokemon = true;
                        a = i;
                        break;
                    }
                    else if (i >= 3 && pokeIntSize >= i)
                    {
                        if (main.PokemonInventory[i - 1].Name == pokeSwap)
                        {
                            validPokemon = true;
                            a = i - 1;
                            break;
                        }
                    }
                }

                if (validPokemon)
                {
                    swap1 = pokeSwap;
                    Console.WriteLine("its working.");
                }
                else
                {
                    Console.WriteLine($"Invalid Pokemon name {pokeSwap} .");
                    Console.WriteLine("Must type in exact name of pokemon!");
                }
            }

            if (validPokemon)
            {
                while (swap2 == "")
                {
                    Console.WriteLine($"\nWho would you like to swap {swap1} with?\n");
                    pokeSwap2 = Program.ReadUpperTrimmedInput();

                    for (int i = 0; i <= main.PokemonInventory.Count - 1; ++i)
                    {
                        if (main.PokemonInventory[i].Name == pokeSwap2)
                        {
                            validPokemon2 = true;
                            swap2 = pokeSwap2;
                            b = i;
                            break;
                        }
                        else if (i >= 3 && pokeIntSize >= i)
                        {
                            if (main.PokemonInventory[i - 1].Name == pokeSwap2)
                            {
                                validPokemon2 = true;
                                b = i - 1;
                                break;
                            }
                        }
                    }

                    if (!validPokemon2)
                    {
                        Console.WriteLine($"Invalid Pokemon name {pokeSwap2} .");
                        Console.WriteLine("Must type in exact name of pokemon!");
                    }
                }
            }

            if (validPokemon && validPokemon2)
            {
                Console.WriteLine($"You are swapping: {swap1} and {swap2} .\n");
                main.SwapPokemonPositions(a, b);
            }
        }
    }
}
