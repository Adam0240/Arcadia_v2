using Arcadia_Mobile.Map;

namespace Arcadia_Mobile.Services;

public sealed class MobileGameSession
{
    private readonly GameMap gameMap;
    private readonly HashSet<RoomId> visitedRoomIds = new();

    public MobileGameSession(GameMap gameMap)
    {
        this.gameMap = gameMap;
        CurrentRoom = gameMap.StartRoom;
        visitedRoomIds.Add(CurrentRoom.Id);
    }

    public Room CurrentRoom { get; private set; }
    public string PlayerName { get; private set; } = string.Empty;
    public IReadOnlyCollection<RoomId> VisitedRoomIds => visitedRoomIds;

    public void StartNewGame(string playerName)
    {
        if (string.IsNullOrWhiteSpace(playerName))
        {
            throw new ArgumentException("Player name cannot be empty.", nameof(playerName));
        }

        PlayerName = playerName.Trim();
        visitedRoomIds.Clear();
        CurrentRoom = gameMap.StartRoom;
        visitedRoomIds.Add(CurrentRoom.Id);
    }

    public void Restore(string playerName, RoomId currentRoomId, IEnumerable<RoomId> visitedRoomIds)
    {
        ArgumentNullException.ThrowIfNull(visitedRoomIds);

        PlayerName = playerName.Trim();
        CurrentRoom = gameMap.GetRoom(currentRoomId);
        this.visitedRoomIds.Clear();

        foreach (RoomId visitedRoomId in visitedRoomIds)
        {
            this.visitedRoomIds.Add(visitedRoomId);
        }

        this.visitedRoomIds.Add(CurrentRoom.Id);
    }

    public bool CanMove(RoomDirection direction)
    {
        return CurrentRoom.HasExit(direction);
    }

    public MoveResult Move(RoomDirection direction)
    {
        Room? destination = CurrentRoom.GetExit(direction);

        if (destination == null)
        {
            return new MoveResult(false, "You cannot travel that way from here.");
        }

        CurrentRoom = destination;
        visitedRoomIds.Add(CurrentRoom.Id);
        return new MoveResult(true, $"Moved to {CurrentRoom.Name}.");
    }

    public string Interact()
    {
        return CurrentRoom.InteractionText;
    }
}
