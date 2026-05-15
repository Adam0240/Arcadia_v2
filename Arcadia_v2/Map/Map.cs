using System;
using System.Collections.Generic;

namespace Arcadia_v2.Map
{
    public class Map
    {
        private const string ProfessorLab = "Professor's Lab";
        private const string Ikena = "Ikena";
        private const string Route1 = "Route 1";
        private const string Route2 = "Route 2";
        private const string OakPass = "Oak Pass";
        private const string Route3 = "Route 3";
        private const string Route4 = "Route 4";
        private const string NewNucleon = "New Nucleon";
        private const string Route5 = "Route 5";
        private const string Route6 = "Route 6";
        private const string Dracoton = "Dracoton";
        private const string Route7 = "Route 7";
        private const string VictoryRoad = "Victory Road";
        private const string Championships = "Championships";
        private const string TheEnd = "The End";

        public Room StartRoom { get; }
        public Room GymLeader1Room { get; }
        public Room GymLeader2Room { get; }
        public Room GymLeader3Room { get; }
        public Room GymLeader4Room { get; }
        public Room ChampionRoom { get; }

        public IReadOnlyDictionary<string, Room> Rooms => mRooms;

        private readonly Dictionary<string, Room> mRooms;

        public Map()
        {
            mRooms = CreateRooms();

            StartRoom = GetRoom(ProfessorLab);
            GymLeader1Room = GetRoom(OakPass);
            GymLeader2Room = GetRoom(NewNucleon);
            GymLeader3Room = GetRoom(Ikena);
            GymLeader4Room = GetRoom(Dracoton);
            ChampionRoom = GetRoom(Championships);

            ConnectRooms();
            PopulateWildPokemon();
        }

        // Creates all rooms first so connections can be added in one dedicated step.
        private static Dictionary<string, Room> CreateRooms()
        {
            return new Dictionary<string, Room>
            {
                [ProfessorLab] = new Room(ProfessorLab, "Where new trainers obtain their first pokemon!"),
                [Ikena] = new Room(Ikena, "Small peaceful town where hero's are born") { IsTown = true },
                [Route1] = new Room(Route1, "Where you make your first step into your Pokemon Journey!"),
                [Route2] = new Room(Route2, ""),
                [OakPass] = new Room(OakPass, "Town surrounded by trees and forest Pokemon") { IsTown = true },
                [Route3] = new Room(Route3, ""),
                [Route4] = new Room(Route4, "Tunnel"),
                [NewNucleon] = new Room(NewNucleon, "Founded after Nucleon incident") { IsTown = true, RequiredBadgesToEnter = 2 },
                [Route5] = new Room(Route5, "") { RequiredBadgesToEnter = 1 },
                [Route6] = new Room(Route6, "") { RequiredBadgesToEnter = 3 },
                [Dracoton] = new Room(Dracoton, "Home of Dragons and Dragon Masters") { IsTown = true, RequiredBadgesToEnter = 4 },
                [Route7] = new Room(Route7, ""),
                [VictoryRoad] = new Room(VictoryRoad, "Expert trainers and future Champions all travel through here") { IsTown = true },
                [Championships] = new Room(Championships, "Where you find out if your the best!"),
                [TheEnd] = new Room(TheEnd, "Decide where you wish to stay") { IsFinalRoom = true, RequiresChampionDefeatToEnter = true }
            };
        }

        // Wires up room neighbors after all room instances already exist.
        private void ConnectRooms()
        {
            Room pokeLab = GetRoom(ProfessorLab);
            Room ikena = GetRoom(Ikena);
            Room route1 = GetRoom(Route1);
            Room route2 = GetRoom(Route2);
            Room oakPass = GetRoom(OakPass);
            Room route3 = GetRoom(Route3);
            Room route4 = GetRoom(Route4);
            Room newNucleon = GetRoom(NewNucleon);
            Room route5 = GetRoom(Route5);
            Room route6 = GetRoom(Route6);
            Room dracoton = GetRoom(Dracoton);
            Room route7 = GetRoom(Route7);
            Room victoryRoad = GetRoom(VictoryRoad);
            Room championships = GetRoom(Championships);
            Room theEnd = GetRoom(TheEnd);

            pokeLab.North = ikena;

            ikena.North = route5;
            ikena.East = route6;
            ikena.South = pokeLab;
            ikena.West = route1;

            route1.East = ikena;
            route1.West = route2;

            route2.East = route1;
            route2.West = oakPass;

            oakPass.North = newNucleon;
            oakPass.East = route2;
            oakPass.West = route3;

            route3.North = route4;
            route3.East = oakPass;

            route4.North = newNucleon;
            route4.South = route3;

            newNucleon.East = route5;
            newNucleon.South = route4;
            newNucleon.West = oakPass;

            route5.West = newNucleon;
            route5.East = dracoton;
            route5.South = ikena;

            route6.North = dracoton;
            route6.West = ikena;

            dracoton.East = route7;
            dracoton.South = route6;
            dracoton.West = route5;

            route7.North = victoryRoad;
            route7.West = dracoton;

            victoryRoad.North = championships;
            victoryRoad.South = route7;

            championships.North = theEnd;
            championships.South = victoryRoad;

            theEnd.South = championships;
        }

        // Populates wild Pokemon room assignments while cloning each entry for isolated encounter state.
        private void PopulateWildPokemon()
        {
            IReadOnlyList<Arcadia_v2.Pokemon> mapPokemon = Arcadia_v2.GameData.CreatePokemon();

            AddPokemonToRoom(Route1, mapPokemon[3]);
            AddPokemonToRoom(Route2, mapPokemon[15]);
            AddPokemonToRoom(Route2, mapPokemon[12]);
            AddPokemonToRoom(Route3, mapPokemon[9]);
            AddPokemonToRoom(Route3, mapPokemon[11]);
            AddPokemonToRoom(Route4, mapPokemon[10]);
            AddPokemonToRoom(Route5, mapPokemon[7]);
            AddPokemonToRoom(Route6, mapPokemon[14]);
            AddPokemonToRoom(Route7, mapPokemon[4]);
            AddPokemonToRoom(Route7, mapPokemon[13]);
            AddPokemonToRoom(VictoryRoad, mapPokemon[8]);
            AddPokemonToRoom(VictoryRoad, mapPokemon[6]);
            AddPokemonToRoom(VictoryRoad, mapPokemon[17]);
            AddPokemonToRoom(TheEnd, mapPokemon[19]);
        }

        // Places one wild Pokemon into a room.
        private void AddPokemonToRoom(string roomName, Arcadia_v2.Pokemon pokemon)
        {
            GetRoom(roomName).SetRoomPokemon(pokemon);
        }

        // Returns a room by name so the game can access specific rooms without more map fields.
        public Room GetRoom(string roomName)
        {
            if (!mRooms.TryGetValue(roomName, out Room? room))
            {
                throw new ArgumentException($"Unknown room: {roomName}", nameof(roomName));
            }

            return room;
        }
    }
}
