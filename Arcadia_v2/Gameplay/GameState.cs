#nullable enable

using System.Collections.Generic;
using Arcadia_v2.Map;

namespace Arcadia_v2
{
    // Holds the shared runtime objects that the gameplay loop currently passes across its legacy branches.
    public sealed class GameState
    {
        public GameState(
            Map.Map gameMap,
            List<Pokemon> mainPokemon,
            List<Pokemon> gymPokemon,
            Player mainPlayer,
            CompPlayer gymLeader1,
            CompPlayer gymLeader2,
            CompPlayer gymLeader3,
            CompPlayer gymLeader4,
            CompPlayer arcadiaChampion)
        {
            GameMap = gameMap;
            MainPokemon = mainPokemon;
            GymPokemon = gymPokemon;
            MainPlayer = mainPlayer;
            GymLeader1 = gymLeader1;
            GymLeader2 = gymLeader2;
            GymLeader3 = gymLeader3;
            GymLeader4 = gymLeader4;
            ArcadiaChampion = arcadiaChampion;
        }

        public Map.Map GameMap { get; }
        public List<Pokemon> MainPokemon { get; }
        public List<Pokemon> GymPokemon { get; }
        public Player MainPlayer { get; }
        public CompPlayer GymLeader1 { get; }
        public CompPlayer GymLeader2 { get; }
        public CompPlayer GymLeader3 { get; }
        public CompPlayer GymLeader4 { get; }
        public CompPlayer ArcadiaChampion { get; }
    }
}
