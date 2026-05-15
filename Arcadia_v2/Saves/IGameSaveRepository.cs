#nullable enable

namespace Arcadia_v2.Saves
{
    public interface IGameSaveRepository
    {
        void Initialize();
        void SaveJson(string saveJson);
        string? LoadJson();
        bool DeleteSave();
    }
}
