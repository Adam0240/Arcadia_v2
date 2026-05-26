using System;
using System.Collections.Generic;

namespace Arcadia_v2.Map
{
    public class Map
    {
        public Room StartRoom { get; }
        public Room Guardian1Room { get; }
        public Room Guardian2Room { get; }
        public Room Guardian3Room { get; }
        public Room Guardian4Room { get; }
        public Room ElementalSanctuaryRoom { get; }

        public IReadOnlyDictionary<string, Room> Rooms => mRoomsByName;

        private readonly Dictionary<RoomId, Room> mRoomsById;
        private readonly Dictionary<string, Room> mRoomsByName;
        private readonly Dictionary<(RoomId From, RoomId To), MovementRequirement> mMovementRequirements = new();

        public Map()
        {
            mRoomsById = CreateRooms();
            mRoomsByName = mRoomsById.Values.ToDictionary(room => room.Name, StringComparer.Ordinal);

            StartRoom = GetRoom(RoomId.MaiaStable);
            Guardian1Room = GetRoom(RoomId.OakPass);
            Guardian2Room = GetRoom(RoomId.NewNucleon);
            Guardian3Room = GetRoom(RoomId.Ikena);
            Guardian4Room = GetRoom(RoomId.Wyrmrest);
            ElementalSanctuaryRoom = GetRoom(RoomId.GuardiansTower);

            ConnectRooms();
            AddMovementRequirements();
            PopulateWildAnimals();
        }

        // Creates all rooms first so connections can be added in one dedicated step.
        private static Dictionary<RoomId, Room> CreateRooms()
        {
            return new Dictionary<RoomId, Room>
            {
                [RoomId.MaiaStable] = new Room(RoomId.MaiaStable, "Maia's Stable", "Where new trainers obtain their first creature!"),
                [RoomId.Ikena] = new Room(RoomId.Ikena, "Ikena", "Small peaceful town where hero's are born") { IsTown = true },
                [RoomId.Road1] = new Room(RoomId.Road1, "Road 1", "Where you make your first step into your Arcadia journey!"),
                [RoomId.Road2] = new Room(RoomId.Road2, "Road 2", ""),
                [RoomId.OakPass] = new Room(RoomId.OakPass, "Oak Pass", "Town surrounded by trees and forest creatures") { IsTown = true },
                [RoomId.Road3] = new Room(RoomId.Road3, "Road 3", ""),
                [RoomId.Road4] = new Room(RoomId.Road4, "Road 4", "Tunnel"),
                [RoomId.NewNucleon] = new Room(RoomId.NewNucleon, "New Nucleon", "Founded after Nucleon incident") { IsTown = true },
                [RoomId.Road5] = new Room(RoomId.Road5, "Road 5", ""),
                [RoomId.Road6] = new Room(RoomId.Road6, "Road 6", ""),
                [RoomId.Road7] = new Room(RoomId.Road7, "Road 7", ""),
                [RoomId.Wyrmrest] = new Room(RoomId.Wyrmrest, "Wyrmrest", "Home of Dragons and Dragon Masters") { IsTown = true },
                [RoomId.Mountains] = new Room(RoomId.Mountains, "Mountains", ""),
                [RoomId.RadioactiveWay] = new Room(RoomId.RadioactiveWay, "Radioactive Way", ""),
                [RoomId.Nucleon] = new Room(RoomId.Nucleon, "Nucleon", "") { IsTown = true },
                [RoomId.FinalTrials] = new Room(RoomId.FinalTrials, "Final Trials", "Expert trainers and future titans all travel through here"),
                [RoomId.GuardiansTower] = new Room(RoomId.GuardiansTower, "Guardian Tower", "Where you find out if you're the best!"),
                [RoomId.Road8] = new Room(RoomId.Road8, "Road 8", ""),
                [RoomId.TheEnd] = new Room(RoomId.TheEnd, "The End", "Decide where you wish to stay") { IsFinalRoom = true }
            };
        }

        // Wires up room neighbors after all room instances already exist.
        private void ConnectRooms()
        {
            Room maiaStable = GetRoom(RoomId.MaiaStable);
            Room ikena = GetRoom(RoomId.Ikena);
            Room road1 = GetRoom(RoomId.Road1);
            Room road2 = GetRoom(RoomId.Road2);
            Room oakPass = GetRoom(RoomId.OakPass);
            Room road3 = GetRoom(RoomId.Road3);
            Room road4 = GetRoom(RoomId.Road4);
            Room newNucleon = GetRoom(RoomId.NewNucleon);
            Room road5 = GetRoom(RoomId.Road5);
            Room road6 = GetRoom(RoomId.Road6);
            Room road7 = GetRoom(RoomId.Road7);
            Room wyrmrest = GetRoom(RoomId.Wyrmrest);
            Room mountains = GetRoom(RoomId.Mountains);
            Room radioactiveWay = GetRoom(RoomId.RadioactiveWay);
            Room nucleon = GetRoom(RoomId.Nucleon);
            Room finalTrials = GetRoom(RoomId.FinalTrials);
            Room guardiansTower = GetRoom(RoomId.GuardiansTower);
            Room road8 = GetRoom(RoomId.Road8);
            Room theEnd = GetRoom(RoomId.TheEnd);

            maiaStable.North = ikena;

            ikena.North = theEnd;
            ikena.East = road6;
            ikena.South = road5;
            ikena.West = road1;

            road1.North = road8;
            road1.East = ikena;
            road1.South = road2;
            road1.West = maiaStable;

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

        private void AddMovementRequirements()
        {
            AddMovementRequirement(RoomId.Ikena, RoomId.Road6, requiredStarFragments: 3);
            AddMovementRequirement(RoomId.Road5, RoomId.Nucleon, requiredStarFragments: 4);
            AddMovementRequirement(RoomId.Ikena, RoomId.Road5, requiredAnimalElement: AnimalElement.Mystic);
            AddMovementRequirement(RoomId.NewNucleon, RoomId.Road5, requiredAnimalElement: AnimalElement.Mystic);
            AddMovementRequirement(RoomId.Road8, RoomId.GuardiansTower, requiresElementalTitanDefeat: true);
            AddMovementRequirement(RoomId.Ikena, RoomId.TheEnd, requiresElementalTitanDefeat: true);
        }

        private void AddMovementRequirement(
            RoomId fromRoomId,
            RoomId toRoomId,
            int requiredStarFragments = 0,
            AnimalElement? requiredAnimalElement = null,
            bool requiresElementalTitanDefeat = false)
        {
            mMovementRequirements[(fromRoomId, toRoomId)] = new MovementRequirement(
                requiredStarFragments,
                requiredAnimalElement,
                requiresElementalTitanDefeat);
        }

        public MovementRequirement GetMovementRequirement(Room currentRoom, Room destination)
        {
            ArgumentNullException.ThrowIfNull(currentRoom);
            ArgumentNullException.ThrowIfNull(destination);

            return mMovementRequirements.TryGetValue((currentRoom.Id, destination.Id), out MovementRequirement requirement)
                ? requirement
                : MovementRequirement.None;
        }

        // Populates wild animal room assignments while cloning each entry for isolated encounter state.
        private void PopulateWildAnimals()
        {
            IReadOnlyList<Arcadia_v2.Animal> mapAnimals = Arcadia_v2.GameData.CreateAnimals();

            AddAnimalToRoom(RoomId.Road1, mapAnimals[3]);
            AddAnimalToRoom(RoomId.Road2, mapAnimals[15]);
            AddAnimalToRoom(RoomId.Road2, mapAnimals[12]);
            AddAnimalToRoom(RoomId.Road3, mapAnimals[9]);
            AddAnimalToRoom(RoomId.Road3, mapAnimals[11]);
            AddAnimalToRoom(RoomId.Road4, mapAnimals[10]);
            AddAnimalToRoom(RoomId.Road5, mapAnimals[7]);
            AddAnimalToRoom(RoomId.Road6, mapAnimals[14]);
            AddAnimalToRoom(RoomId.Road7, mapAnimals[4]);
            AddAnimalToRoom(RoomId.Road7, mapAnimals[13]);
            AddAnimalToRoom(RoomId.Mountains, mapAnimals[8]);
            AddAnimalToRoom(RoomId.RadioactiveWay, mapAnimals[6]);
            AddAnimalToRoom(RoomId.FinalTrials, mapAnimals[17]);
            AddAnimalToRoom(RoomId.TheEnd, mapAnimals.Single(animal => animal.Name == "NU_DRAGON"));
        }

        // Places one wild animal into a room.
        private void AddAnimalToRoom(RoomId roomId, Arcadia_v2.Animal animal)
        {
            GetRoom(roomId).SetRoomAnimal(animal);
        }

        // Returns a room by name so the game can access specific rooms without more map fields.
        public Room GetRoom(string roomName)
        {
            if (!mRoomsByName.TryGetValue(roomName, out Room? room))
            {
                throw new ArgumentException($"Unknown room: {roomName}", nameof(roomName));
            }

            return room;
        }

        public Room GetRoom(RoomId roomId)
        {
            if (!mRoomsById.TryGetValue(roomId, out Room? room))
            {
                throw new ArgumentException($"Unknown room id: {roomId}", nameof(roomId));
            }

            return room;
        }
    }

    public readonly record struct MovementRequirement(
        int RequiredStarFragments,
        AnimalElement? RequiredAnimalElement,
        bool RequiresElementalTitanDefeat)
    {
        public static MovementRequirement None { get; } = new(0, null, false);
    }
}
