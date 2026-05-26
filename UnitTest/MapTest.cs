using Arcadia_v2;
using Arcadia_v2.Map;

namespace UnitTest
{
    public class MapTest
    {
        // Checks that creating a map sets the starting room to Professor's Lab.
        [Fact]
        public void Constructor_SetsStartRoomToProfessorLab()
        {
            Map map = new();

            Assert.Equal("Maia's Stable", map.StartRoom.Name);
        }

        // Checks that the start room connects north to Thunder Sanctuary.
        [Fact]
        public void Constructor_LinksStartRoomNorthToThunderSanctuary()
        {
            Map map = new();

            Assert.NotNull(map.StartRoom.North);
            Assert.Equal("Ikena", map.StartRoom.North!.Name);
        }

        // Checks that the starting room follows the one-way opening from the map spec.
        [Fact]
        public void Constructor_DoesNotLinkThunderSanctuaryBackToStartRoom()
        {
            Map map = new();

            Assert.NotNull(map.StartRoom.North);
            Assert.NotSame(map.StartRoom, map.StartRoom.North!.South);
        }

        // Checks that Maia's Stable can be reached again after the opening.
        [Fact]
        public void Constructor_LinksRoadOneWestToStartRoom()
        {
            Map map = new();

            Assert.Same(map.StartRoom, map.GetRoom("Road 1").West);
        }

        // Checks that each special room property points to the expected named room in the updated map.
        [Fact]
        public void Constructor_AssignsSpecialRoomsToExpectedLocations()
        {
            Map map = new();

            Assert.Equal("Oak Pass", map.Guardian1Room.Name);
            Assert.Equal("New Nucleon", map.Guardian2Room.Name);
            Assert.Equal("Ikena", map.Guardian3Room.Name);
            Assert.Equal("Wyrmrest", map.Guardian4Room.Name);
            Assert.Equal("Guardian Tower", map.ElementalSanctuaryRoom.Name);
        }

        // Checks that GetRoom returns the same room instance stored in the map properties.
        [Fact]
        public void GetRoom_ReturnsSameInstanceAsMappedProperties()
        {
            Map map = new();

            Assert.Same(map.StartRoom, map.GetRoom("Maia's Stable"));
            Assert.Same(map.StartRoom, map.GetRoom(RoomId.MaiaStable));
            Assert.Same(map.Guardian1Room, map.GetRoom("Oak Pass"));
            Assert.Same(map.ElementalSanctuaryRoom, map.GetRoom("Guardian Tower"));
        }

        // Checks that gate requirements are attached to stable room ids instead of display-name string checks.
        [Fact]
        public void GetMovementRequirement_ReturnsNamedTransitionRules()
        {
            Map map = new();

            MovementRequirement roadSixRequirement = map.GetMovementRequirement(
                map.GetRoom(RoomId.Ikena),
                map.GetRoom(RoomId.Road6));
            MovementRequirement roadFiveRequirement = map.GetMovementRequirement(
                map.GetRoom(RoomId.Ikena),
                map.GetRoom(RoomId.Road5));
            MovementRequirement openRequirement = map.GetMovementRequirement(
                map.GetRoom(RoomId.Road1),
                map.GetRoom(RoomId.Road2));

            Assert.Equal(3, roadSixRequirement.RequiredStarFragments);
            Assert.Equal(AnimalElement.Mystic, roadFiveRequirement.RequiredAnimalElement);
            Assert.Equal(MovementRequirement.None, openRequirement);
        }

        // Checks the updated vertical Road 2, Nature Sanctuary, Road 3 chain.
        [Fact]
        public void Constructor_LinksOakPassWithAdjacentRoads()
        {
            Map map = new();
            Room oakPass = map.GetRoom("Oak Pass");

            Assert.NotNull(oakPass.North);
            Assert.NotNull(oakPass.South);
            Assert.Equal("Road 2", oakPass.North!.Name);
            Assert.Equal("Road 3", oakPass.South!.Name);
            Assert.Same(oakPass, oakPass.North.South);
            Assert.Same(oakPass, oakPass.South.North);
        }

        // Checks the new eastern map branch through Wyrmrest and Nucleon.
        [Fact]
        public void Constructor_LinksRoadSevenToNucleonBranch()
        {
            Map map = new();

            Assert.Equal("Wyrmrest", map.GetRoom("Road 7").South!.Name);
            Assert.Equal("Mountains", map.GetRoom("Wyrmrest").South!.Name);
            Assert.Equal("Radioactive Way", map.GetRoom("Mountains").South!.Name);
            Assert.Equal("Nucleon", map.GetRoom("Radioactive Way").South!.Name);
            Assert.Equal("Road 5", map.GetRoom("Nucleon").West!.Name);
        }

        // Checks that requesting an unknown room throws a clear argument exception.
        [Fact]
        public void GetRoom_UnknownRoom_ThrowsArgumentException()
        {
            Map map = new();

            ArgumentException exception = Assert.Throws<ArgumentException>(() => map.GetRoom("Missing Room"));
            Assert.Equal("roomName", exception.ParamName);
        }

        // Checks that the room collection exposes the full set of rooms created by the map.
        [Fact]
        public void Rooms_ContainsAllCreatedRooms()
        {
            Map map = new();

            Assert.Equal(19, map.Rooms.Count);
            Assert.True(map.Rooms.ContainsKey("Maia's Stable"));
            Assert.True(map.Rooms.ContainsKey("Guardian Tower"));
            Assert.True(map.Rooms.ContainsKey("The End"));
        }

        // Checks that the final encounter room contains the intended final creature.
        [Fact]
        public void Constructor_PopulatesTheEndWithNuclearDragon()
        {
            Map map = new();
            Room theEnd = map.GetRoom(RoomId.TheEnd);

            Animal finalEncounter = Assert.Single(theEnd.EncounterAnimals);
            Assert.Equal("NU_DRAGON", finalEncounter.Name);
            Assert.Equal(96, finalEncounter.Id);
        }
    }
}
