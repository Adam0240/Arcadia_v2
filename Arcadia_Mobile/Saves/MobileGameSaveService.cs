using System.Text.Json;
using Arcadia_Mobile.Services;

namespace Arcadia_Mobile.Saves;

public sealed class MobileGameSaveService
{
    private readonly IMobileGameSaveRepository repository;

    public MobileGameSaveService(IMobileGameSaveRepository repository)
    {
        ArgumentNullException.ThrowIfNull(repository);
        this.repository = repository;
    }

    public Task InitializeAsync()
    {
        return repository.InitializeAsync();
    }

    public async Task<MobileSaveCommandResult> SaveAsync(MobileGameSession gameSession)
    {
        MobileGameSaveState saveState = MobileGameStateMapper.Capture(gameSession);
        string saveJson = MobileGameSaveSerializer.Serialize(saveState);

        await repository.SaveJsonAsync(saveJson);

        return MobileSaveCommandResult.Success("Game saved.");
    }

    public async Task<bool> HasSaveAsync()
    {
        return await repository.LoadJsonAsync() != null;
    }

    public async Task<MobileSaveCommandResult> LoadAsync(MobileGameSession gameSession)
    {
        string? saveJson = await repository.LoadJsonAsync();

        if (saveJson == null)
        {
            return MobileSaveCommandResult.Failure("No save data found.");
        }

        try
        {
            MobileGameSaveState saveState = MobileGameSaveSerializer.Deserialize(saveJson);
            MobileGameStateMapper.Apply(gameSession, saveState);
            return MobileSaveCommandResult.Success("Game loaded.");
        }
        catch (Exception exception) when (exception is JsonException or InvalidOperationException or ArgumentException)
        {
            return MobileSaveCommandResult.Failure("Save data could not be loaded.");
        }
    }

    public async Task<MobileSaveCommandResult> DeleteAsync()
    {
        bool deleted = await repository.DeleteSaveAsync();

        return deleted
            ? MobileSaveCommandResult.Success("Save data deleted.")
            : MobileSaveCommandResult.Failure("No save data found.");
    }
}

public readonly record struct MobileSaveCommandResult(bool Succeeded, string Message)
{
    public static MobileSaveCommandResult Success(string message)
    {
        return new MobileSaveCommandResult(true, message);
    }

    public static MobileSaveCommandResult Failure(string message)
    {
        return new MobileSaveCommandResult(false, message);
    }
}
