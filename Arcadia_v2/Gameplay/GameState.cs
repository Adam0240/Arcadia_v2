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
            List<Animal> guardianAnimals,
            Player mainPlayer,
            CompPlayer guardian1,
            CompPlayer guardian2,
            CompPlayer guardian3,
            CompPlayer guardian4,
            CompPlayer elementalTitan)
        {
            GameMap = gameMap;
            MainAnimals = mainAnimals;
            GuardianAnimals = guardianAnimals;
            MainPlayer = mainPlayer;
            Guardian1 = guardian1;
            Guardian2 = guardian2;
            Guardian3 = guardian3;
            Guardian4 = guardian4;
            ElementalTitan = elementalTitan;
        }

        public Map.Map GameMap { get; }
        public List<Animal> MainAnimals { get; }
        public List<Animal> GuardianAnimals { get; }
        public Player MainPlayer { get; }
        public CompPlayer Guardian1 { get; }
        public CompPlayer Guardian2 { get; }
        public CompPlayer Guardian3 { get; }
        public CompPlayer Guardian4 { get; }
        public CompPlayer ElementalTitan { get; }
    }
}
