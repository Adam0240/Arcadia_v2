using Arcadia_Mobile.Map;
using Arcadia_Mobile.Services;

namespace Arcadia_Mobile.Saves;

public static class MobileGameStateMapper
{
    public static MobileGameSaveState Capture(MobileGameSession gameSession)
    {
        ArgumentNullException.ThrowIfNull(gameSession);

        return new MobileGameSaveState
        {
            Version = 1,
            Player = new MobilePlayerSaveState
            {
                CurrentRoomId = gameSession.CurrentRoom.Id.ToString(),
                VisitedRoomIds = gameSession.VisitedRoomIds
                    .Select(roomId => roomId.ToString())
                    .ToList()
            }
        };
    }

    public static void Apply(MobileGameSession gameSession, MobileGameSaveState saveState)
    {
        ArgumentNullException.ThrowIfNull(gameSession);
        ArgumentNullException.ThrowIfNull(saveState);

        RoomId currentRoomId = ParseRoomId(saveState.Player.CurrentRoomId);
        List<RoomId> visitedRoomIds = saveState.Player.VisitedRoomIds
            .Select(ParseRoomId)
            .ToList();

        gameSession.Restore(currentRoomId, visitedRoomIds);
    }

    private static RoomId ParseRoomId(string roomId)
    {
        if (!Enum.TryParse(roomId, out RoomId parsedRoomId) ||
            !Enum.IsDefined(parsedRoomId))
        {
            throw new InvalidOperationException($"Unknown room id in save data: {roomId}");
        }

        return parsedRoomId;
    }
}
