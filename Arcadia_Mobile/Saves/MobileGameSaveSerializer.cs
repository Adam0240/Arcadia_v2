using System.Text.Json;

namespace Arcadia_Mobile.Saves;

public static class MobileGameSaveSerializer
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    public static string Serialize(MobileGameSaveState state)
    {
        return JsonSerializer.Serialize(state, SerializerOptions);
    }

    public static MobileGameSaveState Deserialize(string saveJson)
    {
        MobileGameSaveState? state = JsonSerializer.Deserialize<MobileGameSaveState>(saveJson, SerializerOptions);

        if (state == null)
        {
            throw new InvalidOperationException("Save data did not contain a valid mobile game state.");
        }

        return state;
    }
}
