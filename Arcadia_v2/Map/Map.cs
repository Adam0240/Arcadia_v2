using System;
using System.Collections.Generic;

namespace Arcadia_v2.Map
{
    public class Map
    {
        private const string MaiaStable = "Maia's Stable";
        private const string Ikena = "Ikena";
        private const string Road1 = "Road 1";
        private const string Road2 = "Road 2";
        private const string OakPass = "Oak Pass";
        private const string Road3 = "Road 3";
        private const string Road4 = "Road 4";
        private const string NewNucleon = "New Nucleon";
        private const string Road5 = "Road 5";
        private const string Road6 = "Road 6";
        private const string Road7 = "Road 7";
        private const string Wyrmrest = "Wyrmrest";
        private const string Mountains = "Mountains";
        private const string RadioactiveWay = "Radioactive Way";
        private const string Nucleon = "Nucleon";
        private const string FinalTrials = "Final Trials";
        private const string GuardiansTower = "Guardian's Tower";
        private const string Road8 = "Road 8";
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

            StartRoom = GetRoom(MaiaStable);
            GymLeader1Room = GetRoom(OakPass);
            GymLeader2Room = GetRoom(NewNucleon);
            GymLeader3Room = GetRoom(Ikena);
            GymLeader4Room = GetRoom(Wyrmrest);
            ChampionRoom = GetRoom(GuardiansTower);

            ConnectRooms();
            PopulateWildPokemon();
        }

        // Creates all rooms first so connections can be added in one dedicated step.
        private static Dictionary<string, Room> CreateRooms()
        {
            return new Dictionary<string, Room>
            {
                [MaiaStable] = new Room(MaiaStable, "Where new trainers obtain their first pokemon!"),
                [Ikena] = new Room(Ikena, "Small peaceful town where hero's are born") { IsTown = true },
                [Road1] = new Room(Road1, "Where you make your first step into your Pokemon Journey!"),
                [Road2] = new Room(Road2, ""),
                [OakPass] = new Room(OakPass, "Town surrounded by trees and forest Pokemon") { IsTown = true },
                [Road3] = new Room(Road3, ""),
                [Road4] = new Room(Road4, "Tunnel"),
                [NewNucleon] = new Room(NewNucleon, "Founded after Nucleon incident") { IsTown = true },
                [Road5] = new Room(Road5, ""),
                [Road6] = new Room(Road6, ""),
                [Road7] = new Room(Road7, ""),
                [Wyrmrest] = new Room(Wyrmrest, "Home of Dragons and Dragon Masters") { IsTown = true },
                [Mountains] = new Room(Mountains, ""),
                [RadioactiveWay] = new Room(RadioactiveWay, ""),
                [Nucleon] = new Room(Nucleon, "") { IsTown = true },
                [FinalTrials] = new Room(FinalTrials, "Expert trainers and future champions all travel through here"),
                [GuardiansTower] = new Room(GuardiansTower, "Where you find out if you're the best!"),
                [Road8] = new Room(Road8, ""),
                [TheEnd] = new Room(TheEnd, "Decide where you wish to stay") { IsFinalRoom = true, RequiresChampionDefeatToEnter = true }
            };
        }

        // Wires up room neighbors after all room instances already exist.
        private void ConnectRooms()
        {
            Room maiaStable = GetRoom(MaiaStable);
            Room ikena = GetRoom(Ikena);
            Room road1 = GetRoom(Road1);
            Room road2 = GetRoom(Road2);
            Room oakPass = GetRoom(OakPass);
            Room road3 = GetRoom(Road3);
            Room road4 = GetRoom(Road4);
            Room newNucleon = GetRoom(NewNucleon);
            Room road5 = GetRoom(Road5);
            Room road6 = GetRoom(Road6);
            Room road7 = GetRoom(Road7);
            Room wyrmrest = GetRoom(Wyrmrest);
            Room mountains = GetRoom(Mountains);
            Room radioactiveWay = GetRoom(RadioactiveWay);
            Room nucleon = GetRoom(Nucleon);
            Room finalTrials = GetRoom(FinalTrials);
            Room guardiansTower = GetRoom(GuardiansTower);
            Room road8 = GetRoom(Road8);
            Room theEnd = GetRoom(TheEnd);

            maiaStable.North = ikena;

            ikena.North = theEnd;
            ikena.East = road6;
            ikena.South = road5;
            ikena.West = road1;

            road1.North = road8;
            road1.East = ikena;
            road1.South = road2;

            road2.North = road1;
            road2.South = oakPass;

            oakPass.North = road2;
            oakPass.South = road3;

            road3.North = oakPass;
            road3.South = road4;

            road4.North = road3;
            road4.South = newNucleon;

            newNucleon.North = road4;
            newNucleon.East = road5;

            road5.North = ikena;
            road5.East = nucleon;
            road5.West = newNucleon;

            road6.North = finalTrials;
            road6.South = road7;
            road6.West = ikena;

            road7.North = road6;
            road7.South = wyrmrest;

            wyrmrest.North = road7;
            wyrmrest.South = mountains;

            mountains.North = wyrmrest;
            mountains.South = radioactiveWay;

            radioactiveWay.North = mountains;
            radioactiveWay.South = nucleon;

            nucleon.North = radioactiveWay;
            nucleon.West = road5;

            finalTrials.North = guardiansTower;
            finalTrials.South = road6;

            guardiansTower.East = finalTrials;
            guardiansTower.South = ikena;
            guardiansTower.West = road8;

            road8.North = guardiansTower;
            road8.South = road1;

            theEnd.South = ikena;
        }

        // Populates wild animal room assignments while cloning each entry for isolated encounter state.
        private void PopulateWildPokemon()
        {
            IReadOnlyList<Arcadia_v2.Animal> mapAnimals = Arcadia_v2.GameData.CreateAnimals();

            AddAnimalToRoom(Road1, mapAnimals[3]);
            AddAnimalToRoom(Road2, mapAnimals[15]);
            AddAnimalToRoom(Road2, mapAnimals[12]);
            AddAnimalToRoom(Road3, mapAnimals[9]);
            AddAnimalToRoom(Road3, mapAnimals[11]);
            AddAnimalToRoom(Road4, mapAnimals[10]);
            AddAnimalToRoom(Road5, mapAnimals[7]);
            AddAnimalToRoom(Road6, mapAnimals[14]);
            AddAnimalToRoom(Road7, mapAnimals[4]);
            AddAnimalToRoom(Road7, mapAnimals[13]);
            AddAnimalToRoom(Mountains, mapAnimals[8]);
            AddAnimalToRoom(RadioactiveWay, mapAnimals[6]);
            AddAnimalToRoom(FinalTrials, mapAnimals[17]);
            AddAnimalToRoom(TheEnd, mapAnimals[19]);
        }

        // Places one wild animal into a room.
        private void AddAnimalToRoom(string roomName, Arcadia_v2.Animal animal)
        {
            GetRoom(roomName).SetRoomAnimal(animal);
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
