using Arcadia_v2.Saves;

namespace UnitTest;

internal sealed class FakeGameSaveRepository : IGameSaveRepository
{
    private string? mSaveJson;

    public FakeGameSaveRepository(string? saveJson = null)
    {
        mSaveJson = saveJson;
    }

    public bool HasSave => mSaveJson != null;
    public string? SaveJsonValue => mSaveJson;

    public void Initialize()
    {
    }

    public void SaveJson(string saveJson)
    {
        mSaveJson = saveJson;
    }

    public string? LoadJson()
    {
        return mSaveJson;
    }

    public bool DeleteSave()
    {
        bool hadSave = mSaveJson != null;
        mSaveJson = null;
        return hadSave;
    }
}
