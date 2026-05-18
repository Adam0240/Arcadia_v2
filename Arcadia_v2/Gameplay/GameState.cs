#nullable enable

using System.Collections.Generic;
using Arcadia_v2.Map;

namespace Arcadia_v2
{
    // Holds the shared runtime objects used across gameplay flows.
    public sealed class GameState
    {
        public GameState(
            Map.Map gameMap,
            List<Animal> mainAnimals,
            List<Animal> gymAnimals,
            Player mainPlayer,
            CompPlayer gymLeader1,
            CompPlayer gymLeader2,
            CompPlayer gymLeader3,
            CompPlayer gymLeader4,
            CompPlayer arcadiaChampion)
        {
            GameMap = gameMap;
            MainAnimals = mainAnimals;
            GymAnimals = gymAnimals;
            MainPlayer = mainPlayer;
            GymLeader1 = gymLeader1;
            GymLeader2 = gymLeader2;
            GymLeader3 = gymLeader3;
            GymLeader4 = gymLeader4;
            ArcadiaChampion = arcadiaChampion;
        }

        public Map.Map GameMap { get; }
        public List<Animal> MainAnimals { get; }
        public List<Animal> GymAnimals { get; }
        public Player MainPlayer { get; }
        public CompPlayer GymLeader1 { get; }
        public CompPlayer GymLeader2 { get; }
        public CompPlayer GymLeader3 { get; }
        public CompPlayer GymLeader4 { get; }
        public CompPlayer ArcadiaChampion { get; }
    }
}
