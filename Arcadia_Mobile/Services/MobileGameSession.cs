using Arcadia_Mobile.Map;

namespace Arcadia_Mobile.Services;

public sealed class MobileGameSession
{
    public MobileGameSession(GameMap gameMap)
    {
        CurrentRoom = gameMap.StartRoom;
    }

    public Room CurrentRoom { get; private set; }

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
        return new MoveResult(true, $"Moved to {CurrentRoom.Name}.");
    }

    public string Interact()
    {
        return CurrentRoom.InteractionText;
    }
}
