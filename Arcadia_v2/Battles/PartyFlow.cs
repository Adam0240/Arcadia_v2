#nullable enable

namespace Arcadia_v2
{
    // Handles party-order interactions such as swapping active Pokemon positions.
    public static class PartyFlow
    {
        public static void SwapPokemon(Player main, IGameIO io)
        {
            if (BattleEngine.CanAutoSwapTwoPokemonParty(main))
            {
                SwapPokemonByIndex(main, io, firstIndex: 0, secondIndex: 1);
                return;
            }

            int firstIndex = PromptForPokemonIndex(
                main,
                io,
                "Heres your Pokemon. Who would you like to trade positions with?",
                showInventory: true);

            int secondIndex = PromptForPokemonIndex(
                main,
                io,
                $"\nWho would you like to swap {main.PokemonInventory[firstIndex].Name} with?\n",
                showInventory: false);

            SwapPokemonByIndex(main, io, firstIndex, secondIndex);
        }

        private static void SwapPokemonByIndex(Player main, IGameIO io, int firstIndex, int secondIndex)
        {
            string firstPokemonName = main.PokemonInventory[firstIndex].Name;
            string secondPokemonName = main.PokemonInventory[secondIndex].Name;
            io.WriteLine($"You are swapping: {firstPokemonName} and {secondPokemonName} .\n");
            main.SwapPokemonPositions(firstIndex, secondIndex);
        }

        private static int PromptForPokemonIndex(Player main, IGameIO io, string prompt, bool showInventory)
        {
            while (true)
            {
                io.WriteLine(prompt);

                if (showInventory)
                {
                    io.WriteLine(main.GetPokemonInventoryDisplay());
                    io.WriteLine();
                }

                string pokemonName = Program.ReadUpperTrimmedInput(io);
                int pokemonIndex = FindPokemonIndexByName(main, pokemonName);

                if (pokemonIndex >= 0)
                {
                    return pokemonIndex;
                }

                io.WriteLine($"Invalid Pokemon name {pokemonName} .");
                io.WriteLine("Must type in exact name of pokemon!");
            }
        }

        private static int FindPokemonIndexByName(Player main, string pokemonName)
        {
            for (int i = 0; i < main.PokemonInventory.Count; ++i)
            {
                if (main.PokemonInventory[i].Name == pokemonName)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
