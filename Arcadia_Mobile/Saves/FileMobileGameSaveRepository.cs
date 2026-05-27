namespace Arcadia_Mobile.Saves;

public sealed class FileMobileGameSaveRepository : IMobileGameSaveRepository
{
    private readonly string saveFilePath;

    public FileMobileGameSaveRepository(string saveFilePath)
    {
        if (string.IsNullOrWhiteSpace(saveFilePath))
        {
            throw new ArgumentException("Save file path cannot be empty.", nameof(saveFilePath));
        }

        this.saveFilePath = saveFilePath;
    }

    public Task InitializeAsync()
    {
        string? saveDirectory = Path.GetDirectoryName(saveFilePath);

        if (!string.IsNullOrWhiteSpace(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
        }

        return Task.CompletedTask;
    }

    public async Task SaveJsonAsync(string saveJson)
    {
        if (string.IsNullOrWhiteSpace(saveJson))
        {
            throw new ArgumentException("Save JSON cannot be empty.", nameof(saveJson));
        }

        await InitializeAsync();
        await File.WriteAllTextAsync(saveFilePath, saveJson);
    }

    public async Task<string?> LoadJsonAsync()
    {
        if (!File.Exists(saveFilePath))
        {
            return null;
        }

        return await File.ReadAllTextAsync(saveFilePath);
    }

    public Task<bool> DeleteSaveAsync()
    {
        if (!File.Exists(saveFilePath))
        {
            return Task.FromResult(false);
        }

        File.Delete(saveFilePath);
        return Task.FromResult(true);
    }
}
