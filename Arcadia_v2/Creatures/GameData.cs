#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcadia_v2
{
    // Provides fresh animal roster data for game setup and save restoration.
    public static class GameData
    {
        public static List<Animal> CreateAnimals()
        {
            return new List<Animal>(AnimalFactory.CreateAnimals());
        }

        public static Animal FindAnimal(
            IReadOnlyList<Animal> animals,
            AnimalElement element,
            string speciesName)
        {
            ArgumentNullException.ThrowIfNull(animals);

            if (string.IsNullOrWhiteSpace(speciesName))
            {
                throw new ArgumentException("Species name cannot be empty.", nameof(speciesName));
            }

            string animalName = $"{element} {speciesName}";
            return animals.Single(animal => animal.Name == animalName);
        }
    }
}
