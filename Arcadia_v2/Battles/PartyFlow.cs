#nullable enable

namespace Arcadia_v2
{
    // Handles party-order interactions such as swapping active animal positions.
    public static class PartyFlow
    {
        public static void SwapAnimals(Player main, IGameIO io)
        {
            if (BattleEngine.CanAutoSwapTwoAnimalParty(main))
            {
                SwapAnimalsByIndex(main, io, firstIndex: 0, secondIndex: 1);
                return;
            }

            int firstIndex = PromptForAnimalIndex(
                main,
                io,
                "Here are your animals. Who would you like to trade positions with?",
                showInventory: true);

            int secondIndex = PromptForAnimalIndex(
                main,
                io,
                $"\nWho would you like to swap {main.AnimalInventory[firstIndex].Name} with?\n",
                showInventory: false);

            SwapAnimalsByIndex(main, io, firstIndex, secondIndex);
        }

        private static void SwapAnimalsByIndex(Player main, IGameIO io, int firstIndex, int secondIndex)
        {
            string firstAnimalName = main.AnimalInventory[firstIndex].Name;
            string secondAnimalName = main.AnimalInventory[secondIndex].Name;
            io.WriteLine($"You are swapping: {firstAnimalName} and {secondAnimalName} .\n");
            main.SwapAnimalPositions(firstIndex, secondIndex);
        }

        private static int PromptForAnimalIndex(Player main, IGameIO io, string prompt, bool showInventory)
        {
            while (true)
            {
                io.WriteLine(prompt);

                if (showInventory)
                {
                    io.WriteLine(main.GetAnimalInventoryDisplay());
                    io.WriteLine();
                }

                string animalName = Program.ReadUpperTrimmedInput(io);
                int animalIndex = FindAnimalIndexByName(main, animalName);

                if (animalIndex >= 0)
                {
                    return animalIndex;
                }

                io.WriteLine($"Invalid animal name {animalName} .");
                io.WriteLine("Must type in exact name of animal!");
            }
        }

        private static int FindAnimalIndexByName(Player main, string animalName)
        {
            for (int i = 0; i < main.AnimalInventory.Count; ++i)
            {
                if (main.AnimalInventory[i].Name == animalName)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
