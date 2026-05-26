#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcadia_v2.Creatures
{
    public sealed record AnimalGrowthOption(int PartyIndex, Animal CurrentAnimal, Animal AdultAnimal);

    public static class AnimalGrowthCatalog
    {
        private static readonly Dictionary<string, string> AdultSpeciesByBaseSpecies = new()
        {
            ["CAT"] = "LION",
            ["DOG"] = "WOLF",
            ["HORSE"] = "STALLION",
            ["TURTLE"] = "TORTOISE",
            ["BIRD"] = "EAGLE",
            ["ANT"] = "BEE",
            ["CUB"] = "BEAR",
            ["SERPENT"] = "DRAGON"
        };

        public static IReadOnlyList<AnimalGrowthOption> GetGrowthOptions(Player player)
        {
            ArgumentNullException.ThrowIfNull(player);

            List<AnimalGrowthOption> options = new();

            for (int i = 0; i < player.AnimalInventory.Count; ++i)
            {
                Animal animal = player.AnimalInventory[i];

                if (player.GetBond(animal.Element) < 100)
                {
                    continue;
                }

                if (TryGetAdultForm(animal, out Animal? adultAnimal))
                {
                    options.Add(new AnimalGrowthOption(i, animal, adultAnimal!));
                }
            }

            return options;
        }

        public static bool HasGrowthOptions(Player player)
        {
            return GetGrowthOptions(player).Count > 0;
        }

        private static bool TryGetAdultForm(Animal animal, out Animal? adultAnimal)
        {
            string[] nameParts = animal.Name.Split('_', 2);

            if (nameParts.Length != 2 ||
                !AdultSpeciesByBaseSpecies.TryGetValue(nameParts[1], out string? adultSpecies))
            {
                adultAnimal = null;
                return false;
            }

            string adultName = $"{nameParts[0]}_{adultSpecies}";
            adultAnimal = AnimalFactory.CreateAnimals()
                .Single(candidate => candidate.Name == adultName)
                .Clone();

            return true;
        }
    }
}
