namespace Arcadia_Mobile.Saves;

public sealed class MobileGameSaveState
{
    public int Version { get; set; } = 1;
    public MobilePlayerSaveState Player { get; set; } = new();
}

public sealed class MobilePlayerSaveState
{
    public string CurrentRoomId { get; set; } = string.Empty;
    public List<string> VisitedRoomIds { get; set; } = new();
}
