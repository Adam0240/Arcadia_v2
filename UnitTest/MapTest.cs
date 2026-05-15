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

            Assert.Equal("Professor's Lab", map.StartRoom.Name);
        }

        // Checks that the start room connects north to Ikena.
        [Fact]
        public void Constructor_LinksStartRoomNorthToIkena()
        {
            Map map = new();

            Assert.NotNull(map.StartRoom.North);
            Assert.Equal("Ikena", map.StartRoom.North!.Name);
        }

        // Checks that the room north of the start room links back south to the start room.
        [Fact]
        public void Constructor_CreatesReciprocalNorthSouthConnection()
        {
            Map map = new();

            Assert.NotNull(map.StartRoom.North);
            Assert.Same(map.StartRoom, map.StartRoom.North!.South);
        }

        // Checks that each special room property points to the expected named room in the updated map.
        [Fact]
        public void Constructor_AssignsSpecialRoomsToExpectedLocations()
        {
            Map map = new();

            Assert.Equal("Oak Pass", map.GymLeader1Room.Name);
            Assert.Equal("New Nucleon", map.GymLeader2Room.Name);
            Assert.Equal("Ikena", map.GymLeader3Room.Name);
            Assert.Equal("Dracoton", map.GymLeader4Room.Name);
            Assert.Equal("Championships", map.ChampionRoom.Name);
        }

        // Checks that GetRoom returns the same room instance stored in the map properties.
        [Fact]
        public void GetRoom_ReturnsSameInstanceAsMappedProperties()
        {
            Map map = new();

            Assert.Same(map.StartRoom, map.GetRoom("Professor's Lab"));
            Assert.Same(map.GymLeader1Room, map.GetRoom("Oak Pass"));
            Assert.Same(map.ChampionRoom, map.GetRoom("Championships"));
        }

        // Checks that Oak Pass now uses reciprocal east-west links with Route 2 and Route 3.
        [Fact]
        public void Constructor_LinksOakPassReciprocallyWithAdjacentRoutes()
        {
            Map map = new();
            Room oakPass = map.GetRoom("Oak Pass");

            Assert.NotNull(oakPass.East);
            Assert.NotNull(oakPass.West);
            Assert.Equal("Route 2", oakPass.East!.Name);
            Assert.Equal("Route 3", oakPass.West!.Name);
            Assert.Same(oakPass, oakPass.East.West);
            Assert.Same(oakPass, oakPass.West.East);
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

            Assert.Equal(15, map.Rooms.Count);
            Assert.True(map.Rooms.ContainsKey("Professor's Lab"));
            Assert.True(map.Rooms.ContainsKey("The End"));
        }
    }
}
