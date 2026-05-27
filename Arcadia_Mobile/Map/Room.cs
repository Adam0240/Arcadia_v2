namespace Arcadia_Mobile.Map;

public sealed class Room
{
    private readonly Dictionary<RoomDirection, Room> exits = new();

    public Room(RoomId id, string name, string description, string imageName, string interactionText)
    {
        Id = id;
        Name = name;
        Description = description;
        ImageName = imageName;
        InteractionText = interactionText;
    }

    public RoomId Id { get; }
    public string Name { get; }
    public string Description { get; }
    public string ImageName { get; }
    public string InteractionText { get; }
    public IReadOnlyDictionary<RoomDirection, Room> Exits => exits;

    public void Connect(RoomDirection direction, Room destination)
    {
        ArgumentNullException.ThrowIfNull(destination);
        exits[direction] = destination;
    }

    public Room? GetExit(RoomDirection direction)
    {
        return exits.TryGetValue(direction, out Room? room) ? room : null;
    }

    public bool HasExit(RoomDirection direction)
    {
        return exits.ContainsKey(direction);
    }
}
