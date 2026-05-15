#nullable enable

using System.Text.Json;

namespace Arcadia_v2.Saves
{
    public static class GameSaveSerializer
    {
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = true
        };

        public static string Serialize(GameSaveState state)
        {
            return JsonSerializer.Serialize(state, SerializerOptions);
        }

        public static GameSaveState Deserialize(string saveJson)
        {
            GameSaveState? state = JsonSerializer.Deserialize<GameSaveState>(saveJson, SerializerOptions);

            if (state == null)
            {
                throw new InvalidOperationException("Save data did not contain a valid game state.");
            }

            return state;
        }
    }
}
