namespace Arcadia_Mobile.Map;

public sealed class GameMap
{
    private const string PlaceholderImage = "room_placeholder.svg";
    private readonly Dictionary<RoomId, Room> roomsById;

    public GameMap()
    {
        roomsById = CreateRooms();
        StartRoom = GetRoom(RoomId.MaiaStable);
        ConnectRooms();
    }

    public Room StartRoom { get; }
    public IReadOnlyCollection<Room> Rooms => roomsById.Values;

    public Room GetRoom(RoomId roomId)
    {
        if (!roomsById.TryGetValue(roomId, out Room? room))
        {
            throw new ArgumentException($"Unknown room id: {roomId}", nameof(roomId));
        }

        return room;
    }

    private static Dictionary<RoomId, Room> CreateRooms()
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
                "Tall grass rustles nearby, but this prototype keeps encounters disabled."),
            [RoomId.Road2] = CreatePlaceholderRoom(
                RoomId.Road2,
                "Road 2",
                "A quiet path leading deeper into Arcadia.",
                "The path ahead is clear, with signs of wild creatures nearby."),
            [RoomId.OakPass] = CreatePlaceholderRoom(
                RoomId.OakPass,
                "Oak Pass",
                "Town surrounded by trees and forest creatures.",
                "The old trees sway above the pass while travelers rest beneath them."),
            [RoomId.Road3] = CreatePlaceholderRoom(
                RoomId.Road3,
                "Road 3",
                "A wooded road stretching south from Oak Pass.",
                "Branches shade the path and make the road feel calm but watchful."),
            [RoomId.Road4] = CreatePlaceholderRoom(
                RoomId.Road4,
                "Road 4",
                "Tunnel",
                "The tunnel walls echo softly as you inspect the passage."),
            [RoomId.NewNucleon] = CreatePlaceholderRoom(
                RoomId.NewNucleon,
                "New Nucleon",
                "Founded after Nucleon incident.",
                "Residents keep rebuilding, determined to make the town safer than before."),
            [RoomId.Road5] = CreatePlaceholderRoom(
                RoomId.Road5,
                "Road 5",
                "A crossroads between Ikena, New Nucleon, and Nucleon.",
                "Tracks split across the road, showing steady travel in several directions."),
            [RoomId.Road6] = CreatePlaceholderRoom(
                RoomId.Road6,
                "Road 6",
                "A road branching east from Ikena.",
                "The route opens toward stronger challenges beyond town."),
            [RoomId.Road7] = CreatePlaceholderRoom(
                RoomId.Road7,
                "Road 7",
                "A route leading toward Wyrmrest.",
                "Warm winds move across the path from the dragon lands ahead."),
            [RoomId.Wyrmrest] = CreatePlaceholderRoom(
                RoomId.Wyrmrest,
                "Wyrmrest",
                "Home of Dragons and Dragon Masters.",
                "Dragon banners hang over stone paths throughout Wyrmrest."),
            [RoomId.Mountains] = CreatePlaceholderRoom(
                RoomId.Mountains,
                "Mountains",
                "Steep highlands south of Wyrmrest.",
                "Loose stones shift underfoot as the mountain trail climbs and bends."),
            [RoomId.RadioactiveWay] = CreatePlaceholderRoom(
                RoomId.RadioactiveWay,
                "Radioactive Way",
                "A dangerous route scarred by the Nucleon incident.",
                "Warning markers line the road and faint light pulses in the distance."),
            [RoomId.Nucleon] = CreatePlaceholderRoom(
                RoomId.Nucleon,
                "Nucleon",
                "The town at the center of the old incident.",
                "The streets are quiet, but the place still feels important."),
            [RoomId.FinalTrials] = CreatePlaceholderRoom(
                RoomId.FinalTrials,
                "Final Trials",
                "Expert trainers and future titans all travel through here.",
                "The air feels heavier here, as if the road expects you to prove yourself."),
            [RoomId.GuardiansTower] = CreatePlaceholderRoom(
                RoomId.GuardiansTower,
                "Guardian Tower",
                "Where you find out if you're the best!",
                "The tower rises above Arcadia, waiting for those ready to face its guardians."),
            [RoomId.Road8] = CreatePlaceholderRoom(
                RoomId.Road8,
                "Road 8",
                "A road connecting Road 1 to Guardian Tower.",
                "The route feels like a return path and a final approach at the same time."),
            [RoomId.TheEnd] = CreatePlaceholderRoom(
                RoomId.TheEnd,
                "The End",
                "Decide where you wish to stay.",
                "Everything grows still here, leaving only the weight of your final choice.")
        };
    }

    private static Room CreatePlaceholderRoom(RoomId id, string name, string description, string interactionText)
    {
        return new Room(id, name, description, PlaceholderImage, interactionText);
    }

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

        maiaStable.Connect(RoomDirection.North, ikena);

        ikena.Connect(RoomDirection.North, theEnd);
        ikena.Connect(RoomDirection.East, road6);
        ikena.Connect(RoomDirection.South, road5);
        ikena.Connect(RoomDirection.West, road1);

        road1.Connect(RoomDirection.North, road8);
        road1.Connect(RoomDirection.East, ikena);
        road1.Connect(RoomDirection.South, road2);
        road1.Connect(RoomDirection.West, maiaStable);

        road2.Connect(RoomDirection.North, road1);
        road2.Connect(RoomDirection.South, oakPass);

        oakPass.Connect(RoomDirection.North, road2);
        oakPass.Connect(RoomDirection.South, road3);

        road3.Connect(RoomDirection.North, oakPass);
        road3.Connect(RoomDirection.South, road4);

        road4.Connect(RoomDirection.North, road3);
        road4.Connect(RoomDirection.South, newNucleon);

        newNucleon.Connect(RoomDirection.North, road4);
        newNucleon.Connect(RoomDirection.East, road5);

        road5.Connect(RoomDirection.North, ikena);
        road5.Connect(RoomDirection.East, nucleon);
        road5.Connect(RoomDirection.West, newNucleon);

        road6.Connect(RoomDirection.North, finalTrials);
        road6.Connect(RoomDirection.South, road7);
        road6.Connect(RoomDirection.West, ikena);

        road7.Connect(RoomDirection.North, road6);
        road7.Connect(RoomDirection.South, wyrmrest);

        wyrmrest.Connect(RoomDirection.North, road7);
        wyrmrest.Connect(RoomDirection.South, mountains);

        mountains.Connect(RoomDirection.North, wyrmrest);
        mountains.Connect(RoomDirection.South, radioactiveWay);

        radioactiveWay.Connect(RoomDirection.North, mountains);
        radioactiveWay.Connect(RoomDirection.South, nucleon);

        nucleon.Connect(RoomDirection.North, radioactiveWay);
        nucleon.Connect(RoomDirection.West, road5);

        finalTrials.Connect(RoomDirection.North, guardiansTower);
        finalTrials.Connect(RoomDirection.South, road6);

        guardiansTower.Connect(RoomDirection.East, finalTrials);
        guardiansTower.Connect(RoomDirection.South, ikena);
        guardiansTower.Connect(RoomDirection.West, road8);

        road8.Connect(RoomDirection.North, guardiansTower);
        road8.Connect(RoomDirection.South, road1);

        theEnd.Connect(RoomDirection.South, ikena);
    }
}
