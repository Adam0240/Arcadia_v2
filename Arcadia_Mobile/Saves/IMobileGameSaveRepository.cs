namespace Arcadia_Mobile.Saves;

public interface IMobileGameSaveRepository
{
    Task InitializeAsync();
    Task SaveJsonAsync(string saveJson);
    Task<string?> LoadJsonAsync();
    Task<bool> DeleteSaveAsync();
}
