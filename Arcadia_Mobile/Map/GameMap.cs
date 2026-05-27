namespace Arcadia_Mobile.Map;

public sealed class GameMap
{
    private readonly Dictionary<RoomId, Room> roomsById;

    public GameMap()
    {
        roomsById = CreatePrototypeRooms();
        StartRoom = GetRoom(RoomId.MaiaStable);
        ConnectPrototypeRooms();
    }

    public Room StartRoom { get; }

    public Room GetRoom(RoomId roomId)
    {
        if (!roomsById.TryGetValue(roomId, out Room? room))
        {
            throw new ArgumentException($"Unknown room id: {roomId}", nameof(roomId));
        }

        return room;
    }

    private static Dictionary<RoomId, Room> CreatePrototypeRooms()
    {
        return new Dictionary<RoomId, Room>
        {
            [RoomId.MaiaStable] = new(
                RoomId.MaiaStable,
                "Maia's Stable",
                "Where new trainers obtain their first creature!",
                "maias_stable.svg",
                "Maia checks the starter pens and says the stable is ready for your journey."),
            [RoomId.Ikena] = new(
                RoomId.Ikena,
                "Ikena",
                "Small peaceful town where heroes are born.",
                "ikena.svg",
                "A town guide points out the roads leaving Ikena and reminds you to prepare before traveling."),
            [RoomId.Road1] = new(
                RoomId.Road1,
                "Road 1",
                "Where you make your first step into your Arcadia journey!",
                "road1.svg",
                "Tall grass rustles nearby, but this prototype keeps encounters disabled.")
        };
    }

    private void ConnectPrototypeRooms()
    {
        Room maiaStable = GetRoom(RoomId.MaiaStable);
        Room ikena = GetRoom(RoomId.Ikena);
        Room road1 = GetRoom(RoomId.Road1);

        maiaStable.Connect(RoomDirection.North, ikena);

        ikena.Connect(RoomDirection.West, road1);

        road1.Connect(RoomDirection.East, ikena);
        road1.Connect(RoomDirection.West, maiaStable);
    }
}
