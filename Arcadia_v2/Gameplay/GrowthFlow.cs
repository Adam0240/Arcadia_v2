#nullable enable

using Arcadia_v2.Creatures;

namespace Arcadia_v2
{
    public static class GrowthFlow
    {
        public static bool HasGrowthOptions(Player player)
        {
            return AnimalGrowthCatalog.HasGrowthOptions(player);
        }

        public static void HandleGrowth(IGameIO io, Player player)
        {
            IReadOnlyList<AnimalGrowthOption> growthOptions = AnimalGrowthCatalog.GetGrowthOptions(player);

            if (growthOptions.Count == 0)
            {
                io.WriteLine("No animals are ready to grow up.");
                return;
            }

            io.WriteLine("One of the following animals are ready to grow up");

            for (int i = 0; i < growthOptions.Count; ++i)
            {
                io.WriteLine($"{i + 1}. {growthOptions[i].CurrentAnimal.Name}");
            }

            AnimalGrowthOption selectedOption = ReadGrowthChoice(io, growthOptions);
            player.ReplaceAnimalAt(selectedOption.PartyIndex, selectedOption.AdultAnimal);
            player.ResetBond(selectedOption.CurrentAnimal.Element);

            io.WriteLine($"{selectedOption.CurrentAnimal.Name} grew into {selectedOption.AdultAnimal.Name}!");
        }

        private static AnimalGrowthOption ReadGrowthChoice(
            IGameIO io,
            IReadOnlyList<AnimalGrowthOption> growthOptions)
        {
            while (true)
            {
                io.WriteLine("Enter the number of the animal you wish to grow.");
                string answer = Program.ReadUpperTrimmedInput(io);

                if (int.TryParse(answer, out int selectedNumber) &&
                    selectedNumber >= 1 &&
                    selectedNumber <= growthOptions.Count)
                {
                    return growthOptions[selectedNumber - 1];
                }

                io.WriteLine("Invalid growth choice.");
            }
        }
    }
}
