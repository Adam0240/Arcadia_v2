#nullable enable

using System.Text.Json;

namespace Arcadia_v2.Saves
{
    public sealed class GameSaveService
    {
        private readonly IGameSaveRepository mRepository;

        public GameSaveService(IGameSaveRepository repository)
        {
            ArgumentNullException.ThrowIfNull(repository);
            mRepository = repository;
        }

        public void Initialize()
        {
            mRepository.Initialize();
        }

        public SaveCommandResult Save(GameState gameState)
        {
            GameSaveState saveState = GameStateMapper.Capture(gameState);
            string saveJson = GameSaveSerializer.Serialize(saveState);

            mRepository.SaveJson(saveJson);

            return SaveCommandResult.Success("Game saved.");
        }

        public bool HasSave()
        {
            return mRepository.LoadJson() != null;
        }

        public SaveCommandResult Load(GameState gameState)
        {
            string? saveJson = mRepository.LoadJson();

            if (saveJson == null)
            {
                return SaveCommandResult.Failure("No save data found.");
            }

            try
            {
                GameSaveState saveState = GameSaveSerializer.Deserialize(saveJson);
                GameStateMapper.Apply(gameState, saveState);
                return SaveCommandResult.Success("Game loaded.");
            }
            catch (Exception exception) when (exception is JsonException or InvalidOperationException or ArgumentException)
            {
                return SaveCommandResult.Failure("Save data could not be loaded.");
            }
        }

        public SaveCommandResult Delete()
        {
            bool deleted = mRepository.DeleteSave();

            return deleted
                ? SaveCommandResult.Success("Save data deleted.")
                : SaveCommandResult.Failure("No save data found.");
        }
    }

    public readonly record struct SaveCommandResult(bool Succeeded, string Message)
    {
        public static SaveCommandResult Success(string message)
        {
            return new SaveCommandResult(true, message);
        }

        public static SaveCommandResult Failure(string message)
        {
            return new SaveCommandResult(false, message);
        }
    }
}
