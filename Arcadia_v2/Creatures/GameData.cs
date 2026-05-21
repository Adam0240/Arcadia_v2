#nullable enable
using Arcadia_v2.Creatures;
using System.Collections.Generic;

namespace Arcadia_v2
{
    // Provides fresh animal roster data for game setup and save restoration.
    public static class GameData
    {
        public static List<Animal> CreateAnimals()
        {
            return new List<Animal>(AnimalFactory.CreateAnimals());
        }
    }
}
